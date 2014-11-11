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

namespace AutoWP7.View.Channel.News
{
    public partial class SaleFilterSelectorPage : PhoneApplicationPage
    {
        string filterType = string.Empty;
        public SaleFilterSelectorPage()
        {
            InitializeComponent();
        }

        string videoId = string.Empty;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            filterType = NavigationContext.QueryString["filterType"];
            switch (filterType)
            {
                case "price":
                    pageTitle.Text = "选择价格";
                    break;
                default:
                    break;
            }
            LoadData();
        }

        #region Load Data

        SaleFilterViewModel VM = null;
        SaleFilterModel filterData = null;
        private void LoadData()
        {
            ProgBar.Visibility = Visibility.Visible;

            VM = new SaleFilterViewModel();

            //http://221.192.136.99:804/wpv1.6/mobile/PriceDownOptions.ashx?a=2&pm=3&v=1.6.0&types=price|635140776513304067,level|635140776909066703,buyorder|0
            //http://221.192.136.99:804/wpv1.6/mobile/PriceDownOptions.ashx?a=2&pm=3&v=1.6.0&types=price|0,level|0,buyorder|0
            //string url = string.Format("{0}{1}/news/videopagejson-{2}-vid{3}.html", App.appUrl, App.versionStr, App.AppInfo, videoId);
            string appInfo = string.Format("a={0}&pm={1}&v{2}", App.appId, App.platForm, App.version);
            string url = string.Format("{0}{1}/mobile/PriceDownOptions.ashx?{2}&types=price|0,level|0,buyorder|0", "http://221.192.136.99:804", "/wpv1.6", appInfo);
            VM.LoadDataAysnc(url);
            VM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<SaleFilterModel>>((ss, ee) =>
            {
                if (ee.Error != null)
                {

                }
                else
                {
                    filterData = ee.Result;
                    this.filterListBox.ItemsSource = filterData.FilterGroups[filterType].Filters;
                }
                ProgBar.Visibility = Visibility.Collapsed;
            });

        }

        #endregion

        private void filterItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

    }
}