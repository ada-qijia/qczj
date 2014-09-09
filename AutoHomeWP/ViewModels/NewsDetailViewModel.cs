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
using System.ComponentModel;
using System.Collections.ObjectModel;
using Model;
using ViewModels.Handler;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ViewModels
{
    public class NewsDetailViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// 属性更改时的事件
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;
        public void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        //属性更改完毕事件
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public NewsDetailViewModel()
        {
            _newsDataSource = new ObservableCollection<NewsDetailModel>();
        }
        /// <summary>
        /// 返回的数据集合
        /// </summary>
        private ObservableCollection<NewsDetailModel> _newsDataSource;
        public ObservableCollection<NewsDetailModel> newsDataSource
        {
            get
            {
                return _newsDataSource;
            }
            set
            {
                if (value != _newsDataSource)
                {
                    OnPropertyChanging("newsDataSource");
                    _newsDataSource = value;
                    OnPropertyChanged("newsDataSource");
                }
            }
        }

        //事件通知
        public event EventHandler<APIEventArgs<IEnumerable<NewsDetailModel>>> LoadDataCompleted;


        WebClient wc = null;
        /// <summary>
        /// 请求文章详情页数据
        /// </summary>
        /// <param name="url"></param>
        public void LoadDataAysnc(string url)
        {
            if (wc == null)
            {
                wc = new WebClient();
            }
            if (wc.IsBusy == true)
            {
                wc.CancelAsync();
            }

            //wc.Encoding = new Gb2312Encoding();
            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url, UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<IEnumerable<NewsDetailModel>> apiArgs = new APIEventArgs<IEnumerable<NewsDetailModel>>();
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {
                    //返回的json数据
                    JObject json = JObject.Parse(ee.Result);
                    if (json != null)
                    {
                        JToken array = json.SelectToken("result");


                        NewsDetailModel model = null;
                        if (array != null)
                        {
                            //页数
                            int pageCount = (int)array.SelectToken("pagenum");
                            //标题
                            string pageList = (string)array.SelectToken("pagelist");

                            for (int i = 1; i <= pageCount; i++)
                            {
                                model = new NewsDetailModel();
                                model.Id = i;
                                model.PageTitle = pageList.Split('|')[i - 1];
                                model.Title = (string)array.SelectToken("title");
                                model.Type = (string)array.SelectToken("type");
                                model.PageCount = pageCount;
                                model.ReplyCount = (int)array.SelectToken("replycount");
                                model.WebURL = (string)array.SelectToken("url");
                                newsDataSource.Add(model);
                            }
                        }
                    }
                }
                //注意
                apiArgs.Result = newsDataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }

            });
        }

    }
}
