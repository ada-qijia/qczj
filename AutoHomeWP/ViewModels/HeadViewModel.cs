using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using Model;
using Newtonsoft.Json.Linq;
using ViewModels.Handler;

namespace ViewModels
{
    public class HeadViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public HeadViewModel()
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

        private ObservableCollection<CacheVarModel> cacheDataSource = new ObservableCollection<CacheVarModel>();

        /// <summary>
        /// 请求Json格式的数据
        /// </summary>
        /// <param name="url">请求的地址url</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="isFirstLoad">是否是第一次加载</param>
        public void LoadDataAysnc(string url, int pageIndex, bool isFirstLoad)
        {

            WebClient wc = new WebClient();

            if (wc.IsBusy == false)
            {

                // wc.Encoding = new Gb2312Encoding();

                wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
                wc.Headers["Accept-Charset"] = "utf-8";
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
                        // try
                        // {

                        #region 封装数据

                        //清空数据
                        //  newestDataSource.Clear();

                        NewsModel model = null;
                        using (LocalDataContext ldc = new LocalDataContext())
                        {
                            //返回的json数据
                            JObject json = JObject.Parse(ee.Result);
                            if (json != null)
                            {
                                int headLineFlag = 0;
                                if (isFirstLoad)
                                {
                                    JArray topJosn = (JArray)json.SelectToken("result").SelectToken("focusimg");
                                    if (topJosn != null)
                                    {
                                        //焦点图
                                        model = new NewsModel();
                                        model.id = (int)topJosn[0].SelectToken("id");
                                        model.imgurl = (string)topJosn[0].SelectToken("imgurl");
                                        model.showData = "Collapsed";
                                        model.pageIndex = topJosn[0].SelectToken("pageindex").ToString();
                                        model.mediatype = (int)topJosn[0].SelectToken("mediatype");
                                        newestDataSource.Add(model);
                                        ldc.newestModels.InsertOnSubmit(model);
                                    }

                                    //头条资讯
                                    JToken headJosn = json.SelectToken("result").SelectToken("headlineinfo");
                                    if (headJosn != null)
                                    {
                                        model = new NewsModel();
                                        model.id = (int)headJosn.SelectToken("id");
                                        model.title = (string)headJosn.SelectToken("title");
                                        model.time = (string)headJosn.SelectToken("time");
                                        model.type = "头条";
                                        //(string)headJosn.SelectToken("type");
                                        model.smallpic = (string)headJosn.SelectToken("smallpic");
                                        model.replycount = headJosn.SelectToken("replycount").ToString();
                                        model.pagecount = headJosn.SelectToken("pagecount").ToString();
                                        model.indexdetail = (string)headJosn.SelectToken("indexdetail");
                                        model.pageIndex = headJosn.SelectToken("jumppage").ToString();
                                        model.mediatype = (int)headJosn.SelectToken("mediatype");

                                        model.showData = "Visible";

                                        headLineFlag = model.id;

                                        newestDataSource.Add(model);
                                        ldc.newestModels.InsertOnSubmit(model);
                                    }
                                }


                                if (headLineFlag == 0 && pageIndex == 2)
                                {
                                    //访问第二页时，记录下头条的ID
                                    JToken headJosn = json.SelectToken("result").SelectToken("headlineinfo");
                                    if (headJosn.ToString() != "{}")
                                    {
                                        headLineFlag = (int)headJosn.SelectToken("id");
                                    }
                                }

                                //新闻列表
                                JArray newestJson = (JArray)json.SelectToken("result").SelectToken("newslist");
                                if (newestJson != null)
                                {
                                    for (int i = 0; i < newestJson.Count; i++)
                                    {
                                        //头条可能存在于第一页或第二页，需要根据ID，过滤掉新闻列表中ID相同的新闻
                                        if ((int)newestJson[i].SelectToken("id") != headLineFlag)
                                        {
                                            model = new NewsModel();
                                            model.id = (int)newestJson[i].SelectToken("id");
                                            model.title = (string)newestJson[i].SelectToken("title");
                                            model.time = (string)newestJson[i].SelectToken("time");
                                            string newsType = (string)newestJson[i].SelectToken("type");
                                            model.type = newsType.Equals("说客") ? "说客" : "";
                                            model.smallpic = (string)newestJson[i].SelectToken("smallpic");
                                            model.replycount = newestJson[i].SelectToken("replycount").ToString();
                                            model.pagecount = newestJson[i].SelectToken("pagecount").ToString();
                                            model.pageIndex = newestJson[i].SelectToken("jumppage").ToString();
                                            model.indexdetail = (string)newestJson[i].SelectToken("indexdetail");
                                            model.showData = "Visible";
                                            model.lasttimeandid = (string)newestJson[i].SelectToken("intacttime");
                                            model.mediatype = (int)newestJson[i].SelectToken("mediatype");

                                            newestDataSource.Add(model);

                                        }
                                        if (isFirstLoad)
                                        {
                                            try
                                            {
                                                ldc.newestModels.InsertOnSubmit(model);
                                            }
                                            catch
                                            { }
                                        }
                                    }

                                    model = new NewsModel();
                                    model.loadMore = "点击加载更多...";
                                    model.showData = "Collapsed";

                                    newestDataSource.Add(model);
                                    if (isFirstLoad)
                                    {
                                        ldc.newestModels.InsertOnSubmit(model);
                                        //存入数据库
                                        ldc.SubmitChanges();
                                    }

                                }
                            }
                        }

                        ////大图加载
                        //newestDataSource[0].topImage = new StorageCachedImage(new Uri(newestDataSource[0].imgurl, UriKind.Absolute));

                        ////小图加载
                        //for (int i = 0; i < newestDataSource.Count; i++)
                        //{
                        //    if (!string.IsNullOrEmpty(newestDataSource[i].smallpic))
                        //    {
                        //        newestDataSource[i].bitmap = new StorageCachedImage(new Uri(newestDataSource[i].smallpic, UriKind.Absolute));
                        //    }
                        //}

                        #endregion

                        //  }

                        //  catch (Exception ex)
                        // {

                        // }
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
}
