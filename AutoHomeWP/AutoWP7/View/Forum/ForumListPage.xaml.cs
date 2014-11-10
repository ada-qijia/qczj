using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Model;
using ViewModels;
using ViewModels.Handler;
using Microsoft.Phone.Shell;
using AutoWP7.Utils;

namespace AutoWP7.View.Forum
{
    /// <summary>
    /// 论坛列表页
    /// </summary>
    public partial class ForumListPage : PhoneApplicationPage
    {
        public ForumListPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {
                        this.piv.SelectedIndex = int.Parse(NavigationContext.QueryString["indexId"]);
                    }
                    break;
                case System.Windows.Navigation.NavigationMode.Back:
                    {
                        //读取我的论坛信息
                        LoadUserInfo();
                    }
                    break;
            }
        }


        Thread worker = null;
        bool isCarSeriesLoaded = false;
        bool isAreaLoaded = false;
        bool isSubjectLoaded = false;
        private void piv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (piv.SelectedIndex)
            {
                case 0: //车系论坛
                    {
                        UmengSDK.UmengAnalytics.onEvent("ForumActivity", "车系论坛点击量");
                        if (!isCarSeriesLoaded)
                        {
                            worker = new Thread(carSeriesAllForumLoadData);
                            worker.IsBackground = true;
                            worker.Start();

                            //carSeriesAllForumLoadData();
                            isCarSeriesLoaded = true;
                        }
                    }
                    break;
                case 1: //地区论坛
                    {
                        UmengSDK.UmengAnalytics.onEvent("ForumActivity", "地区论坛点击量");
                        if (!isAreaLoaded)
                        {
                            AreaForumLoadData();
                            isAreaLoaded = true;
                        }
                    }
                    break;
                case 2: //主题论坛
                    {
                        UmengSDK.UmengAnalytics.onEvent("ForumActivity", "主题论坛点击量");
                        if (!isSubjectLoaded)
                        {
                            SubjectLoadData();
                            isSubjectLoaded = true;
                        }
                    }
                    break;
                case 3: //我的论坛
                    {
                        if (!LoadUserInfo())
                        {
                            myForumListbox.Visibility = Visibility.Collapsed;
                            loginPanel.Visibility = Visibility.Visible;
                        }
                    }
                    break;
            }

            //只有我的论坛时搜索按钮不可见，其他情况都可见
            setAppBarSearchButtonVisible(!(piv.SelectedIndex == 3));
        }


        #region 我的论坛
        /// <summary>
        /// 我的论坛
        /// </summary>
        private bool LoadUserInfo()
        {
            var setting = IsolatedStorageSettings.ApplicationSettings;
            string key = "userInfo";
            if (setting.Contains(key))
            {
                using (LocalDataContext ldc = new LocalDataContext())
                {
                    var queryResult = from s in ldc.myForum where s.Id > 0 select s;
                    if (queryResult.Count() > 0)
                    {
                        myForumListbox.Visibility = Visibility.Visible;
                        loginPanel.Visibility = Visibility.Collapsed;
                        myForumListbox.ItemsSource = queryResult;
                    }
                }

                return true;
            }
            return false;
        }
        #endregion

        #region 车系论坛数据加载

        private class CarSeriesAllForumGroup : List<carSeriesAllForumModel>
        {
            public CarSeriesAllForumGroup(string category)
            {
                key = category;
            }

            public string key { get; set; }
            public bool HasItems { get { return Count > 0; } }
        }

        private class CarSeriesAllForumGroupSource : List<CarSeriesAllForumGroup>
        {

        }

        CarSeriesAllForumGroupSource carSeriesAllForumSource = new CarSeriesAllForumGroupSource();

        CarSeriesAllForumViewModel carSeriesForumVM = null;
        /// <summary>
        /// 开始加载
        /// </summary>
        public void carSeriesAllForumLoadData()
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                GlobalIndicator.Instance.Text = "正在获取中...";
                GlobalIndicator.Instance.IsBusy = true;
            });

            LocalDataContext ldc = new LocalDataContext();
            //本地加载
            var result = from s in ldc.carSeriesForums where s.bbsId > 0 select s;
            DateTime lastUpDateTime = DateTime.Now;
            foreach (carSeriesAllForumModel model in result)
            {
                lastUpDateTime = model.CurrentTime;
            }
            //如果本地数据库数据，并且创建时间和当前的时间差小于7天  就从本地读取
            if (result.Count() > 0 && (DateTime.Now - lastUpDateTime).Days < 7)
            {
                var groupBy = from car in result
                              group car by car.letter into c
                              orderby c.Key
                              select new Group<carSeriesAllForumModel>(c.Key, c);

                foreach (var entity in groupBy)
                {
                    CarSeriesAllForumGroup group = new CarSeriesAllForumGroup(entity.key);

                    foreach (var item in entity)
                    {
                        group.Add(item);
                    }
                    carSeriesAllForumSource.Add(group);
                }


                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    GlobalIndicator.Instance.Text = "";
                    GlobalIndicator.Instance.IsBusy = false;
                    carSeriesAllForumGropus.ItemsSource = carSeriesAllForumSource;

                });
            }
            else //网络加载
            {
                //清除旧数据
                var item = from s in ldc.carSeriesForums where s.bbsId > 0 select s;
                ldc.carSeriesForums.DeleteAllOnSubmit(item);
                ldc.SubmitChanges();

                if (carSeriesForumVM == null)
                {
                    carSeriesForumVM = new CarSeriesAllForumViewModel();
                }
                string url = string.Format("{0}{1}/club/clubsseries-a2-pm3-v1.5.0-st0.html", App.appUrl, App.versionStr);
                carSeriesForumVM.LoadDataAysnc(url);

                carSeriesForumVM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<carSeriesAllForumModel>>>(carSeriesForumVM_LoadDataCompleted);
            }
        }

        /// <summary>
        /// 加载完成
        /// </summary>
        void carSeriesForumVM_LoadDataCompleted(object sender, ViewModels.Handler.APIEventArgs<IEnumerable<carSeriesAllForumModel>> e)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                GlobalIndicator.Instance.Text = "";
                GlobalIndicator.Instance.IsBusy = false;
                if (e.Error != null)
                {
                    Common.NetworkAvailablePrompt();
                }
                else
                {
                    var groupBy = from car in e.Result
                                  group car by car.letter into c
                                  orderby c.Key
                                  select new Group<carSeriesAllForumModel>(c.Key, c);
                    foreach (var entity in groupBy)
                    {
                        CarSeriesAllForumGroup group = new CarSeriesAllForumGroup(entity.key);

                        foreach (var item in entity)
                        {
                            group.Add(item);
                        }
                        carSeriesAllForumSource.Add(group);
                    }
                    carSeriesAllForumGropus.ItemsSource = carSeriesAllForumSource;
                }
            });
        }
        #endregion

        #region 地区论坛数据加载


        private class AreaForumGroup : List<AreaForumModel>
        {
            public AreaForumGroup(string category)
            {
                key = category;
            }

            public string key { get; set; }
            public bool HasItems { get { return Count > 0; } }
        }

        private class AreaForumSource : List<AreaForumGroup>
        {

        }
        AreaForumSource areaForumSource = new AreaForumSource();

        AreaForumViewModel areaForumVM = null;
        public void AreaForumLoadData()
        {

            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            //清除旧数据
            LocalDataContext ldc = new LocalDataContext();

            var item = from s in ldc.areaForums where s.bbsId > 0 select s;
            DateTime currentTime = DateTime.Now;
            foreach (AreaForumModel model in item)
            {
                currentTime = model.CreateTime;
            }
            if (item.Count() > 0 && (DateTime.Now - currentTime).Days < 7) //本地加载
            {
                var areaGroupBy = from car in item
                                  group car by car.FirstLetter into c
                                  orderby c.Key
                                  select new Group<AreaForumModel>(c.Key, c);

                foreach (var entity in areaGroupBy)
                {
                    AreaForumGroup group = new AreaForumGroup(entity.key);

                    foreach (var cc in entity)
                    {
                        group.Add(cc);
                    }
                    areaForumSource.Add(group);
                }


                GlobalIndicator.Instance.Text = "";
                GlobalIndicator.Instance.IsBusy = false;
                areaForumGroups.ItemsSource = areaForumSource;
            }

            else//网络加载
            {
                //清除旧数据
                var queryResult = from s in ldc.areaForums where s.bbsId > 0 select s;
                ldc.areaForums.DeleteAllOnSubmit(queryResult);
                ldc.SubmitChanges();

                if (areaForumVM == null)
                {
                    areaForumVM = new AreaForumViewModel();
                }
                string url = string.Format("{0}{1}/club/clubsarea-a2-pm3-v1.5.0-st0.html", App.appUrl, App.versionStr);
                areaForumVM.LoadDataAysnc(url);

                areaForumVM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<AreaForumModel>>>(areaForumVM_LoadDataCompleted);


            }
        }

        void areaForumVM_LoadDataCompleted(object sender, ViewModels.Handler.APIEventArgs<IEnumerable<AreaForumModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                var areaGroupBy = from car in e.Result
                                  group car by car.FirstLetter into c
                                  orderby c.Key
                                  select new Group<AreaForumModel>(c.Key, c);

                foreach (var entity in areaGroupBy)
                {
                    AreaForumGroup group = new AreaForumGroup(entity.key);

                    foreach (var cc in entity)
                    {
                        group.Add(cc);
                    }
                    areaForumSource.Add(group);
                }
                areaForumGroups.ItemsSource = areaForumSource;
            }
        }
        #endregion

        #region 主题论坛数据加载

        SubjectForumViewModel subjectVM = null;
        public void SubjectLoadData()
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            LocalDataContext ldc = new LocalDataContext();
            var item = from s in ldc.subjectForums where s.bbsId > 0 select s;
            DateTime currentTime = DateTime.Now;
            foreach (SubjectForumModel model in item)
            {
                currentTime = model.CreateTime;
            }
            if (item.Count() > 0 && (DateTime.Now - currentTime).Days < 7) //本地加载
            {
                GlobalIndicator.Instance.Text = "";
                GlobalIndicator.Instance.IsBusy = false;
                subjectListbox.ItemsSource = item;
            }
            else //网络加载
            {
                //清除旧数据
                var queryResult = from s in ldc.areaForums where s.bbsId > 0 select s;
                ldc.areaForums.DeleteAllOnSubmit(queryResult);
                ldc.SubmitChanges();

                if (subjectVM == null)
                {
                    subjectVM = new SubjectForumViewModel();
                }
                string url = string.Format("{0}{1}/club/clubstheme-a2-pm3-v1.5.0-st0.html", App.appUrl, App.versionStr);

                subjectVM.LoadDataAysnc(url);
                subjectVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<SubjectForumModel>>>(subjectVM_LoadDataCompleted);

            }
        }

        void subjectVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<SubjectForumModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                subjectListbox.ItemsSource = e.Result;
            }
        }

        #endregion


        string title = string.Empty;
        int bbsId = 0;
        //导向车系论坛列表页
        private void carSeriesForumStack_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ForumActivity", "车系论坛帖子列表页点击量");
            TextBlock ss = (TextBlock)sender;
            using (LocalDataContext ldc = new LocalDataContext())
            {
                var queryResult = from s in ldc.carSeriesForums where s.Id == Convert.ToInt32(ss.Tag) select s;
                foreach (carSeriesAllForumModel model in queryResult)
                {
                    title = model.bbsName;
                    bbsId = model.bbsId;
                }
            }
            this.NavigationService.Navigate(new Uri("/View/Forum/LetterListPage.xaml?bbsId=" + bbsId + "&bbsType=c&id=" + ss.Tag + "&title=" + title, UriKind.Relative));
        }

        //导向地区论坛列表页
        private void areaForumStack_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ForumActivity", "地区论坛帖子列表页点击量");
            TextBlock ss = (TextBlock)sender;
            using (LocalDataContext ldc = new LocalDataContext())
            {
                var queryResult = from s in ldc.areaForums where s.Id == Convert.ToInt32(ss.Tag) select s;
                foreach (AreaForumModel model in queryResult)
                {
                    title = model.bbsName;
                    bbsId = model.bbsId;
                }
            }
            this.NavigationService.Navigate(new Uri("/View/Forum/LetterListPage.xaml?bbsId=" + bbsId + "&bbsType=a&id=" + ss.Tag + "&title=" + title, UriKind.Relative));
        }

        //导向主题论坛列表页
        private void subjectGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ForumActivity", "主题论坛帖子列表页点击量");
            TextBlock gg = (TextBlock)sender;
            using (LocalDataContext ldc = new LocalDataContext())
            {
                var queryResult = from s in ldc.subjectForums where s.Id == Convert.ToInt32(gg.Tag) select s;
                foreach (SubjectForumModel model in queryResult)
                {
                    title = model.bbsName;
                    bbsId = model.bbsId;
                }
            }
            this.NavigationService.Navigate(new Uri("/View/Forum/LetterListPage.xaml?bbsId=" + bbsId + "&bbsType=o&id=" + gg.Tag + "&title=" + title, UriKind.Relative));
        }

        //导向登录页
        private void loginBorder_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/More/LoginPage.xaml", UriKind.Relative));
        }

        //我的论坛
        private void bbsIdTap_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ForumActivity", "我的论坛点击量");
            TextBlock tt = (TextBlock)sender;
            using (LocalDataContext ldc = new LocalDataContext())
            {
                var querySult = from s in ldc.myForum where s.Id == Convert.ToInt32(tt.Tag) select s;
                foreach (MyForumModel model in querySult)
                {
                    title = model.bbsName;
                    bbsId = model.bbsId;
                }
            }

            this.NavigationService.Navigate(new Uri("/View/Forum/LetterListPage.xaml?bbsId=" + bbsId + "&bbsType=c&&id=" + tt.Tag + "&title=" + title, UriKind.Relative));
        }

        #region 设置AppBar搜索按钮

        private void setAppBarSearchButtonVisible(bool isVisible)
        {
            if (isVisible)
            {
                if (ApplicationBar == null)
                {
                    ApplicationBar = (ApplicationBar)this.Resources["searchAppBar"];
                }
            }
            else
            {
                ApplicationBar = null;
            }
        }

        //导航到搜索页面
        private void searchButton_Click(object sender, EventArgs e)
        {
            string searchPageUrl = SearchHelper.GetSearchPageUrlWithParams(SearchType.Forum);
            this.NavigationService.Navigate(new Uri(searchPageUrl, UriKind.Relative));
        }

        #endregion
    }
}