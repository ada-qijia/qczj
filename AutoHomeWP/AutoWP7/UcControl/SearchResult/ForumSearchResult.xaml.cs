using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
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
            this.SearchResultVM.LoadDataCompleted+=SearchResultVM_LoadDataCompleted;
            this.DataContext = this.SearchResultVM;

            InitializeSortTimeFilters();
        }

        void SearchResultVM_LoadDataCompleted(object sender, EventArgs e)
        {
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
            int nextPageIndex = this.SearchResultVM.PageIndex++;
            if (restart)
            {
                this.SearchResultVM.ClearData();
                nextPageIndex = 1;
            }

            int sort = this.sortListPicker.SelectedIndex;
            RelatedBBSModel selectedBBS = (RelatedBBSModel)this.rangeListPicker.SelectedItem;
            int bbsID = selectedBBS.ID;
            string bbsName = selectedBBS.Name;
            int timeRange = this.timeListPicker.SelectedIndex;
            int pageSize = 20;
            //string url = "http://221.192.136.99:804/wpv1.6/sou/club.ashx?a=2&pm=3&v=1.6.0&k=bmw&o=1&b=0&r=0&p=1&s=10&bn=";
            string url = string.Format("{0}{1}/sou/club.ashx?a={2}&pm={3}&v={4}&k={5}&o={6}&b={7}&r={8}&p={9}&s={10}&bn={11}","http://221.192.136.99:804/wpv1.6", "", App.appId, App.platForm, "1.6.0", keyword, sort, bbsID, timeRange, nextPageIndex, pageSize, bbsName);
            //string url = string.Format("{0}{1}/sou/club.ashx?a={2}&pm={3}&v={4}&k={5}&o={6}&b={7}&r={8}&p={9}&s={10}&bn={11}", App.appUrl, App.versionStr, App.appId, App.platForm, App.version, keyword, sort, bbsID, timeRange, nextPageIndex, pageSize, bbsName);
            this.SearchResultVM.LoadDataAysnc(url);
        }

        #endregion
    }
}
