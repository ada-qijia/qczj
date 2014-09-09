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
using System.IO;
using System.Windows.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Model;
using System.Windows.Resources;

namespace ViewModels.Handler
{
    public class CommonHelper:INotifyPropertyChanged,INotifyPropertyChanging
    {
        public CommonHelper()
        {
            _newestDataSource = new ObservableCollection<NewsModel>();
        }

        /// <summary>
        /// 集合
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


        //用于接收返回的Json数据
        public static string jsonResult = string.Empty;

        public event EventHandler<APIEventArgs<IEnumerable<NewsModel>>> LoadDataCompleted;


        //加载次数   刷新按钮时  需要清零
        int loadTimes = 1;
        /// <summary>
        /// 请求Json格式的数据
        /// </summary>
        /// <param name="url">请求的地址url</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        public void LoadDataAysnc(string url, int pageIndex, int pageSize)
        {
           

            WebClient wc = new WebClient();
            if (wc.IsBusy != false)
            {
                wc.CancelAsync();
                return;
            }
            wc.Encoding = new Gb2312Encoding();
            
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url + "&pageIndex=" + pageIndex + "&pageSize=" + pageSize, UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<IEnumerable<NewsModel>> apiArgs = new APIEventArgs<IEnumerable<NewsModel>>();

                #region MyRegion
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {

                    //返回的json数据
                    JObject json = JObject.Parse(ee.Result);
                    JArray newestJson = (JArray)json.SelectToken("body").SelectToken("newsList");
                    //默认背景
                    BitmapImage image = new BitmapImage();
                    StreamResourceInfo info = Application.GetResourceStream(new Uri("/AutoWP7;component/Images/sa.jpg", UriKind.Relative));
                    image.SetSource(info.Stream);

                    if (loadTimes == 1)
                    {
                        JToken topJosn = json.SelectToken("body").SelectToken("headlineinfo");
                        //大图
                        NewsModel topModel = new NewsModel();
                        topModel.id = (int)topJosn.SelectToken("id");
                        topModel.title = (string)topJosn.SelectToken("title");
                        topModel.time = (string)topJosn.SelectToken("time");
                        topModel.type = (string)topJosn.SelectToken("type");
                        topModel.smallpic = (string)topJosn.SelectToken("smallpic");
                        topModel.replycount = (string)topJosn.SelectToken("replycount");
                        topModel.pagecount = (string)topJosn.SelectToken("pagecount");
                        topModel.indexdetail = (string)topJosn.SelectToken("indexdetail");
                        topModel.topImage = image;
                        ////将大图存入本地数据库
                        //using (LocalDataContext ldc = new LocalDataContext())
                        //{
                        //    ldc.newestModels.InsertOnSubmit(topModel);
                        //    ldc.SubmitChanges();
                        //}
                        loadTimes++;
                    }

                    //
                    NewsModel model = null;
                    for (int i = 0; i < newestJson.Count; i++)
                    {
                        model = new NewsModel();
                        model.id = (int)newestJson[i].SelectToken("id");
                        model.title = (string)newestJson[i].SelectToken("title");
                        model.time = (string)newestJson[i].SelectToken("time");
                        model.type = (string)newestJson[i].SelectToken("type");
                        model.smallpic = (string)newestJson[i].SelectToken("smallpic");
                        model.replycount = (string)newestJson[i].SelectToken("replycount");
                        model.pagecount = (string)newestJson[i].SelectToken("pagecount");
                        model.indexdetail = (string)newestJson[i].SelectToken("indexdetail");
                        model.bitmap = image;
                        ////将新闻列表存入本地数据库中
                        //using (LocalDataContext ldc = new LocalDataContext())
                        //{
                        //    ldc.newestModels.InsertOnSubmit(model);
                        //    ldc.SubmitChanges();                        
                        //}                 
                        newestDataSource.Add(model);
                    }

                    apiArgs.Result = newestDataSource;

           
                    newestDataSource[0].topImage = new StorageCachedImage(new Uri(newestDataSource[0].smallpic, UriKind.Absolute));
                    for (int i = 0; i < newestDataSource.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(newestDataSource[i].smallpic))
                        {
                            newestDataSource[i].bitmap = new StorageCachedImage(new Uri(newestDataSource[i].smallpic, UriKind.Absolute));
                        }
                    }
                  

                }
                
                #endregion
                //注意
                apiArgs.Result = newestDataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });


        }
  


        #region 用httpRequest方式向服务器请求数据
        /// <summary>
        /// 用httpRequest方式向服务器请求数据
        /// </summary>
        /// <param name="url">请求的url</param>
        /// <param name="pageIndex">请求的页码</param>
        /// <param name="pageSize">请求的页数</param>
        /// <returns>返回json格式的数据</returns>
        public static void GetJsonContentByHttpWebRequest(string url, string pageIndex, string pageSize)
        {

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST"; //httpRequest.Method = "GET"时禁用缓存策略失效
            httpRequest.Headers["Pragma"] = "no-cache";
            httpRequest.Headers["Cache-Control"] = "no-cache";
            httpRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";  //GET方式此项需注释掉，否则引发异常         
            httpRequest.BeginGetRequestStream(hr =>
        {
            //发送
            HttpWebRequest httpRequest1 = (HttpWebRequest)hr.AsyncState;
            System.IO.Stream postStream = httpRequest1.EndGetRequestStream(hr);

            //发送到服务器的数据
            string postData = "pageIndex=" + pageIndex + "&pageSize=" + pageSize;//HttpUtility.UrlEncode(xmlDoc.ToString());
            //编码
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
            postStream.Write(byteArray, 0, postData.Length);
            postStream.Close();  //必须要关闭数据流
            httpRequest1.BeginGetResponse(hr1 =>
            {
                // string JsonResult;
                //接收
                WebResponse httpResponse1 = null;
                try
                {
                    httpResponse1 = httpRequest1.EndGetResponse(hr1);
                    using (var reader = new System.IO.StreamReader(httpResponse1.GetResponseStream(), System.Text.Encoding.UTF8))
                    {
                        //返回的字符串
                        jsonResult = reader.ReadToEnd();
                        reader.Close();

                        Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            MessageBox.Show(jsonResult);

                        }));

                    }
                }
                catch (Exception ex)
                {
                    Deployment.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show(ex.Message);
                    }));
                }
                finally
                {
                    if (httpRequest != null) httpRequest.Abort();
                    if (httpRequest1 != null) httpRequest1.Abort();
                    if (httpResponse1 != null) httpResponse1.Close();
                }
            }, httpRequest1);
        }, httpRequest);
            //   return jsonResult;

        }
        #endregion


        //public static void DisplayResponse(string msg)
        //{
        //    jsonResult = msg;
        //}


    }
}




