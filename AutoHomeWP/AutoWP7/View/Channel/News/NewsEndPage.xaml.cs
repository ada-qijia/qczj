using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Model;
using Model.Me;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using ViewModels;

namespace AutoWP7.View.Channel.News
{
    /// <summary>
    /// 文章最终页
    /// </summary>
    public partial class NewsEndPage : PhoneApplicationPage
    {
        public NewsEndPage()
        {
            InitializeComponent();
        }

        public static void ShareState(FavoriteArticleModel article)
        {
            PhoneApplicationService.Current.State[Utils.MeHelper.FavoriteStateKey] = article;
        }

        private FavoriteArticleModel news;

        //新闻id
        string newsId = string.Empty;
        //页码
        int pageIndex = 1;
        //页数
        int pageCount;
        //评论数
        static int replyCount;
        //请求页面类型 2-说客
        static int pageType;
        int buttonCount = 2;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            buttonCount = this.ApplicationBar.Buttons.Count;
            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {
                        if (PhoneApplicationService.Current.State.ContainsKey(Utils.MeHelper.FavoriteStateKey))
                        {
                            news = PhoneApplicationService.Current.State[Utils.MeHelper.FavoriteStateKey] as FavoriteArticleModel;
                            PhoneApplicationService.Current.State.Remove(Utils.MeHelper.FavoriteStateKey);
                        }

                        UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "文章最终页的访问次数");

                        newsId = NavigationContext.QueryString["newsid"];
                        pageIndex = Convert.ToInt32(NavigationContext.QueryString["pageIndex"]);
                        pageType = Convert.ToInt32(NavigationContext.QueryString["pageType"]);
                        App.newsid = newsId;
                        wb.IsScriptEnabled = true;
                        if (pageType == 1)
                        {
                            LoadData();
                        }
                        else if (pageType == 2)
                        {
                            LoadShuokeData();
                        }

                        //添加浏览历史
                        AddRecents();
                    }
                    break;
                case System.Windows.Navigation.NavigationMode.Back:
                    {
                        ProgBar.Visibility = Visibility.Collapsed;

                        if (pageCount > 1)
                        {
                            IApplicationBarIconButton Icon;
                            for (int i = 0; i < buttonCount; i++)
                            {
                                Icon = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                                Icon.IsEnabled = App.barStatus[i];
                            }
                        }
                    }
                    break;
            }
            //设置收藏按钮状态
            setFavoriteButton();

            //设置小图模式开关
            bool isSmallImageMode = Utils.MeHelper.GetIsSmallImageMode();
            SetImageModeMenuItem(isSmallImageMode);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if ((App.Current as App).newsPartPageMessage.IsOpen == true)
            {
                (App.Current as App).newsPartPageMessage.Hide();
                e.Cancel = true;
            }
            else
            {
                //获得返回队列数
                int sum = NavigationService.BackStack.Count();
                for (int i = 2; i < sum; i++)
                {
                    NavigationService.RemoveBackEntry();
                }
                base.OnBackKeyPress(e);
            }
        }

        //分页集合
        public static ObservableCollection<NewsDetailModel> newsDataSource = new ObservableCollection<NewsDetailModel>();

        #region 分页数据加载
        NewsDetailViewModel newsVM = null;
        /// <summary>
        /// 分页数据加载
        /// </summary>
        private void LoadData()
        {
            newsVM = new NewsDetailViewModel();
            string url = string.Format("{0}{2}/content/news/newsinfo-a2-pm3-v{3}-i{1}.html", App.newsPageDomain, newsId, App.versionStr, App.version);
            newsVM.LoadDataAysnc(url);
            newsVM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<Model.NewsDetailModel>>>((ss, ee) =>
            {
                if (ee.Error != null)
                {

                }
                else
                {

                    newsDataSource = (ObservableCollection<NewsDetailModel>)ee.Result;

                    foreach (NewsDetailModel model in newsDataSource)
                    {
                        pageCount = model.PageCount;
                        replyCount = model.ReplyCount;
                    }

                    //分页控件
                    if (newsDataSource.Count <= 1)
                    {
                        IApplicationBarIconButton iconButton = this.ApplicationBar.Buttons[1] as IApplicationBarIconButton;
                        iconButton.IsEnabled = false;

                    }
                }

                string newsPageUrl = CreateNewsViewUrl(pageIndex);
                wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
            });

        }
        #endregion

        private void LoadShuokeData()
        {
            string newsPageUrl = AppUrlMgr.ShuoWebViewUrl(Convert.ToInt32(newsId), 0, 1, 1, 0, 0);
            wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
        }

        // 上页
        private void previousPage_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "上页的点击量");
            (App.Current as App).newsPartPageMessage.Hide();

            pageIndex = pageIndex - 1;
            string url = CreateNewsViewUrl(pageIndex);
            wb.Navigate(new Uri(url, UriKind.Absolute));

        }

        // 查看评论
        private void checkComment_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "评论页的访问量");

            for (int i = 0; i < buttonCount; i++)
            {
                App.barStatus[i] = (this.ApplicationBar.Buttons[i] as IApplicationBarIconButton).IsEnabled;

            }
            (App.Current as App).newsPartPageMessage.Hide();
            this.NavigationService.Navigate(new Uri("/View/Channel/News/NewsCommentListPage.xaml?newsid=" + newsId + "&pageType=" + pageType, UriKind.Relative));
        }

        // 下页
        private void nextPage_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "下页的点击量");
            (App.Current as App).newsPartPageMessage.Hide();
            pageIndex = pageIndex + 1;
            //上页可用
            string newsPageUrl = CreateNewsViewUrl(pageIndex);
            wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
            //wb.Navigate(new Uri(App.headUrl + "/news/news.aspx?newsid=" + newsId + "&pageIndex=" + pageIndex, UriKind.Absolute));
        }

        // 分页
        private void partPage_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "分页的点击量");
            for (int i = 0; i < buttonCount; i++)
            {
                App.barStatus[i] = (this.ApplicationBar.Buttons[i] as IApplicationBarIconButton).IsEnabled;

            }
            if ((App.Current as App).newsPartPageMessage.IsOpen == true)
            {
                (App.Current as App).newsPartPageMessage.Hide();
            }
            else
            {
                (App.Current as App).newsPartPageMessage.Show();
            }
        }

        //加载中
        private void wb_Navigating(object sender, NavigatingEventArgs e)
        {
            IApplicationBarIconButton Icon;
            for (int i = 1; i < buttonCount; i++)
            {
                Icon = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                Icon.IsEnabled = false;
            }
            ProgBar.Visibility = Visibility.Visible;
        }

        //加载完成
        private void wb_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

            ProgBar.Visibility = Visibility.Collapsed;
            IApplicationBarIconButton iconButton;
            if (pageCount <= 1)//如果总页数小于等于1  
            {
                for (int i = 1; i < buttonCount; i++)
                {
                    //将上页和分页控件置灰
                    iconButton = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                    iconButton.IsEnabled = false;
                }
                //将评论控件置可见
                iconButton = this.ApplicationBar.Buttons[2] as IApplicationBarIconButton;
                iconButton.IsEnabled = true;

            }
            else
            {

                for (int i = 1; i < buttonCount; i++)
                {
                    iconButton = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                    iconButton.IsEnabled = true;
                }
                //if (pageIndex <= 1)//当前页码为1时
                //{
                //    //将上页控件置灰
                //    iconButton = this.ApplicationBar.Buttons[0] as IApplicationBarIconButton;
                //    iconButton.IsEnabled = false;
                //}
                //else //当前页码为大于等于2时
                //{
                //    if (pageIndex >= pageCount)//如果当前页为最后一页
                //    {
                //        //下页置灰
                //        iconButton = this.ApplicationBar.Buttons[3] as IApplicationBarIconButton;
                //        iconButton.IsEnabled = false;
                //    }
                //    if (pageIndex <= 2)
                //    {
                //        //上页置灰
                //        iconButton = this.ApplicationBar.Buttons[0] as IApplicationBarIconButton;
                //        iconButton.IsEnabled = false;
                //    }
                //    //上页置为可用
                //    iconButton = this.ApplicationBar.Buttons[0] as IApplicationBarIconButton;
                //    iconButton.IsEnabled = true;
                //}

            }

        }

        //大图和链接
        private void wb_ScriptNotify(object sender, NotifyEventArgs e)
        {
            try
            {
                Uri urlSource;
                if (e.Value.Contains("h"))//大图
                {
                    for (int i = 0; i < buttonCount; i++)
                    {
                        App.barStatus[i] = (this.ApplicationBar.Buttons[i] as IApplicationBarIconButton).IsEnabled;
                    }

                    this.NavigationService.Navigate(new Uri("/View/Channel/Newest/ShowBigImage.xaml?type=1&id=" + newsId + "&url=" + e.Value, UriKind.Relative));
                }
                else
                {
                    if (news != null)
                    {
                        NewsEndPage.ShareState(news);
                    }

                    if (e.Value.Contains("-"))//带页码的文章最终页
                    {
                        string[] strArrary = e.Value.Split('-');
                        newsId = strArrary[0];
                        pageIndex = Convert.ToInt32(strArrary[1]);
                        urlSource = new Uri("/View/Channel/News/NewsEndPage.xaml?" + new Guid().ToString() + "&newsid=" + newsId + "&pageIndex=" + pageIndex + "&pageType=" + pageType, UriKind.Relative);
                        this.NavigationService.Navigate(urlSource);
                    }
                    else //不带页码的文章最终页
                    {
                        newsId = e.Value;
                        pageIndex = 1;
                        urlSource = new Uri("/View/Channel/News/NewsEndPage.xaml?" + new Guid().ToString() + "&newsid=" + newsId + "&pageIndex=" + pageIndex + "&pageType=" + pageType, UriKind.Relative);
                        this.NavigationService.Navigate(urlSource);
                    }
                }
            }
            catch
            { }
        }

        //加载失败
        private void wb_NavigationFailed_1(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            wb.Visibility = Visibility.Collapsed;
            ProgBar.Visibility = Visibility.Collapsed;
            refreshButton.Visibility = Visibility.Visible;
            prompt.Visibility = Visibility.Visible;
        }

        //刷新
        private void refreshButton_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string newsPageUrl = pageType == 2 ? CreateShuokeViewUrl() : CreateNewsViewUrl(pageIndex);
            wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
            //wb.Navigate(new Uri(App.headUrl + "/news/news.aspx?newsid=" + newsId + "&pageIndex=" + pageIndex, UriKind.Absolute));
            refreshButton.Visibility = Visibility.Collapsed;
            wb.Visibility = Visibility.Visible;
            prompt.Visibility = Visibility.Collapsed;
        }

        private string CreateNewsViewUrl(int page)
        {
            int isSmallImageMode = Utils.MeHelper.GetIsSmallImageMode() ? 1 : 0;
            return AppUrlMgr.NewsWebViewUrl(Convert.ToInt32(newsId), 1, isSmallImageMode, 0, 1, page, 1, 0, 0);
        }

        private string CreateShuokeViewUrl()
        {
            return AppUrlMgr.ShuoWebViewUrl(Convert.ToInt32(newsId), 0, 1, 1, 0, 0);
        }

        #region 收藏管理

        //收藏文章
        private void favorite_Click(object sender, EventArgs e)
        {
            if (this.news != null)
            {
                var favoriteBtn = this.ApplicationBar.Buttons[0] as ApplicationBarIconButton;
                bool add = !favoriteBtn.Text.Contains("取消");
                this.uploadFavoriteArticle(add);
            }
        }

        private void uploadFavoriteArticle(bool add)
        {
            var curTime = DateTime.Now.ToString(Utils.MeHelper.FavoriteTimeFormat);
            var model = new ViewModels.Me.FavoriteViewModel.FavoriteSyncItem() { id = news.ID, time = curTime, action = add ? 0 : 1 };
            List<ViewModels.Me.FavoriteViewModel.FavoriteSyncItem> series = new List<ViewModels.Me.FavoriteViewModel.FavoriteSyncItem>();
            series.Add(model);
            var articlesStr = CommonLayer.JsonHelper.Serialize(series);

            var userInfoModel = Utils.MeHelper.GetMyInfoModel();
            if (userInfoModel != null)
            {
                string url = Utils.MeHelper.SyncFavoriteCollectionUrl;
                var ua = Common.GetAutoHomeUA();
                string data = string.Format("_appid={6}&authorization={0}&bbslist={1}&topiclist={2}&articlelist={3}&_timestamp={4}&autohomeua={5}", userInfoModel.Authorization, null, null, articlesStr, Common.GetTimeStamp(), ua, Utils.MeHelper.appID);
                data = Common.SortURLParamAsc(data);
                string sign = Common.GetSignStr(data);
                data += "&_sign=" + sign;

                var favoriteVM = ViewModels.Me.FavoriteViewModel.SingleInstance;
                UploadStringCompletedEventHandler uploadClient_UploadCompleted = (object sender, UploadStringCompletedEventArgs e) =>
                {
                    var success = favoriteVM.UploadFavoriteSuccess(e);
                    if (success)
                    {
                        setFavoriteButton(!add);
                        string msg = add ? "收藏成功" : "取消收藏成功";
                        Common.showMsg(msg);
                    }
                    else
                    {
                        saveFavoriteArticleLocally(add);
                    }
                };

                favoriteVM.UploadOthers(url, data, uploadClient_UploadCompleted, null, null, series);
            }
        }

        private void saveFavoriteArticleLocally(bool add)
        {
            if (add)
            {
                bool success = ViewModels.Me.FavoriteViewModel.SingleInstance.Add(FavoriteType.Article, news);
                setFavoriteButton(!success);
                string msg = success ? "收藏成功" : "收藏失败";
                Common.showMsg(msg);
            }
            else
            {
                bool success = ViewModels.Me.FavoriteViewModel.SingleInstance.Remove(FavoriteType.Article, new List<int> { news.ID });
                setFavoriteButton(success);
                string msg = success ? "取消收藏成功" : "取消收藏失败";
                Common.showMsg(msg);
            }
        }

        private void setFavoriteButton(bool? addFavorite = null)
        {
            var favoriteBtn = this.ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            if (news == null)
            {
                favoriteBtn.IsEnabled = false;
            }
            else
            {
                bool add;
                if (addFavorite.HasValue)
                {
                    add = addFavorite.Value;
                }
                else
                {
                    var exist = ViewModels.Me.FavoriteViewModel.SingleInstance.Exist(FavoriteType.Article, news.ID);
                    add = !exist;
                }
                string iconUrl = add ? "/Images/favs.addto.png" : "/Images/favs.png";
                favoriteBtn.IconUri = new Uri(iconUrl, UriKind.Relative);
                favoriteBtn.Text = add ? "收藏" : "取消收藏";
                favoriteBtn.IsEnabled = true;
            }
        }

        #endregion

        #region 大图小图模式

        //切换大图小图模式
        private void ImageMode_Click(object sender, EventArgs e)
        {
            var menuItem = this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
            bool toSmallMode = menuItem.Text.Contains("小");
            System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings[Utils.MeHelper.SmallImageModeKey] = toSmallMode;

            SetImageModeMenuItem(toSmallMode);
            refreshButton_Tap_1(null, null);
        }

        private void SetImageModeMenuItem(bool isSmallImageMode)
        {
            var menuItem = this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
            menuItem.Text = isSmallImageMode ? "大图模式" : "小图模式";
        }

        #endregion

        #region 浏览历史

        private void AddRecents()
        {
            ViewModels.Me.ViewHistoryViewModel.SingleInstance.AddItem(FavoriteType.Article, news);
        }

        #endregion
    }
}