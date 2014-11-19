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
            //只有一条记录时导航到找车-车系车型页
            if (this.SearchResultVM.RowCount == 1)
            {
                throw new NotImplementedException();
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
            this.SearchResultVM.ClearData();
            string url = "http://221.192.136.99:804/wpv1.6/sou/series.ashx?app=2&platform=3&version=1.6.0&kw=5";
            //string url = string.Format("{0}{1}/sou/series.ashx?app={2}&platform={3}&version={4}&kw={5}", App.appUrl, App.versionStr, App.appId, App.platForm, App.version, keyword);
            this.SearchResultVM.LoadDataAysnc(url);
        }

        #endregion

        //导航到多条时车系综述/只一条时找车-车系车型页
        private void CarSeries_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (this.SearchResultVM.RowCount == 1)
            {

            }
            else
            {

            }
        }
    }
}
