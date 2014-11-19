using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Model;
using ViewModels;
using ViewModels.Handler;
using CommonLayer;

namespace AutoWP7.View.Sale
{
    public partial class SaleBrandFilterPage : PhoneApplicationPage
    {
        public SaleBrandFilterPage()
        {
            InitializeComponent();
        }

        public int BrandId = 0;
        public int SeriesId = 0;
        public string SeriesName = string.Empty;
        public ShowListTypeEnum CurType = ShowListTypeEnum.BrandList;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.carListGropus.Visibility = System.Windows.Visibility.Visible;
            this.carSeriesListGropus.Visibility = System.Windows.Visibility.Collapsed;
            this.carSpecListGropus.Visibility = System.Windows.Visibility.Collapsed;

            carBrandLoadData();
        }

        #region  品牌数据加载

        private class CarBrandGroup : List<CarBrandModel>
        {
            public CarBrandGroup(string category)
            {
                key = category;
            }
            public string key { get; set; }
            public bool HasItems { get { return Count > 0; } }
        }

        private class CarBrandSource : List<CarBrandGroup>
        {
        }

        CarBrandSource carBrandSource = new CarBrandSource();
        CarBrandViewModel carVM = null;

        public void carBrandLoadData()
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            if (carVM == null)
            {
                carVM = new CarBrandViewModel();
                carVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<CarBrandModel>>>(carVM_LoadDataCompleted);
            }

            //http://221.192.136.99:804/wpv1.6/cars/brandsdealer-a2-pm1-v3.0.1-ts635174268324922500.html
            string url = string.Format("{0}{1}/cars/brandsdealer-{2}-ts{3}.html", App.appUrl, App.versionStr, App.AppInfo, 0);
            carVM.LoadDataAysnc(url);
        }

        void carVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<CarBrandModel>> e)
        {
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                var groupBy = from car in e.Result
                              group car by car.Letter into c
                              orderby c.Key
                              select new Group<CarBrandModel>(c.Key, c);

                //add special item
                CarBrandGroup allBrandGroup = new CarBrandGroup("#");
                CarBrandModel allBrandItem = new CarBrandModel() { Id = 0, Name = "全部品牌" };
                allBrandGroup.Add(allBrandItem);
                carBrandSource.Add(allBrandGroup);

                foreach (var entity in groupBy)
                {
                    CarBrandGroup group = new CarBrandGroup(entity.key + "         ");

                    foreach (var item in entity)
                    {
                        group.Add(item);
                    }
                    carBrandSource.Add(group);
                }
                carListGropus.ItemsSource = carBrandSource;
            }

            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
        }

        #endregion

        #region 车系数据加载

        private class CarSeriesGroup : List<CarSeriesModel>
        {
            public CarSeriesGroup(string category)
            {
                key = category;
            }

            public string key { get; set; }
            public bool HasItems { get { return Count > 0; } }
        }

        private class CarSeriesSource : List<CarSeriesGroup>
        {
        }

        CarSeriesSource carSeriesSource = new CarSeriesSource();
        CarSeriesViewModel carSeriesVM = null;

        public void CarSeriesLoadData(string bid)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            if (carSeriesVM == null)
            {
                carSeriesVM = new CarSeriesViewModel();
                carSeriesVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<CarSeriesModel>>>(carSeriesVM_LoadDataCompleted);
            }

            //http://221.192.136.99:804/wpv1.6/cars/seriesprice-a2-pm3-v1.6.0-b33-t1.html
            string url = string.Format("{0}{1}/cars/seriesprice-{2}-b{3}-t1.html", App.appUrl, App.versionStr, App.AppInfo, bid);
            carSeriesVM.LoadDataAysnc(url);
        }

        void carSeriesVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<CarSeriesModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                if (e.Result.Count() < 1)
                {
                    Common.showMsg("暂无此车系数据");
                }
                else
                {
                    var groupBy = from car in e.Result
                                  group car by car.FctName into c
                                  select new Group<CarSeriesModel>(c.Key, c);

                    //add special item
                    CarSeriesGroup allSeriesGroup = new CarSeriesGroup("#");
                    CarSeriesModel allSeriesItem = new CarSeriesModel() { Id = 0, Name = "全部车系" };
                    allSeriesGroup.Add(allSeriesItem);
                    carSeriesSource.Add(allSeriesGroup);

                    foreach (var entity in groupBy)
                    {
                        string temKey = entity.key;

                        //此操作目的使key长度一样，以便展示时，背景宽度一样
                        if (entity.key.GetStringChLength() < 15)
                            for (double i = 0; i < 15 - entity.key.GetStringChLength(); i += 0.5)
                                temKey += "  ";
                        CarSeriesGroup group = new CarSeriesGroup(temKey);

                        foreach (var item in entity)
                        {
                            group.Add(item);
                        }
                        carSeriesSource.Add(group);
                    }
                    carSeriesListGropus.ItemsSource = carSeriesSource;
                }

            }
        }

        #endregion

        #region 车型数据加载

        private class CarSeriesQuteGroup : List<CarSeriesQuoteModel>
        {
            public CarSeriesQuteGroup(string category)
            {
                key = category;
            }

            public string key { get; set; }
            public bool HasItems { get { return Count > 0; } }
        }

        private class CarSeriesQuteSource : List<CarSeriesQuteGroup>
        {
        }

        CarSeriesQuteSource carSeriesQuteSource = new CarSeriesQuteSource();
        SaleCarSpecListViewModel carSeriesQuoteVM = null;

        public void CarSpecLoadData(string sid)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            if (carSeriesQuoteVM == null)
            {
                carSeriesQuoteVM = new SaleCarSpecListViewModel();
                carSeriesQuoteVM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<Model.CarSeriesQuoteModel>>>(carSeriesQuoteVM_LoadDataCompleted);
            }

            //http://221.192.136.99:804/wpv1.6/cars/specslist-a2-pm3-v1.6.0-t0x000c-ss18.html
            string url = string.Format("{0}{1}/cars/specslist-{2}-t0x000c-ss{3}.html", App.appUrl, App.versionStr, App.AppInfo, sid);
            carSeriesQuoteVM.LoadDataAysnc(url, false);
        }

        void carSeriesQuoteVM_LoadDataCompleted(object sender, ViewModels.Handler.APIEventArgs<IEnumerable<Model.CarSeriesQuoteModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            if (e.Error != null)
                Common.NetworkAvailablePrompt();
            else
            {
                if (e.Result.Count() < 1)
                {
                    carSpecListGropus.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    var groupBy = from car in e.Result
                                  group car by car.GroupName into c
                                  select new Group<CarSeriesQuoteModel>(c.Key, c);

                    //add special item
                    CarSeriesQuteGroup allSpecGroup = new CarSeriesQuteGroup("#");
                    CarSeriesQuoteModel allSpecItem = new CarSeriesQuoteModel() { Id = 0, Name = "全部车型" };
                    allSpecGroup.Add(allSpecItem);
                    carSeriesQuteSource.Add(allSpecGroup);

                    foreach (var entity in groupBy)
                    {
                        CarSeriesQuteGroup group = new CarSeriesQuteGroup(entity.key);
                        foreach (var item in entity)
                        {
                            group.Add(item);
                        }
                        carSeriesQuteSource.Add(group);
                    }
                    carSpecListGropus.ItemsSource = carSeriesQuteSource;
                }
            }
        }

        #endregion

        private void carSeriesGird_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid gg = (Grid)sender;
            string id = gg.Tag.ToString();
            if (CurType == ShowListTypeEnum.BrandList)
            {
                this.carSeriesListGropus.Visibility = System.Windows.Visibility.Visible;
                this.carListGropus.Visibility = System.Windows.Visibility.Collapsed;
                this.carSpecListGropus.Visibility = System.Windows.Visibility.Collapsed;
                CurType = ShowListTypeEnum.SeriesList;
                CarSeriesLoadData(id);
            }
            else if (CurType == ShowListTypeEnum.SeriesList)
            {
                SeriesId = Convert.ToInt32(id);
                TextBlock tb = CommonLayer.CommonHelper.FindFirstElementInVisualTree<TextBlock>(gg) as TextBlock;
                if (tb != null)
                    SeriesName = tb.Text;
                this.carSeriesListGropus.Visibility = System.Windows.Visibility.Collapsed;
                this.carListGropus.Visibility = System.Windows.Visibility.Collapsed;
                this.carSpecListGropus.Visibility = System.Windows.Visibility.Visible;
                CurType = ShowListTypeEnum.SpecList;
                CarSpecLoadData(id);
            }
        }

        private void carSpecGird_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ////获取车系id 添加到对比列表           
            //Grid gg = (Grid)sender;
            //int specId = Convert.ToInt32(gg.Tag);
            //string specName = string.Empty;
            //TextBlock tb = CommonLayer.CommonHelper.FindFirstElementInVisualTree<TextBlock>(gg);
            //if (tb != null)
            //    specName = tb.Text;

            //int resultCode = carCompareVM.AddCompareSpec(carCompareVM);
            //if (resultCode == 0) //添加成功
            //{
            //    if (Action == CarChooseTypeEnum.CarCompare)
            //        this.NavigationService.Navigate(new Uri("/View/Car/CarComparePage.xaml", UriKind.Relative));
            //    else if (Action == CarChooseTypeEnum.CarCompareList)
            //        this.NavigationService.Navigate(new Uri("/View/Car/CarCompareListPage.xaml", UriKind.Relative));
            //    else
            //        MessageBox.Show("此车型添加对比成功");
            //    //跳转到对比页
            //}
            //else if (resultCode == 1)
            //{
            //    MessageBox.Show("添加对比车型到对比库");
            //}
            //else
            //{
            //    MessageBox.Show("抱歉，最多可添加9款车型");
            //}
        }
    }

    public enum ShowListTypeEnum
    {
        BrandList = 0,
        SeriesList = 1,
        SpecList = 2
    }
}