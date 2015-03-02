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
    public partial class MyRecents : PhoneApplicationPage
    {
        private ViewHistoryViewModel recentsVM;

        public MyRecents()
        {
            InitializeComponent();

            //设置无结果时显示内容
            this.NoArticleResultView.SetContent("暂无文章浏览历史");
            this.NoCarSeriesResultView.SetContent("暂无车系浏览历史");
            this.NoCarSpecResultView.SetContent("暂无车型浏览历史");
            this.NoForumResultView.SetContent("暂无论坛浏览历史");
            this.NoTopicResultView.SetContent("暂无帖子浏览历史");

            this.recentsVM = ViewHistoryViewModel.SingleInstance;
            this.DataContext = this.recentsVM;
            this.recentsVM.Refresh(FavoriteType.All);
        }

        #region item navigation

        //导航到找车-车系-车型页
        private void CarSeriesItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var element = sender as FrameworkElement;
            var model = element.DataContext as FavoriteCarSeriesModel;
            if (model != null)
            {
                View.Car.CarSeriesDetailPage.ShareModel(model);
                string url = string.Format("/View/Car/CarSeriesDetailPage.xaml?indexId=0&carSeriesId={0}", model.ID);
                NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
        }

        //导航到找车-车型-经销商页
        private void CarSpecItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var element = sender as FrameworkElement;
            var model = element.DataContext as FavoriteCarSpecModel;
            if (model != null)
            {
                View.Car.CarSeriesQuotePage.ShareModel(model);
                string url = string.Format("/View/Car/CarSeriesQuotePage.xaml?carId={0}&selectedPage={1}", model.ID, "dealer");
                NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
        }

        //导航到文章最终页
        private void ArticleItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var element = sender as FrameworkElement;
            var model = element.DataContext as FavoriteArticleModel;
            if (model != null)
            {
                View.Channel.News.NewsEndPage.ShareState(model);
                //暂按文章处理
                string url = string.Format("/View/Channel/News/NewsEndPage.xaml?newsid={0}&pageIndex={1}&pageType={2}", model.ID, 1, 1);
                NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
        }

        //导航到论坛-帖子列表页
        private void ForumItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var element = sender as FrameworkElement;
            var model = element.DataContext as FavoriteForumModel;
            if (model != null)
            {
                string url = string.Format("/View/Forum/LetterListPage.xaml?bbsId={0}&bbsType={1}&id={2}&title={3}", model.ID, model.Type, "", model.Name);
                NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
        }

        //导航到论坛帖子最终页
        private void TopicItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var element = sender as FrameworkElement;
            var model = element.DataContext as FavoriteTopicModel;
            if (model != null)
            {
                View.Forum.TopicDetailPage.ShareTitle(model.Title);
                string url = string.Format("/View/Forum/TopicDetailPage.xaml?from=0&bbsId={0}&topicId={1}&bbsType={2}", model.BBSID, model.ID, model.BBSType);
                NavigationService.Navigate(new Uri(url, UriKind.Relative));
            }
        }

        #endregion

        #region loadMore

        private void LoadMoreCarSeries_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.recentsVM.LoadMore(FavoriteType.CarSeries);
        }

        private void LoadMoreCarSpec_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.recentsVM.LoadMore(FavoriteType.CarSpec);
        }

        private void LoadMoreArticle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.recentsVM.LoadMore(FavoriteType.Article);
        }

        private void LoadMoreForum_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.recentsVM.LoadMore(FavoriteType.Forum);
        }

        private void LoadMoreTopic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.recentsVM.LoadMore(FavoriteType.Topic);
        }

        #endregion

        private void DeleteAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("将会清空所有的浏览记录，是否继续？", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                this.recentsVM.DeleteAll();
            }
        }
    }
}