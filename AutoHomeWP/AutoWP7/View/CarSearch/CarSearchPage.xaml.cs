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
using System.Collections.ObjectModel;

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
            if (!filterLoaded)
            {
                LoadFilters();
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (SpecPanel.Visibility == Visibility.Visible)
            {
                SpecPanel.Visibility = Visibility.Collapsed;
                e.Cancel = true;
            }
            else if (SeriesPanel.Visibility == Visibility.Visible)
            {
                SeriesPanel.Visibility = Visibility.Collapsed;
                e.Cancel = true;
            }
            else if (filterPopupShown)
            {
                HideFilterPopups();
                e.Cancel = true;
            }

            base.OnBackKeyPress(e);
        }

        #endregion

        #region Filter Groups

        CarSearchFilterViewModel filterVM = null;
        bool filterLoaded = false;

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
                Search(true);
                filterLoaded = true;
            }
        }

        #endregion

        #region Filter Selection

        bool filterPopupShown = false;
        CarSearchFilterGroupModel selectedFilterGroup = null;
        CarFilderFilterItem selectedFilterGroupControl = null;
        string selectedFilterKey = string.Empty;
        List<CarFilderFilterItem> filterControls = new List<CarFilderFilterItem>();

        private void FilterControl_Loaded(object sender, RoutedEventArgs e)
        {
            var control = sender as CarFilderFilterItem;
            filterControls.Add(control);
        }

        private void FilterGroup_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            selectedFilterGroupControl = sender as CarFilderFilterItem;
            var group = sender.GetDataContext<CarSearchFilterGroupModel>();
            selectedFilterGroup = group;
            selectedFilterKey = group.key;

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
            filterPanelTitle.Text = "选择" + group.DisplayName;
        }

        private void HideFilterPopups()
        {
            singleSelectionPanel.Visibility = Visibility.Collapsed;
            multiSelectionPanel.Visibility = Visibility.Collapsed;
            filterPopupShown = false;
            filterPanelTitle.Text = "条件筛选";
        }

        private void singleFilterItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var item = sender.GetDataContext<CarSearchFilterItemModel>();
            selectedFilterGroupControl.SelectedFilter = item.name;
            HideFilterPopups();
            SetSearchParam(selectedFilterKey, item.value);
            Search(true);
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
            Search(true);
        }

        #endregion

        #region Search

        string mip = "0";
        string map = "0";
        string l = "0";
        string c = "0";
        string b = "0";
        string st = "0";
        string mid = "0";
        string mad = "0";
        string conf = "0";
        string o = "1";
        string bid = "0";
        string f = "0";

        CarSearchResultViewModel searchVM = null;

        private void SetSearchParam(string key, string value)
        {
            string[] strArr = null;
            switch (key)
            {
                case "structure"://结构
                    st = value;
                    break;
                case "gearbox"://变速箱
                    b = value;
                    break;
                case "price"://价格
                    strArr = value.Split('|');
                    mip = (int.Parse(strArr[0]) * 10000).ToString();
                    map = (int.Parse(strArr[1]) * 10000).ToString();
                    break;
                case "level"://级别
                    l = value;
                    break;
                case "country"://国别
                    c = value;
                    break;
                case "findorder"://"排序";
                    o = value;
                    break;
                case "displacement"://"排量";
                    strArr = value.Split('|');
                    mid = strArr[0];
                    mad = strArr[1];
                    break;
                case "configs"://配置
                    conf = value;
                    break;
                case "fueltype"://燃料
                    f = value;
                    break;
                default:
                    break;
            }
        }

        private void Search(bool reload)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            if (searchVM == null)
            {
                searchVM = new CarSearchResultViewModel();
                searchVM.LoadDataCompleted += searchVM_LoadDataCompleted;
                searchResultListBox.ItemsSource = searchVM.DataSource;
            }

            int page_index = reload ? 1 : searchVM.PageIndex + 1;

            //http://221.192.136.99:804/wpv1.6/cars/series-a2-pm3-V1.6.0-mip100000-map150000-l0-c0-b1-st1-mid1-mad1000-conf1-o1-p1-s20-bid0-f0.html
            string format = App.appUrl + App.versionStr + "/cars/series-" + App.AppInfo + "-mip{0}-map{1}-l{2}-c{3}-b{4}-st{5}-mid{6}-mad{7}-conf{8}-o{9}-p{10}-s{11}-bid{12}-f{13}.html";
            string url = string.Format(format, mip, map, l, c, b, st, mid, mad, conf, o, page_index, searchVM.PageSize, bid, f);
            searchVM.LoadDataAysnc(url, reload);
        }

        void searchVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<CarSearchResultSeriesItemModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                //if (searchVM.DataSource.Count == 0)
                //{
                //    Common.showMsg("暂无降价活动");
                //}
                //else if (searchVM.IsEndPage)
                //{
                //    Common.showMsg("已经是最后一页了");
                //}
                searchResultSeriesCount.Text = searchResultSeriesCount2.Text = searchVM.RowCount.ToString();
                searchResultSpecCount.Text = searchResultSpecCount2.Text = searchVM.TotalSpecCount.ToString();
            }
        }

        private void searchLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Search(false);
        }

        #endregion

        private void goSearch_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SeriesPanel.Visibility = Visibility.Visible;
            //this.NavigationService.Navigate(new Uri("/View/CarSearch/CarSearchResultPage.xaml", UriKind.Relative));
        }

        private void searchResultItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var carSeries = sender.GetDataContext<CarSearchResultSeriesItemModel>();
            PopulateSpecList(carSeries);
            SpecPanel.Visibility = Visibility.Visible;
        }

        private void resetSearch_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            mip = "0";
            map = "0";
            l = "0";
            c = "0";
            b = "0";
            st = "0";
            mid = "0";
            mad = "0";
            conf = "0";
            o = "1";
            bid = "0";
            f = "0";

            foreach (var control in filterControls)
            {
                control.SelectedFilter = "不限";
            }

            Search(true);
        }

        #region Spec List

        class CarSeriesGroup : List<CarSearchResultSpecItemModel>
        {
            public CarSeriesGroup()
            { }
            public CarSeriesGroup(string groupname)
            {
                key = groupname;
            }

            public string key { get; set; }
            public bool HasItems { get { return Count > 0; } }
        }

        ObservableCollection<CarSeriesGroup> specsGroups = null;

        void PopulateSpecList(CarSearchResultSeriesItemModel carSeries)
        {
            SpecPanel.DataContext = carSeries;
            specsGroups = new ObservableCollection<CarSeriesGroup>();
            foreach (var specGroup in carSeries.specitemgroups)
            {
                CarSeriesGroup groupitem = new CarSeriesGroup(specGroup.groupname);
                foreach (var spec in specGroup.specitems)
                {
                    groupitem.Add(spec);
                }
                specsGroups.Add(groupitem);
            }
            carSpecListBox.ItemsSource = specsGroups;
        }

        #endregion

        private void carSeriesSummary_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var series = sender.GetDataContext<CarSearchResultSeriesItemModel>();
            string url = string.Format("/View/Car/CarSeriesDetailPage.xaml?indexId=0&carSeriesId={0}", series.id);
            this.NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }

        private void carSpec_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var spec = sender.GetDataContext<CarSearchResultSpecItemModel>();
            string url = string.Format("/View/Car/CarSeriesQuotePage.xaml?carId={0}",spec.id);
            this.NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }


    }
}