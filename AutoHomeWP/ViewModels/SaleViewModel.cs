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
    public class SaleViewModel : INotifyPropertyChanged
    {
        public SaleViewModel()
        {
            PageIndex = 0;
            PageSize = 20;
        }

        private ObservableCollection<SaleItemModel> _DataSource = new ObservableCollection<SaleItemModel>();
        public ObservableCollection<SaleItemModel> DataSource
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

        SaleItemModel LoadMoreButtonItem = new SaleItemModel() { IsLoadMore = true };

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

        public event EventHandler<APIEventArgs<IEnumerable<SaleItemModel>>> LoadDataCompleted;

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
            Uri urlSource = new Uri(url, UriKind.Absolute);
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
            APIEventArgs<IEnumerable<SaleItemModel>> apiArgs = new APIEventArgs<IEnumerable<SaleItemModel>>();
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
                    JArray ja = (JArray)json.SelectToken("result").SelectToken("carlist");

                    SaleItemModel model = null;
                    for (int i = 0; i < ja.Count; i++)
                    {
                        model = new SaleItemModel();
                        model.seriesid = (int)ja[i].SelectToken("seriesid");
                        model.seriesname = (string)ja[i].SelectToken("seriesname");
                        model.specid = (int)ja[i].SelectToken("specid");
                        model.specname = (string)ja[i].SelectToken("specname");
                        model.specpic = (string)ja[i].SelectToken("specpic");
                        model.inventorystate = (int)ja[i].SelectToken("inventorystate");
                        model.dealerprice = (string)ja[i].SelectToken("dealerprice");
                        model.fctprice = (string)ja[i].SelectToken("fctprice");
                        model.articleid = (int)ja[i].SelectToken("articleid");
                        model.articletype = (int)ja[i].SelectToken("articletype");

                        //dealer
                        var jo = ja[i].SelectToken("dealer");
                        if (jo != null)
                        {
                            SaleDealer dealer = new SaleDealer();
                            dealer.id = (int)jo.SelectToken("id");
                            dealer.name = (string)jo.SelectToken("name");
                            dealer.shortname = (string)jo.SelectToken("shortname");
                            dealer.city = (string)jo.SelectToken("city");
                            dealer.phone = (string)jo.SelectToken("phone");
                            model.dealer = dealer;
                        }

                        DataSource.Add(model);
                    }

                    this.PageCount = (int)json.SelectToken("result").SelectToken("pagecount");
                    this.PageIndex = (int)json.SelectToken("result").SelectToken("pageindex");

                    EnsureMoreButton();
                }
                catch
                { }
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
