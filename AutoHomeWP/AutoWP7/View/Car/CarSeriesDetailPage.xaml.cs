using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
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
using System.Windows.Media;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace AutoWP7.View.Car
{
    /// <summary>
    /// 车系详情页
    /// </summary>
    public partial class CarSeriesDetailPage : PhoneApplicationPage
    {
        public CarSeriesDetailPage()
        {
            InitializeComponent();
        }

        bool isBack = false;

        //论坛id
        int bbsId = 0;
        //论坛类型
        string bbsType = string.Empty;
        //车系id
        string carSeriesId = string.Empty;
        //车型id
        string cityId = string.Empty;
        //页码id
        string indexId = string.Empty;
        //加载页容量
        int loadPageSize = 20;
        string currentPage = "-p1-";
        //经销商数据集合
        private ObservableCollection<DealerModel> dealerDataSource = new ObservableCollection<DealerModel>();

        //论坛数据集合
        private ObservableCollection<ForumModel> ForumDataSource = new ObservableCollection<ForumModel>();

        //车系文章数据集合
        public ObservableCollection<NewsModel> acticleDataSource = new ObservableCollection<NewsModel>();

        public ObservableCollection<CarSeriesQuteGroup> specsGroups = new ObservableCollection<CarSeriesQuteGroup>();

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

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            InitBtn();

            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {
                        //车系id
                        carSeriesId = NavigationContext.QueryString["carSeriesId"];
                        //全局化
                        App.CarSeriesId = carSeriesId;

                        if (string.IsNullOrEmpty(App.CityId)) //默认北京
                        {
                            cityId = "110100";
                        }
                        else
                        {
                            cityId = App.CityId;
                        }

                        //车系名
                        using (LocalDataContext ldc = new LocalDataContext())
                        {
                            var name = from s in ldc.carSeries where s.Id == int.Parse(carSeriesId) select s.Name;
                            foreach (var n in name)
                            {
                                App.CarSeriesName = n;
                                autoName.Text = n;
                            }
                        }

                        //导航到的具体页码
                        indexId = NavigationContext.QueryString["indexId"];
                        if (!string.IsNullOrEmpty(indexId))
                        {
                            piv.SelectedIndex = int.Parse(indexId);
                        }
                    }
                    break;
                case System.Windows.Navigation.NavigationMode.Back:
                    {
                        if (!string.IsNullOrEmpty(App.CityId))
                        {
                            //更新城市id
                            cityId = App.CityId;
                            using (LocalDataContext ldc = new LocalDataContext())
                            {
                                var result = from s in ldc.provinces where s.Id == int.Parse(App.CityId) select s.Name;
                                foreach (var name in result)
                                {
                                    detailChooseCity.Content = name;
                                }
                            }

                            // DealerLoadData();
                        }

                        ////更新城市id
                        cityId = App.CityId;
                        if (piv.SelectedIndex == 3)
                        {
                            //更新城市id
                            if (!string.IsNullOrEmpty(App.CityId))
                            {

                                //重新加载经销商数据
                                DealerLoadData();
                            }
                        }
                        if (piv.SelectedIndex == 0)
                            InitSeriesSpecsInfo();
                    }
                    break;
            }
        }

        private void InitSeriesSpecsInfo()
        {
            if (carSeriesQuoteListGropus.ItemsSource != null)
                carSeriesQuoteListGropus.ItemsSource.Clear();
            using (LocalDataContext ldc = new LocalDataContext())
            {
                var groupBy = from car in ldc.carQuotes
                              orderby car.GroupOrder ascending, car.COrder ascending
                              group car by new { N = car.GroupName, O = car.GroupOrder } into c
                              orderby c.Key.O ascending
                              select new Group<CarSeriesQuoteModel>(c.Key.N, c);
                var carCompareSpecs = new CarCompareViewModel().GetCompareSpec();
                specsGroups = new ObservableCollection<CarSeriesQuteGroup>();
                foreach (var entity in groupBy)
                {
                    CarSeriesQuteGroup group = new CarSeriesQuteGroup(entity.key);
                    var entity1 = from c in entity orderby c.COrder ascending select c;
                    foreach (var item in entity1)
                    {
                        Int32 spedid = item.Id;
                        //蓝色，可以添加对比
                        item.Compare = "#3CACEB";
                        item.CompareText = "添加对比";
                        //商用车id>1000000或参数配置不显示（ParamIsShow=0）的不能参加对比
                        if (item.ParamIsShow == 0 || spedid > 1000000)
                        {
                            //灰色，按钮不可用
                            item.Compare = "#CCCCCC";
                            item.CompareText = "添加对比";
                        }
                        else
                        {
                            if (carCompareSpecs != null && carCompareSpecs.Count(c => c.SpecId == spedid) > 0)
                            {
                                item.Compare = "#CCCCCC";
                                item.CompareText = "已添加";
                            }
                        }
                        group.Add(item);
                    }
                    specsGroups.Add(group);
                    carSeriesQuteSource.Add(group);
                }
                carSeriesQuoteListGropus.ItemsSource = specsGroups;
            }
        }
        //bool IsLoading = false;
        //object o = new object();
        //void carSeriesQuoteListGropus_ItemRealized(object sender, ItemRealizationEventArgs e)
        //{
        //    lock (o)
        //    {
        //        if (!IsLoading)
        //        {
        //            System.Threading.Thread.Sleep(50);
        //            if (e.ItemKind == LongListSelectorItemKind.Item)
        //            {
        //                CarSeriesQuoteModel gsqg = e.Container.Content as CarSeriesQuoteModel;
        //                CarSeriesQuteGroup temp = carSeriesQuteSource.FirstOrDefault(f => f.key == gsqg.GroupName);
        //                if (temp != null) 
        //                {
        //                    CarSeriesQuoteModel i = temp.FirstOrDefault(f => f.Id == gsqg.Id);
        //                    if (i.Compare != gsqg.Compare && i.CompareText != gsqg.CompareText)
        //                    {
        //                        e.Container.Content = i;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

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

        }

        //标识状态（false 未加载，true已加载）
        bool isQuoteLoaded = false;
        bool isArticLoaded = false;
        bool isDealerLoaded = false;
        bool isForumLoaded = false;
        bool isAlibiLoaded = false;

        //论坛接口地址
        string forumUrl = string.Empty;
        //文章接口地址
        //string articleUrl = string.Empty;

        private void piv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tag = (piv.SelectedItem as PivotItem).Tag.ToString();

            switch (tag)
            {
                case "quote": //报价
                    {
                        UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车系报价页点击量");
                        ApplicationBar.IsVisible = true;
                        this.ApplicationBar.Buttons.Clear();
                        this.ApplicationBar.Buttons.Add(ToVS);
                        if (!isQuoteLoaded)
                        {
                            CarSeriesQuoteLoadData();
                            isQuoteLoaded = true;
                        }
                    }
                    break;
                case "image": //图片
                    ApplicationBar.IsVisible = false;
                    break;
                case "article": //文章
                    {
                        UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车系文章页~新闻点击量");
                        ApplicationBar.IsVisible = true;
                        ApplicationBar = Resources["appArticle"] as ApplicationBar;
                        if (!isArticLoaded)
                        {
                            curCityId = "0";
                            curNewsType = 10001;

                            ArticleLoadData(true, 1);
                            isArticLoaded = true;

                        }
                    }
                    break;
                case "dealer": //经销商
                    {
                        UmengSDK.UmengAnalytics.onEvent("SeriesActivity", " 车系经销商点击量");
                        ApplicationBar.IsVisible = false;
                        if (!isDealerLoaded)
                        {
                            UpdateLocalCity();
                            DealerLoadData();
                            isDealerLoaded = true;
                        }
                    }
                    break;
                case "forum": //论坛
                    {
                        UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车系页~最后回复论坛点击量");
                        ApplicationBar.IsVisible = true;
                        ApplicationBar = (ApplicationBar)Resources["appForum"];
                        if (!isForumLoaded)
                        {
                            isForumLoaded = true;
                            forumUrl = CreateTopicsUrl("c", false, 0);
                            ForumLoadData(forumUrl, true);
                        }
                    }
                    break;
                case "alibi": //口碑
                    {
                        UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车系页~口碑点击量");
                        ApplicationBar.IsVisible = false;
                        if (!isAlibiLoaded)
                        {
                            CarSeriesQuoteLoadData();
                            isAlibiLoaded = true;
                        }

                    }
                    break;
            }
        }

        /// <summary>
        /// 更新本地城市
        /// </summary>
        private void UpdateLocalCity()
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
                        detailChooseCity.Content = name;
                    }
                }
                App.CityId = setting[key].ToString();
            }
        }

        #region 报价

        public class CarSeriesQuteGroup : List<CarSeriesQuoteModel>
        {
            public CarSeriesQuteGroup()
            { }
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
        /// 车系报价
        /// </summary>
        public void CarSeriesQuoteLoadData()
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

            string url = string.Format("{0}{2}/cars/seriessummary-a2-pm3-v1.4.0-s{1}-t0xffff-c0.html", App.appUrl, carSeriesId, App.versionStr);
            carSeriesQuoteVM.LoadDataAysnc(url, true);
            //这里已经有标准接口carSeriesQuoteVM.LoadDataAysnc(App.appUrl  + "/autov2.5.5/cars/seriessummary-a2-pm3-v2.5.5-s" + carSeriesId + "-t0XFFFF.html");
            carSeriesQuoteVM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<Model.CarSeriesQuoteModel>>>(carSeriesQuoteVM_LoadDataCompleted);
        }

        void carSeriesQuoteVM_LoadDataCompleted(object sender, ViewModels.Handler.APIEventArgs<IEnumerable<Model.CarSeriesQuoteModel>> e)
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
                    notCarseriesQuotePropmt.Visibility = Visibility.Visible;
                    carSeriesQuoteListGropus.Visibility = Visibility.Collapsed;
                }
                else
                {

                    var groupBy = from car in e.Result
                                  group car by car.GroupName into c
                                  //orderby c.Key
                                  select new Group<CarSeriesQuoteModel>(c.Key, c);
                    var carCompareSpecs = new CarCompareViewModel().GetCompareSpec();
                    specsGroups = new ObservableCollection<CarSeriesQuteGroup>();
                    foreach (var entity in groupBy)
                    {
                        CarSeriesQuteGroup groupitem = new CarSeriesQuteGroup(entity.key);
                        foreach (var item in entity)
                        {
                            //蓝色，可以添加对比
                            item.Compare = "#3CACEB";
                            item.CompareText = "添加对比";
                            //商用车id>1000000或参数配置不显示（ParamIsShow=0）的不能参加对比
                            if (item.ParamIsShow == 0 || item.Id > 1000000)
                            {
                                //灰色，按钮不可用
                                item.Compare = "#CCCCCC";
                                item.CompareText = "添加对比";
                            }
                            else
                            {
                                if (carCompareSpecs != null && carCompareSpecs.Count(c => c.SpecId == item.Id) > 0)
                                {
                                    item.Compare = "#CCCCCC";
                                    item.CompareText = "已添加";
                                }
                            }
                            groupitem.Add(item);
                        }
                        specsGroups.Add(groupitem);
                        carSeriesQuteSource.Add(groupitem);
                    }
                    carSeriesQuoteListGropus.ItemsSource = specsGroups;
                    
                }

            }
        }

        #endregion

        #region 文章

        CarSeriesActicleViewModel carSeriesArticleVM = null;
        int articlePageIndex = 1; //页码
        /// <summary>
        /// 
        /// </summary>
        public void ArticleLoadData(bool isFirst, int pageIndex)
        {
            if (isFirst)
            {
                articlePageIndex = 1;
            }
            GlobalIndicator.Instance.Text = "正在获取中......";
            GlobalIndicator.Instance.IsBusy = true;
            carSeriesArticleVM = new CarSeriesActicleViewModel();
            string url = CreateSeriesNewsUrl(pageIndex, loadPageSize);
            //url = url.Replace("-p0-", "-p" + pageIndex + "-");
            carSeriesArticleVM.LoadDataAysnc(url);
            carSeriesArticleVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<NewsModel>>>(comm_LoadDataCompleted);
        }

        /// <summary>
        /// 分页加载
        /// </summary>
        private void btnLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //清楚集合中的更多按钮
            acticleDataSource.RemoveAt((acticleDataSource.Count - 1));
            //分页加载
            articlePageIndex++;
            ArticleLoadData(false, articlePageIndex);
        }


        /// <summary>
        /// 完成
        /// </summary>
        void comm_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<NewsModel>> e)
        {
            isLoading = false;
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                try
                {
                    if (e.Result.Count() < 1)
                    {
                        notCarseriesArticlePropmt.Visibility = Visibility.Visible;
                        carSeriesActicleListbox.Visibility = Visibility.Collapsed;
                    }
                    if (e.Result.Count() <= 1 && articlePageIndex > 1)
                    {
                        Common.showMsg("已经是最后一页了~~");
                    }
                    else
                    {
                        notCarseriesArticlePropmt.Visibility = Visibility.Collapsed; ;
                        carSeriesActicleListbox.Visibility = Visibility.Visible;
                        int total = 0;
                        foreach (NewsModel model in e.Result)
                        {
                            total = model.Total;
                            acticleDataSource.Add(model);
                        }
                        if (acticleDataSource.Count < 20)
                        {
                            acticleDataSource.RemoveAt((acticleDataSource.Count - 1));
                        }
                        carSeriesActicleListbox.ItemsSource = acticleDataSource;
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        #endregion

        #region 经销商

        DealerViewModel DealerVM = null;
        /// <summary>
        /// 经销商
        /// </summary>
        public void DealerLoadData()
        {
            try
            {
                using (LocalDataContext ldc = new LocalDataContext())
                {
                    //先从本地数据库中获得数据
                    var queryResult = from d in ldc.dealerModels where d.CarId == int.Parse(carSeriesId) && d.CityId == int.Parse(cityId) select d;

                    if (queryResult.Count() > 0)
                    {
                        dealerListbox.ItemsSource = queryResult;
                    }
                    else
                    {
                        //清除数据库中的数据
                        var deleteAllItem = from d in ldc.dealerModels where d.id > 0 select d;
                        ldc.dealerModels.DeleteAllOnSubmit(deleteAllItem);
                        ldc.SubmitChanges();

                        //从服务器中获得数据
                        GlobalIndicator.Instance.Text = "正在获取中...";
                        GlobalIndicator.Instance.IsBusy = true;

                        DealerVM = new DealerViewModel();
                        string url = string.Format("{0}{3}/dealer/pddealers-a2-pm1-v1.4.0-sp0-ss{1}-c{2}-sc0-p1-s200.html", App.appUrl, carSeriesId, cityId, App.versionStr);
                        DealerVM.LoadDataAysnc(url, carSeriesId, cityId);
                        //DealerVM.LoadDataAysnc(App.headUrl + "/dealers/Profile/Getlist.ashx?action=0x45b5&cityid=" + cityId + "&seriesid=" + carSeriesId, carSeriesId, cityId);
                        DealerVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<DealerModel>>>(DealerVM_LoadDataCompleted);
                    }
                }
            }
            catch (Exception ex)
            {

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
                    dealerListbox.ItemsSource = null;
                    dealerDataSource.Clear();
                    notCarseriesPropmt.Visibility = Visibility.Visible;
                    dealerListbox.Visibility = Visibility.Collapsed;
                    //Common.showMsg("暂无经销商信息~~");
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

        #endregion

        #region 论坛

        CarSeriesForumViewModel forumVM = null;
        int forumPageIndex = 1; //页码

        // 
        public void ForumLoadData(string url, bool isFirst)
        {

            GlobalIndicator.Instance.Text = "正在获取中......";
            GlobalIndicator.Instance.IsBusy = true;
            if (isFirst)    //第一次加载
            {
                forumPageIndex = 1;
            }
            forumVM = new CarSeriesForumViewModel();
            forumVM.LoadDataAysnc(url);
            forumVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<ForumModel>>>(forumVM_LoadDataCompleted);
        }

        // 完成
        void forumVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<ForumModel>> e)
        {
            isLoading = false;

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

                    if (e.Result.Count() < 1)
                    {
                        //无数据提示
                        notCarseriesForumsPropmt.Visibility = Visibility.Visible;
                        forumListbox.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        if (e.Result.Count() <= 1 && forumPageIndex > 1)
                        {
                            Common.showMsg("已经是最后一页了~~");
                        }
                        else
                        {
                            notCarseriesForumsPropmt.Visibility = Visibility.Collapsed;
                            forumListbox.Visibility = Visibility.Visible;
                            foreach (ForumModel model in e.Result)
                            {
                                bbsId = model.bbsId;
                                bbsType = model.bbsType;
                                ForumDataSource.Add(model);
                            }

                            if (ForumDataSource.Count < 20)
                            {
                                ForumDataSource.RemoveAt(ForumDataSource.Count - 1);
                            }
                            forumListbox.ItemsSource = ForumDataSource;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        // 分页加载
        private void forumLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //清楚集合中的更多按钮
            ForumDataSource.RemoveAt((ForumDataSource.Count - 1));
            //分页加载
            forumPageIndex++;
            string page = "-p" + forumPageIndex.ToString() + "-";
            forumUrl = forumUrl.Replace(currentPage, page);
            ForumLoadData(forumUrl, false);
        }
        #endregion



        //标识正在加载
        bool isLoading = false;
        //最新帖子
        private void newestAppbar_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车系页~最新论坛点击量");
            if (isLoading == false)
            {
                isLoading = true;

                forumListbox.ItemsSource = null;
                ForumDataSource.Clear();
                forumUrl = CreateTopicsUrl("c", false, 2);
                ForumLoadData(forumUrl, true);
            }
        }

        //精华帖子
        private void refineAppbar_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车系页~精华论坛点击量");
            if (isLoading == false)
            {
                isLoading = true;
                forumListbox.ItemsSource = null;
                ForumDataSource.Clear();
                forumUrl = CreateTopicsUrl("c", true, 0);
                ForumLoadData(forumUrl, true);
            }
        }

        //最后回复
        private void lastReplyAppbar_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车系页~最后回复论坛点击量");
            if (isLoading == false)
            {
                isLoading = true;
                forumListbox.ItemsSource = null;
                ForumDataSource.Clear();
                forumUrl = CreateTopicsUrl("c", false, 0);
                ForumLoadData(forumUrl, true);
            }
        }
        /// <summary>
        /// 生成帖子列表url
        /// </summary>
        /// <returns></returns>
        private string CreateTopicsUrl(string bbsType, bool isRefine, int order)
        {
            return string.Format(AppUrlMgr.TopicsUrl, 0, "c", isRefine ? 1 : 0, carSeriesId, order, 1, 20);
        }

        //导向车系报价页面
        private void carSeriesGird_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid gg = (Grid)sender;
            int paramIsShow = 0;
            bool isFind = false;
            foreach (CarSeriesQuteGroup item in carSeriesQuteSource)
            {
                foreach (var ii in item)
                    if (ii.Id == Convert.ToInt32(gg.Tag))
                    {
                        paramIsShow = ii.ParamIsShow;
                        isFind = true;
                        break;
                    }
                if (isFind)
                    break;
            }
            this.NavigationService.Navigate(new Uri("/View/Car/CarSeriesQuotePage.xaml?carId=" + gg.Tag + "&paramisshow=" + paramIsShow, UriKind.Relative));
        }

        /// <summary>
        /// 车型列表页对比按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addComPare_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            #region 用Border控件时，添加对比事件
            //添加车型到对比库
            Border bd = (Border)sender;
            int specid = Convert.ToInt32(bd.Tag);
            string specName = string.Empty;
            string bdColor = ((SolidColorBrush)bd.Background).Color.ToString();
            if (bdColor.Contains("3CACEB")) //可以添加对比
            {
                bool isFine = false;
                CarSeriesQuoteModel curItem = new CarSeriesQuoteModel();
                foreach (CarSeriesQuteGroup item in specsGroups)
                {
                    foreach (var ii in item)
                        if (ii.Id == specid)
                        {
                            curItem = ii;
                            specName = ii.Name;
                            isFine = true;
                            break;
                        }
                    if (isFine)
                        break;
                }
                int sid = 0;
                int.TryParse(App.CarSeriesId, out sid);
                CarCompareViewModel carCompareViewModel = new CarCompareViewModel()
                {
                    SpecId = specid,
                    SpecName = specName,
                    SeriesId = sid,
                    SeriesName = autoName.Text.Trim(),
                    IsChoosed = false
                };
                int resultCode = carCompareViewModel.AddCompareSpec(carCompareViewModel);
                if (resultCode == 0)
                {
                    curItem.Compare = "#CCCCCC";
                    curItem.CompareText = "已添加";
                    Common.showMsg("添加成功");
                    ////InitSeriesSpecsInfo();
                    //bd.Background = new SolidColorBrush(Color.FromArgb(0xff, 0xCC, 0xCC, 0xCC));
                    //TextBlock tb = CommonLayer.CommonHelper.FindFirstElementInVisualTree<TextBlock>(bd);
                    //if (tb != null)
                    //    tb.Text = "已添加";
                }
                else if (resultCode == 2)
                    Common.showMsg("抱歉，最多可添加9款车型");
                else
                { }
            }
            else
            {
            }
            #endregion
        }

        // 拨打电话
        private void callDealer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "电话拨打点击量");
            Image bb = (Image)sender;
            PhoneCallTask phoneCall = new PhoneCallTask();
            phoneCall.PhoneNumber = bb.Tag.ToString();
            phoneCall.Show();
        }
        // 车系经销商询价
        private void callPriceSeries_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (string.IsNullOrEmpty(App.CityId)) //默认北京
            {
                cityId = "110100";
            }
            else
            {
                cityId = App.CityId;
            }
            Border gg = (Border)sender;
            this.NavigationService.Navigate(new Uri("/View/Car/AskPrice.xaml?dealerid=0&cityID=" + cityId + "&seriesID=" + carSeriesId + "&specID=" + gg.Tag, UriKind.Relative));
        }
        //车系询价
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
            this.NavigationService.Navigate(new Uri("/View/Car/AskPrice.xaml?dealerid=" + gg.Tag + "&cityID=" + cityId + "&seriesID=" + carSeriesId + "&specID=" + 0, UriKind.Relative));
        }
        // 导向经销商详情页面
        private void dealerDeatail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image bb = (Image)sender;
            this.NavigationService.Navigate(new Uri("/View/Car/DealerDetailPage.xaml?id=" + bb.Tag, UriKind.Relative));
        }

        //导向文章最终页页面
        private void acticleStack_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            Grid gg = (Grid)sender;
            ///View/Channel/Newest/ArticleEndPage.xaml?newsid=" + gg.Tag + "&pageIndex=1
            this.NavigationService.Navigate(new Uri("/View/Channel/Newest/ArticleEndPage.xaml?newsid=" + gg.Tag + "&pageIndex=1", UriKind.Relative));
            //this.NavigationService.Navigate(new Uri("/View/Car/CarSeriesArticleEndPage.xaml?newsid=" + gg.Tag + "&pageIndex=1", UriKind.Relative));
        }

        //导向帖子最终页
        private void topicIdGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid gg = (Grid)sender;
            //this.NavigationService.Navigate(new Uri("/View/Car/TopicDetailPage.xaml?topicId=" + gg.Tag + "&bbsId=" + bbsId + "&bbsType=" + bbsType, UriKind.Relative));
            this.NavigationService.Navigate(new Uri("/View/Forum/TopicDetailPage.xaml?from=1&topicId=" + gg.Tag + "&bbsId=" + bbsId + "&bbsType=" + bbsType, UriKind.Relative));
        }

        //导向车身外观页
        private void facade_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            this.NavigationService.Navigate(new Uri("/View/Car/carSeriesImagePage.xaml?indexId=0&type=1&carId=" + carSeriesId, UriKind.Relative));
        }

        //中控方向盘
        private void wheel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            this.NavigationService.Navigate(new Uri("/View/Car/carSeriesImagePage.xaml?indexId=1&type=1&carId=" + carSeriesId, UriKind.Relative));
        }

        //车厢座椅
        private void carCompartment_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            this.NavigationService.Navigate(new Uri("/View/Car/carSeriesImagePage.xaml?indexId=2&type=1&carId=" + carSeriesId, UriKind.Relative));
        }

        //其他细节
        private void elseDetail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            this.NavigationService.Navigate(new Uri("/View/Car/carSeriesImagePage.xaml?indexId=3&type=1&carId=" + carSeriesId, UriKind.Relative));
        }

        // 省市选择
        private void detailChooseCity_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车系页城市选择点击量");
            this.NavigationService.Navigate(new Uri("/View/Channel/News/CityListPage.xaml?action=1", UriKind.Relative));
        }

        int curNewsType = 10001;
        string curCityId = "0";
        //新闻
        private void newsBar_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车系文章页~新闻点击量");
            if (isLoading == false)
            {
                isLoading = true;
                carSeriesActicleListbox.ItemsSource = null;
                acticleDataSource.Clear();
                curNewsType = 10001;
                curCityId = "0";
                ArticleLoadData(true, 1);
            }
        }

        //行情
        private void qutotationsBar_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车系文章页~行情点击量");
            if (isLoading == false)
            {
                isLoading = true;
                carSeriesActicleListbox.ItemsSource = null;
                acticleDataSource.Clear();
                curNewsType = 10003;
                curCityId = string.IsNullOrEmpty(App.CityId) ? "0" : App.CityId;
                ArticleLoadData(true, 1);
            }
        }

        //评测导购
        private void driveBar_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车系文章页~试驾点击量");

            if (isLoading == false)
            {
                isLoading = true;
                carSeriesActicleListbox.ItemsSource = null;
                acticleDataSource.Clear();

                curNewsType = 10002;
                curCityId = "0";
                ArticleLoadData(true, 1);
            }
        }

        //汽车周边
        private void shoppingGuidBar_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车系文章页~导购点击量");
            if (isLoading == false)
            {
                isLoading = true;
                carSeriesActicleListbox.ItemsSource = null;
                acticleDataSource.Clear();

                curNewsType = 10004;
                curCityId = "0";

                ArticleLoadData(true, 1);
            }
        }

        //发帖
        private void sendLetter_Click_1(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Forum/SendLetterPage.xaml?title=" + autoName.Text + "&bbsId=" + bbsId + "&bbsType=" + bbsType, UriKind.Relative));
        }

        private string CreateSeriesNewsUrl(int pageIndex = 1, int pageSize = 20)
        {
            return string.Format("{0}{6}/news/seriesnews-a2-pm3-v1.4.0-ss{1}-cs{2}-c{3}-p{4}-s{5}.html", App.appUrl, carSeriesId, curNewsType, curCityId, pageIndex, pageSize, App.versionStr);
        }
    }
}