using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ViewModels.Me;
using Model.Me;
using System.IO.IsolatedStorage;

namespace AutoWP7.View.Me
{
    public partial class MyCommentReply : PhoneApplicationPage
    {
        private CommentReplyViewModel ReplyVM;
        private bool isLogin;

        public MyCommentReply()
        {
            InitializeComponent();

            this.ReplyVM = new CommentReplyViewModel();
            this.ReplyVM.LoadDataCompleted += ReplyVM_LoadDataCompleted;
            this.DataContext = this.ReplyVM;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.New)
            {
                this.isLogin = Utils.MeHelper.GetMyInfoModel()!=null;
                if (this.isLogin)
                {
                    this.LoadMore(true);
                }
                else
                {
                    //未登录，跳转到登录页
                    this.NavigationService.Navigate(new Uri("/View/More/LoginPage.xaml", UriKind.Relative));
                }
            }
            else if (e.NavigationMode == NavigationMode.Back)
            {
                bool isNewLogin = isLogin == false && Utils.MeHelper.GetMyInfoModel() != null;
                if (isNewLogin)
                {
                    isLogin = true;
                    this.LoadMore(true);
                }
            }
        }

        private void ReplyVM_LoadDataCompleted(object sender, EventArgs e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            bool noResult = this.ReplyVM.RowCount == 0;
            if (noResult)
            {
                this.NoResultUC.SetContent("暂无评论回复");
            }
            this.NoResultUC.Visibility = noResult ? Visibility.Visible : Visibility.Collapsed;
            this.ReplyListBox.Visibility = noResult ? Visibility.Collapsed : Visibility.Visible;
        }

        private void LoadMore(bool restart)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            int nextPageIndex = this.ReplyVM.PageIndex + 1;
            if (restart)
            {
                this.ReplyVM.ClearData();
                nextPageIndex = 1;
            }

            string url = Utils.MeHelper.GetMyCommentReplyUrl(nextPageIndex);
            this.ReplyVM.LoadDataAysnc(url);
        }

        #region UI interaction

        //导航到他的主页
        private void OthersHome_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            CommentReplyModel model = (sender as FrameworkElement).DataContext as CommentReplyModel;
            if (model != null)
            {
                string url = string.Format("/View/Me/OthersHomePage.xaml?userID={0}", model.ReplyUserID);
                this.NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
        }

        private void Topic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            CommentReplyModel model = (sender as FrameworkElement).DataContext as CommentReplyModel;
            //文章评论可点击查看详情
            if (model != null && model.ReplyType == 1)
            {
                //导航到文章评论列表页
                if (model.ImgID == 0)
                {
                    UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "评论页的访问量");
                    this.NavigationService.Navigate(new Uri("/View/Channel/Newest/CommentListPage.xaml?newsid=" + model.ID + "&pageType=1", UriKind.Relative));
                }
            }
        }

        private void LoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LoadMore(false);
        }

        //导航到评论回复页
        private void Reply_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Model.Me.CommentReplyModel model = (sender as FrameworkElement).DataContext as Model.Me.CommentReplyModel;
            if (model != null)
            {
                var userInfoModel = Utils.MeHelper.GetMyInfoModel();
                if (userInfoModel != null)
                {
                    this.NavigationService.Navigate(new Uri("/View/Channel/Newest/CommentPage.xaml?newsid=" +
                          model.ID + "&replyid=" + model.ReplyID + "&userid=" + userInfoModel.UserName + "&authorization=" + userInfoModel.Authorization
                          + "&pageType=" + model.ReplyType, UriKind.Relative));
                }
            }
        }

        #endregion

    }
}