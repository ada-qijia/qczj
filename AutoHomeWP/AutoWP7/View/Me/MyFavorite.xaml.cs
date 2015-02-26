using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Model.Me;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ViewModels.Me;

namespace AutoWP7.View.Me
{
    public partial class MyFavorite : Utils.MultiSelectablePage
    {
        private FavoriteViewModel FavoriteVM;

        public MyFavorite()
        {
            InitializeComponent();

            //设置无结果时显示内容
            this.NoArticleResultView.SetContent("暂无文章收藏");
            this.NoCarSeriesResultView.SetContent("暂无车系收藏");
            this.NoCarSpecResultView.SetContent("暂无车型收藏");
            this.NoForumResultView.SetContent("暂无论坛收藏");
            this.NoTopicResultView.SetContent("暂无帖子收藏");

            this.FavoriteVM = FavoriteViewModel.SingleInstance;
            this.DataContext = this.FavoriteVM;

            this.FavoriteVM.ReloadLocally(FavoriteType.All);
            this.syncData();
        }

        #region 同步数据

        private void syncData()
        {
            this.uploadCarData();
            this.uploadOthersData();
            this.downloadFavorite();
        }

        private void uploadCarData()
        {
            //上传车系，车型数据
            var series = this.FavoriteVM.GetUnsyncedItems(FavoriteType.CarSeries);
            var seriesStr = series == null || series.Count == 0 ? null : CommonLayer.JsonHelper.Serialize(series);
            var specs = this.FavoriteVM.GetUnsyncedItems(FavoriteType.CarSpec);
            var specStr = specs == null || specs.Count == 0 ? null : CommonLayer.JsonHelper.Serialize(specs);

            var userInfoModel = Utils.MeHelper.GetMyInfoModel();
            if (userInfoModel != null && (seriesStr != null || specStr != null))
            {
                string url = Utils.MeHelper.SyncFavoriteCarUrl;
                string data = string.Format("_appid=app.wp&uc_ticket={0}&seriesStr={1}&specStr={2}&_timestamp={3}&autohomeua={4}", userInfoModel.Authorization, seriesStr, specStr, Common.GetTimeStamp(), Common.GetAutoHomeUA());
                data = Common.SortURLParamAsc(data);
                string sign = Common.GetSignStr(data);
                data += "&_sign=" + sign;
                this.FavoriteVM.UploadCar(url, data, null, series, specs);
            }
        }

        private void uploadOthersData()
        {
            //上传论坛，帖子，文章
            var bbslist = this.FavoriteVM.GetUnsyncedItems(FavoriteType.Forum);
            var bbsStr = bbslist == null || bbslist.Count == 0 ? null : CommonLayer.JsonHelper.Serialize(bbslist);
            var topics = this.FavoriteVM.GetUnsyncedItems(FavoriteType.Topic);
            var topicStr = topics == null || topics.Count == 0 ? null : CommonLayer.JsonHelper.Serialize(topics);
            var articles = this.FavoriteVM.GetUnsyncedItems(FavoriteType.Article);
            var articleStr = articles == null || articles.Count == 0 ? null : CommonLayer.JsonHelper.Serialize(articles);

            var userInfoModel = Utils.MeHelper.GetMyInfoModel();
            if (userInfoModel != null && (bbsStr != null || topicStr != null || articleStr != null))
            {
                string url = Utils.MeHelper.SyncFavoriteCollectionUrl;
                string data = string.Format("_appid=app.wp&authorization={0}&bbslist={1}&topiclist={2}&articlelist={3}&_timestamp={4}&autohomeua={5}",userInfoModel.Authorization,bbsStr,topicStr,articleStr,Common.GetTimeStamp(),Common.GetAutoHomeUA());
                data = Common.SortURLParamAsc(data);
                string sign = Common.GetSignStr(data);
                data += "&_sign=" + sign;
                this.FavoriteVM.UploadOthers(url, data, null, bbslist, topics, articles);
            }
        }

        private void downloadFavorite()
        {
            GlobalIndicator.Instance.Text = "正在加载";
            GlobalIndicator.Instance.IsBusy = true;

            string url = Utils.MeHelper.GetFavoriteUrl(0, 1);
            this.FavoriteVM.LoadFromServer(url, FavoriteVM_DownloadFavoriteCompleted);
        }

        private void FavoriteVM_DownloadFavoriteCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
        }

        #endregion

        #region multiSelection

        private void FavoritePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCurrentList();
        }

        private void UpdateCurrentList()
        {
            var selectPivot = FavoritePivot.SelectedItem as PivotItem;
            var grid = selectPivot.Content as Grid;
            this.CurrentList = grid.Children[0] as LongListMultiSelector;
        }

        public override void AfterDeleteItems(System.Collections.IList selectedItems)
        {
            base.AfterDeleteItems(selectedItems);
            //  this.FavoriteVM.Remove();
        }

        #endregion

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
                string url = string.Format("/View/Car/CarSeriesQuotePage.xaml?carId={0}&selectedPage={1}&seriesName={2}", model.ID, "dealer", model.SeriesName);
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
            this.LoadMoreItems(FavoriteType.CarSeries);
        }

        private void LoadMoreCarSpec_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LoadMoreItems(FavoriteType.CarSpec);
        }

        private void LoadMoreArticle_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LoadMoreItems(FavoriteType.Article);
        }

        private void LoadMoreForum_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LoadMoreItems(FavoriteType.Forum);
        }

        private void LoadMoreTopic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LoadMoreItems(FavoriteType.Topic);
        }

        private void LoadMoreItems(FavoriteType type)
        {
            if (this.FavoriteVM.FavoriteBlockHeaders.ContainsKey(type))
            {
                var header = this.FavoriteVM.FavoriteBlockHeaders[type];
                if (header != null)
                {
                    GlobalIndicator.Instance.Text = "正在加载";
                    GlobalIndicator.Instance.IsBusy = true;

                    int typeValue = (int)type;
                    string url = Utils.MeHelper.GetFavoriteUrl(typeValue, header.PageIndex + 1);
                    this.FavoriteVM.LoadFromServer(url, FavoriteVM_DownloadFavoriteCompleted);
                }
            }
        }

        #endregion
    }
}