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

        LocalDataContext ldc = new LocalDataContext();
        public void LoadData()
        {
            //本地数据库获取
            var queryProvince = from s in ldc.provinces where s.Id > 0 select s;
            if (queryProvince.Count() > 0)
            {
                var groupBy = from p in queryProvince
                              group p by p.FatherName into c
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
            else //服务器获取
            {
                ProvinceLoadData();

            }
        }
        #endregion


        #region  省市数据网络获取

        CityListViewModel provinceVM = null;
        public void ProvinceLoadData()
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;
            if (provinceVM == null)
            {
                provinceVM = new CityListViewModel();
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
                LoadData();
            }
        }

        #endregion

        private void cityNameStack_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
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
    }
}