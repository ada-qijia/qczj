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
    public partial class CarSearchPage : PhoneApplicationPage
    {


        public CarSearchPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadFilters();
        }

        #region Load Filters

        CarSearchFilterViewModel filterVM = null;
        Dictionary<string, CarFilderFilterItem> filterControls = new Dictionary<string, CarFilderFilterItem>();

        public void LoadFilters()
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;
            if (filterVM == null)
            {
                filterVM = new CarSearchFilterViewModel();
                filterVM.LoadDataCompleted += filterVM_LoadDataCompleted;
            }

            //http://221.192.136.99:804/wpv1.6/mobile/SearchCarsOptions.ashx?a=2&pm=1&v=1.6.0&types=structure|0,gearbox|0,price|0,level|0,country|0,findorder|0,displacement|0,configs|0,fueltype|0

            filterVM.LoadDataAysnc(string.Format("{0}{1}/mobile/SearchCarsOptions.ashx?{2}&types=structure|0,gearbox|0,price|0,level|0,country|0,findorder|0,displacement|0,configs|0,fueltype|0",
                App.appUrl, App.versionStr, App.AppInfo));
        }

        void filterVM_LoadDataCompleted(object sender, APIEventArgs<List<CarSearchFilterGroupModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                filterListBox.ItemsSource = e.Result;
            }
        }

        #endregion

        private void Filter_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var group = sender.GetDataContext<CarSearchFilterGroupModel>();
            switch (group.key)
            {
                case "price":

                    break;
                default:
                    break;
            }
        }

        private void Filter_Loaded(object sender, RoutedEventArgs e)
        {
            string key = sender.GetDataContext<CarSearchFilterGroupModel>().key;
            CarFilderFilterItem control = sender as CarFilderFilterItem;
            filterControls.Add(key, control);
        }

    }
}