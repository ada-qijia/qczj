using System;
using System.Net;
using System.Windows;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Newtonsoft.Json.Linq;
using Microsoft.Phone.Shell;

namespace AutoWP7.View.Car
{
    /// <summary>
    /// 帖子列表页
    /// </summary>
    public partial class TopicDetailPage : PhoneApplicationPage
    {
        public TopicDetailPage()
        {
            InitializeComponent();
        }

        //论坛id
        string bbsId = string.Empty;
        //论坛类型
        string bbsType = string.Empty;
        //帖子Id
        string topicId = string.Empty;
        //总楼数
        int replyCount;
        //页数
        int pageCount;
        //初始楼数
        int pageIndex = 1;
        //int pageSize = 20;
        Uri urlSource;
        int issend = 0;//是否是发帖后马上加载最终页（1-是，加载写库数据；0-否，加载读库数据，会有延迟）
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {
                        UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车系页~帖子最终页点击量");
                        topicId = NavigationContext.QueryString["topicId"];
                        bbsId = this.NavigationContext.QueryString["bbsId"];
                        bbsType = this.NavigationContext.QueryString["bbsType"];
                        if (this.NavigationContext.QueryString.ContainsKey("issend"))
                        {
                            if (!string.IsNullOrEmpty(this.NavigationContext.QueryString["issend"]))
                            {
                                int iw = 0;
                                int.TryParse(this.NavigationContext.QueryString["issend"], out iw);
                                issend = iw == 1 ? 1 : 0;
                            }
                        }
                        webTopicDetail.IsScriptEnabled = true;
                        // urlSource = new Uri(App.headUrl + "/clubapp/topicdetail/topic_version110.aspx?topicId=" + topicId + "&bbsType=" + bbsType + "&owner=0&pageIndex=1&pageSize=20", UriKind.Absolute);
                        //urlSource = new Uri("http://club.autohome.com.cn/bbs/mobile-" + bbsType + "-" + bbsId + "-" + topicId + "-1.html?clienttype=WP", UriKind.Absolute);
                        string url = CreateTopicViewUrl(1);
                        urlSource = new Uri(url, UriKind.Absolute);
                        LoadData();
                    }
                    break;
                case System.Windows.Navigation.NavigationMode.Back:
                    {
                        IApplicationBarIconButton Icon;
                        for (int i = 0; i < 3; i++)
                        {
                            Icon = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                            Icon.IsEnabled = App.barStatus[i]; ;
                        }

                        ProgBar.Visibility = Visibility.Collapsed;
                        if (App.IsLoadTag)
                        {
                            App.IsLoadTag = false;
                            try
                            {
                                pageIndex = App.pageIndex;
                                string url = CreateTopicViewUrl(1);
                                urlSource = new Uri(url, UriKind.Absolute);

                            }
                            catch
                            { }
                            finally
                            {
                                LoadData();
                            }
                        }
                    }
                    break;
            }

            //设置小图模式开关
            bool isSmallImageMode = Utils.MeHelper.GetIsSmallImageMode();
            SetImageModeMenuItem(isSmallImageMode);
        }
        #region 首次加载

        WebClient wc = null;
        public void LoadData()
        {

            if (wc == null)
            {
                wc = new WebClient();
            }
            //Uri urlSource = new Uri(App.headUrl + "/clubapp/topicdetail/ownerReplyCount.ashx?" + new Guid().ToString() + "&topicId=" + topicId + "&bbsType=c&owner=0", UriKind.Absolute);

            Uri url = new Uri(string.Format("{0}{2}/forum/club/topicinfo-a2-pm3-v1.6.0-t{1}-i0.html", App.topicPageDomain, topicId, App.versionStr), UriKind.Absolute);
            wc.DownloadStringAsync(url);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
        }

        void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    Common.NetworkAvailablePrompt();
                }
                else if (e.Cancelled == false)
                {
                    JObject json = JObject.Parse(e.Result);
                    replyCount = (int)json.SelectToken("result").SelectToken("topicinfo")[0].SelectToken("replycounts");

                    if (replyCount % 20 == 0)
                    {
                        pageCount = replyCount / 20;
                    }
                    else
                    {
                        pageCount = replyCount / 20 + 1;
                    }
                    webTopicDetail.Navigate(urlSource);
                }
            }
            catch
            { }

        }
        #endregion

        //下页
        private void nextPage_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ForumActivity", "帖子最终页下一页点击量");
            pageIndex++;
            //  webTopicDetail.Navigate(new Uri(App.headUrl + "/clubapp/topicdetail/topic_version110.aspx?topicId=" + topicId + "&bbsType=c&owner=0&pageIndex=" + pageIndex + "&pageSize=20", UriKind.Absolute));
            string url = CreateTopicViewUrl(pageIndex);
            webTopicDetail.Navigate(new Uri(url, UriKind.Absolute));
        }

        //上页
        private void previousPage_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ForumActivity", "帖子最终页上一页点击量");
            pageIndex--;
            // webTopicDetail.Navigate(new Uri(App.headUrl + "/clubapp/topicdetail/topic_version110.aspx?topicId=" + topicId + "&bbsType=c&owner=0&pageIndex=" + pageIndex + "&pageSize=20", UriKind.Absolute));
            string url = CreateTopicViewUrl(pageIndex);

            webTopicDetail.Navigate(new Uri(url, UriKind.Absolute));
        }

        //加载中
        private void webTopicDetail_Navigating(object sender, NavigatingEventArgs e)
        {
            IApplicationBarIconButton Icon;
            for (int i = 0; i < 3; i++)
            {
                Icon = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                Icon.IsEnabled = false;
            }
            ProgBar.Visibility = Visibility.Visible;
        }

        //加载完成
        private void webTopicDetail_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            //进度条
            ProgBar.Visibility = Visibility.Collapsed;
            IApplicationBarIconButton Icon;
            if (pageCount <= 1)//页数为1时
            {
                //上页置灰
                Icon = this.ApplicationBar.Buttons[0] as IApplicationBarIconButton;
                Icon.IsEnabled = false;
                //评论
                Icon = this.ApplicationBar.Buttons[1] as IApplicationBarIconButton;
                Icon.IsEnabled = true;
                //下页
                Icon = this.ApplicationBar.Buttons[2] as IApplicationBarIconButton;
                Icon.IsEnabled = false;
            }
            else //页数大于1时
            {
                for (int i = 0; i < 3; i++)
                {
                    Icon = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                    Icon.IsEnabled = true;
                }
                if (pageIndex <= 1) //页码为1时
                {
                    Icon = this.ApplicationBar.Buttons[0] as IApplicationBarIconButton;
                    Icon.IsEnabled = false;
                }
                else
                {
                    if (pageIndex >= pageCount) //如果为最后一页
                    {
                        //下页置灰
                        Icon = this.ApplicationBar.Buttons[2] as IApplicationBarIconButton;
                        Icon.IsEnabled = false;
                    }
                    if (pageIndex <= 2)
                    {
                        //上页置灰
                        Icon = this.ApplicationBar.Buttons[0] as IApplicationBarIconButton;
                        Icon.IsEnabled = false;
                    }
                    //上页置为可用
                    Icon = this.ApplicationBar.Buttons[0] as IApplicationBarIconButton;
                    Icon.IsEnabled = true;
                }

            }
        }

        //查看大图
        private void webTopicDetail_ScriptNotify(object sender, NotifyEventArgs e)
        {

            if (e.Value.Contains("h"))   //查看大图
            {
                for (int i = 0; i < 3; i++)
                {
                    App.barStatus[i] = (this.ApplicationBar.Buttons[i] as IApplicationBarIconButton).IsEnabled;

                }
                this.NavigationService.Navigate(new Uri("/View/Forum/ShowBigImage.xaml?url=" + e.Value, UriKind.Relative));
            }
            else//回复
            {
                this.NavigationService.Navigate(new Uri("/View/Forum/ReplyCommentPage.xaml?bbsId=" + bbsId + "&topicId=" +
                    topicId + "&bbsType=" + bbsType + "&targetReplyId=" + e.Value + "&url=creatReply", UriKind.Relative));
            }

        }

        //快速回复
        private void fastReply_Click_1(object sender, EventArgs e)
        {
            string url = "createReplyManyImage";
            this.NavigationService.Navigate(new Uri("/View/Forum/ReplyCommentPage.xaml?bbsId=" + bbsId + "&bbsType=" +
               bbsType + "&topicId=" + topicId + "&targetReplyId=0&url=" + url + "", UriKind.Relative));
        }

        private string CreateTopicViewUrl(int pageIndex)
        {
            int isSmallImageMode = Utils.MeHelper.GetIsSmallImageMode() ? 1 : 0;
            return AppUrlMgr.TopicWebViewUrl(Convert.ToInt64(topicId), 0, pageIndex, 20, 1, 0, 0, isSmallImageMode, 0, 0, issend);
        }

        #region 大图小图模式

        //刷新
        private void Refresh()
        {
            string url = CreateTopicViewUrl(1);
            webTopicDetail.Navigate(new Uri(url, UriKind.Absolute));
        }

        //切换大图小图模式
        private void ImageMode_Click(object sender, EventArgs e)
        {
            var menuItem = this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
            bool toSmallMode = menuItem.Text.Contains("小");
            System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings[Utils.MeHelper.SmallImageModeKey] = toSmallMode;

            SetImageModeMenuItem(toSmallMode);
            Refresh();
        }

        private void SetImageModeMenuItem(bool isSmallImageMode)
        {
            var menuItem = this.ApplicationBar.MenuItems[0] as ApplicationBarMenuItem;
            menuItem.Text = isSmallImageMode ? "大图模式" : "小图模式";
        }

        #endregion
    }
}