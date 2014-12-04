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
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

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
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            int nextPageIndex = this.SearchResultVM.PageIndex + 1;
            if (restart)
            {
                this.SearchResultVM.ClearData();
                nextPageIndex = 1;
            }

            ArticleFilterModel filterModel = this.typeListPicker.SelectedItem as ArticleFilterModel;
            string classId = filterModel == null ? "0" : filterModel.Type;
            int summarySize = 30;
            int pageSize = 20;

            string url = string.Format("{0}{1}/sou/news.ashx?app={2}&platform={3}&version={4}&entry=&sort=&classId={5}&contentsize={6}&q={7}&pagestart={8}&pagesize={9}", App.appUrl, App.versionStr, App.appId, App.platForm, App.version, classId, summarySize, keyword, nextPageIndex, pageSize);

            this.SearchResultVM.LoadDataAysnc(url);
        }

        #endregion

        #region UI interaction

        private void typeListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.RemovedItems.Count > 0)
            {
                this.LoadMore(true);
            }
        }

        private void Article_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ArticleModel model = (sender as FrameworkElement).DataContext as ArticleModel;
            if (model != null)
            {
                string url = string.Format("/View/Channel/News/NewsEndPage.xaml?newsid={0}&pageIndex=1&pageType={2}", model.ID, 1);
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
