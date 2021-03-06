﻿using System;
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
    public partial class VideoSearchResult : UserControl
    {
        private VideoSearchResultViewModel SearchResultVM;

        private string keyword;

        public VideoSearchResult(string keyword)
        {
            InitializeComponent();

            this.keyword = keyword;

            this.SearchResultVM = new VideoSearchResultViewModel();
            this.SearchResultVM.LoadDataCompleted += SearchResultVM_LoadDataCompleted;
            this.DataContext = this.SearchResultVM;
        }

        void SearchResultVM_LoadDataCompleted(object sender, EventArgs e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            bool noResult = this.SearchResultVM.RowCount == 0;
            if (noResult)
            {
                this.NoResultUC.SetContent(keyword, "视频");
            }
            this.NoResultUC.Visibility = noResult ? Visibility.Visible : Visibility.Collapsed;
            this.ResultPanel.Visibility = noResult ? Visibility.Collapsed : Visibility.Visible;
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

            int pageSize = 20;
            string url = string.Format("{0}{1}/sou/video.ashx?a={2}&pm={3}&v={4}&q={5}&p={6}&s={7}", App.appUrl, App.versionStr, App.appId, App.platForm, App.version, keyword, nextPageIndex, pageSize);

            this.SearchResultVM.LoadDataAysnc(url);
        }

        #endregion

        #region UI interaction

        private void video_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var model = (sender as FrameworkElement).DataContext as VideoSearchModel;
            if (model != null)
            {
                string url = string.Format("/View/Channel/News/VideoEndPage.xaml?videoid={0}", model.ID);
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
