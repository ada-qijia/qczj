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
        #region Lifecycle

        public CarSearchPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadFilters();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (filterPopupShown)
            {
                HideFilterPopups();
                e.Cancel = true;
            }
            base.OnBackKeyPress(e);
        }

        #endregion

        #region Filter Groups

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
                filterGroupListBox.ItemsSource = filterVM.DataSource;
            }
        }

        private void FilterGroup_Loaded(object sender, RoutedEventArgs e)
        {
            string key = sender.GetDataContext<CarSearchFilterGroupModel>().key;
            CarFilderFilterItem control = sender as CarFilderFilterItem;
            filterControls.Add(key, control);
        }

        #endregion

        #region Filter Selection

        private bool filterPopupShown = false;
        private CarSearchFilterGroupModel selectedFilterGroup = null;
        private CarFilderFilterItem selectedFilterGroupControl = null;

        private void FilterGroup_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectedFilterGroupControl = sender as CarFilderFilterItem;
            var group = sender.GetDataContext<CarSearchFilterGroupModel>();
            selectedFilterGroup = group;

            if (group.key == "configs")
            {
                multiSelectionPanel.Visibility = Visibility.Visible;
                if (multiFilterListBox.ItemsSource == null)
                {
                    multiFilterListBox.ItemsSource = filterVM.FilterGroups[group.key].filters;
                    filterVM.FilterGroups[group.key].filters[0].Selected = true;
                }
            }
            else
            {
                singleSelectionPanel.Visibility = Visibility.Visible;
                singleFilterListBox.ItemsSource = filterVM.FilterGroups[group.key].filters;
            }
            filterPopupShown = true;
            pageTitle.Text = "选择" + group.DisplayName;
        }

        private void HideFilterPopups()
        {
            singleSelectionPanel.Visibility = Visibility.Collapsed;
            multiSelectionPanel.Visibility = Visibility.Collapsed;
            filterPopupShown = false;
            pageTitle.Text = "条件筛选";
        }

        private void singleFilterItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var item = sender.GetDataContext<CarSearchFilterItemModel>();
            selectedFilterGroupControl.SelectedFilter = item.name;
            HideFilterPopups();
        }

        private void multiFilterItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var item = sender.GetDataContext<CarSearchFilterItemModel>();

            if (item.value == "0")
            {
                item.Selected = true;
                foreach (var filter in selectedFilterGroup.filters)
                {
                    if (filter.value != "0")
                    {
                        filter.Selected = false;
                    }
                }
                return;
            }
            else
            {
                item.Selected = !item.Selected;
                if (item.Selected)
                {
                    filterVM.FilterGroups["configs"].filters[0].Selected = false;
                }

                bool noItemIsChecked = true;
                foreach (var filter in selectedFilterGroup.filters)
                {
                    if (filter.value != "0" && filter.Selected)
                    {
                        noItemIsChecked = false;
                        break;
                    }
                }
                filterVM.FilterGroups["configs"].filters[0].Selected = noItemIsChecked;
            }
        }

        private void multiSelectionOK_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string filterNames = string.Empty;
            foreach (var filter in selectedFilterGroup.filters)
            {
                if (filter.Selected)
                {
                    filterNames += filter.name + ",";
                }
            }
            if (filterNames.Length > 0)
            {
                filterNames = filterNames.Trim(',');
                selectedFilterGroupControl.SelectedFilter = filterNames;
            }
            HideFilterPopups();
        }

        #endregion


    }
}