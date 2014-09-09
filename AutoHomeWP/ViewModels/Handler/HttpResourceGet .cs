using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;

namespace ViewModels.Handler
{

    /// <summary>
    /// 从网络读取流的回调
    /// </summary>
    public delegate void GetDataStreamCallback(Stream stream);
    /// <summary>
    /// 生成了图片源之后的回调
    /// </summary>
    public delegate void GetPicCallback(BitmapSource bimage);


    /// <summary>
    /// 可以撤销的操作
    /// </summary>
    interface IRevocable
    {

        void RevokeAsync();

        event Action ProcessCompleted;
    }

    public class HttpResourceGet : IRevocable
    {
        public event GetDataStreamCallback OnDataStreamGenerated;
        public event Action ProcessCompleted;
        WebClient m_client;
        public HttpResourceGet()
        {
            m_client = new WebClient();
            m_client.OpenReadCompleted += ((send, ev) =>
            {
                do
                {
                    if (ev.Error != null || ev.Cancelled)
                    {
                        break;
                    }
                    if (OnDataStreamGenerated != null)
                    {
                        OnDataStreamGenerated(ev.Result);
                        ev.Result.Close();
                    }
                } while (false);

                if (ProcessCompleted != null)
                {
                    ProcessCompleted();
                }
            });
        }

        public void BeginGetData(string url)
        {
            m_client.OpenReadAsync(new Uri(url));
        }

        public void RevokeAsync()
        {
            m_client.CancelAsync();
        }

    }


    /// <summary>
    /// 把网络数据包装为图片源
    /// </summary>
    public class HttpPicGet : IRevocable
    {
        

        public event GetPicCallback OnImageLoadCompleted;
        public event Action ProcessCompleted;
        HttpResourceGet m_httpGet;
        public HttpPicGet()
        {
            m_httpGet = new HttpResourceGet();
            m_httpGet.OnDataStreamGenerated += (stream =>
            {
                BitmapSource bi = new BitmapImage();
                bi.SetSource(stream);
                if (OnImageLoadCompleted != null)
                {
                    OnImageLoadCompleted(bi);
                }
            });
            m_httpGet.ProcessCompleted += (() =>
            {
                if (ProcessCompleted != null)
                {
                    ProcessCompleted();
                }
            });
        }

        public void BeginLoadPic(string url)
        {
            m_httpGet.BeginGetData(url);
        }


        public void RevokeAsync()
        {
            m_httpGet.RevokeAsync();
        }

    }


    /// <summary>
    /// 做一个容器,用来处理多条任务
    /// </summary>
    public class RevocableContainer
    {
        private class QueueItem
        {
            public GetPicCallback action;
            public string url;
        }

        const int Threshold = 3;

        AutoResetEvent m_event;
        int m_count;
        bool m_isThreadProcessing;
        Queue<QueueItem> m_queue;
        List<IRevocable> m_list;
        object m_lock;
        public RevocableContainer()
        {
            m_event = new AutoResetEvent(false);
            m_queue = new Queue<QueueItem>();
            m_list = new List<IRevocable>();
            m_lock = new object();
            m_count = Threshold;
            m_isThreadProcessing = false;
        }

        void HttpRequestThread()
        {
            while (true)
            {
                if (m_count == 0)
                {
                    m_event.WaitOne();
                }
                QueueItem item = null;
                //out from queue
                lock (m_queue)
                {
                    if (!m_isThreadProcessing)
                    {
                        break;
                    }
                    if (m_queue.Count == 0)
                    {
                        break;
                    }

                    item = m_queue.Dequeue();
                    Interlocked.Decrement(ref  m_count);

                }

                //do request
                HttpPicGet pic = new HttpPicGet();
                pic.OnImageLoadCompleted += (img =>
                {
                    item.action(img);
                });

                pic.ProcessCompleted += (() =>
                {
                    lock (m_list)
                    {
                        m_list.Remove(pic);
                    }
                    if (m_count == 0)
                    {
                        m_event.Set();
                    }
                    Interlocked.Increment(ref m_count);
                });
                pic.BeginLoadPic(item.url);

                //into list
                lock (m_list)
                {
                    m_list.Add(pic);
                }

                Thread.Sleep(1);
            }
        }


        public void EnQueue(string url, GetPicCallback action)
        {
            QueueItem item = new QueueItem() { action = action, url = url };
            BackgroundWorker worker = null;
            lock (m_queue)
            {
                m_queue.Enqueue(item);
                if (!m_isThreadProcessing)
                {
                    m_isThreadProcessing = true;
                    worker = new BackgroundWorker();
                }
            }

            if (worker != null)
            {
                worker.DoWork += ((send, ev) => HttpRequestThread());
                worker.RunWorkerCompleted += ((send, ev) =>
                {
                    lock (m_queue)
                    {
                        m_isThreadProcessing = false;
                    }
                });

                worker.RunWorkerAsync();
            }

        }

        public void CancelAll()
        {

            lock (m_queue)
            {
                m_isThreadProcessing = false;
                m_queue.Clear();
            }
            lock (m_list)
            {
                foreach (IRevocable item in m_list)
                {
                    item.RevokeAsync();
                }
            }
        }
    }


    /// <summary>
    /// 异步绑定
    /// </summary>
    public class MyImage : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
     
        string m_url;
        public string URL
        {
            get { return m_url; }
            set
            {
                if (m_url != value)
                {
                    m_url = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("URL"));
                }
            }
        }

        BitmapSource m_source;
        public BitmapSource Source
        {
            get { return m_source; }
            set
            {
                if (m_source != value)
                {
                    m_source = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Source"));
                }
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, args);
        }
    }
}
