﻿using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Model.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ViewModels.Search;

namespace AutoWP7.UcControl.SearchResult
{
    public partial class GeneralSearchResult : UserControl
    {
        private GeneralSearchResultViewModel SearchResultVM;

        public GeneralSearchResult(string keyword)
        {
            InitializeComponent();

            this.SearchResultVM = new GeneralSearchResultViewModel();
            this.SearchResultVM.Keyword = keyword;
            //this.SearchResultVM.PropertyChanged += SearchResultVM_PropertyChanged;
            this.SearchResultVM.LoadDataCompleted += SearchResultVM_LoadDataCompleted;
            this.DataContext = this.SearchResultVM;

            this.DealerCityNameTextBlock.Text = App.CityName;
        }

        //如果结果为空，显示没有结果提示
        void SearchResultVM_LoadDataCompleted(object sender, EventArgs e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            bool noResult = this.SearchResultVM.RowCount == 0;
            if (noResult)
            {
                this.NoResultUC.SetContent(this.SearchResultVM.Keyword, "内容");
            }
            this.NoResultUC.Visibility = noResult ? Visibility.Visible : Visibility.Collapsed;
            this.ResultPanel.Visibility = noResult ? Visibility.Collapsed : Visibility.Visible;
        }

        #region set blocks visibility of different match type

        void SearchResultVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MatchType")
            {
                UIElement[] blocks;
                switch (this.SearchResultVM.MatchType)
                {
                    case GeneralSearchResultMatchType.Brand:
                        blocks = new UIElement[] { BrandItemsControl, NaturalStackPanel };
                        break;
                    case GeneralSearchResultMatchType.CarSeries:
                        blocks = new UIElement[] { CarSeriesPresenter, CarModelStackPanel, ImgStackPanel, NaturalStackPanel };
                        break;
                    case GeneralSearchResultMatchType.CarType:
                        blocks = new UIElement[] { CarSeriesPresenter, CarModelStackPanel, ImgStackPanel, NaturalStackPanel };
                        break;
                    case GeneralSearchResultMatchType.FindCars:
                        blocks = new UIElement[] { FindSeriesItemsControl, NaturalStackPanel };
                        break;
                    case GeneralSearchResultMatchType.Firm:
                        blocks = new UIElement[] { BrandItemsControl, DealerStackPanel, NaturalStackPanel };
                        break;
                    case GeneralSearchResultMatchType.Forum:
                        blocks = new UIElement[] { JingxuanStackPanel };
                        break;
                    default:
                        blocks = new UIElement[] { };
                        break;
                }
                this.SetVisibleBlocks(blocks);
            }
        }

        private void SetVisibleBlocks(IEnumerable<UIElement> blocks)
        {
            foreach (var item in this.BlocksStackPanel.Children)
            {
                item.Visibility = blocks.Contains(item) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

        #region public methods

        public void LoadMore(bool restart)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            int nextPageIndex = this.SearchResultVM.PageIndex + 1;
            if (restart)
            {
                this.SearchResultVM.ClearData();
                nextPageIndex = 1;
            }

            int pageSize = 20;
            string cityName = App.CityName ?? string.Empty;
            string url = string.Format("{0}{1}/sou/search.ashx?a={2}&pm={3}&v={4}&q={5}&p={6}&s={7}&c={8}&cn={9}", App.appUrl, App.versionStr, App.appId, App.platForm, App.version, this.SearchResultVM.Keyword, nextPageIndex, pageSize, App.CityId, cityName);

            this.SearchResultVM.LoadDataAysnc(url);
        }

        #endregion

        #region UI interaction

        #region 内容导航

        //进入找车-车系车型页
        private void CarSeries_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigateToCarSeriesModel(0);
        }

        //跳至关注度最高的车型参数配置页
        private void CarSeriesConfig_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string url = string.Format("/View/Car/CarSeriesQuotePage.xaml?carId={0}&selectedPage={1}", this.SearchResultVM.Series.HotSpecID, "spec");
            this.Navigate(url);
        }

        //找车-车系论坛帖子列表页
        private void CarSeriesBBS_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigateToCarSeriesModel(3);
        }

        //找车-车系口碑页
        private void CarSeriesKoubei_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigateToCarSeriesModel(4);
        }

        //找车-车型经销商页
        private void CarModel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var model = (sender as FrameworkElement).DataContext as SpecModel;
            if (model != null && model.ID != 0)
            {
                string url = string.Format("/View/Car/CarSeriesQuotePage.xaml?carId={0}&selectedPage={1}", model.ID, "dealer");
                this.Navigate(url);
            }
        }

        //找车-车系车型页
        private void CarModelMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigateToCarSeriesModel(0);
        }

        //找车-车系车型页
        private void Brand_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var model = (sender as FrameworkElement).DataContext as SeriesModel;
            if (model != null && model.ID != 0)
            {
                //var favorite = new Model.Me.FavoriteCarSeriesModel() { ID = model.ID, Name = model.Name, PriceBetween = model.PriceBetween };
                //View.Car.CarSeriesDetailPage.ShareModel(favorite);
                string url = string.Format("/View/Car/CarSeriesDetailPage.xaml?indexId=0&carSeriesId={0}", model.ID);
                this.Navigate(url);
            }
        }

        //进入图片浏览页
        private void Img_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var imgModel = ((FrameworkElement)sender).DataContext as ImgModel;
            int index = this.SearchResultVM.ImgList.IndexOf(imgModel);

            List<string> bigImageList = new List<string>();
            foreach (var item in this.SearchResultVM.ImgList)
            {
                bigImageList.Add(item.Img);
            }
            App.BigImageList = bigImageList;

            string url = string.Format("/View/Car/ImageViewer.xaml?pageTitle={0}&imageIndex={1}", this.SearchResultVM.Keyword, index);
            this.Navigate(url);
        }

        //车系图片页
        private void ImgMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.NavigateToCarSeriesModel(1);
        }

        //找车-车系车型页
        private void RelatedSeries_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var model = (sender as FrameworkElement).DataContext as RelatedSeriesModel;
            if (model != null && model.ID != 0)
            {
                string url = string.Format("/View/Car/CarSeriesDetailPage.xaml?indexId=0&carSeriesId={0}", model.ID);
                this.Navigate(url);
            }
        }

        //找车-车系车型页
        private void FindSeries_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var model = (sender as FrameworkElement).DataContext as FindSeriesModel;
            if (model != null && model.ID != 0)
            {
                string url = string.Format("/View/Car/CarSeriesDetailPage.xaml?indexId=0&carSeriesId={0}", model.ID);
                this.Navigate(url);
            }
        }

        //帖子详情页
        private void Jingxuan_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var model = (sender as FrameworkElement).DataContext as JingxuanModel;
            if (model != null)
            {
                View.Forum.TopicDetailPage.ShareTitle(model.Title);
                string url = string.Format("/View/Forum/TopicDetailPage.xaml?from=0&bbsId={0}&topicId={1}&bbsType={2}", model.BBSID, model.TopicID, model.BBSType);
                this.Navigate(url);
            }
        }

        //文章1，帖子5，视频3，说客2跳转到相应详情页
        private void NaturalModel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var model = (sender as FrameworkElement).DataContext as NaturalModel;
            if (model != null)
            {
                int mediatype = model.MediaType;
                if (mediatype == 1 || mediatype == 2)
                {
                    this.Navigate(string.Format("/View/Channel/News/NewsEndPage.xaml?newsid={0}&pageIndex=1&pageType={1}", model.ID, model.MediaType));
                }
                else if (mediatype == 3)
                {
                    this.Navigate(string.Format("/View/Channel/News/VideoEndPage.xaml?videoid={0}", model.ID));
                }
                else if (mediatype == 5)
                {
                    View.Forum.TopicDetailPage.ShareTitle(model.Title);
                    string url = string.Format("/View/Forum/TopicDetailPage.xaml?from=0&bbsId={0}&topicId={1}&bbsType={2}", model.JumpPage, model.ID, model.Type);
                    this.Navigate(url);
                }
            }
        }

        #region common private methods

        private void NavigateToCarSeriesModel(int index)
        {
            var model = this.SearchResultVM.Series;
            if (model != null && model.ID != 0)
            {
                //共享车系
                Model.Me.FavoriteCarSeriesModel favoriteModel = new Model.Me.FavoriteCarSeriesModel();
                favoriteModel.ID = model.ID;
                favoriteModel.Img = model.Img;
                favoriteModel.Name = model.Name;
                favoriteModel.PriceBetween = model.PriceBetween;
                View.Car.CarSeriesDetailPage.ShareModel(favoriteModel);

                string url = string.Format("/View/Car/CarSeriesDetailPage.xaml?indexId={0}&carSeriesId={1}", index, model.ID);
                this.Navigate(url);
            }
        }

        private void Navigate(string relativeUrl)
        {
            var frame = Application.Current.RootVisual as PhoneApplicationFrame;
            frame.Navigate(new Uri(relativeUrl, UriKind.Relative));
        }

        #endregion

        #endregion

        private void LoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LoadMore(false);
        }

        private void DealerCall_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            DealerSearchModel model = (sender as FrameworkElement).DataContext as DealerSearchModel;
            UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "电话拨打点击量");
            PhoneCallTask phoneCall = new PhoneCallTask();
            phoneCall.PhoneNumber = model.Tel;
            phoneCall.Show();
        }

        #endregion
    }
}
