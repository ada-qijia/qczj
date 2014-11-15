using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Model;
using ViewModels;
using ViewModels.Handler;
using AutoWP7.Utils;

namespace AutoWP7.View.Forum
{
    /// <summary>
    /// 帖子列表页
    /// </summary>
    public partial class LetterListPage : PhoneApplicationPage
    {
        public LetterListPage()
        {
            InitializeComponent();
        }

        string id = string.Empty;

        string title = string.Empty;
        //论坛id
        string bbsId = string.Empty;
        //论坛类型
        string bbsType = string.Empty;
        //标识加载状态（false 未加载 true 正在加载）
        bool isLoading = false;
        //加载页容量
        int loadPageSize = 20;
        string currentPage = "-p1-";
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                bbsId = NavigationContext.QueryString["bbsId"];
                //最新回复数据加载

                id = NavigationContext.QueryString["id"];

                title = NavigationContext.QueryString["title"];

                bbsType = this.NavigationContext.QueryString["bbsType"];
                url = CreateTopicsUrl(false, 0);
                //url = string.Format(App.headUrl + "/clubapp/topicList/topiclist.ashx?orderBy=replyDate&bbsId={0}&bbs={1}", bbsId,bbsType);
                ForumListLoadData(url, true);
            }
        }



        #region 论坛数据加载

        string url = string.Empty;
        public ObservableCollection<ForumModel> forumDataSource = new ObservableCollection<ForumModel>();
        CarSeriesForumViewModel forumVM = null;
        int forumPageIndex = 1; //页码
        /// <summary>
        /// 数据加载
        /// </summary>
        public void ForumListLoadData(string url, bool isFirst)
        {
            if (isFirst)
            {
                forumPageIndex = 1;
                //标题
                letterName.Text = title;
            }
            GlobalIndicator.Instance.Text = "正在获取中......";
            GlobalIndicator.Instance.IsBusy = true;
            forumVM = new CarSeriesForumViewModel();
            forumVM.LoadDataAysnc(url);
            forumVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<ForumModel>>>(forumVM_LoadDataCompleted);
        }

        /// <summary>
        /// 数据加载完成
        /// </summary>
        void forumVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<ForumModel>> e)
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
                if (e.Result.Count() == 1 && forumPageIndex == 1)
                {
                    Common.showMsg("暂无数据");
                }
                else
                {
                    if (e.Result.Count() <= 1 && forumPageIndex > 1)
                    {
                        Common.showMsg("已经是最后一页了~~");
                    }
                    else
                    {
                        //总评论数
                        int totalCount = 0;
                        foreach (ForumModel model in e.Result)
                        {
                            forumDataSource.Add(model);
                            totalCount = model.TotalCount;
                        }

                        //帖子数
                        letterTotal.Text = totalCount.ToString();

                        //去掉最后一页的加载更多按钮
                        if (forumDataSource.Count - 1 == totalCount)
                        {
                            forumDataSource.RemoveAt((forumDataSource.Count - 1));
                        }
                        //绑定数据
                        forumListbox.ItemsSource = forumDataSource;
                    }
                }
            }
        }

        /// <summary>
        /// 分页加载
        /// </summary>
        private void forumLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //清楚集合中的更多按钮
            forumDataSource.RemoveAt((forumDataSource.Count - 1));
            //分页加载
            forumPageIndex++;
            string page = "-p" + forumPageIndex.ToString() + "-";
            url = url.Replace(currentPage, page);
            ForumListLoadData(url, false);
            currentPage = page;
        }


        #endregion


        //最后回复
        private void lastestReply_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ForumActivity", "最后回复点击量");
            if (isLoading == false)
            {
                isLoading = true;
                forumListbox.ItemsSource = null;
                forumDataSource.Clear();
                url = CreateTopicsUrl(false, 0);
                ForumListLoadData(url, true);
            }
        }

        //最新发帖
        private void newest_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ForumActivity", "最新发帖点击量");
            if (isLoading == false)
            {
                isLoading = true;
                forumListbox.ItemsSource = null;
                forumDataSource.Clear();
                url = CreateTopicsUrl(false, 2);
                ForumListLoadData(url, true);
            }
        }

        //精华
        private void refine_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ForumActivity", "精华帖点击量");
            if (isLoading == false)
            {
                isLoading = true;
                forumListbox.ItemsSource = null;
                forumDataSource.Clear();
                url = CreateTopicsUrl(true, 0);
                //string.Format(App.headUrl + "/clubapp/topicList/topiclist.ashx?bbsId={0}&refine=jing&bbs={1}", bbsId, bbsType);
                ForumListLoadData(url, true);
            }
        }
        /// <summary>
        /// 生成帖子列表url
        /// </summary>
        /// <returns></returns>
        private string CreateTopicsUrl(bool isRefine, int order)
        {
            return string.Format(AppUrlMgr.TopicsUrl, bbsId, bbsType, isRefine ? 1 : 0, 0, order, 1, 20);
        }

        // 导向帖子详情页
        private void bbsIdGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid gg = (Grid)sender;
            this.NavigationService.Navigate(new Uri("/View/Forum/TopicDetailPage.xaml?from=0&bbsId=" + bbsId + "&topicId=" + gg.Tag + "&bbsType=" + bbsType, UriKind.Relative));
        }

        //发新帖
        private void sendLetter_Click(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Forum/SendLetterPage.xaml?title=" + title + "&bbsId=" + bbsId + "&bbsType=" + bbsType, UriKind.Relative));
        }

        //进入搜索页
        private void search_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string searchPageUrl = View.Search.SearchPage.GetSearchPageUrlWithParams(SearchType.Forum);
            this.NavigationService.Navigate(new Uri(searchPageUrl, UriKind.Relative));
        }

    }
}