using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Net;
using ViewModels.Handler;

namespace ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged, INotifyPropertyChanging
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

        public LoginViewModel()
        {
            _userInfoDataSource = new ObservableCollection<MyForumModel>();
        }
        /// <summary>
        /// 用户登录信息集合
        /// </summary>
        private ObservableCollection<MyForumModel> _userInfoDataSource;
        public ObservableCollection<MyForumModel> UserInfoDataSource
        {
            get
            {
                return _userInfoDataSource;
            }
            set
            {
                if (value != _userInfoDataSource)
                {
                    OnPropertyChanging("UserInfoDataSource");
                    _userInfoDataSource = value;
                    OnPropertyChanged("UserInfoDataSource");
                }
            }
        }



        //事件通知
        public event EventHandler<APIEventArgs<IEnumerable<MyForumModel>>> LoadDataCompleted;

        WebClient wc = null;
        //--------------
        //private void StreamCallback(IAsyncResult result)
        //{
        //    HttpWebRequest httpWebRequest = (HttpWebRequest)result.AsyncState;

        //    using (Stream stream = httpWebRequest.EndGetRequestStream(result))
        //    {
        //        byte[] entryBytes = Encoding.GetEncoding("gb2312").GetBytes(data);
        //        stream.Write(entryBytes, 0, entryBytes.Length);
        //    }

        //    //这里就和上面GET方法获得回复回调一样，用 private void ResponseCallback(IAsyncResult result) 这个方法。
        //    httpWebRequest.BeginGetResponse(ResponseCallback, httpWebRequest);
        //}
        //private void ResponseCallback(IAsyncResult result)
        //{
        //    string response = null;
        //    Stream stream = null;

        //    try
        //    {
        //        //获取异步操作返回的的信息 
        //        HttpWebRequest httpWebRequest = (HttpWebRequest)result.AsyncState;
        //        //结束对 Internet 资源的异步请求
        //        WebResponse webResponse = httpWebRequest.EndGetResponse(result);
        //        stream = webResponse.GetResponseStream();
        //        using (StreamReader sr = new StreamReader(stream))
        //        {
        //            response = sr.ReadToEnd();
        //        }
        //    }
        //    catch { }
        //    finally
        //    {
        //        if (stream != null)
        //        {
        //            stream.Close();
        //        }
        //    }

        //    //这里不为空代表获得回复了
        //    if (response != null)
        //    {
        //        //因为HttpWebRequest是异步，不在UI线程上。所以要改变UI线程上的控件属性就要用Dispatcher.BeginInvoke()。
        //        //Dispatcher.BeginInvoke(() =>
        //        //{
        //        //    textBlock.Text = response;
        //        //});
        //    }
        //}
        ////------------------------
        string data = string.Empty;
        public void LoadDataAsync(string url, string postData)
        {
            if (wc == null)
            {
                wc = new WebClient();
            }

            if (!wc.IsBusy)
            {
                wc.Encoding = DBCSEncoding.GetDBCSEncoding("gb2312");
                // wc.Headers["Accept-Charset"] = "utf-8";
                //wc.Headers["Accept-Charset"] = "RequestCodeType";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                data = "Authorization=" + postData;

                wc.UploadStringAsync(new Uri(url), "POST", data);
                APIEventArgs<IEnumerable<MyForumModel>> apiArgs = new APIEventArgs<IEnumerable<MyForumModel>>();
                wc.UploadStringCompleted += new UploadStringCompletedEventHandler((ss, ee) =>
                {

                    try
                    {

                        if (ee.Error != null)
                        {
                            apiArgs.Error = ee.Error;
                        }
                        else
                        {
                            JObject json = JObject.Parse(ee.Result);
                            //返回的json数据
                            JToken resultJson = json.SelectToken("userInfo");
                            MyForumModel model = null;
                            model = new MyForumModel();
                            model.Success = (int)json.SelectToken("sucess");
                            model.Message = (string)json.SelectToken("message");
                            if ((int)json.SelectToken("sucess") == 1)
                            {

                                #region 登录成功
                                //用户信息
                                model.UserPic = (string)resultJson.SelectToken("userPic");
                                model.UserName = (string)resultJson.SelectToken("userName");
                                model.Grade = (string)resultJson.SelectToken("grade");
                                model.WeiWang = (string)resultJson.SelectToken("weiWang");
                                model.Authorization = (string)json.SelectToken("authorization");
                                model.UserID = (int)resultJson.SelectToken("userId");
                                UserInfoDataSource.Add(model);


                                //存入文件
                                var setting = IsolatedStorageSettings.ApplicationSettings;
                                string key = "userInfo";

                                if (setting.Contains(key))
                                {
                                    setting[key] = model;
                                }
                                else
                                {
                                    setting.Add(key, model);
                                }
                                setting.Save();

                                //将用户的论坛数据存入数据库
                                using (LocalDataContext ldc = new LocalDataContext())
                                {
                                    //车系列表
                                    JArray items = (JArray)resultJson.SelectToken("clubList");
                                    for (int i = 0; i < items.Count; i++)
                                    {

                                        model = new MyForumModel();
                                        model.bbsType = (string)items[i].SelectToken("bbsType");
                                        model.bbsName = (string)items[i].SelectToken("bbsName");
                                        model.bbsId = Convert.ToInt32(items[i].SelectToken("bbsId").ToString());
                                        ldc.GetTable<MyForumModel>().InsertOnSubmit(model);
                                    }

                                    //注册车系
                                    //JToken items2 = resultJson.SelectToken("regCar");
                                    //model = new UserInfoModel();
                                    //model.bbsType = (string)items2.SelectToken("bbsType");
                                    //model.bbsName = (string)items2.SelectToken("bbsName");
                                    //model.bbsId = (int)items2.SelectToken("bbsId");
                                    //UserInfoDataSource.Add(model);
                                    //ldc.userInfoforums.InsertOnSubmit(model);

                                    //存入到数据库
                                    ldc.SubmitChanges();
                                }

                                #endregion
                            }
                            else //登录失败 
                            {
                                UserInfoDataSource.Add(model);
                            }


                            //注意
                            apiArgs.Result = UserInfoDataSource;

                        }
                    }
                    catch 
                    {}

                    if (LoadDataCompleted != null)
                    {
                        LoadDataCompleted(this, apiArgs);
                    }
                });

            }



        }
    }


}
