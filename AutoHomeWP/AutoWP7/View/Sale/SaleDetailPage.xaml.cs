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

namespace AutoWP7.View.Sale
{
    public partial class SaleDetailPage : PhoneApplicationPage
    {
        public SaleDetailPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string seriesid = NavigationContext.QueryString["seriesid"];
            string specid = NavigationContext.QueryString["specid"];
            string articleid = NavigationContext.QueryString["articleid"];
            string articletype = NavigationContext.QueryString["articletype"];
            string dealerid = NavigationContext.QueryString["dealerid"];

            //http://221.192.136.99:804/wpv1.6/content/dealer/downprice-a2-pm1-v4.3.0-sp11007-n11983239-t0-d85260-ss806-nt0.html
            string url = string.Format("{0}{1}/content/dealer/downprice-{2}-sp{3}-n{4}-t{5}-d{6}-ss{7}-nt0.html",
                App.appUrl, App.versionStr, App.AppInfo, specid, articleid, articletype, dealerid, seriesid);
            wb.Navigate(new Uri(url, UriKind.Absolute));
        }

    }
}