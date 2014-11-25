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
    public class CarSearchResultViewModel : INotifyPropertyChanged
    {
        public CarSearchResultViewModel()
        {
            PageIndex = 0;
            PageSize = 20;
        }

        private ObservableCollection<CarSearchResultSeriesItemModel> _DataSource = new ObservableCollection<CarSearchResultSeriesItemModel>();
        public ObservableCollection<CarSearchResultSeriesItemModel> DataSource
        {
            get
            {
                return _DataSource;
            }
            set
            {
                if (value != _DataSource)
                {
                    _DataSource = value;
                    OnPropertyChanged("newestDataSource");
                }
            }
        }

        CarSearchResultSeriesItemModel LoadMoreButtonItem = new CarSearchResultSeriesItemModel() { IsLoadMore = true };

        public int PageSize { get; set; }

        public int PageCount { get; set; }

        public int PageIndex { get; set; }

        public int RowCount { get; set; }

        public int TotalSpecCount { get; set; }

        public bool IsEndPage
        {
            get
            {
                return PageIndex >= PageCount;
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

        public event EventHandler<APIEventArgs<IEnumerable<CarSearchResultSeriesItemModel>>> LoadDataCompleted;

        public void LoadDataAysnc(string url, bool reload)
        {
            WebClient wc = new WebClient();
            if (wc.IsBusy != false)
            {
                wc.CancelAsync();
                return;
            }

            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            Uri urlSource = new Uri(url + "&" + Guid.NewGuid().ToString(), UriKind.Absolute);
            wc.DownloadStringCompleted += wc_DownloadStringCompleted;
            if (reload)
            {
                PageIndex = 1;
                PageCount = 1;
                DataSource.Clear();
            }
            wc.DownloadStringAsync(urlSource);
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            APIEventArgs<IEnumerable<CarSearchResultSeriesItemModel>> apiArgs = new APIEventArgs<IEnumerable<CarSearchResultSeriesItemModel>>();
            if (e.Error != null)
            {
                apiArgs.Error = e.Error;
            }
            else
            {
                try
                {
                    TryRemoveMoreButton();

                    JObject json = JObject.Parse(e.Result);
                    JArray ja = (JArray)json.SelectToken("result").SelectToken("seriesitems");

                    CarSearchResultSeriesItemModel model = null;
                    for (int i = 0; i < ja.Count; i++)
                    {
                        model = new CarSearchResultSeriesItemModel();
                        model.id = (int)ja[i].SelectToken("id");
                        model.name = (string)ja[i].SelectToken("name");
                        model.img = (string)ja[i].SelectToken("img");
                        model.level = (string)ja[i].SelectToken("level");
                        model.price = (string)ja[i].SelectToken("price");
                        model.count = (int)ja[i].SelectToken("count");

                        //spec item groups
                        var ja2 = (JArray)ja[i].SelectToken("specitemgroups");
                        foreach (var jo2 in ja2)
                        {
                            CarSearchResultSpecItemGroupModel group = new CarSearchResultSpecItemGroupModel();
                            group.groupname = (string)ja2.SelectToken("groupname");
                            
                            //spec items
                            var ja3 = (JArray)jo2.SelectToken("specitems");
                            foreach (var jo3 in ja3)
                            {
                                CarSearchResultSpecItemModel spec = new CarSearchResultSpecItemModel();
                                spec.id = (int)jo3.SelectToken("id");
                                spec.name = (string)jo3.SelectToken("name");
                                spec.price = (string)jo3.SelectToken("price");
                                spec.description = (string)jo3.SelectToken("description");
                                group.specitems.Add(spec);
                            }
                            model.specitemgroups.Add(group);
                        }
                        DataSource.Add(model);
                    }

                    this.PageCount = (int)json.SelectToken("result").SelectToken("pagecount");
                    this.PageIndex = (int)json.SelectToken("result").SelectToken("pageindex");
                    this.RowCount = (int)json.SelectToken("result").SelectToken("rowcount");
                    this.TotalSpecCount = (int)json.SelectToken("result").SelectToken("totalspeccount");

                    EnsureMoreButton();
                }
                catch (Exception ex)
                {

                }
            }

            apiArgs.Result = DataSource;

            if (LoadDataCompleted != null)
            {
                LoadDataCompleted(this, apiArgs);
            }
        }

        private void EnsureMoreButton()
        {
            if (!this.IsEndPage && !DataSource.Contains(LoadMoreButtonItem))
            {
                DataSource.Add(LoadMoreButtonItem);
            }
        }

        private void TryRemoveMoreButton()
        {
            if (DataSource.Contains(LoadMoreButtonItem))
            {
                DataSource.Remove(LoadMoreButtonItem);
            }
        }

    }
}
