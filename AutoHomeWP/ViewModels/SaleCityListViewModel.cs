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
    public class SaleCityListViewModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        public SaleCityListViewModel()
        {
            _provinceDataSource = new ObservableCollection<ProvinceModel>();
            _cityDataSource = new ObservableCollection<ProvinceModel>();
        }

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

        public event PropertyChangingEventHandler PropertyChanging;
        public void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event EventHandler<APIEventArgs<IEnumerable<ProvinceModel>>> LoadDataCompleted;

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
                    JObject json = JObject.Parse(ee.Result);
                    JArray carJson = (JArray)json.SelectToken("result").SelectToken("provinces");

                    ProvinceModel model = null;

                    //全国
                    var allProvinces = new ProvinceModel() { Id = 0, Name = "全国", FirstLetter="#" };
                    ProvinceDataSource.Add(allProvinces);

                    for (int i = 0; i < carJson.Count; i++)
                    {
                        int provinceID = 0;
                        model = new ProvinceModel();
                        model.Father = 0;
                        model.Id = provinceID = (int)carJson[i].SelectToken("id");
                        model.Name = (string)carJson[i].SelectToken("name");
                        model.FirstLetter = (string)carJson[i].SelectToken("firstletter");
                        ProvinceDataSource.Add(model);


                        //全省
                        var allCities = new ProvinceModel() { Id = 0, Name = "全省", FirstLetter = "#", Father = provinceID };
                        CityDataSource.Add(allCities);

                        JArray items = (JArray)carJson[i].SelectToken("citys");
                        for (int k = 0; k < items.Count; k++)
                        {
                            model = new ProvinceModel();
                            model.Father = provinceID;// (int)carJson[i].SelectToken("id");

                            model.Id = (int)items[k].SelectToken("id");
                            model.FirstLetter = (string)items[k].SelectToken("firstletter");
                            model.Name = (string)items[k].SelectToken("name");
                            CityDataSource.Add(model);
                        }
                    }
                }

                apiArgs.Result = ProvinceDataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });


        }


    }
}
