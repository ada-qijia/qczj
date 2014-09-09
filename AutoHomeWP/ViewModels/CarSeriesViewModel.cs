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
using ViewModels.Handler;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
using Model;

namespace ViewModels
{
    public class CarSeriesViewModel: INotifyPropertyChanged, INotifyPropertyChanging
    {

        public CarSeriesViewModel()
        {
            _carSeriesDataSource = new ObservableCollection<CarSeriesModel>();
        }
        /// <summary>
        /// 返回的数据集合
        /// </summary>
        private ObservableCollection<CarSeriesModel> _carSeriesDataSource;
        public ObservableCollection<CarSeriesModel> CarSeriesDataSource
        {
            get
            {
                return _carSeriesDataSource;
            }
            set
            {
                if (value != _carSeriesDataSource)
                {
                    OnPropertyChanging("CarSeriesDataSource");
                    _carSeriesDataSource = value;
                    OnPropertyChanged("CarSeriesDataSource");
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
        public event EventHandler<APIEventArgs<IEnumerable<CarSeriesModel>>> LoadDataCompleted;

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

          //  wc.Encoding = new Gb2312Encoding();
            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url+"&a="+Guid.NewGuid().ToString(), UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<IEnumerable<CarSeriesModel>> apiArgs = new APIEventArgs<IEnumerable<CarSeriesModel>>();
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {

                    //返回的json数据
                    JObject json = JObject.Parse(ee.Result);

                    JArray carSeriedJson = (JArray)json.SelectToken("result").SelectToken("fctlist");

                    using (LocalDataContext ldc = new LocalDataContext())
                    {
                        
                        CarSeriesModel model = null;
                        for (int i = 0; i < carSeriedJson.Count; i++)
                        {
                            JArray carItemJson = (JArray)carSeriedJson[i].SelectToken("serieslist");

                            for (int j = 0; j < carItemJson.Count; j++)
                            {
                                model = new CarSeriesModel();
                                model.FctName = (string)carSeriedJson[i].SelectToken("name");
                                model.Id = (int)carItemJson[j].SelectToken("id");
                                model.Name = (string)carItemJson[j].SelectToken("name");
                                model.ImgUrl = (string)carItemJson[j].SelectToken("imgurl");
                                model.PriceBetween = (string)carItemJson[j].SelectToken("price");
                                ldc.carSeries.InsertOnSubmit(model);
                                CarSeriesDataSource.Add(model);
                            }
                            ldc.SubmitChanges();
                        }
                    }
                 
                    //加载图片
                    for (int i = 0; i < CarSeriesDataSource.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(CarSeriesDataSource[i].ImgUrl))
                        {
                            CarSeriesDataSource[i].bitmap = new StorageCachedImage(new Uri(CarSeriesDataSource[i].ImgUrl, UriKind.Absolute));
                        }
                    }

                }

                //注意
                apiArgs.Result = CarSeriesDataSource;

                if (LoadDataCompleted != null)
                {
                   LoadDataCompleted(this, apiArgs);
                }
            });
        }
   

    }
}
