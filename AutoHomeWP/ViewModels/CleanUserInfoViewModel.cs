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
   
    public class CleanUserInfoViewModel : INotifyPropertyChanged, INotifyPropertyChanging
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

        public CleanUserInfoViewModel()
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
       
        
        public void LoadDataAsync(string url)
        {
            if (wc == null)
            {
                wc = new WebClient();
            }

            if (!wc.IsBusy)
            {
                //wc.Encoding = DBCSEncoding.GetDBCSEncoding("gb2312");

                wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
                wc.Headers["Accept-Charset"] = "utf-8";
                wc.Headers["User-Agent"] = "WP\t8\tautohome\t1.7.0\tWP";

                Uri urlSource = new Uri(url + "&" + Guid.NewGuid().ToString(), UriKind.Absolute);
                wc.DownloadStringAsync(urlSource);
                APIEventArgs<IEnumerable<MyForumModel>> apiArgs = new APIEventArgs<IEnumerable<MyForumModel>>();
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
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
                            if (0 == (int)json.SelectToken("returncode"))
                            {
                                var setting = IsolatedStorageSettings.ApplicationSettings;
                                string key = "userInfo";
                                string cleanKey = "isclean";
                                if (setting.Contains(key))
                                {
                                   
                                    int returncode = (int)json.SelectToken("returncode");
                                    if (returncode == 0)
                                    {
                                        JToken resultJson = json.SelectToken("result");
                                        string pcPopClub = (string)resultJson.SelectToken("pcPopClub");
                                        MyForumModel model=(MyForumModel)setting[key];
                                        model.Authorization = pcPopClub;
                                        setting[key] = model;

                                        if (setting.Contains(cleanKey))
                                        {
                                            setting[cleanKey] = 1;
                                        }
                                        else
                                        {
                                            setting.Add(cleanKey,1);
                                        }
                                        setting.Save();
                                    }
                                }
                            }

                        }
                    }
                    catch
                    { }

                    if (LoadDataCompleted != null)
                    {
                        LoadDataCompleted(this, apiArgs);
                    }
                });

            }



        }


    }
}
