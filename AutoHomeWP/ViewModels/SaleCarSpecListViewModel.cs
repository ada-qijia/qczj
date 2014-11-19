using System;
using System.Net;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Model;
using ViewModels.Handler;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ViewModels
{
    public class SaleCarSpecListViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {

        public SaleCarSpecListViewModel()
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
                    JObject json = JObject.Parse(ee.Result);
                    CarSeriesQuoteDataSource = new ObservableCollection<CarSeriesQuoteModel>();
                    JArray carJson = (JArray)json.SelectToken("result").SelectToken("list");

                    CarSeriesQuoteModel model = null;
                    for (int i = 0; i < carJson.Count; i++)
                    {
                        string groupName = (string)carJson[i].SelectToken("name");
                        JArray carItemJson = (JArray)carJson[i].SelectToken("speclist");
                        for (int j = 0; j < carItemJson.Count; j++)
                        {
                            model = new CarSeriesQuoteModel();
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

                            model.GroupName = groupName;
                            CarSeriesQuoteDataSource.Add(model);
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
