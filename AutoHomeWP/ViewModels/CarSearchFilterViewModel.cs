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
    public class CarSearchFilterViewModel : BindableBase
    {
        public CarSearchFilterViewModel()
        {
            _dataSource = new List<CarSearchFilterGroupModel>();
        }

        private List<CarSearchFilterGroupModel> _dataSource;
        public List<CarSearchFilterGroupModel> DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                if (value != _dataSource)
                {
                    _dataSource = value;
                    OnPropertyChanged("DataSource");
                }
            }
        }

        public event EventHandler<APIEventArgs<List<CarSearchFilterGroupModel>>> LoadDataCompleted;

        WebClient wc = null;
        public void LoadDataAysnc(string url)
        {
            if (wc == null)
            {
                wc = new WebClient();
            }
            if (wc.IsBusy == true)
            {
                wc.CancelAsync();
            }

            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url, UriKind.Absolute);
            wc.DownloadStringAsync(urlSource);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<List<CarSearchFilterGroupModel>> apiArgs = new APIEventArgs<List<CarSearchFilterGroupModel>>();
                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {
                    JObject json = JObject.Parse(ee.Result);
                    JArray carJson = (JArray)json.SelectToken("result").SelectToken("metalist");

                    CarSearchFilterGroupModel group = null;

                    for (int i = 0; i < carJson.Count; i++)
                    {
                        string key = (string)carJson[i].SelectToken("key");

                        if (key == "findorder")
                        {
                            continue;
                        }

                        group = new CarSearchFilterGroupModel();
                        group.key = key;

                        JArray items = (JArray)carJson[i].SelectToken("list");
                        for (int k = 0; k < items.Count; k++)
                        {
                            CarSearchFilterItemModel filter = new CarSearchFilterItemModel();
                            filter.name = (string)items[k].SelectToken("name");
                            filter.value = (string)items[k].SelectToken("value");
                            group.filters.Add(filter);
                        }
                        DataSource.Add(group);
                    }
                }

                apiArgs.Result = DataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }

            });
        }

    }
}
