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

namespace AutoWP7.View.Sale
{
    public partial class SaleCityFilterPage : PhoneApplicationPage
    {
        private string provinceID = string.Empty;
        private string provinceName = string.Empty;

        public SaleCityFilterPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadData();
        }

        #region 省市数据获取

        private class ProvinceGroup : List<ProvinceModel>
        {
            public ProvinceGroup(string category)
            {
                key = category;
            }

            public string key { get; set; }
            public bool HasItems { get { return Count > 0; } }
        }

        private class ProvinceSource : List<ProvinceGroup>
        {

        }

        ProvinceSource provinceSource = new ProvinceSource();
        SaleCityListViewModel provinceVM = null;

        public void LoadData()
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;
            if (provinceVM == null)
            {
                provinceVM = new SaleCityListViewModel();
            }
            provinceVM.LoadDataAysnc(string.Format("{0}{1}/news/province-{2}-ts0.html", App.appUrl, App.versionStr, App.AppInfo));
            provinceVM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<Model.ProvinceModel>>>(provinceVM_LoadDataCompleted);
        }

        void provinceVM_LoadDataCompleted(object sender, ViewModels.Handler.APIEventArgs<IEnumerable<Model.ProvinceModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                var groupBy = from p in provinceVM.ProvinceDataSource
                              group p by p.FirstLetter into c
                              orderby c.Key
                              select new Group<ProvinceModel>(c.Key, c);

                foreach (var entity in groupBy)
                {
                    ProvinceGroup group = new ProvinceGroup(entity.key);

                    foreach (var item in entity)
                    {
                        group.Add(item);
                    }
                    provinceSource.Add(group);
                }
                provinceListGroups.ItemsSource = provinceSource;
            }
        }

        #endregion

        private void provinceNameStack_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var province = sender.GetDataContext<ProvinceModel>();

            if (province.Id == 0)
            {
                App.SaleFilterSelector_FilterType = "country";
                App.SaleFilterSelector_SelectedName = "地区";
                App.SaleFilterSelector_SelectedValue = "0";
                this.NavigationService.GoBack();
            }
            else
            {
                provinceID = province.Id.ToString();
                provinceName = province.Name;

                var cities = from c in provinceVM.CityDataSource
                             where c.Father == province.Id
                             select c;

                provinceListGroups.Visibility = Visibility.Collapsed;
                cityListBox.Visibility = Visibility.Visible;
                cityListBox.ItemsSource = cities.ToList();
            }
        }

        private void cityItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ProvinceModel city = sender.GetDataContext<ProvinceModel>();
            if (city.Id == 0)
            {
                App.SaleFilterSelector_FilterType = "province";
                App.SaleFilterSelector_SelectedName = provinceName;
                App.SaleFilterSelector_SelectedValue = provinceID;
                this.NavigationService.GoBack();
            }
            else
            {
                App.SaleFilterSelector_FilterType = "city";
                App.SaleFilterSelector_SelectedName = city.Name;
                App.SaleFilterSelector_SelectedValue = city.Id.ToString();
                this.NavigationService.GoBack();
            }
        }
    }
}