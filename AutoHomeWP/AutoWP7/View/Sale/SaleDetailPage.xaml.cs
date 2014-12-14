
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Model;
using ViewModels;
using Microsoft.Phone.Tasks;
using AutoWP7.Utils;
using Newtonsoft.Json.Linq;

namespace AutoWP7.View.Sale
{
    public partial class SaleDetailPage : PhoneApplicationPage
    {
        string seriesid = string.Empty;
        string specid = string.Empty;
        string dealerid = string.Empty;
        string articleid;
        string articletype;

        public SaleDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            seriesid = NavigationContext.QueryString["seriesid"];
            specid = NavigationContext.QueryString["specid"];
            articleid = NavigationContext.QueryString["articleid"];
            articletype = NavigationContext.QueryString["articletype"];
            dealerid = NavigationContext.QueryString["dealerid"];

            //http://221.192.136.99:804/wpv1.6/content/dealer/downprice-a2-pm1-v4.3.0-sp11007-n11983239-t0-d85260-ss806-nt0.html
            string url = string.Format("{0}{1}/content/dealer/downprice-{2}-sp{3}-n{4}-t{5}-d{6}-ss{7}-nt0.html",
                App.appUrl, App.versionStr, App.AppInfo, specid, articleid, articletype, dealerid, seriesid);
            wb.Navigate(new Uri(url, UriKind.Absolute));
        }

        private void wb_ScriptNotify(object sender, NotifyEventArgs e)
        {
            JObject json = JObject.Parse(e.Value);
            string eventName = (string)json.SelectToken("event");
            string key = (string)json.SelectToken("key");

            //{"event":"dealertel","key":"4009314862"}
            if (eventName == "dealertel")
            {
                CallDealer(key);
            }
            else if (eventName == "queryprice")
            {
                AskPrice();
            }
        }

        private void CallDealer(string phoneNumber)
        {
            UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "电话拨打点击量");
            PhoneCallTask phoneCall = new PhoneCallTask();
            phoneCall.PhoneNumber = phoneNumber;
            phoneCall.Show();
        }

        private void AskPrice()
        {
            string cityID = "110100";//默认北京
            if (!string.IsNullOrEmpty(App.CityId))
            {
                cityID = App.CityId;
            }

            this.NavigationService.Navigate(new Uri("/View/Car/AskPrice.xaml?dealerid=" + dealerid + "&cityID=" + cityID + "&seriesID=" + seriesid + "&specID=" + specid, UriKind.Relative));
        }

    }
}