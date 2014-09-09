using System;
using System.Net;
using System.Windows;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Newtonsoft.Json.Linq;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Model;
using ViewModels.Handler;
using System.Windows.Navigation;
using System.Collections;
using System.Collections.Generic;

namespace AutoWP7.View.Forum
{
    /// <summary>
    /// 帖子最终页
    /// </summary>
    public partial class TopicDetailPage : PhoneApplicationPage
    {
        public TopicDetailPage()
        {
            InitializeComponent();
        }

        //论坛id
        string bbsId = string.Empty;
        //帖子Id
        string topicId = string.Empty;
        //类型
        string bbsType = string.Empty;
        //总楼数     
        int replyCount;
        //总页数
        int pageCount;
        //初始页码
        int pageIndex = 1;
        int pageSize = 20;
        int floor = 0;
        int isOnlyOwner = 0;
        int issend = 0;//是否是发帖后马上加载最终页（1-是，加载写库数据；0-否，加载读库数据，会有延迟）
        //最终页地址
        Uri urlSource;
        //0:跳转来自论坛帖子列表，1:跳转来自车系综述页论坛帖子列表
        int fromType = 0;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (this.NavigationContext.QueryString.ContainsKey("floor"))
                floor = Convert.ToInt16(this.NavigationContext.QueryString["floor"]);
            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {
                        if (this.NavigationContext.QueryString.ContainsKey("from"))
                        {
                            fromType = Convert.ToInt16(this.NavigationContext.QueryString["from"]);
                        }
                        if (fromType == 1)
                            UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车系页~帖子最终页点击量");
                        else
                            UmengSDK.UmengAnalytics.onEvent("ForumActivity", "帖子最终页点击量");
                        topicId = NavigationContext.QueryString["topicId"];
                        bbsType = this.NavigationContext.QueryString["bbsType"];
                        bbsId = this.NavigationContext.QueryString["bbsId"];                        
                        if (this.NavigationContext.QueryString.ContainsKey("isowner"))
                        {
                            if (!string.IsNullOrEmpty(this.NavigationContext.QueryString["isowner"]))
                            {
                                int iw = 0;
                                int.TryParse(this.NavigationContext.QueryString["isowner"], out iw);
                                isOnlyOwner = iw == 1 ? 1 : 0;
                            }
                        }
                        if (this.NavigationContext.QueryString.ContainsKey("issend"))
                        {
                            if (!string.IsNullOrEmpty(this.NavigationContext.QueryString["issend"]))
                            {
                                int iw = 0;
                                int.TryParse(this.NavigationContext.QueryString["issend"], out iw);
                                issend = iw == 1 ? 1 : 0;
                            }
                        }
                        string url = CreateTopicView(pageIndex, true);
                        urlSource = new Uri(url, UriKind.Absolute);

                        LoadData();
                    }
                    break;
                case System.Windows.Navigation.NavigationMode.Back:
                    {

                        IApplicationBarIconButton Icon;
                        for (int i = 0; i < 4; i++)
                        {
                            Icon = this.ApplicationBar.Buttons[i] as IApplicationBarIconButton;
                            Icon.IsEnabled = App.barStatus[i]; ;
                        }
                        InitApplicationIconButton();

                        ProgBar.Visibility = Visibility.Collapsed;
                        if (App.IsLoadTag)
                        {
                            App.IsLoadTag = false;
                            try
                            {
                                pageIndex = App.pageIndex;
                                string url = App.TopicUrl;
                                urlSource = new Uri(url + "?data=" + DateTime.Now, UriKind.Absolute);
                            }
                            catch (Exception ex)
                            {

                            }
                            finally
                            {
                                LoadData();
                            }
                        }
                    }

                    break;

            }

            ApplicationBarIconButton abbtn = this.ApplicationBar.Buttons[2] as ApplicationBarIconButton;
            abbtn.Text = isOnlyOwner == 0 ? "只看楼主" : "查看全部";
            abbtn.IconUri = isOnlyOwner == 0 ? new Uri("/Images/bar_louzhu.png", UriKind.Relative) : new Uri("/Images/bar_all.png", UriKind.Relative);
        }


        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);


            //获得返回队列数
            IEnumerable<JournalEntry> items = this.NavigationService.BackStack;
            int itemsCount = 0;
            foreach (JournalEntry item in items)
            {
                itemsCount++;

            }
            for (int i = 0; i < itemsCount - 3; i++)
            {
                this.NavigationService.RemoveBackEntry();
            }

        }

        #region 首次加载

        WebClient wc = null;
        public void LoadData()
        {

            if (wc == null)
            {
                wc = new WebClient();
            }

            Uri url = new Uri(string.Format("{0}{2}/club/topicinfo-a2-pm3-v1.4.0-t{1}-i0.html", App.appUrl, topicId, App.versionStr), UriKind.Absolute);

            wc.DownloadStringAsync(url);
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                JObject json = JObject.Parse(ee.Result);
                if (json != null)
                {
                    replyCount = (int)json.SelectToken("result").SelectToken("ownerreplycount");


                    if (replyCount % 20 == 0)
                    {
                        pageCount = replyCount / 20;
                    }
                    else
                    {
                        pageCount = replyCount / 20 + 1;
                    }
                }
                webTopicDetail.Navigate(urlSource);
            });
        }
        #endregion

        //下页
        private void nextPage_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ForumActivity", "帖子最终页下一页点击量");
            pageIndex++;
            string url = CreateTopicView(pageIndex, false);
            webTopicDetail.Navigate(new Uri(url, UriKind.Absolute));
        }

        //上页
        private void previousPage_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ForumActivity", "帖子最终页上一页点击量");
            pageIndex--;
            string url = CreateTopicView(pageIndex, false);
            webTopicDetail.Navigate(new Uri(url, UriKind.Absolute));
        }

        //加载中
        private void webTopicDetail_Navigating(object sender, NavigatingEventArgs e)
        {
            IApplicationBarIconButton Icon;
            for (int i = 0; i < 4; i++)
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
            InitApplicationIconButton();
        }

        private void InitApplicationIconButton()
        {
            IApplicationBarIconButton Icon;
            if (pageCount <= 1)//页数为1时
            {
                //上页置灰
                Icon = this.ApplicationBar.Buttons[0] as IApplicationBarIconButton;
                Icon.IsEnabled = false;
                //评论
                Icon = this.ApplicationBar.Buttons[1] as IApplicationBarIconButton;
                Icon.IsEnabled = true;
                //只看楼主
                Icon = this.ApplicationBar.Buttons[2] as IApplicationBarIconButton;
                Icon.IsEnabled = true;
                //下页
                Icon = this.ApplicationBar.Buttons[3] as IApplicationBarIconButton;
                Icon.IsEnabled = false;
            }
            else //页数大于1时
            {
                for (int i = 0; i < 4; i++)
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
                        Icon = this.ApplicationBar.Buttons[3] as IApplicationBarIconButton;
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

        //大图和回复
        private void webTopicDetail_ScriptNotify(object sender, NotifyEventArgs e)
        {
            if (e.Value.Contains("h"))   //查看大图
            {
                for (int i = 0; i < 3; i++)
                {
                    App.barStatus[i] = (this.ApplicationBar.Buttons[i] as IApplicationBarIconButton).IsEnabled;

                }
                //this.NavigationService.Navigate(new Uri("/View/Forum/ShowBigImage.xaml?type=2&id=" + topicId + "&url=" + e.Value, UriKind.Relative));
                this.NavigationService.Navigate(new Uri("/View/Channel/Newest/ShowBigImage.xaml?type=2&id=" + topicId + "&url=" + e.Value, UriKind.Relative));
            }
            else//回复
            {
                this.NavigationService.Navigate(new Uri("/View/Forum/ReplyCommentPage.xaml?bbsId=" + bbsId + "&topicId=" +
                    topicId + "&bbsType=" + bbsType + "&targetReplyId=" + e.Value + "&url=creatReply" + "&pageindex=" + pageIndex, UriKind.Relative));
            }

        }

        //快速回复
        private void fastReply_Click_1(object sender, EventArgs e)
        {
            string url = "createReplyManyImage";
            this.NavigationService.Navigate(new Uri("/View/Forum/ReplyCommentPage.xaml?bbsId=" + bbsId + "&bbsType=" +
               bbsType + "&topicId=" + topicId + "&targetReplyId=0&url=" + url + "&pageindex=" + pageIndex, UriKind.Relative));

        }

        /// <summary>
        /// 生成帖子最终页分页地址
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        private string CreateTopicView(int pageIndex, bool jumpFloor)
        {
            return AppUrlMgr.TopicWebViewUrl(Convert.ToInt64(topicId), isOnlyOwner, pageIndex, 20, 1, 0, 0, 0, floor, 0,issend);
        }

        private void onlyowner_Click_1(object sender, EventArgs e)
        {
            if (isOnlyOwner == 0)
                this.NavigationService.Navigate(new Uri("/View/Forum/TopicDetailPage.xaml?from=0&bbsId=" + bbsId + "&topicId=" + topicId + "&bbsType=" + bbsType + "&isowner=1", UriKind.Relative));
            else
                this.NavigationService.Navigate(new Uri("/View/Forum/TopicDetailPage.xaml?from=0&bbsId=" + bbsId + "&topicId=" + topicId + "&bbsType=" + bbsType + "&isowner=0", UriKind.Relative));
        }
    }


}