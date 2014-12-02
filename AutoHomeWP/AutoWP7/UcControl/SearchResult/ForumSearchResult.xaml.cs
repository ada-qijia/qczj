﻿using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using ViewModels.Search;
using Model.Search;

namespace AutoWP7.UcControl.SearchResult
{
    public partial class ForumSearchResult : UserControl
    {
        private ForumSearchResultViewModel SearchResultVM;

        private string keyword;

        public ForumSearchResult(string keyword, RelatedBBSModel defaultRange = null)
        {
            InitializeComponent();

            this.keyword = keyword;
            RelatedBBSModel defaultRangeItem = defaultRange == null ? new RelatedBBSModel() { ID = 0, Name = "全部论坛" } : defaultRange;

            this.SearchResultVM = new ForumSearchResultViewModel();
            this.SearchResultVM.DefaultRelatedBBS = defaultRangeItem;
            this.SearchResultVM.LoadDataCompleted += SearchResultVM_LoadDataCompleted;
            this.DataContext = this.SearchResultVM;

            InitializeSortTimeFilters();
        }

        void SearchResultVM_LoadDataCompleted(object sender, EventArgs e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            bool noResult = this.SearchResultVM.RowCount == 0;
            if (noResult)
            {
                this.NoResultUC.SetContent(keyword, "内容");
            }
            this.NoResultUC.Visibility = noResult ? Visibility.Visible : Visibility.Collapsed;
            this.ResultPanel.Visibility = noResult ? Visibility.Collapsed : Visibility.Visible;
        }

        private void InitializeSortTimeFilters()
        {
            string[] sortFilters = new string[] { "最相关", "最新发布", "最多回复" };
            string[] timeFilters = new string[] { "全部时间", "近一周", "近一月", "近一年" };

            this.sortListPicker.DataContext = sortFilters;
            this.timeListPicker.DataContext = timeFilters;
        }

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

            int sort = this.sortListPicker.SelectedIndex;
            string bbsID = "";
            string bbsName = "";
            RelatedBBSModel selectedBBS = (RelatedBBSModel)this.rangeListPicker.SelectedItem;
            if (selectedBBS != null)
            {
                bbsID = selectedBBS.ID.ToString();
                bbsName = selectedBBS.Name ?? "";
            }
            int timeRange = this.timeListPicker.SelectedIndex;
            int pageSize = 20;
            string url = string.Format("{0}{1}/sou/club.ashx?a={2}&pm={3}&v={4}&k={5}&o={6}&b={7}&r={8}&p={9}&s={10}&bn={11}", App.appUrl, App.versionStr, App.appId, App.platForm, App.version, keyword, sort, bbsID, timeRange, nextPageIndex, pageSize, bbsName);
            this.SearchResultVM.LoadDataAysnc(url);
        }

        #endregion

        #region UI interaction

        private void sortListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.RemovedItems.Count > 0)
            {
                this.LoadMore(true);
            }
        }

        private void rangeListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.RemovedItems.Count > 0)
            {
                this.LoadMore(true);
            }
        }

        private void timeListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.RemovedItems.Count > 0)
            {
                this.LoadMore(true);
            }
        }

        private void BBS_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var model = (sender as FrameworkElement).DataContext as BBSModel;
            if (model != null)
            {
                string url = string.Format("/View/Forum/LetterListPage.xaml?bbsId={0}&bbsType={1}&id={2}&title={3}", model.ID, model.Type, "", model.Name);
                var frame = Application.Current.RootVisual as PhoneApplicationFrame;
                frame.Navigate(new Uri(url, UriKind.Relative));
            }
        }

        private void Topic_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var model = (sender as FrameworkElement).DataContext as TopicModel;
            if (model != null)
            {
                string url = string.Format("/View/Forum/TopicDetailPage.xaml?from=0&bbsId={0}&topicId={1}&bbsType={2}", model.BBSID, model.ID, model.BBSType);
                var frame = Application.Current.RootVisual as PhoneApplicationFrame;
                frame.Navigate(new Uri(url, UriKind.Relative));
            }
        }

        private void LoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LoadMore(false);
        }

        #endregion

    }
}