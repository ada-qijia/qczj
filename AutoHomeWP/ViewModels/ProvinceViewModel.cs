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
    /// <summary>
    /// 省市viewmodel
    /// </summary>
    public class ProvinceViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public ProvinceViewModel()
        {
            _provinceDataSource = new ObservableCollection<ProvinceModel>();
            _cityDataSource = new ObservableCollection<ProvinceModel>();
        }

        /// <summary>
        /// 返回的数据集合
        /// </summary>
        private ObservableCollection<ProvinceModel> _provinceDataSource;
        public ObservableCollection<ProvinceModel> ProvinceDataSource
        {
            get
            {
                return _provinceDataSource;
            }
            set
            {
                if (value != _provinceDataSource)
                {
                    OnPropertyChanging("ProvinceDataSource");
                    _provinceDataSource = value;
                    OnPropertyChanged("ProvinceDataSource");
                }
            }
        }

        private ObservableCollection<ProvinceModel> _cityDataSource;
        public ObservableCollection<ProvinceModel> CityDataSource
        {
            get
            {
                return _cityDataSource;
            }
            set
            {
                if (value != _cityDataSource)
                {
                    OnPropertyChanging("CityDataSource");
                    _cityDataSource = value;
                    OnPropertyChanged("CityDataSource");
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
                    JArray carJson = (JArray)json.SelectToken("body").SelectToken("provinces");

                    ProvinceModel model = null;
                    for (int i = 0; i < carJson.Count; i++)
                    {
                        model = new ProvinceModel();
                        model.Father = 0;
                        model.Id = (int)carJson[i].SelectToken("Id");
                        model.Name = (string)carJson[i].SelectToken("Name");
                        model.FirstLetter = (string)carJson[i].SelectToken("FirstLetter");

                        ProvinceDataSource.Add(model);
                        JArray items = (JArray)carJson[i].SelectToken("Citys");
                        for (int k = 0; k < items.Count; k++)
                        {
                            model = new ProvinceModel();
                            model.Father = (int)carJson[i].SelectToken("Id");

                            model.Id = (int)items[k].SelectToken("Id");
                            model.FirstLetter = (string)items[k].SelectToken("FirstLetter");
                            model.Name = (string)items[k].SelectToken("Name");
                            CityDataSource.Add(model);
                        }
                    }
                }

                //注意
                apiArgs.Result = ProvinceDataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });


        }


    }
}
