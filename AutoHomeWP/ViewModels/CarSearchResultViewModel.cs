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

                        ////dealer
                        //var jo = ja[i].SelectToken("dealer");
                        //if (jo != null)
                        //{
                        //    SaleDealer dealer = new SaleDealer();
                        //    dealer.id = (int)jo.SelectToken("id");
                        //    dealer.name = (string)jo.SelectToken("name");
                        //    dealer.shortname = (string)jo.SelectToken("shortname");
                        //    dealer.city = (string)jo.SelectToken("city");
                        //    dealer.phone = (string)jo.SelectToken("phone");
                        //    model.dealer = dealer;
                        //}

                        DataSource.Add(model);
                    }

                    this.PageCount = (int)json.SelectToken("result").SelectToken("pagecount");
                    this.PageIndex = (int)json.SelectToken("result").SelectToken("pageindex");

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
