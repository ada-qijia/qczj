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
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace ViewModels
{
    public class CarBrandViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {

        public CarBrandViewModel()
        {
            _carBrandDataSource = new ObservableCollection<CarBrandModel>();
        }
        /// <summary>
        /// 返回的数据集合
        /// </summary>
        private ObservableCollection<CarBrandModel> _carBrandDataSource;
        public ObservableCollection<CarBrandModel> CarBrandDataSource
        {
            get
            {
                return _carBrandDataSource;
            }
            set
            {
                if (value != _carBrandDataSource)
                {
                    OnPropertyChanging("CarBrandDataSource");
                    _carBrandDataSource = value;
                    OnPropertyChanged("CarBrandDataSource");
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
        public event EventHandler<APIEventArgs<IEnumerable<CarBrandModel>>> LoadDataCompleted;

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
            Uri urlSource = new Uri(url, UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<IEnumerable<CarBrandModel>> apiArgs = new APIEventArgs<IEnumerable<CarBrandModel>>();
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {

                    //返回的json数据
                    JObject json = JObject.Parse(ee.Result);

                    JArray carBrandJson = (JArray)json.SelectToken("result").SelectToken("brandlist");


                    //using (LocalDataContext ldc = new LocalDataContext())
                    //{
                        CarBrandModel model = null;
                        for (int i = 0; i < carBrandJson.Count; i++)
                        {

                            JArray carItemJson = (JArray)carBrandJson[i].SelectToken("list");
                            for (int j = 0; j < carItemJson.Count; j++)
                            {
                                model = new CarBrandModel();
                                model.Letter = (string)carBrandJson[i].SelectToken("letter");
                                model.Id = (int)carItemJson[j].SelectToken("id");
                                model.Name = (string)carItemJson[j].SelectToken("name");
                                model.ImgUrl = (string)carItemJson[j].SelectToken("imgurl");
                                model.CurrentTime = DateTime.Now;
                                //ldc.carBrandModels.InsertOnSubmit(model);
                                CarBrandDataSource.Add(model);
                            }

                        }
                    //    //存入本地数据库
                    //    ldc.SubmitChanges();
                    //}

                  //  //加载图片
                    //for (int i = 1; i < CarBrandDataSource.Count; i++)
                    //{
                    //    if (!string.IsNullOrEmpty(CarBrandDataSource[i].ImgUrl))
                    //    {
                    //        CarBrandDataSource[i].bitmap = new StorageCachedImage(new Uri(CarBrandDataSource[i].ImgUrl, UriKind.Absolute),1);
                    //    }
                    //}

                }


                //注意
                apiArgs.Result = CarBrandDataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });
        }

    }
}
