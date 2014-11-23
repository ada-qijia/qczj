using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Controls;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Model;
using ViewModels;
using ViewModels.Handler;
using AutoWP7.Utils;
using System.Windows;
using AutoWP7.UcControl;

namespace AutoWP7.View.CarSearch
{
    public partial class CarSearchResultPage : PhoneApplicationPage
    {
        #region Properties

        string mip = "100000";
        string map = "150000";
        string l = "0";
        string c = "0";
        string b = "1";
        string st = "1";
        string mid = "1";
        string mad = "1000";
        string conf = "1";
        string o = "1";
        string bid = "0";
        string f = "0";

        #endregion

        #region Lifecycle

        public CarSearchResultPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //mip = this.NavigationContext.QueryString["mip"];
            //map = this.NavigationContext.QueryString["map"];
            //l = this.NavigationContext.QueryString["l"];
            //c = this.NavigationContext.QueryString["c"];
            //b = this.NavigationContext.QueryString["b"];
            //st = this.NavigationContext.QueryString["st"];
            //mid = this.NavigationContext.QueryString["mid"];
            //mad = this.NavigationContext.QueryString["mad"];
            //conf = this.NavigationContext.QueryString["conf"];
            //o = this.NavigationContext.QueryString["o"];
            //bid = this.NavigationContext.QueryString["bid"];
            //f = this.NavigationContext.QueryString["f"];

            LoadData(false);
        }

        #endregion

        #region Filter Groups

        CarSearchResultViewModel VM = null;

        public void LoadData(bool reload)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;
            if (VM == null)
            {
                VM = new CarSearchResultViewModel();
                VM.LoadDataCompleted += filterVM_LoadDataCompleted;
            }

            int page_index = reload ? 1 : VM.PageIndex + 1;

            //http://221.192.136.99:804/wpv1.6/cars/series-a2-pm3-V1.6.0-mip100000-map150000-l0-c0-b1-st1-mid1-mad1000-conf1-o1-p1-s20-bid0-f0.html
            string format = App.appUrl + App.versionStr + "/cars/series-" + App.AppInfo + "-mip{0}-map{1}-l{2}-c{3}-b{4}-st{5}-mid{6}-mad{7}-conf{8}-o{9}-p{10}-s{11}-bid{12}-f{13}.html";
            string url = string.Format(format, mip, map, l, c, b, st, mid, mad, conf, o, page_index, VM.PageSize, bid, f);
            VM.LoadDataAysnc(url, reload);
        }

        void filterVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<CarSearchResultSeriesItemModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                searchResultListBox.ItemsSource = VM.DataSource;
            }
        }

        #endregion

        private void searchResultItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void loadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

    }
}