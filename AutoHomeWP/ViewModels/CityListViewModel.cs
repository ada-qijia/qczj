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
    public class CityListViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public CityListViewModel()
        {
            _cityListDataSource = new ObservableCollection<ProvinceModel>();
        }

        /// <summary>
        /// 返回的数据集合
        /// </summary>
        private ObservableCollection<ProvinceModel> _cityListDataSource;
        public ObservableCollection<ProvinceModel> CityListDataSource
        {
            get
            {
                return _cityListDataSource;
            }
            set
            {
                if (value != _cityListDataSource)
                {
                    OnPropertyChanging("CityListDataSource");
                    _cityListDataSource = value;
                    OnPropertyChanged("CityListDataSource");
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
        public event EventHandler<APIEventArgs<IEnumerable<ProvinceModel>>> LoadDataCompleted;

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
            Uri urlSource = new Uri(url, UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<IEnumerable<ProvinceModel>> apiArgs = new APIEventArgs<IEnumerable<ProvinceModel>>();
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

                            JArray dataJson = (JArray)json.SelectToken("result").SelectToken("provinces");
                            if (dataJson != null)
                            {
                                using (LocalDataContext ldc = new LocalDataContext())
                                {
                                    ProvinceModel model = null;
                                    for (int i = 0; i < dataJson.Count; i++)
                                    {

                                        JArray items = (JArray)dataJson[i].SelectToken("citys");
                                        if (items != null)
                                        {
                                            for (int k = 0; k < items.Count; k++)
                                            {
                                                model = new ProvinceModel();
                                                model.Id = (int)items[k].SelectToken("id");
                                                model.FirstLetter = (string)items[k].SelectToken("firstletter");
                                                model.FatherName = (string)dataJson[i].SelectToken("name");
                                                model.Name = (string)items[k].SelectToken("name");
                                                //放入本地数据库
                                                ldc.provinces.InsertOnSubmit(model);
                                                CityListDataSource.Add(model);
                                            }
                                        }
                                    }
                                    //提交到本地数据库
                                    ldc.SubmitChanges();

                                }
                            }
                        
                    }

                }

                //注意
                apiArgs.Result = CityListDataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });


        }


    }
}

