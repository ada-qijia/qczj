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

namespace AutoWP7.View.Me
{
    public partial class MyForumReply : PhoneApplicationPage
    {
        private ForumReplyViewModel ReplyVM;

        public MyForumReply()
        {
            InitializeComponent();

            this.ReplyVM = new ForumReplyViewModel();
            this.ReplyVM.LoadDataCompleted += ReplyVM_LoadDataCompleted;
            this.DataContext = this.ReplyVM;
            this.LoadMore(true);
        }

        #region load data

        private void ReplyVM_LoadDataCompleted(object sender, EventArgs e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            bool noResult = this.ReplyVM.RowCount == 0;
            if (noResult)
            {
                this.NoResultUC.SetContent("暂无论坛回复");
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

            string url = Utils.MeHelper.GetMyForumReplyUrl(nextPageIndex);
            this.ReplyVM.LoadDataAysnc(url);
        }

        #endregion

        #region UI interaction

        //导航到他的主页
        private void OthersHome_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
             var model = (sender as FrameworkElement).DataContext as ForumReplyModel;
             if (model != null)
             {
                 string url = string.Format("/View/Me/OthersHomePage.xaml?userID={0}", model.ReplyUserID);
                 this.NavigationService.Navigate(new Uri(url, UriKind.Relative));
             }
        }

        //导航到帖子最终页
        private void Topic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var model = (sender as FrameworkElement).DataContext as ForumReplyModel;
            if (model != null)
            {
                View.Forum.TopicDetailPage.ShareTitle(model.Title);
                string url = string.Format("/View/Forum/TopicDetailPage.xaml?from=0&bbsId={0}&topicId={1}&bbsType={2}", model.BBSID, model.ID, model.BBSType);
                this.NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
        }

        //导航到回帖页
        private void Reply_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
             var model = (sender as FrameworkElement).DataContext as ForumReplyModel;
             if (model != null)
             {
                 string url = string.Format("/View/Forum/ReplyCommentPage.xaml?bbsId={0}&bbsType={1}&targetReplyId={2}&topicId={3}&url=&pageindex={4}&title={5}",model.BBSID,model.BBSType,model.ReplyID,model.ID, model.PartOfPage, model.Title);
                 this.NavigationService.Navigate(new Uri(url, UriKind.Relative));
             }
        }

        private void LoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LoadMore(false);
        }

        #endregion
    }
}