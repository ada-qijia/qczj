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
    public partial class VideoSearchResult : UserControl
    {
        private VideoSearchResultViewModel SearchResultVM;

        private string keyword;

        public VideoSearchResult(string keyword)
        {
            InitializeComponent();

            this.keyword = keyword;

            this.SearchResultVM = new VideoSearchResultViewModel();
            this.DataContext = this.SearchResultVM;
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

            int pageSize = 20;

            //string url = "http://221.192.136.99:804/wpv1.6/sou/video.ashx?a=2&pm=3&v=1.6.0&q=bmw&p=1&s=10";
            string url = string.Format("{0}{1}/sou/video.ashx?a={2}&pm={3}&v={4}&q={5}&p={6}&s={7}", "http://221.192.136.99:804/wpv1.6", "", App.appId, App.platForm, "1.6.0", keyword, nextPageIndex, pageSize);
            //string url = string.Format("{0}{1}/sou/video.ashx?a={2}&pm={3}&v={4}&q={5}&p={6}&s={7}", App.appUrl, App.versionStr, App.appId, App.platForm, App.version, keyword, nextPageIndex, pageSize);
            
            this.SearchResultVM.LoadDataAysnc(url);
        }

        #endregion

    }
}
