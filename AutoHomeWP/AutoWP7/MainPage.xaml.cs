using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Model;
using ViewModels;
using ViewModels.Handler;
using System.Windows.Media.Animation;
using System.Windows.Media;
using Microsoft.Phone.Data.Linq;

namespace AutoWP7
{
    // 主界面
    // </summary>
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
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

                    }
                    break;
                case System.Windows.Navigation.NavigationMode.Back:
                    {
                        //更新本地城市
                        UpdateLocalCity();
                    }
                    break;
            }
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
            }
        }

        // 更新本地城市
        public void UpdateLocalCity()
        {
            var setting = IsolatedStorageSettings.ApplicationSettings;

            if (setting.Contains(Global.SettingKey_CityID))
            {
                App.CityId = setting[Global.SettingKey_CityID].ToString();
            }
            if (setting.Contains(Global.SettingKey_CityName))
            {
                App.CityName = setting[Global.SettingKey_CityName].ToString();
                chooseCity.Content = App.CityName;
            }

            //try
            //{
            //    var setting = IsolatedStorageSettings.ApplicationSettings;
            //    string key = "cityId";
            //    if (setting.Contains(key))
            //    {
            //        using (LocalDataContext ldc = new LocalDataContext())
            //        {
            //            var result = from s in ldc.provinces where s.Id == int.Parse(setting[key].ToString()) select s.Name;
            //            foreach (var name in result)
            //            {
            //                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            //                {
            //                        chooseCity.Content = name;
            //                });
            //            }
            //        }
            //        App.CityId = setting[key].ToString();
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
        }

        //标识找车字母索引是否打开
        bool isOpened = false;
        //退出程序
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

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
                    //Thread thread = new Thread(DeletCached);
                    //thread.IsBackground = true;
                    //thread.Start();
                }
                else
                {
                    e.Cancel = true;
                }

            }
        }

        // 清除缓存
        //public void DeletCached()
        //{

        //    using (LocalDataContext ldc = new LocalDataContext())
        //    {
        //        //经销商
        //        var deleteAllItem = from d in ldc.dealerModels where d.id > 0 select d;
        //        ldc.dealerModels.DeleteAllOnSubmit(deleteAllItem);
        //        //车系
        //        var deletecarSeries = from d in ldc.carSeries where d.Id > 0 select d;
        //        ldc.carSeries.DeleteAllOnSubmit(deletecarSeries);
        //        //报价
        //        var deleteCarQuto = from d in ldc.carQuotes where d.Id > 0 select d;
        //        ldc.carQuotes.DeleteAllOnSubmit(deleteCarQuto);

        //        ldc.SubmitChanges();
        //    }
        //    //清除缓存

        //}

        #region 头条数据加载

        //计时器
        DispatcherTimer timer;
        public void NewestLoadData()
        {
            SetNetWorkNewestLoadData(1, loadPageSize, "0", true);
            
            //try
            //{
            //    //如果为true,更新本地数据
            //    if (SetLocalLoadData())
            //    {
            //        System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            //        {
            //            timer = new DispatcherTimer();
            //            timer.Interval = TimeSpan.FromSeconds(2);
            //            timer.Tick += new EventHandler(timer_Tick);
            //            timer.Start();
            //        });


            //    }
            //}
            //catch (Exception ex)
            //{

            //}
        }

        ////更新本地数据
        //void timer_Tick(object sender, EventArgs e)
        //{
        //    SetNetWorkNewestLoadData(1, loadPageSize, "0", true);
        //    timer.Stop();
        //}

        ////本地加载
        //LocalDataContext ldc;
        //public bool SetLocalLoadData()
        //{
        //    ldc = new LocalDataContext();
        //    var item = from s in ldc.newestModels where s.LocalID > 0 select s;
        //    if (item.Count() > 0) //本地加载
        //    {
        //        System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
        //        {
        //            //绑定到前台
        //            acticleListbox.ItemsSource = item;

        //            GlobalIndicator.Instance.Text = "";
        //            GlobalIndicator.Instance.IsBusy = false;

        //        });

        //        return true;
        //    }
        //    else //网络加载
        //    {
        //        SetNetWorkNewestLoadData(1, loadPageSize, "0", true);
        //        return false;
        //    }
        //}

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
                ////清除旧数据
                //using (LocalDataContext ldc = new LocalDataContext())
                //{
                //    var item = from s in ldc.newestModels where s.LocalID > 0 select s;
                //    ldc.newestModels.DeleteAllOnSubmit(item);
                //    ldc.SubmitChanges();
                //}
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
                        articleReloadButton.Visibility = Visibility.Visible;
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

        private void RefreshArticles()
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

        private void articleReloadButton_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            articleReloadButton.Visibility = Visibility.Collapsed;
            RefreshArticles();
        }

        #endregion

        #region  品牌找车数据加载

        //汽车品牌网络加载
        CarBrandViewModel carVM = null;
        Dictionary<string, IList<CarBrandModel>> dic = new Dictionary<string, IList<CarBrandModel>>();

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
            SetWebCarBrandLoadData();
            
            //System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            //{
            //    GlobalIndicator.Instance.Text = "正在获取中...";
            //    GlobalIndicator.Instance.IsBusy = true;
            //});
            //if (ldc == null)
            //{
            //    ldc = new LocalDataContext();
            //}
            //var queryResult = from s in ldc.carBrandModels select s;
            ////上次更新时间
            //DateTime lastUpdateTime = DateTime.Now;
            //foreach (CarBrandModel model in queryResult)
            //{
            //    lastUpdateTime = model.CurrentTime;
            //}
            ////如果数据库中有数据，并其上次更新的时间和当前的时间差在一周内  本地数据库读取
            //if (queryResult.Count() > 0 && (DateTime.Now - lastUpdateTime).Days < 7)
            //{
            //    System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            //    {
            //        var groupBy = from car in queryResult
            //                      group car by car.Letter into c
            //                      orderby c.Key
            //                      select new Group<CarBrandModel>(c.Key, c);

            //        foreach (var entity in groupBy)
            //        {
            //            CarBrandGroup group = new CarBrandGroup(entity.key);

            //            foreach (var item in entity)
            //            {
            //                group.Add(item);
            //            }
            //            carBrandSource.Add(group);
            //        }
            //        carBrandListGropus.ItemsSource = carBrandSource;

            //        GlobalIndicator.Instance.Text = "";
            //        GlobalIndicator.Instance.IsBusy = false;
            //    });

            //}
            //else  //网络加载
            //{

            //    //清除旧数据
            //    var deleteResult = from s in ldc.carBrandModels select s;
            //    ldc.carBrandModels.DeleteAllOnSubmit(deleteResult);
            //    ldc.SubmitChanges();
            //    Common.DeleteDirectory("CycleCachedImages");

            //    SetWebCarBrandLoadData();
            //}
        }

        public void SetWebCarBrandLoadData()
        {
            if (carVM == null)
            {
                carVM = new CarBrandViewModel();
                carVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<CarBrandModel>>>(carVM_LoadDataCompleted);
            }
            try
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    GlobalIndicator.Instance.Text = "正在获取中...";
                    GlobalIndicator.Instance.IsBusy = true;
                });
                string url = string.Format("{0}{2}/cars/brands-a2-pm3-v1.5.0-ts{1}.html", App.appUrl, 0, App.versionStr);
                carVM.LoadDataAysnc(url);
            }
            catch (Exception ex)
            {

            }
        }

        void carVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<CarBrandModel>> e)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                GlobalIndicator.Instance.Text = "";
                GlobalIndicator.Instance.IsBusy = false;

                if (e.Error != null)
                {
                    Common.NetworkAvailablePrompt();
                    carListReloadButton.Visibility = Visibility.Visible;
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

        private void carListReloadButton_Click(object sender, System.Windows.Input.GestureEventArgs e)
        {
            carListReloadButton.Visibility = Visibility.Collapsed;
            carBrandLoadData();
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
                if (mediatype == 1 || mediatype == 2)
                {
                    pageindex = news.pageIndex;
                    this.NavigationService.Navigate(new Uri("/View/Channel/News/NewsEndPage.xaml?newsid=" + news.id + "&pageIndex=" + pageindex + "&pageType=" + mediatype, UriKind.Relative));
                }
                else if (mediatype == 3)
                {
                    this.NavigationService.Navigate(new Uri("/View/Channel/News/VideoEndPage.xaml?videoid=" + news.id, UriKind.Relative));
                }
            }
            return;

            /**************************************
             
            Grid gg = (Grid)sender;
            var news = ldc.newestModels.Where(o => o.id == (int)gg.Tag).FirstOrDefault();
            string pageIndex = "1";
            string newstype = string.Empty;
            if (news != null)
            {
                //取得文章类型（新闻or说客）
                newstype = news.type;
                pageIndex = news.pageIndex;
            }
            this.NavigationService.Navigate(new Uri("/View/Channel/Newest/ArticleEndPage.xaml?newsid=" + gg.Tag + "&pageIndex=" + pageIndex + "&newsType=" + newstype, UriKind.Relative));
        
             **************************************/
        }

        // 焦点图
        private void topImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var news = (sender as FrameworkElement).DataContext as NewsModel;
            if (news != null)
            {
                int mediatype = news.mediatype;
                if (mediatype == 1 || mediatype == 3)
                {
                    this.NavigationService.Navigate(new Uri("/View/Channel/News/NewsEndPage.xaml?newsid=" + news.id + "&pageIndex=" + focusImagePageIndex + "&pageType=" + mediatype, UriKind.Relative));
                }
                else if (mediatype == 2)
                {
                    this.NavigationService.Navigate(new Uri("/View/Channel/News/VideoEndPage.xaml?videoid=" + news.id, UriKind.Relative));
                }
            }
            //this.NavigationService.Navigate(new Uri("/View/Channel/Newest/ArticleEndPage.xaml?newsid=" + img.Tag + "&pageIndex=" + focusImagePageIndex + "&newsType=", UriKind.Relative));
        }

        //刷新状态
        bool isRefreshing = false;
        //刷新
        private void refresh_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            RefreshArticles();
        }

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



    }
}