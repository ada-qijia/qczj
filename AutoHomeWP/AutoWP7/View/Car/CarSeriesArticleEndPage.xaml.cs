using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Model;
using ViewModels;

namespace AutoWP7.View.Car
{
    /// <summary>
    /// 车系文章最终页
    /// </summary>
    public partial class CarSeriesArticleEndPage : PhoneApplicationPage
    {
        public CarSeriesArticleEndPage()
        {
            InitializeComponent();
        }

        //文章id
        string newsId = string.Empty;
        //页码
        int pageIndex;
        //页数
        int pageCount;
        //评论数
        static int replyCount;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {

                        UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "文章最终页的访问次数");
                        newsId = NavigationContext.QueryString["newsid"];
                        pageIndex = Convert.ToInt32(NavigationContext.QueryString["pageIndex"]);
                        App.newsid = newsId;
                        wb.IsScriptEnabled = true;
                        LoadData();

                    }
                    break;
                case System.Windows.Navigation.NavigationMode.Back:
                    {
                        ProgBar.Visibility = Visibility.Collapsed;
                        (App.Current as App).carSeriesPartPageMessage.Hide();
                        if (pageCount > 1)
                        {
                            IApplicationBarIconButton Icon;
                            for (int i = 0; i < 4; i++)
                            {
                                Icon = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                                Icon.IsEnabled = App.barStatus[i]; ;
                            }

                        }

                    }
                    break;


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
                        IApplicationBarIconButton iconButton = this.ApplicationBar.Buttons[1] as IApplicationBarIconButton;
                        iconButton.IsEnabled = false;

                    }
                }
                //wb.Navigate(new Uri(App.headUrl + "/news/news.aspx?newsid=" + newsId + "&pageIndex=" + pageIndex, UriKind.Absolute));
                string newsPageUrl = CreateNewsViewUrl(pageIndex);
                wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
            });

        }
        #endregion


        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if ((App.Current as App).carSeriesPartPageMessage.IsOpen == true)
            {
                (App.Current as App).carSeriesPartPageMessage.Hide();
            }
            else
            {
                //获得返回队列数
                int sum = NavigationService.BackStack.Count();
                for (int i = 3; i < sum; i++)
                {
                    NavigationService.RemoveBackEntry();
                }
                base.OnBackKeyPress(e);
            }
        }

        // 上页
        private void previousPage_Click(object sender, EventArgs e)
        {
            //关闭分页控件
            (App.Current as App).carSeriesPartPageMessage.Hide();
            UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "上页的点击量");
            pageIndex = pageIndex - 1;
            //wb.Navigate(new Uri(App.headUrl + "/news/news.aspx?newsid=" + newsId + "&pageIndex=" + pageIndex, UriKind.Absolute));
            string newsPageUrl = CreateNewsViewUrl(pageIndex);
            wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
        }

        // 查看评论
        private void checkComment_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "评论页的访问量");
            (App.Current as App).carSeriesPartPageMessage.Hide();
            for (int i = 0; i < 4; i++)
            {
                App.barStatus[i] = (this.ApplicationBar.Buttons[i] as IApplicationBarIconButton).IsEnabled;

            }
            this.NavigationService.Navigate(new Uri("/View/Channel/Newest/CommentListPage.xaml?newsid=" + newsId, UriKind.Relative));


        }

        // 下页
        private void nextPage_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "下页的点击量");
            (App.Current as App).carSeriesPartPageMessage.Hide();
            pageIndex = pageIndex + 1;
            //wb.Navigate(new Uri(App.headUrl + "/news/news.aspx?newsid=" + newsId + "&pageIndex=" + pageIndex, UriKind.Absolute));
            string newsPageUrl = CreateNewsViewUrl(pageIndex);
            wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
        }

        //分页
        private void partPage_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "分页的点击量");
            for (int i = 0; i < 4; i++)
            {
                App.barStatus[i] = (this.ApplicationBar.Buttons[i] as IApplicationBarIconButton).IsEnabled;

            }
            if ((App.Current as App).carSeriesPartPageMessage.IsOpen == true)
            {
                (App.Current as App).carSeriesPartPageMessage.Hide();
            }
            else
            {
                (App.Current as App).carSeriesPartPageMessage.Show();
            }

        }

        //加载中
        private void wb_Navigating(object sender, NavigatingEventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                IApplicationBarIconButton Icon = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                Icon.IsEnabled = false;
            }
            ProgBar.Visibility = Visibility.Visible;
        }

        //加载完成
        private void wb_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            ProgBar.Visibility = Visibility.Collapsed;

            if (pageCount <= 1)//如果总页数小于等于1  
            {
                for (int i = 0; i < 2; i++)
                {
                    //将上页和分页控件置灰
                    IApplicationBarIconButton iconButton = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                    iconButton.IsEnabled = false;
                    //将下页控件置灰
                    IApplicationBarIconButton Icon = this.ApplicationBar.Buttons[3] as IApplicationBarIconButton;
                    Icon.IsEnabled = false;
                }
                //将评论控件置可见
                IApplicationBarIconButton commentIcon = this.ApplicationBar.Buttons[2] as IApplicationBarIconButton;
                commentIcon.IsEnabled = true;

            }
            else
            {

                for (int i = 1; i < 4; i++)
                {
                    IApplicationBarIconButton Icon = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                    Icon.IsEnabled = true;
                }
                if (pageIndex <= 1)//当前页码为1时
                {
                    //将上页控件置灰
                    IApplicationBarIconButton priviousIcon = this.ApplicationBar.Buttons[0] as IApplicationBarIconButton;
                    priviousIcon.IsEnabled = false;
                }
                else //当前页码为大于等于2时
                {
                    if (pageIndex >= pageCount)//如果当前页为最后一页
                    {
                        //下页置灰
                        IApplicationBarIconButton nextIcon = this.ApplicationBar.Buttons[3] as IApplicationBarIconButton;
                        nextIcon.IsEnabled = false;
                    }
                    if (pageIndex <= 2)
                    {
                        //上页置灰
                        IApplicationBarIconButton priviouesIcon = this.ApplicationBar.Buttons[0] as IApplicationBarIconButton;
                        priviouesIcon.IsEnabled = false;
                    }
                    //上页置为可用
                    IApplicationBarIconButton priviousIcon = this.ApplicationBar.Buttons[0] as IApplicationBarIconButton;
                    priviousIcon.IsEnabled = true;
                }

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
                    for (int i = 0; i < 4; i++)
                    {
                        App.barStatus[i] = (this.ApplicationBar.Buttons[i] as IApplicationBarIconButton).IsEnabled;

                    }

                    this.NavigationService.Navigate(new Uri("/View/Channel/Newest/ShowBigImage.xaml?id=" + newsId + "&url=" + e.Value, UriKind.Relative));
                }
                else
                {
                    if (e.Value.Contains("-"))//带页码的文章最终页
                    {
                        string[] strArrary = e.Value.Split('-');
                        newsId = strArrary[0];
                        pageIndex = Convert.ToInt32(strArrary[1]);

                        urlSource = new Uri("/View/Channel/News/NewsEndPage.xaml?newsid=" + newsId + "&pageIndex=" + pageIndex + "&pageType=1", UriKind.Relative);
                        //urlSource = new Uri("/View/Channel/Newest/ArticleEndPage.xaml?" + new Guid().ToString() + "&newsid=" + newsId + "&pageIndex=" + pageIndex, UriKind.Relative);
                        this.NavigationService.Navigate(urlSource);

                    }
                    else  //不带页码的文章最终页
                    {
                        newsId = e.Value;
                        pageIndex = 1;
                        urlSource = new Uri("/View/Channel/News/NewsEndPage.xaml?newsid=" + newsId + "&pageIndex=" + pageIndex + "&pageType=1", UriKind.Relative);
                        //urlSource = new Uri("/View/Channel/Newest/ArticleEndPage.xaml?" + new Guid().ToString() + "&newsid=" + newsId + "&pageIndex=" + pageIndex, UriKind.Relative);
                        this.NavigationService.Navigate(urlSource);
                    }
                }
            }
            catch
            { }
        }

        private string CreateNewsViewUrl(int pageIndex)
        {
            int isSmallImageMode = Utils.MeHelper.GetIsSmallImageMode() ? 1 : 0;
            return AppUrlMgr.NewsWebViewUrl(Convert.ToInt32(newsId), 1, isSmallImageMode, 0, 1, pageIndex, 1, 0, 0);
        }
    }
}