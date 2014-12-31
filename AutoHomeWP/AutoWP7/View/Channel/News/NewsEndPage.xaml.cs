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
                            LoadEndPage();
                        }
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
                                Icon.IsEnabled = App.barStatus[i]; ;
                            }
                        }


                    }
                    break;

            }

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

                string newsPageUrl = CreateNewsViewUrl(pageIndex);
                wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
            });

        }
        #endregion

        private void LoadEndPage()
        {
            if (pageType == 2)
            {
                string newsPageUrl = AppUrlMgr.ShuoWebViewUrl(Convert.ToInt32(newsId), 0, 1, 1, 0, 0);
                wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
            }
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
            for (int i = 0; i < buttonCount; i++)
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
                for (int i = 0; i < buttonCount; i++)
                {
                    //将上页和分页控件置灰
                    iconButton = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                    iconButton.IsEnabled = false;
                }
                //将评论控件置可见
                iconButton = this.ApplicationBar.Buttons[1] as IApplicationBarIconButton;
                iconButton.IsEnabled = true;

            }
            else
            {

                for (int i = 0; i < buttonCount; i++)
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
            string newsPageUrl = CreateNewsViewUrl(pageIndex);
            wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
            //wb.Navigate(new Uri(App.headUrl + "/news/news.aspx?newsid=" + newsId + "&pageIndex=" + pageIndex, UriKind.Absolute));
            refreshButton.Visibility = Visibility.Collapsed;
            wb.Visibility = Visibility.Visible;
            prompt.Visibility = Visibility.Collapsed;
        }

        private string CreateNewsViewUrl(int page)
        {
            return AppUrlMgr.NewsWebViewUrl(Convert.ToInt32(newsId), 1, 0, 0, 1, page, 1, 0, 0);
        }

        //收藏
        private void favorite_Click(object sender, EventArgs e)
        {

        }
    }
}