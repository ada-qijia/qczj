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
            this.DataContext = this.SearchResultVM;

            string[] typeFilters = new string[] { "全部", "行情", "新闻", "评测", "导购", "改装", "技术", "文化", "用车" };
            this.typeListPicker.DataContext = typeFilters;
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

            string entry = "47";//???
            int classId = this.typeListPicker.SelectedIndex;
            int sort = 1;//最相关
            int summarySize = 30;
            int pageSize = 20;

            //string url = "http://221.192.136.99:804/wpv1.6/sou/news.ashx?app=2&platform=3&version=1.6.0&entry=47&sort=0&classId=0&contentsize=200&q=4&pagestart=1&pagesize=20";
            string url = string.Format("{0}{1}/sou/news.ashx?app={2}&platform={3}&version={4}&entry={5}&sort={6}&classId={7}&contentsize={8}&q={9}&pagestart={10}&pagesize={11}", "http://221.192.136.99:804/wpv1.6", "", App.appId, App.platForm, "1.6.0", entry, sort, classId, summarySize, keyword, nextPageIndex, pageSize);
            //string url = string.Format("{0}{1}/sou/news.ashx?app={2}&platform={3}&version={4}&entry={5}&sort={6}&classId={7}&contentsize={8}&q={9}&pagestart={10}&pagesize={11}", App.appUrl, App.versionStr, App.appId, App.platForm, App.version, entry, sort, classId, summarySize, keyword, nextPageIndex, pageSize);
            this.SearchResultVM.LoadDataAysnc(url);
        }

        #endregion
    }
}
