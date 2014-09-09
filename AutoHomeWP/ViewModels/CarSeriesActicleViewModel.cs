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
    public class CarSeriesActicleViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public CarSeriesActicleViewModel()
        {
            _carSeriesActicleDataSource = new ObservableCollection<NewsModel>();
        }

        /// <summary>
        /// 返回的数据集合
        /// </summary>
        private ObservableCollection<NewsModel> _carSeriesActicleDataSource;
        public ObservableCollection<NewsModel> CarSeriesActicleDataSource
        {
            get
            {
                return _carSeriesActicleDataSource;
            }
            set
            {
                if (value != _carSeriesActicleDataSource)
                {
                    OnPropertyChanging("CarSeriesActicleDataSource");
                    _carSeriesActicleDataSource = value;
                    OnPropertyChanged("CarSeriesActicleDataSource");
                }
            }
        }

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


        //事件通知
        public event EventHandler<APIEventArgs<IEnumerable<NewsModel>>> LoadDataCompleted;

        /// <summary>
        /// 请求Json格式的数据
        /// </summary>
        /// <param name="url">请求的地址url</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        public void LoadDataAysnc(string url)
        {
            WebClient wc = new WebClient();
            if (wc.IsBusy != false)
            {
                wc.CancelAsync();
                return;
            }

          //  wc.Encoding = new Gb2312Encoding();
            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url, UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<IEnumerable<NewsModel>> apiArgs = new APIEventArgs<IEnumerable<NewsModel>>();
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {

                    //返回的json数据
                    JObject json = JObject.Parse(ee.Result);
                    JArray newestJson = (JArray)json.SelectToken("result").SelectToken("list");


                    NewsModel model = null;
                    for (int i = 0; i < newestJson.Count; i++)
                    {
                        model = new NewsModel();
                        model.id = (int)newestJson[i].SelectToken("id");
                        model.title = (string)newestJson[i].SelectToken("title");
                        model.time = (string)newestJson[i].SelectToken("time");
                        model.replycount = newestJson[i].SelectToken("replycount").ToString();
                        CarSeriesActicleDataSource.Add(model);
                    }
                    model = new NewsModel();
                    model.Total = (int)json.SelectToken("result").SelectToken("rowcount");
                    model.loadMore = "点击加载更多......";
                    CarSeriesActicleDataSource.Add(model);

                }

                //注意
                apiArgs.Result = CarSeriesActicleDataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });


        }


    }
}
