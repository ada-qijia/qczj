using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using ViewModels.Search;
using Model.Search;

namespace AutoWP7.UcControl.SearchResult
{
    public partial class CarSeriesSearchResult : UserControl
    {
        private CarSeriesSearchResultViewModel SearchResultVM;

        private string keyword;

        public CarSeriesSearchResult(string keyword)
        {
            InitializeComponent();

            this.keyword = keyword;

            this.SearchResultVM = new CarSeriesSearchResultViewModel();
            this.SearchResultVM.LoadDataCompleted += SearchResultVM_LoadDataCompleted;
            this.DataContext = this.SearchResultVM;
        }

        void SearchResultVM_LoadDataCompleted(object sender, EventArgs e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            //只有一条记录时导航到找车-车系车型页
            if (this.SearchResultVM.RowCount == 1)
            {
                string url = string.Format("/View/Car/CarSeriesDetailPage.xaml?indexId=0&carSeriesId={0}", this.SearchResultVM.CarSeriesList[0].ID);
                this.Navigate(url);
            }
            else
            {
                bool noResult = this.SearchResultVM.RowCount == 0;
                if (noResult)
                {
                    this.NoResultUC.SetContent(keyword, "车系");
                }
                this.NoResultUC.Visibility = noResult ? Visibility.Visible : Visibility.Collapsed;
                this.ResultPanel.Visibility = noResult ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        #region public methods

        public void ReLoad()
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            this.SearchResultVM.ClearData();
            string url = string.Format("{0}{1}/sou/series.ashx?app={2}&platform={3}&version={4}&kw={5}", App.appUrl, App.versionStr, App.appId, App.platForm, App.version, keyword);
            this.SearchResultVM.LoadDataAysnc(url);
        }

        #endregion

        //导航到多条时车系综述/只一条时找车-车系车型页
        private void CarSeries_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            CarSeriesSearchModel model = (sender as FrameworkElement).DataContext as CarSeriesSearchModel;
            if (model != null)
            {
                string url = string.Format("/View/Car/CarSeriesDetailPage.xaml?indexId=0&carSeriesId={0}", model.ID);
                this.Navigate(url);
            }
        }

        private void Navigate(string relativeUrl)
        {
            var frame = Application.Current.RootVisual as PhoneApplicationFrame;
            frame.Navigate(new Uri(relativeUrl, UriKind.Relative));
        }
    }
}
