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
    public class CarSeriesConfigurationViewModel: INotifyPropertyChanged, INotifyPropertyChanging
    {

        public CarSeriesConfigurationViewModel()
        {
            _carSeriesConfigurationDataSource = new ObservableCollection<CarSeriesConfigurationModel>();
        }
        /// <summary>
        /// 返回的数据集合
        /// </summary>
        private ObservableCollection<CarSeriesConfigurationModel> _carSeriesConfigurationDataSource;
        public ObservableCollection<CarSeriesConfigurationModel> CarSeriesConfigurationDataSource
        {
            get
            {
                return _carSeriesConfigurationDataSource;
            }
            set
            {
                if (value != _carSeriesConfigurationDataSource)
                {
                    OnPropertyChanging("CarSeriesConfigurationDataSource");
                    _carSeriesConfigurationDataSource = value;
                    OnPropertyChanged("CarSeriesConfigurationDataSource");
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
        public event EventHandler<APIEventArgs<IEnumerable<CarSeriesConfigurationModel>>> LoadDataCompleted;

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
            wc.Headers["Accept-Charset"] = "utf-8";
           // wc.Encoding = new Gb2312Encoding();
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url, UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<IEnumerable<CarSeriesConfigurationModel>> apiArgs = new APIEventArgs<IEnumerable<CarSeriesConfigurationModel>>();
                try
                {
                    if (ee.Error != null)
                    {
                        apiArgs.Error = ee.Error;
                    }
                    else
                    {

                        //返回的json数据
                        JObject json = JObject.Parse(ee.Result);

                        JArray carJson = (JArray)json.SelectToken("result").SelectToken("paramitems");

                        //读取数据
                        CarSeriesConfigurationModel model = null;
                        for (int i = 0; i < carJson.Count; i++)
                        {

                            JArray carItemJson = (JArray)carJson[i].SelectToken("items");
                            for (int j = 0; j < carItemJson.Count; j++)
                            {
                                model = new CarSeriesConfigurationModel();
                                model.GroupName = (string)carJson[i].SelectToken("itemtype");

                                JArray valueJson = (JArray)carItemJson[j].SelectToken("modelexcessids");

                                model.Val = (string)valueJson[0].SelectToken("value");
                                model.Name = (string)carItemJson[j].SelectToken("name");
                                CarSeriesConfigurationDataSource.Add(model);
                            }

                        }


                    }
                }
                catch (Exception ex)
                {
                    
                }

                //返回结果集
                apiArgs.Result = CarSeriesConfigurationDataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });
        }
    

    }
}
