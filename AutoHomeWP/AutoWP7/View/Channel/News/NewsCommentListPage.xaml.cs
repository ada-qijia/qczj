using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Model;
using ViewModels;
using System.Windows.Controls;
using System.Windows;

namespace AutoWP7.View.Channel.News
{
    //频道文章之评论页
    public partial class NewsCommentListPage : PhoneApplicationPage
    {
        public NewsCommentListPage()
        {
            InitializeComponent();
        }

        NewsCommentViewModel newsModel = null;
        int newsPageIndex = 1;
        int newsPageSize = 20;
        int pageType = 1;
        //文章id
        int newsid = 0;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {
                        string id = NavigationContext.QueryString["newsid"];
                        pageType =Convert.ToInt16( NavigationContext.QueryString["pageType"]);
                        newsid = int.Parse(id);
                        if (newsModel == null)
                        {
                            newsModel = new NewsCommentViewModel();
                        }
                        CommentLoadData(newsPageIndex, newsPageSize);
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

        /// <summary>
        /// 最新评论集合
        /// </summary>
        private ObservableCollection<CommentModel> newsCommentDataSource = new ObservableCollection<CommentModel>();

        #region 评论数据加载

        NewsCommentViewModel newsCommentVM = null;
        /// <summary>
        /// 评论数据加载
        /// </summary>
        private void CommentLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            newsCommentVM = new NewsCommentViewModel();
            string url = string.Empty;
            if (pageType == 1)
            {
                url = string.Format("{0}{4}/news/comments-a2-pm3-v1.4.0-n{1}-o0-p{2}-s{3}.html", App.appUrl, newsid, pageIndex, pageSize, App.versionStr);
            }
            else if (pageType == 2)
            {
                url = string.Format("{0}{4}/news/shuokecomments-a2-pm1-v1.4.0-n{1}-o0-p{2}-s{3}.html", App.appUrl, newsid, pageIndex, pageSize, App.versionStr);
            }
            else if (pageType == 3)
            {
    //http://app.api.autohome.com.cn/wpv1.4/news/videocomments-a1-pm3-v1.5.0-vi23-o23-p1-s20.html
                url = string.Format("{0}{4}/news/videocomments-a2-pm3-v1.5.0-vi{1}-o0-p{2}-s{3}.html", App.appUrl, newsid, pageIndex, pageSize, App.versionStr);
            }
            newsCommentVM.LoadDataAysnc(url);
            newsCommentVM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<CommentModel>>>(newsCommentVM_LoadDataCompleted);

        }

        void newsCommentVM_LoadDataCompleted(object sender, ViewModels.Handler.APIEventArgs<IEnumerable<CommentModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            try
            {

                 notDataPropmt.Visibility=Visibility.Collapsed;
                comment.Visibility=Visibility.Visible;
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
                        if (e.Result.Count() < 1)
                        {

                        }
                        if (e.Result.Count() <= 1 && newsPageIndex > 1)
                        {
                            Common.showMsg("已经是最后一页了");
                        }
                        else
                        {
                            int total = 0;
                            foreach (CommentModel model in e.Result)
                            {
                                newsCommentDataSource.Add(model);
                                total = model.Total;
                            }
                            comment.ItemsSource = newsCommentDataSource;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                notDataPropmt.Visibility=Visibility.Visible;
                comment.Visibility=Visibility.Collapsed;
            }
        }

        //分页加载
        private void loadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            newsCommentDataSource.RemoveAt(newsCommentDataSource.Count - 1);
            newsPageIndex++;
            CommentLoadData(newsPageIndex, newsPageSize);
        }

        #endregion

        //新闻资讯之评论回复
        private void reply_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Border bb = (Border)sender;
            NavigateComment(bb.Tag.ToString());
        }

        //刷新
        private void refresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }
        private void RefreshData()
        {
            newsCommentDataSource.Clear();
            newsPageIndex = 1;
            CommentLoadData(newsPageIndex, newsPageSize);
        }

        /// <summary>
        ///导向文章评论页
        /// </summary>
        private void commentIconButton_Click(object sender, EventArgs e)
        {
            NavigateComment("0");
        }

        public void NavigateComment(string replyId)
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
                        newsid + "&replyid=" + replyId + "&userid=" + userInfoModel.UserName + "&authorization=" + userInfoModel.Authorization+"&pageType="+pageType, UriKind.Relative));
                }
            }
            else //导向登录页
            {
                this.NavigationService.Navigate(new Uri("/View/More/LoginPage.xaml", UriKind.Relative));
            }
        }
    }
}