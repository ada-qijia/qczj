using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Model;
using ViewModels;

namespace AutoWP7.View.Channel.Newest
{
    public partial class ArticleEndPage : PhoneApplicationPage
    {
        public ArticleEndPage()
        {
            InitializeComponent();
            wb.ScriptNotify += new EventHandler<NotifyEventArgs>(wb_ScriptNotify);
        }

        //文章Id
        string newsId = string.Empty;
        //页码
        int pageIndex;
        //页数
        int pageCount;
        //评论数
        static int replyCount;
        bool isLoaded = false;
        int buttomCount = 2;
        string newsType = string.Empty;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            buttomCount = this.ApplicationBar.Buttons.Count;
            switch (e.NavigationMode)
            {

                //第一次进入页面
                case System.Windows.Navigation.NavigationMode.New:
                    {
                        UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "文章最终页的访问次数");
                        newsId = NavigationContext.QueryString["newsid"];
                        pageIndex = Convert.ToInt32(NavigationContext.QueryString["pageIndex"]);
                        if (this.NavigationContext.QueryString.ContainsKey("newsType"))
                            newsType = NavigationContext.QueryString["newsType"];
                        App.newsid = newsId;
                        wb.IsScriptEnabled = true;
                        if (newsType == "说客")
                        {
                            LoadShuokeData(newsId);
                        }
                        else
                        {
                            LoadData(newsId);
                        }
                    }
                    break;
                //返回页面
                case System.Windows.Navigation.NavigationMode.Back:
                    {
                        ProgBar.Visibility = Visibility.Collapsed;

                        //if (pageCount > 1)
                        //{
                        IApplicationBarIconButton Icon;
                        for (int i = 0; i < buttomCount; i++)
                        {
                            Icon = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                            Icon.IsEnabled = App.barStatus[i]; ;
                        }
                        // }

                    }
                    break;


            }

        }

        //文章内的链接
        void wb_ScriptNotify(object sender, NotifyEventArgs e)
        {
            try
            {
                Uri urlSource;
                if (e.Value.Contains("h"))//大图
                {
                    for (int i = 0; i < buttomCount; i++)
                    {
                        App.barStatus[i] = (this.ApplicationBar.Buttons[i] as IApplicationBarIconButton).IsEnabled;
                    }

                    this.NavigationService.Navigate(new Uri("/View/Channel/Newest/ShowBigImage.xaml?type=1&id=" + newsId + "&url=" + e.Value, UriKind.Relative));
                }
                else
                {
                    if (e.Value.Contains("-"))//带页码的文章最终页
                    {
                        string[] strArrary = e.Value.Split('-');
                        newsId = strArrary[0];
                        pageIndex = Convert.ToInt32(strArrary[1]);
                        urlSource = new Uri("/View/Channel/Newest/ArticleEndPage.xaml?" + new Guid().ToString() + "&newsid=" + newsId + "&pageIndex=" + pageIndex, UriKind.Relative);
                        this.NavigationService.Navigate(urlSource);

                    }
                    else  //不带页码的文章最终页
                    {
                        newsId = e.Value;
                        pageIndex = 1;
                        urlSource = new Uri("/View/Channel/Newest/ArticleEndPage.xaml?" + new Guid().ToString() + "&newsid=" + newsId + "&pageIndex=" + pageIndex, UriKind.Relative);
                        this.NavigationService.Navigate(urlSource);
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        //分页集合
        public static ObservableCollection<NewsDetailModel> newsDataSource = new ObservableCollection<NewsDetailModel>();

        #region 分页数据加载

        NewsDetailViewModel newsVM = null;
        // 分页数据加载
        private void LoadData(string newsId)
        {
            newsVM = new NewsDetailViewModel();
            string url = string.Format("{0}{2}/content/news/newsinfo-a2-pm3-v1.6.0-i{1}.html", App.newsPageDomain, newsId, App.versionStr);
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
                        IApplicationBarIconButton iconButton = this.ApplicationBar.Buttons[0] as IApplicationBarIconButton;
                        iconButton.IsEnabled = false;

                    }
                }
                //string newsPageUrl = "";
                string newsPageUrl = string.Empty;

                newsPageUrl = CreateNewsPageUrl(pageIndex);

                wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
            });

        }
        private void LoadShuokeData(string newsId)
        {
            string newsPageUrl = string.Empty;
            newsPageUrl = CreateShuokeViewUrl();
            wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
        }
        #endregion

        //退出页面
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if ((App.Current as App).newestPartPageMessage.IsOpen == true)
            {
                (App.Current as App).newestPartPageMessage.Hide();
                e.Cancel = true;
            }
            else
            {

                ////获得返回队列数
                //int sum = NavigationService.BackStack.Count();
                //for (int i = 1; i < sum; i++)
                //{
                //    NavigationService.RemoveBackEntry();
                //}
                base.OnBackKeyPress(e);
            }
        }

        // 上页
        private void previousPage_Click(object sender, EventArgs e)
        {

            UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "上页的点击量");
            (App.Current as App).newestPartPageMessage.Hide();
            pageIndex = pageIndex - 1;
            string newsPageUrl = CreateNewsPageUrl(pageIndex);
            wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
            //wb.Navigate(new Uri(App.headUrl + "/news/news.aspx?newsid=" + newsId + "&pageIndex=" + pageIndex, UriKind.Absolute));
        }

        // 查看评论
        private void checkComment_Click(object sender, EventArgs e)
        {

            UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "评论页的访问量");

            for (int i = 0; i < buttomCount; i++)
            {
                App.barStatus[i] = (this.ApplicationBar.Buttons[i] as IApplicationBarIconButton).IsEnabled;

            }
            int pageType = newsType == "说客" ? 2 : 1;
            (App.Current as App).newestPartPageMessage.Hide();
            this.NavigationService.Navigate(new Uri("/View/Channel/Newest/CommentListPage.xaml?newsid=" + newsId + "&pageType=" + pageType, UriKind.Relative));



        }

        // 下页
        private void nextPage_Click(object sender, EventArgs e)
        {

            UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "下页的点击量");
            (App.Current as App).newestPartPageMessage.Hide();
            pageIndex = pageIndex + 1;
            string newsPageUrl = CreateNewsPageUrl(pageIndex);
            wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
            //wb.Navigate(new Uri(App.headUrl + "/news/news.aspx?newsid=" + newsId + "&pageIndex=" + pageIndex, UriKind.Absolute));
        }


        //分页
        private void partPage_Click(object sender, EventArgs e)
        {

            UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "分页的点击量");
            for (int i = 0; i < buttomCount; i++)
            {
                App.barStatus[i] = (this.ApplicationBar.Buttons[i] as IApplicationBarIconButton).IsEnabled;

            }
            if ((App.Current as App).newestPartPageMessage.IsOpen == true)
            {
                (App.Current as App).newestPartPageMessage.Hide();
            }
            else
            {
                (App.Current as App).newestPartPageMessage.Show();
            }

        }

        //页面加载中
        private void wb_Navigating(object sender, NavigatingEventArgs e)
        {
            IApplicationBarIconButton Icon;
            for (int i = 0; i < buttomCount; i++)
            {
                Icon = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                Icon.IsEnabled = false;
            }
            ProgBar.Visibility = Visibility.Visible;
        }

        //页面加载完成
        private void wb_Navigated(object sender, NavigationEventArgs e)
        {
            ProgBar.Visibility = Visibility.Collapsed;
            IApplicationBarIconButton iconButton;
            if (pageCount <= 1)//如果总页数小于等于1  
            {
                //新版本没有分页功能  wkx 2014-04-24修改
                //for (int i = 0; i < 2; i++)
                //{
                //    //将上页和分页控件置灰
                //    iconButton = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                //    iconButton.IsEnabled = false;
                //}
                //将下页控件置灰
                //iconButton = this.ApplicationBar.Buttons[3] as IApplicationBarIconButton;
                //iconButton.IsEnabled = false;

                //将分页控件置灰
                iconButton = this.ApplicationBar.Buttons[0] as IApplicationBarIconButton;
                iconButton.IsEnabled = false;

                //将评论控件置可见
                iconButton = this.ApplicationBar.Buttons[1] as IApplicationBarIconButton;
                iconButton.IsEnabled = true;

            }
            else
            {

                for (int i = 0; i < buttomCount; i++)
                {
                    iconButton = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                    iconButton.IsEnabled = true;
                }
                //新版本没有分页功能
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

        //加载失败
        private void wb_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            //失败提示
            prompt.Visibility = Visibility.Visible;
            refreshButton.Visibility = Visibility.Visible;

            wb.Visibility = Visibility.Collapsed;
            ProgBar.Visibility = Visibility.Collapsed;
        }

        //刷新
        private void refreshButton_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {

            string newsPageUrl = string.Empty;
            if (newsType.Equals("说客"))
            {
                newsPageUrl = CreateShuokeViewUrl();
            }
            else
            {
                newsPageUrl = CreateNewsPageUrl(pageIndex);

            }

            wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
            //wb.Navigate(new Uri(App.headUrl + "/news/news.aspx?newsid=" + newsId + "&pageIndex=" + pageIndex, UriKind.Absolute));
            refreshButton.Visibility = Visibility.Collapsed;
            wb.Visibility = Visibility.Visible;
            prompt.Visibility = Visibility.Collapsed;
        }
        /// <summary>
        /// 生成说客最终页地址
        /// </summary>
        /// <returns></returns>
        private string CreateShuokeViewUrl()
        {
            return AppUrlMgr.ShuoWebViewUrl(Convert.ToInt32(newsId), 0, 1, 1, 0, 0);
        }
        /// <summary>
        /// 生成资讯最终页地址
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        private string CreateNewsPageUrl(int pageIndex)
        {
            return AppUrlMgr.NewsWebViewUrl(Convert.ToInt32(newsId), 1, 0, 0, 1, pageIndex, 1, 0, 0);
        }
    }
}