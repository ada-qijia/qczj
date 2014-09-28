using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Model;
using ViewModels;
using ViewModels.Handler;
using System.Windows;

namespace AutoWP7.View.Car
{
    /// <summary>
    /// 车系报价详情页
    /// </summary>
    public partial class CarSeriesQuotePage : PhoneApplicationPage
    {
        public CarSeriesQuotePage()
        {
            InitializeComponent();
            koubeiListBox.ItemsSource = koubeiList;
        }
        //车系id
        string seriesID = string.Empty;
        //车型id
        string carId = string.Empty;
        //城市id
        string cityId = string.Empty;

        int pageSize = 20;

        /// <summary>
        /// 是否显示参数配置，决定是否可以添加对比
        /// </summary>
        int paramIsShow = 0;

        public ApplicationBarIconButton AddVS = new ApplicationBarIconButton()
        {
            IconUri = new Uri("/Images/car_addvs.png", UriKind.Relative),
            Text = "添加"
        };
        public ApplicationBarIconButton ToVS = new ApplicationBarIconButton()
        {
            IconUri = new Uri("/Images/vs1.png", UriKind.Relative),
            Text = "对比"
        };

        public ObservableCollection<DealerModel> dealerDataSource = new ObservableCollection<DealerModel>();
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (this.NavigationContext.QueryString.ContainsKey("paramisshow"))
            {
                paramIsShow = Convert.ToInt16(this.NavigationContext.QueryString["paramisshow"]);
            }

            InitBtn();

            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {
                        App.timerId++;
                        //将车型id存放于全局
                        App.CarTypeId = this.NavigationContext.QueryString["carId"];
                        carId = App.CarTypeId;
                        seriesID = App.CarSeriesId;
                        if (string.IsNullOrEmpty(App.CityId))
                        {
                            App.CityId = "110100";
                        }
                        cityId = App.CityId;

                        //车系名
                        using (LocalDataContext ldc = new LocalDataContext())
                        {
                            var name = from s in ldc.carQuotes where s.Id == int.Parse(carId) select s.Name;
                            foreach (var n in name)
                            {
                                carTypeName.Text = n;
                            }
                        }

                        if (this.NavigationContext.QueryString.ContainsKey("selectedPage"))
                        {
                            string selectedPage = this.NavigationContext.QueryString["selectedPage"];
                            switch (selectedPage)
                            {
                                case "alibi":
                                    piv.SelectedIndex = 2;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    break;
                case System.Windows.Navigation.NavigationMode.Back:
                    {
                        if (!string.IsNullOrEmpty(App.CityId))
                        {
                            using (LocalDataContext ldc = new LocalDataContext())
                            {
                                var result = from s in ldc.provinces where s.Id == int.Parse(App.CityId) select s.Name;
                                foreach (var name in result)
                                {
                                    qutoChooseCity.Content = name;
                                }
                            }
                        }

                        if (piv.SelectedIndex == 0)
                        {
                            if (!string.IsNullOrEmpty(App.CityId))
                            {
                                cityId = App.CityId;
                                DealerLoadData();
                            }
                        }
                    }
                    break;

            }

        }

        private void InitBtn()
        {
            AddVS.Click += AddVS_Click;
            ToVS.Click += ToVS_Click;
        }

        /// <summary>
        /// 去对比
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToVS_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Car/CarCompareListPage.xaml?action=0", UriKind.Relative));
        }

        /// <summary>
        /// 添加到对比库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AddVS_Click(object sender, EventArgs e)
        {
            CarCompareViewModel model = new CarCompareViewModel()
            {
                SpecId = Convert.ToInt32(App.CarTypeId),
                SpecName = carTypeName.Text,
                IsChoosed = false,
                SeriesId = Convert.ToInt32(App.CarSeriesId),
                SeriesName = App.CarSeriesName
            };
            int resultCode = model.AddCompareSpec(model);
            if (resultCode == 0)
                Common.showMsg("添加成功");
            else if (resultCode == 2)
                Common.showMsg("抱歉，最多可添加9款车型");
            else if (resultCode == 1)
                Common.showMsg("此车型已添加");
            else
            { }
        }

        //标志是否加载数据
        bool isDealerLoaded = false;
        bool isCarSeriesConfigLoaded = false;
        bool isAlibiLoaded = false;

        private void piv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ApplicationBar.Buttons.Clear();
            this.ApplicationBar.IsVisible = true;
            //商用车specid>1000000和不显示参数配置paramIsShow=0的车型不能对比
            if (!string.IsNullOrEmpty(carId) && Convert.ToInt32(carId) < 1000000 && paramIsShow == 1)
            {
                this.ApplicationBar.Buttons.Add(AddVS);
            }
            this.ApplicationBar.Buttons.Add(ToVS);
            switch (this.piv.SelectedIndex)
            {
                case 0: //经销商
                    {
                        UmengSDK.UmengAnalytics.onEvent("MotorcycleTypeActivity", "车型页~经销商点击量");
                        if (!isDealerLoaded)
                        {
                            DealerLoadData();
                            isDealerLoaded = true;
                        }
                    }
                    break;
                case 1: //配置参数
                    {
                        UmengSDK.UmengAnalytics.onEvent("MotorcycleTypeActivity", "车型参数配置点击量");
                        if (!isCarSeriesConfigLoaded)
                        {
                            CarSeriesConfigLoadData();
                            isCarSeriesConfigLoaded = true;
                        }
                    }
                    break;
                case 2: //口碑
                    this.ApplicationBar.IsVisible = false;
                    UmengSDK.UmengAnalytics.onEvent("MotorcycleTypeActivity", "车型页~口碑点击量");
                    if (!isAlibiLoaded)
                    {
                        AlibiLoadData();
                        isAlibiLoaded = true;
                    }
                    break;
                case 3: //图片
                    this.ApplicationBar.IsVisible = false;
                    break;
            }
        }

        #region 配置参数

        private class CarSeriesConfigGroup : List<CarSeriesConfigurationModel>
        {
            public CarSeriesConfigGroup(string category)
            {
                key = category;
            }

            public string key { get; set; }
            public bool HasItems { get { return Count > 0; } }
        }

        private class CarSeriesConfigSource : List<CarSeriesConfigGroup>
        {

        }

        CarSeriesConfigSource carSeriesConfigSource = new CarSeriesConfigSource();

        CarSeriesConfigurationViewModel carSeriesConfigVM = null;
        /// <summary>
        /// 配置参数
        /// </summary>
        public void CarSeriesConfigLoadData()
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;
            if (carSeriesConfigVM == null)
            {
                carSeriesConfigVM = new CarSeriesConfigurationViewModel();
            }
            string url = string.Format("{0}{2}/cars/speccompare-a2-pm3-v1.4.0-t1-s{1}.html", App.appUrl, carId, App.versionStr);
            carSeriesConfigVM.LoadDataAysnc(url);
            carSeriesConfigVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<Model.CarSeriesConfigurationModel>>>(carSeriesConfigVM_LoadDataCompleted);

        }

        void carSeriesConfigVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<Model.CarSeriesConfigurationModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            try
            {
                if (e.Error != null)
                {
                    Common.NetworkAvailablePrompt();
                }
                else
                {
                    // orderby c.Key  ascending 
                    var groupBy = from car in e.Result
                                  group car by car.GroupName into c
                                  select new Group<CarSeriesConfigurationModel>(c.Key, c);


                    foreach (var entity in groupBy)
                    {
                        CarSeriesConfigGroup group = new CarSeriesConfigGroup(entity.key);

                        foreach (var item in entity)
                        {
                            group.Add(item);
                        }
                        carSeriesConfigSource.Add(group);
                    }
                    carSeriesConfigListGropus.ItemsSource = carSeriesConfigSource;
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region 经销商

        DealerViewModel DealerVM = null;

        public void DealerLoadData()
        {
            //从服务器中获得数据
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            using (LocalDataContext ldc = new LocalDataContext())
            {
                if (!string.IsNullOrEmpty(App.CityId))
                {
                    var result = from s in ldc.provinces where s.Id == int.Parse(App.CityId) select s.Name;
                    foreach (var name in result)
                    {
                        qutoChooseCity.Content = name;
                    }
                }

                //清除数据库中的数据
                var deleteAllItem = from d in ldc.dealerModels where d.id > 0 select d;
                ldc.dealerModels.DeleteAllOnSubmit(deleteAllItem);
                ldc.SubmitChanges();

                ////先从本地数据库中获得数据
                //var queryResult = from d in ldc.dealerModels where d.CarId == int.Parse(carId) && d.CityId == int.Parse(cityId) select d;

                //if (queryResult.Count() > 0)
                //{
                //    dealerListbox.ItemsSource = queryResult;
                //    GlobalIndicator.Instance.Text = "";
                //    GlobalIndicator.Instance.IsBusy = false;
                //}

                //else
                //{

                DealerVM = new DealerViewModel();
                string url = string.Format("{0}{4}/dealer/pddealers-a2-pm3-v1.4.0-sp{1}-ss{2}-c{3}-sc0-p1-s200.html", App.appUrl, carId, 0, cityId, App.versionStr);
                DealerVM.LoadDataAysnc(url, carId, cityId);
                DealerVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<DealerModel>>>(DealerVM_LoadDataCompleted);
            }
        }

        void DealerVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<DealerModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            //清空数据
            dealerDataSource.Clear();

            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                if (e.Result.Count() < 1)
                {
                    Common.showMsg("暂无经销商信息");
                }
                else
                {
                    foreach (DealerModel model in e.Result)
                    {
                        dealerDataSource.Add(model);
                    }

                    dealerListbox.ItemsSource = dealerDataSource;
                }
            }
        }

        private void qutoChooseCity_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("MotorcycleTypeActivity", "车型页城市选择点击量");
            this.NavigationService.Navigate(new Uri("/View/Channel/News/CityListPage.xaml?action=2", UriKind.Relative));
        }

        private void callDealer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "电话拨打点击量");
            Image bb = (Image)sender;
            PhoneCallTask phoneCall = new PhoneCallTask();
            phoneCall.PhoneNumber = bb.Tag.ToString();
            phoneCall.Show();
        }

        private void callPrice_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (string.IsNullOrEmpty(App.CityId)) //默认北京
            {
                cityId = "110100";
            }
            else
            {
                cityId = App.CityId;
            }
            Image gg = (Image)sender;
            this.NavigationService.Navigate(new Uri("/View/Car/AskPrice.xaml?dealerid=" + gg.Tag + "&cityID=" + cityId + "&seriesID=" + seriesID + "&specID=" + carId, UriKind.Relative));
        }

        private void dealerDeatail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image bb = (Image)sender;
            this.NavigationService.Navigate(new Uri("/View/Car/DealerDetailPage.xaml?id=" + bb.Tag, UriKind.Relative));
        }

        #endregion

        #region 图片

        //车身外观
        private void facadeGo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Car/carSeriesImagePage.xaml?carId=" + carId + "&type=2&indexId=0", UriKind.Relative));
        }
        //中控
        private void wheelGo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            this.NavigationService.Navigate(new Uri("/View/Car/carSeriesImagePage.xaml?carId=" + carId + "&type=2&indexId=1", UriKind.Relative));
        }
        //车厢
        private void comparmentGo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            this.NavigationService.Navigate(new Uri("/View/Car/carSeriesImagePage.xaml?carId=" + carId + "&type=2&indexId=2", UriKind.Relative));
        }
        //其它
        private void elseGo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            this.NavigationService.Navigate(new Uri("/View/Car/carSeriesImagePage.xaml?carId=" + carId + "&type=2&indexId=3", UriKind.Relative));
        }

        #endregion

        #region 口碑

        SpecAlibiViewModel alibiVM = null;
        ObservableCollection<KoubeiModel> koubeiList = new ObservableCollection<KoubeiModel>();
        int alibiPageIndex = 1;
        KoubeiModel koubeiMoreButtonItem = new KoubeiModel() { IsMoreButton = true };

        public void AlibiLoadData()
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            if (alibiVM == null)
            {
                alibiVM = new SpecAlibiViewModel();
                alibiVM.LoadDataCompleted += alibiVM_LoadDataCompleted;
            }

            //string url = "http://app.api.autohome.com.cn/wpv1.4/alibi/specalibilist-a2-pm3-v1.5.0-sp12129-p1-s20.html";
            string url = string.Format("{0}{1}/alibi/specalibilist-{2}-sp{3}-p{4}-s{5}.html", App.appUrl, App.versionStr, App.AppInfo, App.CarTypeId, alibiPageIndex, pageSize);
            alibiVM.LoadDataAysnc(url);
        }

        void alibiVM_LoadDataCompleted(object sender, APIEventArgs<SpecAlibiModel> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                SpecAlibiModel data = e.Result;
                alibiPanel.DataContext = data;

                TryRemoveMoreButton();
                if (data.Koubeis != null)
                {
                    foreach (var item in data.Koubeis)
                    {
                        koubeiList.Add(item);
                    }
                    if (data.PageIndex < data.PageCount)
                    {
                        EnsureMoreButton();
                    }
                }

                if (data.Grades != null && data.Grades.Count == 8)
                {
                    alibiChart.SetColumns(
                        data.Grades["空间"].Grade, data.Grades["动力"].Grade, data.Grades["操控"].Grade, data.Grades["油耗"].Grade,
                        data.Grades["舒适性"].Grade, data.Grades["外观"].Grade, data.Grades["内饰"].Grade, data.Grades["性价比"].Grade);
                }
            }
        }

        private void alibiLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            alibiPageIndex++;
            AlibiLoadData();
        }

        private void TryRemoveMoreButton()
        {
            if (koubeiList.Contains(koubeiMoreButtonItem))
            {
                koubeiList.Remove(koubeiMoreButtonItem);
            }
        }

        private void EnsureMoreButton()
        {
            if (!koubeiList.Contains(koubeiMoreButtonItem))
            {
                koubeiList.Add(koubeiMoreButtonItem);
            }
        }

        private void koubei_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            KoubeiModel koubei = (sender as FrameworkElement).DataContext as KoubeiModel;
            if (koubei != null)
            {
                this.NavigationService.Navigate(new Uri(string.Format("/View/Car/AlibiDetailPage.xaml?id={0}&koubeiImage={1}", koubei.ID, koubei.MedalImage), UriKind.Relative));
            }
        }

        #endregion

    }
}