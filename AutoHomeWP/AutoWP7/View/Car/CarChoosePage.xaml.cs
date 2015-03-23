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

namespace AutoWP7.View.Car
{
    public partial class CarChoosePage : PhoneApplicationPage
    {
        public CarChoosePage()
        {
            InitializeComponent();
        }
        //使用项：0,对比库添加对比车型使用
        public CarChooseTypeEnum Action;
        public int BrandId;
        public int SeriesId;
        public string SeriesName;
        /// <summary>
        /// 当前类型值：0，品牌列表;1,车系列表;2，车型列表
        /// </summary>
        public ShowListTypeEnum CurType = 0;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            BrandId = 0;
            SeriesId = 0;
            SeriesName = "";
            //默认品牌列表
            CurType = ShowListTypeEnum.BrandList;

            Action = CarChooseTypeEnum.CarCompare;
            if (NavigationContext.QueryString.ContainsKey("action"))
                Action = (CarChooseTypeEnum)Convert.ToInt32(NavigationContext.QueryString["action"]);

            this.carListGropus.Visibility = System.Windows.Visibility.Visible;
            this.carSeriesListGropus.Visibility = System.Windows.Visibility.Collapsed;
            this.carSpecListGropus.Visibility = System.Windows.Visibility.Collapsed;
            this.proptyNoSpecGrid.Visibility = System.Windows.Visibility.Collapsed;
            carBrandLoadData();
        }

        /// <summary>
        /// 获取第一个符合类型的控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parentElement"></param>
        /// <returns></returns>
        private T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
                return null;
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    var result = FindFirstElementInVisualTree<T>(child);
                    if (result != null)
                        return result;
                }
            }
            return null;
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

        public void carBrandLoadData()
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                GlobalIndicator.Instance.Text = "正在获取中...";
                GlobalIndicator.Instance.IsBusy = true;
            });
            if (ldc == null)
            {
                ldc = new LocalDataContext();
            }
            var queryResult = from s in ldc.carBrandModels select s;
            //上次更新时间
            DateTime lastUpdateTime = DateTime.Now;
            foreach (CarBrandModel model in queryResult)
            {
                lastUpdateTime = model.CurrentTime;
            }
            //如果数据库中有数据，并其上次更新的时间和当前的时间差在一周内  本地数据库读取
            if (queryResult.Count() > 0 && (DateTime.Now - lastUpdateTime).Days < 7)
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var groupBy = from car in queryResult
                                  group car by car.Letter into c
                                  orderby c.Key
                                  select new Group<CarBrandModel>(c.Key, c);

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

                    GlobalIndicator.Instance.Text = "";
                    GlobalIndicator.Instance.IsBusy = false;
                });

            }
            else  //网络加载
            {

                //清除旧数据
                var deleteResult = from s in ldc.carBrandModels select s;
                ldc.carBrandModels.DeleteAllOnSubmit(deleteResult);
                ldc.SubmitChanges();
                Common.DeleteDirectory("CycleCachedImages");

                SetWebCarBrandLoadData();
            }

        }

        //汽车品牌网络加载
        CarBrandViewModel carVM = null;
        /// <summary>
        /// 品牌找车
        /// </summary>
        public void SetWebCarBrandLoadData()
        {
            if (carVM == null)
            {
                carVM = new CarBrandViewModel();
            }
            //carVM.LoadDataAysnc(App.headUrl + "/cars/APP/BrandAll.ashx");
            string url = string.Format("{0}{1}/cars/brands-{2}-ts{3}.html", App.appUrl, App.versionStr, App.AppInfo, 0);
            carVM.LoadDataAysnc(url);
            carVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<CarBrandModel>>>(carVM_LoadDataCompleted);

        }
        Dictionary<string, IList<CarBrandModel>> dic = new Dictionary<string, IList<CarBrandModel>>();
        void carVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<CarBrandModel>> e)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (e.Error != null)
                {
                    Common.NetworkAvailablePrompt();
                }
                else
                {
                    foreach (CarBrandModel model in e.Result)
                    {
                        if (!string.IsNullOrEmpty(model.ImgUrl))
                        {
                            model.bitmap = new StorageCachedImage(new Uri(model.ImgUrl, UriKind.Absolute), 1);
                        }
                    }
                    var groupBy = from car in e.Result
                                  group car by car.Letter into c
                                  orderby c.Key
                                  select new Group<CarBrandModel>(c.Key, c);
                    //string Groups = "#abcdefghijklmnopqrstuvwxyz";

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

            });
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
        /// <summary>
        /// 车系
        /// </summary>
        public void CarSeriesLoadData(string bid)
        {
            //清除表中以前的数据
            using (LocalDataContext ldc = new LocalDataContext())
            {
                var item = from s in ldc.carSeries where s.Id > 0 select s;
                ldc.carSeries.DeleteAllOnSubmit(item);
                ldc.SubmitChanges();
            }

            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;
            carSeriesVM = new CarSeriesViewModel();

            carSeriesVM.LoadDataAysnc(string.Format("{0}{2}/cars/seriesprice-a2-pm3-v{3}-b{1}-t2.html", App.appUrl, bid, App.versionStr, App.version));
            carSeriesVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<CarSeriesModel>>>(carSeriesVM_LoadDataCompleted);

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
                    Common.showMsg("暂无此车系数据哦");
                }
                else
                {
                    var groupBy = from car in e.Result
                                  group car by car.FctName into c
                                  select new Group<CarSeriesModel>(c.Key, c);
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
        CarSeriesQuoteViewModel carSeriesQuoteVM = null;
        /// <summary>
        /// 
        /// </summary>
        public void CarSpecLoadData(string sid)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;
            //清除表中以前的数据
            using (LocalDataContext ldc = new LocalDataContext())
            {
                var item = from s in ldc.carQuotes where s.Id > 0 select s;
                ldc.carQuotes.DeleteAllOnSubmit(item);
                ldc.SubmitChanges();
            }

            if (carSeriesQuoteVM == null)
            {
                carSeriesQuoteVM = new CarSeriesQuoteViewModel();
            }
            //carSeriesQuoteVM.LoadDataAysnc(App.headUrl + "/cars/APP/SpecBySeries.ashx?seriesid=" + sid);
            carSeriesQuoteVM.LoadDataAysnc(string.Format("{0}{2}/cars/seriessummary-a2-pm3-v{3}-s{1}-t0xffff-c0.html", App.appUrl, sid, App.versionStr, App.version), true);

            carSeriesQuoteVM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<Model.CarSeriesQuoteModel>>>(carSeriesQuoteVM_LoadDataCompleted);

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
                    proptyNoSpecGrid.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    CarCompareViewModel carCompareVM = new CarCompareViewModel();
                    var compareSpceList = carCompareVM.GetCompareSpec();
                    var groupBy = from car in e.Result
                                  group car by car.GroupName into c
                                  select new Group<CarSeriesQuoteModel>(c.Key, c);
                    foreach (var entity in groupBy)
                    {
                        CarSeriesQuteGroup group = new CarSeriesQuteGroup(entity.key);
                        foreach (var item in entity)
                            if (item.Id > 1000000 || item.ParamIsShow == 0 || compareSpceList.Count(c => c.SpecId == item.Id) > 0)
                            { //不能添加对比的车型：1、商用车id>1000000；2、不显示参数配置的paramisshow=0;3、已经在对比列表的车型
                            }
                            else
                                group.Add(item);
                        if (group.Count > 0)
                            carSeriesQuteSource.Add(group);
                    }
                    carSpecListGropus.ItemsSource = carSeriesQuteSource;
                }
            }
        }

        #endregion

        //本地加载
        LocalDataContext ldc;

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
            //获取车系id 添加到对比列表           
            Grid gg = (Grid)sender;
            int specId = Convert.ToInt32(gg.Tag);
            string specName = string.Empty;
            TextBlock tb = CommonLayer.CommonHelper.FindFirstElementInVisualTree<TextBlock>(gg);
            if (tb != null)
                specName = tb.Text;
            CarCompareViewModel carCompareVM = new CarCompareViewModel()
            {
                //如果是从对比最终页选车时，选中的车型要默认是选中状态
                IsChoosed = Action == CarChooseTypeEnum.CarCompare,
                SpecName = specName,
                SpecId = specId,
                SeriesId = SeriesId,
                SeriesName = SeriesName
            };
            int resultCode = carCompareVM.AddCompareSpec(carCompareVM);
            if (resultCode == 0) //添加成功
            {
                if (Action == CarChooseTypeEnum.CarCompare)
                    this.NavigationService.Navigate(new Uri("/View/Car/CarComparePage.xaml", UriKind.Relative));
                else if (Action == CarChooseTypeEnum.CarCompareList)
                    this.NavigationService.Navigate(new Uri("/View/Car/CarCompareListPage.xaml", UriKind.Relative));
                else
                    MessageBox.Show("此车型添加对比成功");
                //跳转到对比页
            }
            else if (resultCode == 1)
            {
                MessageBox.Show("添加对比车型到对比库");
            }
            else
            {
                MessageBox.Show("抱歉，最多可添加9款车型");
            }
        }
    }


    /// <summary>
    /// 选择页面用途枚举
    /// </summary>
    public enum CarChooseTypeEnum
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default = 0,
        /// <summary>
        /// 车型对比页选择对比车型
        /// </summary>
        CarCompare = 2,
        /// <summary>
        /// 对比列表添加对比对比车型
        /// </summary>
        CarCompareList = 1
    }
    /// <summary>
    /// 当前列表显示枚举
    /// </summary>
    public enum ShowListTypeEnum
    {
        BrandList = 0,
        SeriesList = 1,
        SpecList = 2
    }
}