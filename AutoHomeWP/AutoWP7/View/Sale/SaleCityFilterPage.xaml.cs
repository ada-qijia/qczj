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
            //cityListBox.ItemsSource = 
            var cities = from c in provinceVM.CityDataSource
                         where c.Father == province.Id
                         select c;

            provinceListGroups.Visibility = Visibility.Collapsed;
            cityListBox.Visibility = Visibility.Visible;
            cityListBox.ItemsSource = cities.ToList();

            return;

            TextBlock ss = (TextBlock)sender;

            //独立存储城市id
            var setting = IsolatedStorageSettings.ApplicationSettings;
            string key = "cityId";

            if (setting.Contains(key))
            {
                setting[key] = ss.Tag.ToString();
            }
            else
            {
                setting.Add(key, ss.Tag.ToString());
            }
            setting.Save();
            App.CityId = ss.Tag.ToString();
            this.NavigationService.GoBack();
        }

        private void cityItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }
    }
}