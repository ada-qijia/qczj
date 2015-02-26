using AutoWP7.Handler;
using AutoWP7.Utils;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using ViewModels;
using ViewModels.Handler;
using ViewModels.Me;

namespace AutoWP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            CreateApplicationBarItems();
            this.UnreadDraftBorder.DataContext = DraftViewModel.SingleInstance;

            //推送通知
            Utils.PushNotificationHelper.ToastNotificationReceived += PushNotificationHelper_ToastNotificationReceived;
            Utils.PushNotificationHelper.OpenChannel();
        }

        //最新资讯集合
        private ObservableCollection<NewsModel> NewsDataSource = new ObservableCollection<NewsModel>();

        //加载页尺寸
        private int loadPageSize = 20;
        Thread worker;
        Thread worker1;
        public string focusImagePageIndex = "1";
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //点击推送进入
            if (this.NavigationContext.QueryString.ContainsKey("a"))
            {
                this.ToastNavigate(this.NavigationContext.QueryString);
            }

            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {
                        UmengSDK.UmengAnalytics.onEvent("ArticleActivity", "最新点击量");

                        // 最新资讯
                        worker = new Thread(NewestLoadData);
                        worker.IsBackground = true;
                        worker.Start();

                        // NewestLoadData();

                        //更新本地城市
                        worker1 = new Thread(UpdateLocalCity);
                        worker1.IsBackground = true;
                        worker1.Start();

                        //  UpdateLocalCity();
                        //设置AppBar搜索按钮可见
                        ResetAppBar();
                    }
                    break;
                case System.Windows.Navigation.NavigationMode.Back:
                    {
                        //更新本地城市
                        UpdateLocalCity();

                        HandleSaleFilterSelection();
                    }
                    break;
            }

            //更新登录状态
            UpdateMeInfo();
        }

        bool isLoaded = false;
        Thread workerCar;
        private void pano_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (pano.SelectedIndex)
            {
                case 2:
                    {
                        //品牌找车
                        if (!isLoaded)
                        {
                            workerCar = new Thread(carBrandLoadData);
                            workerCar.IsBackground = true;
                            workerCar.Start();
                            isLoaded = true;
                        }
                    }
                    break;
                case 3:
                    {
                        if (!saleLoaded)
                        {
                            SaleLoadData(false);
                        }
                    }
                    break;
                case 5:
                    UpdateMeInfo();
                    break;
            }

            //设置AppBar
            ResetAppBar();
        }

        // 更新本地城市
        public void UpdateLocalCity()
        {
            try
            {
                var setting = IsolatedStorageSettings.ApplicationSettings;
                string key = "cityId";
                if (setting.Contains(key))
                {
                    using (LocalDataContext ldc = new LocalDataContext())
                    {
                        var result = from s in ldc.provinces where s.Id == int.Parse(setting[key].ToString()) select s.Name;
                        foreach (var name in result)
                        {
                            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                chooseCity.Content = name;
                            });
                        }
                    }
                    App.CityId = setting[key].ToString();
                }
            }
            catch (Exception ex)
            {

            }
        }

        //标识找车字母索引是否打开
        bool isOpened = false;
        //退出程序
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (isSaleFilterShown)
            {
                e.Cancel = true;
                HideSaleFilter();
                return;
            }

            if (isOpened)
            {
                e.Cancel = true;
                //carBrandListGropus.CloseGroupView();
                isOpened = false;
            }
            else
            {
                if (MessageBox.Show("你真的要离开吗？", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    Common.DeleteDirectory("CachedImages");
                    Thread thread = new Thread(DeletCached);
                    thread.IsBackground = true;
                    thread.Start();
                }
                else
                {
                    e.Cancel = true;
                }
            }

            base.OnBackKeyPress(e);
        }

        // 清除缓存
        public void DeletCached()
        {

            using (LocalDataContext ldc = new LocalDataContext())
            {
                //经销商
                var deleteAllItem = from d in ldc.dealerModels where d.id > 0 select d;
                ldc.dealerModels.DeleteAllOnSubmit(deleteAllItem);
                //车系
                var deletecarSeries = from d in ldc.carSeries where d.Id > 0 select d;
                ldc.carSeries.DeleteAllOnSubmit(deletecarSeries);
                //报价
                var deleteCarQuto = from d in ldc.carQuotes where d.Id > 0 select d;
                ldc.carQuotes.DeleteAllOnSubmit(deleteCarQuto);

                ldc.SubmitChanges();
            }
            //清除缓存

        }

        #region 最新

        //计时器
        DispatcherTimer timer;
        public void NewestLoadData()
        {
            try
            {
                //如果为true,更新本地数据
                if (SetLocalLoadData())
                {
                    System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(2);
                        timer.Tick += new EventHandler(timer_Tick);
                        timer.Start();
                    });


                }
            }
            catch (Exception ex)
            {

            }
        }
        //更新本地数据
        void timer_Tick(object sender, EventArgs e)
        {
            SetNetWorkNewestLoadData(1, loadPageSize, "0", true);
            timer.Stop();
        }

        //本地加载
        LocalDataContext ldc;
        public bool SetLocalLoadData()
        {
            ldc = new LocalDataContext();
            var item = from s in ldc.newestModels where s.LocalID > 0 select s;
            if (item.Count() > 0) //本地加载
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    //绑定到前台
                    acticleListbox.ItemsSource = item;

                    GlobalIndicator.Instance.Text = "";
                    GlobalIndicator.Instance.IsBusy = false;

                });

                return true;
            }
            else //网络加载
            {
                SetNetWorkNewestLoadData(1, loadPageSize, "0", true);
                return false;
            }
        }

        HeadViewModel HeadVM = null;
        int headPageIndex = 1; //页码

        // 网络加载
        // </summary>
        public void SetNetWorkNewestLoadData(int pageIndex, int pageSize, string lastTime, bool isFirstLoad)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                GlobalIndicator.Instance.Text = "正在获取中...";
                GlobalIndicator.Instance.IsBusy = true;
            });
            if (isFirstLoad)
            {
                //重置页码
                headPageIndex = 1;
                //清除旧数据
                using (LocalDataContext ldc = new LocalDataContext())
                {
                    var item = from s in ldc.newestModels where s.LocalID > 0 select s;
                    ldc.newestModels.DeleteAllOnSubmit(item);
                    ldc.SubmitChanges();
                }
            }
            if (HeadVM == null)
            {
                HeadVM = new HeadViewModel();
                HeadVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<NewsModel>>>(comm_LoadDataCompleted);
            }
            string url = string.Format(AppUrlMgr.NewsListUrl, 0, 0, pageIndex, pageSize, lastTime);
            HeadVM.LoadDataAysnc(url, pageIndex, isFirstLoad);
        }


        // 分页加载
        // </summary>
        private void btnLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //清楚集合中的更多按钮
            NewsDataSource.RemoveAt((NewsDataSource.Count - 1));

            //分页加载
            headPageIndex++;
            SetNetWorkNewestLoadData(headPageIndex, loadPageSize, App.newsLastTime, false);
        }

        // 头条—数据加载完成
        // </summary>
        void comm_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<NewsModel>> e)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    //更新刷新状态
                    isRefreshing = false;
                    GlobalIndicator.Instance.Text = "";
                    GlobalIndicator.Instance.IsBusy = false;

                    if (e.Error != null)
                    {
                        Common.NetworkAvailablePrompt();
                    }
                    else
                    {
                        if (e.Result.Count() <= 1 && headPageIndex > 1)
                        {
                            Common.showMsg("暂无数据");
                        }
                        else
                        {

                            NewsDataSource = (ObservableCollection<NewsModel>)e.Result;
                            focusImagePageIndex = NewsDataSource[0].pageIndex;
                            acticleListbox.ItemsSource = NewsDataSource;
                            var lastnews = NewsDataSource.Where(o => !string.IsNullOrEmpty(o.lasttimeandid)).LastOrDefault();
                            if (lastnews != null)
                            {
                                App.newsLastTime = lastnews.lasttimeandid;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            });
        }

        /* 焦点图和头条 mediatype：1文章；2视频 ；3说客；
         * 最新新闻列表 mediatype：1-文章2-说客 3-视频 */

        // 文章最终页
        private void NaviGoArticleEndPage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var news = (sender as FrameworkElement).DataContext as NewsModel;
            if (news != null)
            {
                string pageindex = "1";
                int mediatype = news.mediatype;
                if (mediatype == 1 || mediatype == 2)//文章 和 说客 按同一种类型处理
                {
                    pageindex = news.pageIndex;
                    //共享文章
                    this.ShareNewsWithNewsEndPage(news);
                    this.NavigationService.Navigate(new Uri("/View/Channel/News/NewsEndPage.xaml?newsid=" + news.id + "&pageIndex=" + pageindex + "&pageType=" + mediatype, UriKind.Relative));
                }
                else if (mediatype == 3)
                {
                    this.NavigationService.Navigate(new Uri("/View/Channel/News/VideoEndPage.xaml?videoid=" + news.id, UriKind.Relative));
                }
            }
            return;
        }

        // 焦点图
        private void topImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var news = (sender as FrameworkElement).DataContext as NewsModel;
            if (news != null)
            {
                int mediatype = news.mediatype;
                if (mediatype == 1 || mediatype == 3)//文章 和 说客 按同一种类型处理
                {
                    //共享文章
                    this.ShareNewsWithNewsEndPage(news);
                    this.NavigationService.Navigate(new Uri("/View/Channel/News/NewsEndPage.xaml?newsid=" + news.id + "&pageIndex=" + focusImagePageIndex + "&pageType=" + mediatype, UriKind.Relative));
                }
                else if (mediatype == 2)
                {
                    this.NavigationService.Navigate(new Uri("/View/Channel/News/VideoEndPage.xaml?videoid=" + news.id, UriKind.Relative));
                }
            }
        }

        private void ShareNewsWithNewsEndPage(NewsModel news)
        {
            Model.Me.FavoriteArticleModel favoriteModel = new Model.Me.FavoriteArticleModel();
            favoriteModel.ID = news.id;
            favoriteModel.Img = news.imgurl;
            favoriteModel.Title = news.title;
            favoriteModel.PublishTime = news.time;
            int reply;
            if (int.TryParse(news.replycount, out reply))
            {
                favoriteModel.ReplyCount = reply;
            }
            View.Channel.News.NewsEndPage.ShareState(favoriteModel);
        }

        //刷新状态
        bool isRefreshing = false;
        //刷新
        private void refresh_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (isRefreshing == false)
            {
                isRefreshing = true;

                //刷新点击统计
                UmengSDK.UmengAnalytics.onEvent("refresh", "刷新点击量");

                if (NewsDataSource.Count() > 0)
                {
                    NewsDataSource.Clear();
                }
                //隐藏刷新按钮
                // refreshImg.Visibility = Visibility.Collapsed;

                //刷新数据
                SetNetWorkNewestLoadData(1, loadPageSize, "0", true);
            }

        }

        #endregion

        #region  找车

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
                        CarBrandGroup group = new CarBrandGroup(entity.key);

                        foreach (var item in entity)
                        {
                            group.Add(item);
                        }
                        carBrandSource.Add(group);
                    }
                    carBrandListGropus.ItemsSource = carBrandSource;

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

        CarBrandViewModel carVM = null;

        // 品牌找车
        public void SetWebCarBrandLoadData()
        {
            if (carVM == null)
            {
                carVM = new CarBrandViewModel();
            }
            try
            {
                string url = string.Format("{0}{1}/cars/brands-{2}-ts{3}.html", App.appUrl, App.versionStr, App.AppInfo, 0);
                carVM.LoadDataAysnc(url);
                carVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<CarBrandModel>>>(carVM_LoadDataCompleted);
            }
            catch (Exception ex)
            {

            }
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
                        CarBrandGroup group = new CarBrandGroup(entity.key);

                        foreach (var item in entity)
                        {
                            group.Add(item);
                        }
                        carBrandSource.Add(group);
                    }
                    carBrandListGropus.ItemsSource = carBrandSource;

                }

                GlobalIndicator.Instance.Text = "";
                GlobalIndicator.Instance.IsBusy = false;

            });
        }

        private void carBrandListButton_Click(object sender, RoutedEventArgs e)
        {
            carFinderFilterPanel.Visibility = Visibility.Collapsed;
            carBrandListGropus.Visibility = Visibility.Visible;
        }

        private void carFinderFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            carFinderFilterPanel.Visibility = Visibility.Visible;
            carBrandListGropus.Visibility = Visibility.Collapsed;
        }

        //条件找车
        private void carSearchButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/CarSearch/CarSearchPage.xaml", UriKind.Relative));
        }

        #endregion

        #region 降价

        bool saleLoaded = false;

        SaleViewModel saleVM = null;

        string sale_param_pi = "0";
        string sale_param_c = "0";
        string sale_param_o = "0";
        string sale_param_b = "0";
        string sale_param_ss = "0";
        string sale_param_sp = "0";
        string sale_param_l = "0";
        string sale_param_minp = "0";
        string sale_param_maxp = "0";

        private void SaleReloadData()
        {
            SaleLoadData(true);
        }

        private void SaleLoadData(bool reload)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            if (saleVM == null)
            {
                saleVM = new SaleViewModel();
                saleVM.LoadDataCompleted += SaleVM_LoadDataCompleted;
                saleListbox.ItemsSource = saleVM.DataSource;
            }

            int page_index = reload ? 1 : saleVM.PageIndex + 1;

            //http://221.192.136.99:804/wpv1.6/dealer/pdspecs-a2-pm3-v1.6.0-pi0-c0-o0-b0-ss0-sp0-p1-s20-l0-minp10-maxp20.html
            string format = App.appUrl + App.versionStr + "/dealer/pdspecs-" + App.AppInfo + "-pi{0}-c{1}-o{2}-b{3}-ss{4}-sp{5}-p{6}-s{7}-l{8}-minp{9}-maxp{10}.html";
            string url = string.Format(format, sale_param_pi, sale_param_c, sale_param_o, sale_param_b,
                sale_param_ss, sale_param_sp, page_index, saleVM.PageSize, sale_param_l, sale_param_minp, sale_param_maxp);
            saleVM.LoadDataAysnc(url, reload);
        }

        void SaleVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<SaleItemModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                saleLoaded = true;
                if (saleVM.DataSource.Count == 0)
                {
                    Common.showMsg("暂无降价活动");
                }
                else if (saleVM.IsEndPage)
                {
                    Common.showMsg("已经是最后一页了");
                }

                if (isSaleFilterShown)
                {
                    HideSaleFilter();
                }
            }
        }

        private void saleLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SaleLoadData(false);
        }


        /************** Filter ****************/
        bool isSaleFilterShown = false;

        private void saleFilterButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowSaleFilter();
        }

        private void saleItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var data = sender.GetDataContext<SaleItemModel>();
            string url = string.Format("/View/Sale/SaleDetailPage.xaml?seriesid={0}&specid={1}&articleid={2}&articletype={3}&dealerid={4}",
                data.seriesid, data.specid, data.articleid, data.articletype, data.dealer.id);
            this.NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }

        private void saleItem_CallDealer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var data = sender.GetDataContext<SaleItemModel>();
            UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "电话拨打点击量");
            Image bb = (Image)sender;
            PhoneCallTask phoneCall = new PhoneCallTask();
            phoneCall.PhoneNumber = data.dealer.phone;
            phoneCall.Show();
        }

        private void saleItem_CallPrice_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var data = sender.GetDataContext<SaleItemModel>();
            string cityId = string.Empty;
            if (string.IsNullOrEmpty(App.CityId)) //默认北京
            {
                cityId = "110100";
            }
            else
            {
                cityId = App.CityId;
            }
            string url = string.Format("/View/Car/AskPrice.xaml?dealerid={0}&cityID={1}&seriesID={2}&specID={3}", data.dealer.id, cityId, data.seriesid, data.specid);
            this.NavigationService.Navigate(new Uri(url, UriKind.Relative));
        }

        protected void ShowSaleFilter()
        {
            VisualStateManager.GoToState(this, "VSSaleFilterShown", true);
            isSaleFilterShown = true;
        }

        protected void HideSaleFilter()
        {
            VisualStateManager.GoToState(this, "VSSaleFilterHidden", true);
            isSaleFilterShown = false;
        }

        private void SaleFilter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //reset the key, and if user press backbutton without any selection, this page knows and ignores it
            App.SaleFilterSelector_FilterType = string.Empty;

            string tag = ((FrameworkElement)sender).Tag.ToString();
            //HideSaleFilter();
            if (tag == "brand")
            {
                this.NavigationService.Navigate(new Uri("/View/Sale/SaleBrandFilterPage.xaml", UriKind.Relative));
            }
            else if (tag == "area")
            {
                this.NavigationService.Navigate(new Uri("/View/Sale/SaleCityFilterPage.xaml", UriKind.Relative));
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/View/Sale/SaleFilterSelectorPage.xaml?filterType=" + tag, UriKind.Relative));
            }
        }

        private void HandleSaleFilterSelection()
        {
            if (string.IsNullOrEmpty(App.SaleFilterSelector_FilterType))
            {
                return;
            }

            switch (App.SaleFilterSelector_FilterType)
            {
                case "country":
                    sale_param_pi = "0";
                    sale_param_c = "0";
                    SaleFilter1.Content = "地区";
                    break;
                case "province":
                    sale_param_pi = App.SaleFilterSelector_SelectedValue;
                    sale_param_c = "0";
                    SaleFilter1.Content = App.SaleFilterSelector_SelectedName;
                    break;
                case "city":
                    sale_param_pi = "0";
                    sale_param_c = App.SaleFilterSelector_SelectedValue;
                    SaleFilter1.Content = App.SaleFilterSelector_SelectedName;
                    break;
                case "price":
                    var strArray = App.SaleFilterSelector_SelectedValue.Split('|');
                    sale_param_minp = strArray[0];
                    sale_param_maxp = strArray[1];
                    if (App.SaleFilterSelector_SelectedValue == "0|0")
                    {
                        SaleFilter3.Content = "价格";
                    }
                    else
                    {
                        SaleFilter3.Content = App.SaleFilterSelector_SelectedName;
                    }
                    break;
                case "level":
                    sale_param_l = App.SaleFilterSelector_SelectedValue;
                    if (App.SaleFilterSelector_SelectedValue == "0")
                    {
                        SaleFilter4.Content = "级别";
                    }
                    else
                    {
                        SaleFilter4.Content = App.SaleFilterSelector_SelectedName;
                    }
                    break;
                case "buyorder":
                    sale_param_o = App.SaleFilterSelector_SelectedValue;
                    if (App.SaleFilterSelector_SelectedValue == "0")
                    {
                        SaleFilter5.Content = "排序";
                    }
                    else
                    {
                        SaleFilter5.Content = App.SaleFilterSelector_SelectedName;
                    }
                    break;
                case "b":
                    sale_param_b = App.SaleFilterSelector_SelectedValue;
                    sale_param_ss = "0";
                    sale_param_sp = "0";
                    SaleFilter2.Content = App.SaleFilterSelector_SelectedName;
                    break;
                case "ss":
                    sale_param_b = "0";
                    sale_param_ss = App.SaleFilterSelector_SelectedValue;
                    sale_param_sp = "0";
                    SaleFilter2.Content = App.SaleFilterSelector_SelectedName;
                    break;
                case "sp":
                    sale_param_b = "0";
                    sale_param_ss = "0";
                    sale_param_sp = App.SaleFilterSelector_SelectedValue;
                    SaleFilter2.Content = App.SaleFilterSelector_SelectedName;
                    break;
                default:
                    break;
            }
            SaleLoadData(true);
            App.SaleFilterSelector_FilterType = string.Empty;
        }

        #endregion

        #region  频道

        // for all
        private void ChannelTile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string tag = (sender as MyPhoneControls.HubTile).Title;
            this.NavigationService.Navigate(CreateChannelUrl(tag));
        }

        //// 新闻
        //private void newsHub_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    string tag = (sender as HubTile).Title;
        //    this.NavigationService.Navigate(CreateChannelUrl(tag));
        //}

        //// 视频
        //private void videoHub_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    this.NavigationService.Navigate(CreateChannelUrl(1));
        //}

        //// 评测
        //private void evaluatingHub_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    this.NavigationService.Navigate(CreateChannelUrl(1));
        //}

        //// 行情
        //private void quotationsHub_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    this.NavigationService.Navigate(CreateChannelUrl(2));
        //}

        //// 导购
        //private void shoppingGuidHub_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    this.NavigationService.Navigate(CreateChannelUrl(3));
        //}

        //// 用车
        //private void useCarHub_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    this.NavigationService.Navigate(CreateChannelUrl(4));
        //}

        //// 文化
        //private void cultureHub_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    this.NavigationService.Navigate(CreateChannelUrl(5));
        //}

        //// 改装
        //private void changeHub_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    this.NavigationService.Navigate(CreateChannelUrl(6));
        //}

        //// 说客
        //private void competitionHub_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    this.NavigationService.Navigate(CreateChannelUrl(7));
        //}

        //// 游记
        //private void travelsHub_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    this.NavigationService.Navigate(CreateChannelUrl(8));

        //}

        //// 技术
        //private void technologyhub_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    this.NavigationService.Navigate(CreateChannelUrl(9));
        //}

        ////原创视频
        //private void originalVideoHub_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        //{
        //    this.NavigationService.Navigate(CreateChannelUrl(11));
        //}

        #endregion

        // 导向车系列表页
        private void carBrandIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid stac = (Grid)sender;
            this.NavigationService.Navigate(new Uri("/View/Car/CarSeriesListPage.xaml?id=" + stac.Tag, UriKind.Relative));
        }

        private Uri CreateClubListUrl(int index)
        {
            return new Uri(string.Format("/View/Forum/ForumListPage.xaml?indexId={0}", index), UriKind.Relative);
        }

        //导向我的论坛
        private void myForum_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(CreateClubListUrl(3));
        }

        //导向车系论坛
        private void carSeriesForum_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(CreateClubListUrl(0));
        }

        //导向地区论坛
        private void areaForum_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(CreateClubListUrl(1));
        }

        //导向主题论坛
        private void subjectForum_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(CreateClubListUrl(2));
        }

        //城市选择
        private void chooseCity_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "城市选择点击量");
            this.NavigationService.Navigate(new Uri("/View/Channel/News/CityListPage.xaml", UriKind.Relative));
        }

        //关于
        private void about_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("AboutActivity", "关于点击量");
            this.NavigationService.Navigate(new Uri("/View/More/AboutPage.xaml", UriKind.Relative));
        }

        //反馈
        private void feedback_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("FeedbackActivity", "反馈点击量");
            this.NavigationService.Navigate(new Uri("/View/More/FeedBackPage.xaml", UriKind.Relative));
        }

        //更多
        private void moreIconButton_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("MoreActivity", "更多页点击量");
            this.NavigationService.Navigate(new Uri("/View/More/MorePage.xaml", UriKind.Relative));
        }

        // 新闻频道页
        private Uri CreateChannelUrl(string tag)
        {
            return new Uri(string.Format("/View/Channel/NewsListPage.xaml?tag={0}", tag), UriKind.Relative);
        }

        #region 设置AppBar搜索按钮

        ApplicationBarIconButton searchButton;
        ApplicationBarMenuItem settingItem;
        ApplicationBarIconButton refreshButton;

        private void CreateApplicationBarItems()
        {
            refreshButton = new ApplicationBarIconButton();
            refreshButton.IconUri = new Uri("/Images/refresh.png", UriKind.Relative);
            refreshButton.Text = "刷新";
            refreshButton.Click += refresh_Click;

            settingItem = new ApplicationBarMenuItem();
            settingItem.Text = "设置";
            settingItem.Click += settingItem_Click;

            searchButton = new ApplicationBarIconButton();
            searchButton.IconUri = new Uri("/Images/bar_search.png", UriKind.Relative);
            searchButton.Text = "搜索";
            searchButton.Click += searchButton_Click;
        }

        private void ResetAppBar()
        {
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();

            //为最新和论坛设置AppBar搜索按钮
            bool searchButtonVisible = pano.SelectedIndex == 0 || pano.SelectedIndex == 2 || pano.SelectedIndex == 4;
            if (searchButtonVisible)
            {
                ApplicationBar.Buttons.Add(searchButton);
                ApplicationBar.Mode = ApplicationBarMode.Default;
            }
            else if (pano.SelectedIndex == 5)//我
            {
                if (Common.isLogin())
                {
                    ApplicationBar.Buttons.Add(refreshButton);
                }
                ApplicationBar.MenuItems.Add(settingItem);
                ApplicationBar.Mode = ApplicationBarMode.Default;
            }
            else
            {
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
            }
        }

        //导航到搜索页面
        private void searchButton_Click(object sender, EventArgs e)
        {
            //最新搜索为综合，找车搜索为车系，论坛搜索为论坛
            SearchType type = SearchType.General;
            switch (pano.SelectedIndex)
            {
                case 0:
                    type = SearchType.General;
                    break;
                case 2:
                    type = SearchType.CarSeries;
                    break;
                case 4:
                    type = SearchType.Forum;
                    break;
                default:
                    break;
            }
            string searchPageUrl = View.Search.SearchPage.GetSearchPageUrlWithParams(type);
            this.NavigationService.Navigate(new Uri(searchPageUrl, UriKind.Relative));
        }

        #endregion

        #region 我

        MeViewModel MeVM;

        private void UpdateMeInfo()
        {
            if (MeVM == null)
            {
                MeVM = new MeViewModel();
                MeVM.LoadDataCompleted += MeVM_LoadDataCompleted;
                this.MePanoItem.DataContext = MeVM;
            }

            if (Common.isLogin())
            {
                if (MeVM.Model == null)
                {
                    GlobalIndicator.Instance.Text = "正在获取中...";
                    GlobalIndicator.Instance.IsBusy = true;

                    string url = MeHelper.GetUserInfoUrl();
                    MeVM.LoadDataAysnc(url);
                }
            }
            else
            {
                MeVM.ClearData();
            }

            ResetAppBar();
        }

        void MeVM_LoadDataCompleted(object sender, EventArgs e)
        {
            ResetAppBar();

            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
        }

        private void settingItem_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Me/Settings.xaml", UriKind.Relative));
        }

        private void refresh_Click(object sender, EventArgs e)
        {
            if (MeVM != null)
            {
                MeVM.ClearData();
            }
            UpdateMeInfo();
        }

        private void Login_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //未登录，跳转到登录页
            this.NavigationService.Navigate(new Uri("/View/More/LoginPage.xaml", UriKind.Relative));
        }

        //进入个人资料页
        private void MyInfo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            PhoneApplicationService.Current.State[Utils.MeHelper.MyInfoStateKey] = this.MeVM.Model;
            this.NavigationService.Navigate(new Uri("/View/Me/MyInfoDetail.xaml", UriKind.Relative));
        }

        //评论回复
        private void CommentReply_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Me/MyCommentReply.xaml", UriKind.Relative));
        }

        //论坛回复
        private void ForumReply_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Me/MyForumReply.xaml", UriKind.Relative));
        }

        //私信
        private void PrivateMessage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Me/PrivateMessageFriends.xaml", UriKind.Relative));
        }

        private void MyCollection_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Me/MyFavorite.xaml", UriKind.Relative));
        }

        private void MyTritan_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Me/MyTritan.xaml", UriKind.Relative));
        }

        private void DraftBox_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Me/DraftBox.xaml", UriKind.Relative));
        }

        private void Recent_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Me/MyRecents.xaml", UriKind.Relative));
        }

        private void CarCompare_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("对比", "对比点击量");
            this.NavigationService.Navigate(new Uri("/View/Car/CarCompareListPage.xaml?action=0", UriKind.Relative));
        }

        #endregion

        #region 推送通知

        private void PushNotificationHelper_ToastNotificationReceived(object sender, ToastNotificationEventArgs e)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (MessageBox.Show(e.Subtitle, e.Title, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        var argArray = e.CustomParam.Split(new char[] { '?', '&', '=' });
                        if (argArray.Length > 1)
                        {
                            Dictionary<string, string> args = new Dictionary<string, string>();
                            for (int i = 1; i < argArray.Length - 1; i = i + 2)
                            {
                                args.Add(argArray[i], argArray[i + 1]);
                            }
                            ToastNavigate(args);
                        }
                    }
                });
        }

        private void ToastNavigate(IDictionary<string, string> args)
        {
            //分析参数，导航到各页面
            if (args.ContainsKey("t"))
            {
                switch (args["t"])
                {
                    //评论 回复
                    case "1":
                        NavigationService.Navigate(new Uri("/View/Me/MyCommentReply.xaml", UriKind.Relative));
                        break;
                    //论坛 回复
                    case "2":
                        NavigationService.Navigate(new Uri("/View/Me/MyForumReply.xaml", UriKind.Relative));
                        break;
                    //系统消息
                    case "3":
                        //浏览器打开url
                        if (args.ContainsKey("p1") && (!string.IsNullOrEmpty(args["p1"])))
                        {
                            string url = UrlDecoder.UrlDecode(args["p1"]);
                            this.wb.Navigate(new Uri(url, UriKind.Absolute));
                            this.SystemMsgGrid.Visibility = Visibility.Visible;
                            this.ApplicationBar.IsVisible = false;
                        }
                        break;
                    //私信
                    case "5":
                        NavigationService.Navigate(new Uri("/View/Me/PrivateMessageFriends.xaml", UriKind.Relative));
                        break;
                    default:
                        break;
                }
            }
        }

        //关闭浏览器
        private void CloseBrowser_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.SystemMsgGrid.Visibility = Visibility.Collapsed;
            this.ApplicationBar.IsVisible = true;
        }

        //页面加载中
        private void wb_Navigating(object sender, NavigatingEventArgs e)
        {
            this.failPrompt.Visibility = Visibility.Collapsed;
            this.ProgBar.Visibility = Visibility.Visible;
        }

        //页面加载完成
        private void wb_Navigated(object sender, NavigationEventArgs e)
        {
            this.ProgBar.Visibility = Visibility.Collapsed;
        }

        //加载失败
        private void wb_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            this.ProgBar.Visibility = Visibility.Collapsed;
            this.failPrompt.Visibility = Visibility.Visible;
        }

        #endregion

    }
}