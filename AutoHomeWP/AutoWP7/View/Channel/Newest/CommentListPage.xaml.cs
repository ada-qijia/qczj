using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows.Controls;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Model;
using ViewModels;
using ViewModels.Handler;
using System.Windows;

namespace AutoWP7.View.Channel.Newest
{
    /// <summary>
    /// 文章评论列表页(最新)
    /// </summary>
    public partial class CommentListPage : PhoneApplicationPage
    {
        public CommentListPage()
        {
            InitializeComponent();
        }

        //文章id
        int newsid = 0;
        //页码
        int commentPageIndex = 1;
        //页大小
        int commentPageSize = 20;
        int pageType = 1;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {
                        string id = NavigationContext.QueryString["newsid"];
                        pageType = Convert.ToInt16(NavigationContext.QueryString["pageType"]);
                        newsid = int.Parse(id);
                        CommentLoadData(1, commentPageSize);
                    }
                    break;
                case System.Windows.Navigation.NavigationMode.Back:
                    {

                        if (App.IsLoadTag)
                        {
                            App.IsLoadTag = false;
                            //刷新数据
                            RefreshData();
                        }

                    }
                    break;
            }
        }

        public ObservableCollection<CommentModel> commentDataSource = new ObservableCollection<CommentModel>();

        #region 评论数据加载

        NewestCommentViewModel newestCommenteVM = null;
        /// <summary>
        /// 评论数据加载
        /// </summary>
        public void CommentLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            newestCommenteVM = new NewestCommentViewModel();
            string url = string.Empty;
            if (pageType == 1)
            {
                url = string.Format("{0}{4}/news/comments-a2-pm3-v1.5.0-n{1}-o0-p{2}-s{3}.html", App.appUrl, newsid, pageIndex, pageSize, App.versionStr);
            }
            else if (pageType == 2)
            {
                url = string.Format("{0}{4}/news/shuokecomments-a2-pm1-v1.5.0-n{1}-o0-p{2}-s{3}.html", App.appUrl, newsid, pageIndex, pageSize, App.versionStr);
            }
            newestCommenteVM.LoadDataAysnc(url);

            //newestCommenteVM.LoadDataAysnc(App.headUrl + "/clubapp/comment/replylist.ashx", newsid, pageIndex, pageSize);
            newestCommenteVM.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<CommentModel>>>(newestCommenteVM_LoadDataCompleted);

        }

        void newestCommenteVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<CommentModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            try
            {
                notDataPropmt.Visibility = Visibility.Collapsed;
                comment.Visibility = Visibility.Visible;

                if (e.Error != null)
                {
                    Common.NetworkAvailablePrompt();
                }
                else
                {
                    if (e.Result.Count() < 1)
                    {
                        notDataPropmt.Visibility = Visibility.Visible;
                        comment.Visibility = Visibility.Collapsed;
                    }
                    else
                    {

                        if (e.Result.Count() <= 1 && commentPageIndex > 1)
                        {
                            Common.showMsg("已经是最后一页了~~");
                        }
                        else
                        {
                            int total = 0;
                            foreach (CommentModel model in e.Result)
                            {
                                commentDataSource.Add(model);
                                total = model.Total;
                            }
                        
                            //标志是否正在刷新,重置标志位
                            isRefreshing = false;
                            comment.ItemsSource = commentDataSource;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

       
        }

        //分页加载
        private void loadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            commentDataSource.RemoveAt(commentDataSource.Count - 1);
            commentPageIndex++;
            CommentLoadData(commentPageIndex, commentPageSize);

        }

        #endregion

        //回复
        private void reply_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "文章回复");
            Border bb = (Border)sender;
            NavigateCommentPage(bb.Tag.ToString());

        }

        //刷新
        private void refresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        //标志是否正在刷新
        private bool isRefreshing = false;
        private void RefreshData()
        {
            if (isRefreshing == false)
            {
                isRefreshing = true;

                commentPageIndex = 1;
                commentDataSource.Clear();
                CommentLoadData(commentPageIndex, commentPageSize);
            }
        }

        //发评论
        private void commentIconButton_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "发评论");
            //文章评论
            NavigateCommentPage("0");
        }

        private void NavigateCommentPage(string replyId)
        {

            var setting = IsolatedStorageSettings.ApplicationSettings;
            string key = "userInfo";
            MyForumModel userInfoModel = null;
            if (setting.Contains(key))
            {
                userInfoModel = setting[key] as MyForumModel;
                if (userInfoModel.Success == 1) //如果已经登录,直接导向回复页
                {
                    //  Button bb = (Button)sender;  replyid=0 表示评论文章  其他表示回复具体某楼
                    //回复参数  文章id  回复楼数id   评论人id
                    this.NavigationService.Navigate(new Uri("/View/Channel/Newest/CommentPage.xaml?newsid=" +
                        newsid + "&replyid=" + replyId + "&userid=" + userInfoModel.UserName + "&authorization=" + userInfoModel.Authorization + "&pageType=" + pageType, UriKind.Relative));
                }
            }
            else //导向登录页
            {
                this.NavigationService.Navigate(new Uri("/View/More/LoginPage.xaml", UriKind.Relative));
            }
        }
    }
}