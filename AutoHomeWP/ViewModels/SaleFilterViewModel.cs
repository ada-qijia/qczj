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
    public class SaleFilterViewModel : BindableBase
    {
        public SaleFilterViewModel()
        {
        }

        private SaleFilterModel _dataSource;
        public SaleFilterModel DataSource
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

        public event EventHandler<APIEventArgs<SaleFilterModel>> LoadDataCompleted;

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
                APIEventArgs<SaleFilterModel> apiArgs = new APIEventArgs<SaleFilterModel>();
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
                        JToken result = json.SelectToken("result");

                        SaleFilterModel model = null;
                        if (result != null)
                        {
                            model = new SaleFilterModel();

                            var ja = (JArray)result.SelectToken("metalist");
                            if (ja != null && ja.Count > 0)
                            {
                                var filterGroups = new Dictionary<string, SaleFilterGroup>();
                                foreach (var filterGroup in ja)
                                {
                                    SaleFilterGroup group = new SaleFilterGroup();
                                    group.Key = (string)filterGroup.SelectToken("key");
                                    var ja2 = (JArray)filterGroup.SelectToken("list");
                                    if (ja2 != null && ja2.Count > 0)
                                    {
                                        var list = new List<SaleFilterItemModel>();
                                        foreach (var item in ja2)
                                        {
                                            SaleFilterItemModel filter = new SaleFilterItemModel();
                                            filter.name = (string)item.SelectToken("name");
                                            filter.value = (string)item.SelectToken("value");
                                            filter.type = (string)item.SelectToken("type");
                                            list.Add(filter);
                                        }
                                        group.Filters = list;
                                    }
                                    filterGroups.Add(group.Key, group);
                                }
                                model.FilterGroups = filterGroups;
                            }

                            DataSource = model;
                        }
                    }
                }
                //注意
                apiArgs.Result = DataSource;

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }

            });
        }

    }
}
