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
    public partial class ArticleSearchResult : UserControl
    {
        private ArticleSearchResultViewModel SearchResultVM;

        private string keyword;

        public ArticleSearchResult(string keyword)
        {
            InitializeComponent();

            this.keyword = keyword;
            this.SearchResultVM = new ArticleSearchResultViewModel();
            this.SearchResultVM.LoadDataCompleted += SearchResultVM_LoadDataCompleted;
            this.DataContext = this.SearchResultVM;
        }

        void SearchResultVM_LoadDataCompleted(object sender, EventArgs e)
        {
            bool noResult = this.SearchResultVM.RowCount == 0;
            if (noResult)
            {
                this.NoResultUC.SetContent(keyword, "文章");
            }
            this.NoResultUC.Visibility = noResult ? Visibility.Visible : Visibility.Collapsed;
            this.ResultPanel.Visibility = noResult ? Visibility.Collapsed : Visibility.Visible;
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

            ArticleFilterModel filterModel = this.typeListPicker.SelectedItem as ArticleFilterModel;
            string classId = filterModel == null ? "0" : filterModel.Type;
            int summarySize = 30;
            int pageSize = 20;

            //string url = "http://221.192.136.99:804/wpv1.6/sou/news.ashx?app=2&platform=3&version=1.6.0&entry=&sort=&classId=0&contentsize=200&q=4&pagestart=1&pagesize=20";
            string url = string.Format("{0}{1}/sou/news.ashx?app={2}&platform={3}&version={4}&entry=&sort=&classId={5}&contentsize={6}&q={7}&pagestart={8}&pagesize={9}", "http://221.192.136.99:804/wpv1.6", "", App.appId, App.platForm, "1.6.0", classId, summarySize, keyword, nextPageIndex, pageSize);
            //string url = string.Format("{0}{1}/sou/news.ashx?app={2}&platform={3}&version={4}&entry={5}&sort={6}&classId={7}&contentsize={8}&q={9}&pagestart={10}&pagesize={11}", App.appUrl, App.versionStr, App.appId, App.platForm, App.version, entry, sort, classId, summarySize, keyword, nextPageIndex, pageSize);
            this.SearchResultVM.LoadDataAysnc(url);
        }

        #endregion
    }
}
