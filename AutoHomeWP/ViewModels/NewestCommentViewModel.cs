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
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ViewModels
{

    public class NewestCommentViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        //属性更改时的事件
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

        public NewestCommentViewModel()
        {
            _newestCommentDataSource = new ObservableCollection<CommentModel>();
        }

        /// <summary>
        /// 最新评论集合
        /// </summary>
        public ObservableCollection<CommentModel> _newestCommentDataSource;
        public ObservableCollection<CommentModel> newestCommentDataSource
        {
            get
            {
                return _newestCommentDataSource;
            }
            set
            {
                if (value != _newestCommentDataSource)
                {
                    OnPropertyChanging("newestCommentDataSource");
                    _newestCommentDataSource = value;
                    OnPropertyChanged("newestCommentDataSource");
                }
            }
        }


        //事件通知
        public event EventHandler<APIEventArgs<IEnumerable<CommentModel>>> LoadDataCompleted;


        WebClient wc = null;
        /// <summary>
        /// 去服务器获取数据
        /// </summary>
        /// <param name="newsid">新闻id</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        public void LoadDataAysnc(string url)
        {
            if (wc == null)
            {
                wc = new WebClient();
            }
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            // wc.Encoding = new Gb2312Encoding();
            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers["User-Agent"] = "WP\t8\tautohome\t1.5.0\tWP";
            Uri urlSource = new Uri(url +  "&" + Guid.NewGuid().ToString(), UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {

                APIEventArgs<IEnumerable<CommentModel>> apiArgs = new APIEventArgs<IEnumerable<CommentModel>>();
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {
                    try
                    {
                        //清空数据
                        newestCommentDataSource.Clear();

                        //返回的json数据
                        JObject json = JObject.Parse(ee.Result);
                        if (json != null)
                        {
                            JArray commentJson = (JArray)json.SelectToken("result").SelectToken("list");

                            CommentModel model = null;
                            for (int i = 0; i < commentJson.Count; i++)
                            {
                                model = new CommentModel();
                                model.id = ((int)commentJson[i].SelectToken("id")).ToString();
                                model.floor = ((int)commentJson[i].SelectToken("floor")).ToString();
                                model.name = (string)commentJson[i].SelectToken("name");
                                model.isadd = ((bool )commentJson[i].SelectToken("isadd")).ToString();
                                model.time = (string)commentJson[i].SelectToken("time");
                                model.content = (string)commentJson[i].SelectToken("content");
                                model.SourceId = ((int)commentJson[i].SelectToken("sourcenameid")).ToString();
                                model.sourcename = (string)commentJson[i].SelectToken("sourcename");
                                model.sourcecontent = (string)commentJson[i].SelectToken("sourcecontent");
                                model.ShowData = "Visible";
                                newestCommentDataSource.Add(model);
                            }

                            if (commentJson.Count >= 20)
                            {
                                model = new CommentModel();
                                model.ShowData = "Collapsed";
                                model.LoadMore = "点击加载更多...";
                                model.Total = (int)json.SelectToken("result").SelectToken("rowcount");
                                newestCommentDataSource.Add(model);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    //注意
                    apiArgs.Result = newestCommentDataSource;


                }

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }


            });

        }


    }
}
