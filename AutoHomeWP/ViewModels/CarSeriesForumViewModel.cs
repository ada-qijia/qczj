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
    public class CarSeriesForumViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public CarSeriesForumViewModel()
        {
            _forumCarSeriesDataSource = new ObservableCollection<ForumModel>();
        }

        /// <summary>
        /// 返回的数据集合
        /// </summary>
        private ObservableCollection<ForumModel> _forumCarSeriesDataSource;
        public ObservableCollection<ForumModel> ForumCarSeriesDataSource
        {
            get
            {
                return _forumCarSeriesDataSource;
            }
            set
            {
                if (value != _forumCarSeriesDataSource)
                {
                    OnPropertyChanging("ForumCarSeriesDataSource");
                    _forumCarSeriesDataSource = value;
                    OnPropertyChanged("ForumCarSeriesDataSource");
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
        public event EventHandler<APIEventArgs<IEnumerable<ForumModel>>> LoadDataCompleted;

        /// <summary>
        /// 请求Json格式的数据
        /// </summary>
        /// <param name="url">请求的地址url</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        public void LoadDataAysnc(string url)
        {
            WebClient wc = new WebClient();
            wc.Headers["Accept-Charset"] = "utf-8";
           // wc.Encoding = DBCSEncoding.GetDBCSEncoding("gb2312");
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url+"&" + Guid.NewGuid().ToString(), UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<IEnumerable<ForumModel>> apiArgs = new APIEventArgs<IEnumerable<ForumModel>>();
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {
                    try
                    {

                        //返回的json数据
                        JObject json = JObject.Parse(ee.Result);
                        JArray forumJson = (JArray)json.SelectToken("result").SelectToken("list");

                        ////清除旧数据
                        //ForumCarSeriesDataSource.Clear();

                        ForumModel model = null;
                        for (int i = 0; i < forumJson.Count; i++)
                        {
                            model = new ForumModel();

                            model.Title = (string)forumJson[i].SelectToken("title");
                            model.PostUserName = (string)forumJson[i].SelectToken("postusername");
                            model.ReplyCounts = (int)forumJson[i].SelectToken("replycounts");
                            model.LastReplyDate = (string)forumJson[i].SelectToken("lastreplydate");
                            model.TopicType = (string)forumJson[i].SelectToken("topictype");
                            model.TopicId = (int)forumJson[i].SelectToken("topicid");
                            model.bbsType = (string)forumJson[i].SelectToken("bbstype");
                            model.bbsId = (int)forumJson[i].SelectToken("bbsid");
                            model.IsView = (int)forumJson[i].SelectToken("isview");
                            model.IsClosed = (int)forumJson[i].SelectToken("isclosed");
                            model.ShowData = "Visible";
                            ForumCarSeriesDataSource.Add(model);
                        }


                        model = new ForumModel();
                        model.TotalCount = (int)json.SelectToken("result").SelectToken("rowcount");
                        model.ShowData = "Collapsed";
                        model.LoadMore = "点击加载更多......";
                        model.bbsType = (string)forumJson[0].SelectToken("bbstype");
                        model.bbsId = (int)forumJson[0].SelectToken("bbsid");
                        ForumCarSeriesDataSource.Add(model);
                    }
                    catch (Exception ex)
                    {

                    }

                    //注意
                    apiArgs.Result = ForumCarSeriesDataSource;


                }


                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }

            });


        }


    }
}
