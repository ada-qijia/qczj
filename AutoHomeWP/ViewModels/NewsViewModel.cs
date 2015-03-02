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
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace ViewModels
{
    public class NewsViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public NewsViewModel()
        {
            _newestDataSource = new ObservableCollection<NewsModel>();
        }

        /// <summary>
        /// 返回的数据集合
        /// </summary>
        private ObservableCollection<NewsModel> _newestDataSource;
        public ObservableCollection<NewsModel> newestDataSource
        {
            get
            {
                return _newestDataSource;
            }
            set
            {
                if (value != _newestDataSource)
                {
                    OnPropertyChanging("newestDataSource");
                    _newestDataSource = value;
                    OnPropertyChanged("newestDataSource");
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
        /// <param name="datatype">数据类型，1-文章类数据（默认），2-说客，3-视频</param>
        public void LoadDataAysnc(string url, int datatype = 1)
        {
            WebClient wc = new WebClient();
            if (wc.IsBusy != false)
            {
                wc.CancelAsync();
                return;
            }

            //wc.Encoding = new Gb2312Encoding();
            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url + "&" + Guid.NewGuid().ToString(), UriKind.Absolute);
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
                    //清除旧的数据
                    // newestDataSource.Clear();
                    try
                    {
                        //返回的json数据
                        JObject json = JObject.Parse(ee.Result);
                        if (datatype == 1)
                        {
                            JArray newestJson = (JArray)json.SelectToken("result").SelectToken("newslist");

                            NewsModel model = null;
                            for (int i = 0; i < newestJson.Count; i++)
                            {
                                model = new NewsModel();
                                model.id = (int)newestJson[i].SelectToken("id");
                                model.title = (string)newestJson[i].SelectToken("title");
                                model.time = (string)newestJson[i].SelectToken("time");
                                model.type = (string)newestJson[i].SelectToken("type");
                                model.smallpic = (string)newestJson[i].SelectToken("smallpic");
                                model.replycount = newestJson[i].SelectToken("replycount").ToString();
                                model.pagecount = newestJson[i].SelectToken("pagecount").ToString();
                                model.indexdetail = (string)newestJson[i].SelectToken("indexdetail");
                                model.pageIndex = newestJson[i].SelectToken("jumppage").ToString();
                                model.showData = "Visible";
                                newestDataSource.Add(model);
                            }

                            model = new NewsModel();
                            model.Total = (int)json.SelectToken("result").SelectToken("rowcount");
                            model.showData = "Collapsed";
                            model.loadMore = "点击加载更多......";
                            newestDataSource.Add(model);
                        }
                        else if (datatype == 2)
                        {
                            JArray newestJson = (JArray)json.SelectToken("result").SelectToken("list");

                            NewsModel model = null;
                            for (int i = 0; i < newestJson.Count; i++)
                            {
                                model = new NewsModel();
                                model.id = (int)newestJson[i].SelectToken("id");
                                model.title = (string)newestJson[i].SelectToken("title");
                                model.time = (string)newestJson[i].SelectToken("time");
                                model.type = (string)newestJson[i].SelectToken("type");
                                model.smallpic = (string)newestJson[i].SelectToken("smallpic");
                                model.replycount = newestJson[i].SelectToken("replycount").ToString();
                                model.pagecount = newestJson[i].SelectToken("pagecount").ToString();
                                model.pageIndex = newestJson[i].SelectToken("jumppage").ToString();
                                model.showData = "Visible";
                                newestDataSource.Add(model);
                            }

                            model = new NewsModel();
                            model.Total = (int)json.SelectToken("result").SelectToken("total");
                            model.showData = "Collapsed";
                            model.loadMore = "点击加载更多......";
                            newestDataSource.Add(model);
                        }
                        else if (datatype == 3)
                        {
                            JArray newestJson = (JArray)json.SelectToken("result").SelectToken("list");

                            VideoModel model = null;
                            for (int i = 0; i < newestJson.Count; i++)
                            {
                                model = new VideoModel();
                                model.id = (int)newestJson[i].SelectToken("id");
                                model.title = (string)newestJson[i].SelectToken("title");
                                model.time = (string)newestJson[i].SelectToken("time");
                                model.type = (string)newestJson[i].SelectToken("type");
                                model.indexdetail = (string)newestJson[i].SelectToken("indexdetail");
                                model.smallpic = (string)newestJson[i].SelectToken("smallimg");
                                model.replycount = newestJson[i].SelectToken("replycount").ToString();
                                model.playcount = newestJson[i].SelectToken("playcount").ToString();
                                model.nickname = (string)newestJson[i].SelectToken("nickname");
                                model.videoaddress = (string)newestJson[i].SelectToken("videoaddress");
                                model.shareaddress = (string)newestJson[i].SelectToken("shareaddress");
                                model.showData = "Visible";
                                newestDataSource.Add(model);
                            }

                            model = new VideoModel();
                            model.Total = (int)json.SelectToken("result").SelectToken("rowcount");
                            model.showData = "Collapsed";
                            model.loadMore = "点击加载更多......";
                            newestDataSource.Add(model);
                        }
                        //newestDataSource[0].topImage =new StorageCachedImage(new Uri(newestDataSource[0].smallpic, UriKind.Absolute));
                        for (int i = 0; i < newestDataSource.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(newestDataSource[i].smallpic))
                            {
                                newestDataSource[i].bitmap = new StorageCachedImage(new Uri(newestDataSource[i].smallpic, UriKind.Absolute));
                            }
                        }
                    }
                    catch
                    { }
                }

                //注意
                apiArgs.Result = newestDataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });


        }

    }
}
