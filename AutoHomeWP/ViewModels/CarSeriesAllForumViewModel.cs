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
    public class CarSeriesAllForumViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public CarSeriesAllForumViewModel()
        {
            _carSeriesAllForumDataSource = new ObservableCollection<carSeriesAllForumModel>();
        }

        /// <summary>
        /// 返回的数据集合
        /// </summary>
        private ObservableCollection<carSeriesAllForumModel> _carSeriesAllForumDataSource;
        public ObservableCollection<carSeriesAllForumModel> CarSeriesAllForumDataSource
        {
            get
            {
                return _carSeriesAllForumDataSource;
            }
            set
            {
                if (value != _carSeriesAllForumDataSource)
                {
                    OnPropertyChanging("CarSeriesAllForumDataSource");
                    _carSeriesAllForumDataSource = value;
                    OnPropertyChanged("CarSeriesAllForumDataSource");
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
        public event EventHandler<APIEventArgs<IEnumerable<carSeriesAllForumModel>>> LoadDataCompleted;

        /// <summary>
        /// 请求Json格式的数据
        /// </summary>
        /// <param name="url">请求的地址url</param>
        public void LoadDataAysnc(string url)
        {
            WebClient wc = new WebClient();
            if (wc.IsBusy != false)
            {
                wc.CancelAsync();
                return;
            }
            
           // wc.Encoding = new Gb2312Encoding();
            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url,UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<IEnumerable<carSeriesAllForumModel>> apiArgs = new APIEventArgs<IEnumerable<carSeriesAllForumModel>>();
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {

                    //返回的json数据
                    JObject json = JObject.Parse(ee.Result);
                    JArray forumJson = (JArray)json.SelectToken("result").SelectToken("list");

                    using (LocalDataContext ldc = new LocalDataContext())
                    {
                        carSeriesAllForumModel model = null;
                        for (int i = 0; i < forumJson.Count; i++)
                        {
                            JArray item = (JArray)forumJson[i].SelectToken("seriesclub");
                            for (int k = 0; k < item.Count; k++)
                            {
                                model = new carSeriesAllForumModel();
                                model.brandName = (string)forumJson[i].SelectToken("brandname");
                                model.letter = (string)forumJson[i].SelectToken("letter");
                                model.bbsId = (int)item[k].SelectToken("bbsid");
                                model.bbsName = (string)item[k].SelectToken("bbsname");
                                model.bbsType = (string)item[k].SelectToken("bbstype");
                                model.CurrentTime = DateTime.Now;
                                CarSeriesAllForumDataSource.Add(model);

                                ldc.carSeriesForums.InsertOnSubmit(model);
                            }

                        }
                        //存入数据库
                        ldc.SubmitChanges();
                    }
                    
                }

                //注意
                apiArgs.Result = CarSeriesAllForumDataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });


        }
    }
}
