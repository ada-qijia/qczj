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
    public class CarSeriesQuoteViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {

        public CarSeriesQuoteViewModel()
        {
            _carSeriesQuoteDataSource = new ObservableCollection<CarSeriesQuoteModel>();
        }
        /// <summary>
        /// 返回的数据集合
        /// </summary>
        private ObservableCollection<CarSeriesQuoteModel> _carSeriesQuoteDataSource;
        public ObservableCollection<CarSeriesQuoteModel> CarSeriesQuoteDataSource
        {
            get
            {
                return _carSeriesQuoteDataSource;
            }
            set
            {
                if (value != _carSeriesQuoteDataSource)
                {
                    OnPropertyChanging("CarSeriesQuoteDataSource");
                    _carSeriesQuoteDataSource = value;
                    OnPropertyChanged("CarSeriesQuoteDataSource");
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
        public event EventHandler<APIEventArgs<IEnumerable<CarSeriesQuoteModel>>> LoadDataCompleted;

        /// <summary>
        /// 请求Json格式的数据
        /// </summary>
        /// <param name="url">请求的地址url</param>
        public void LoadDataAysnc(string url, bool isSeriesSummary = false)
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
            url = url + "&a=" + Guid.NewGuid().ToString();
            Uri urlSource = new Uri(url, UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<IEnumerable<CarSeriesQuoteModel>> apiArgs = new APIEventArgs<IEnumerable<CarSeriesQuoteModel>>();
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {

                    //返回的json数据
                    JObject json = JObject.Parse(ee.Result);
                    CarSeriesQuoteDataSource = new ObservableCollection<CarSeriesQuoteModel>();
                    if (isSeriesSummary)
                    {
                        JArray carJson = (JArray)json.SelectToken("result").SelectToken("enginelist");

                        using (LocalDataContext ldc = new LocalDataContext())
                        {
                            //读取数据
                            CarSeriesQuoteModel model = null;
                            for (int i = 0; i < carJson.Count; i++)
                            {
                                JArray carItemJson = (JArray)carJson[i].SelectToken("speclist");
                                for (int j = 0; j < carItemJson.Count; j++)
                                {
                                    model = new CarSeriesQuoteModel();
                                    model.GroupName = (string)carJson[i].SelectToken("name");
                                    model.Id = (int)carItemJson[j].SelectToken("id");
                                    model.Name = (string)carItemJson[j].SelectToken("name");
                                    model.Price = (string)carItemJson[j].SelectToken("price");
                                    model.GearBox = (string)carItemJson[j].SelectToken("gearbox") ?? "";
                                    model.Structure = (string)carItemJson[j].SelectToken("structure");
                                    model.Transmission = (string)carItemJson[j].SelectToken("description");
                                    model.ParamIsShow = string.IsNullOrEmpty((string)carItemJson[j].SelectToken("paramisshow")) ? 0 : Convert.ToInt32((string)carItemJson[j].SelectToken("paramisshow"));
                                    model.COrder = j + 1;
                                    model.GroupOrder = i + 1;
                                    model.Compare = "";
                                    model.CompareText = "";
                                    ldc.carQuotes.InsertOnSubmit(model);
                                    CarSeriesQuoteDataSource.Add(model);
                                }
                            }
                            ldc.SubmitChanges();
                        }

                    }
                    else
                    {
                        JArray carJson = (JArray)json.SelectToken("result").SelectToken("list");

                        using (LocalDataContext ldc = new LocalDataContext())
                        {
                            //读取数据
                            CarSeriesQuoteModel model = null;

                            for (int i = 0; i < carJson.Count; i++)
                            {

                                JArray carItemJson = (JArray)carJson[i].SelectToken("speclist");
                                for (int j = 0; j < carItemJson.Count; j++)
                                {
                                    model = new CarSeriesQuoteModel();

                                    model.Id = (int)carItemJson[j].SelectToken("id");
                                    model.Name = (string)carItemJson[j].SelectToken("name");

                                    ldc.carQuotes.InsertOnSubmit(model);
                                    CarSeriesQuoteDataSource.Add(model);
                                }
                            }
                            ldc.SubmitChanges();
                        }
                    }


                }

                //返回结果集
                apiArgs.Result = CarSeriesQuoteDataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });
        }


    }
}
