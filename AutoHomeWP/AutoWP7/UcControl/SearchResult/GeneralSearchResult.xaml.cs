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
    public partial class GeneralSearchResult : UserControl
    {
        private GeneralSearchResultViewModel SearchResultVM;

        private string keyword;

        public GeneralSearchResult(string keyword)
        {
            InitializeComponent();

            this.keyword = keyword;
            this.SearchResultVM = new GeneralSearchResultViewModel();
            //this.SearchResultVM.PropertyChanged += SearchResultVM_PropertyChanged;
            this.SearchResultVM.LoadDataCompleted += SearchResultVM_LoadDataCompleted;
            this.DataContext = this.SearchResultVM;
        }

        //如果结果为空，显示没有结果提示
        void SearchResultVM_LoadDataCompleted(object sender, EventArgs e)
        {
            bool noResult=this.SearchResultVM.RowCount==0;
            if(noResult)
            {
                this.NoResultUC.SetContent(keyword, "内容");                
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
                        blocks = new UIElement[] { CarSeriesPresenter, CarModelStackPanel, ImgStackPanel, RelatedSeriesItemsControl, NaturalStackPanel };
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
            int nextPageIndex = this.SearchResultVM.PageIndex++;
            if (restart)
            {
                this.SearchResultVM.ClearData();
                nextPageIndex = 1;
            }

            int pageSize = 20;

            string url = string.Format("http://221.192.136.99:804/wpv1.6/sou/search.ashx?a=2&pm=3&v=1.6.0&q={0}&p=1&s=10",keyword);
            //string url = string.Format("{0}{1}/sou/search.ashx?a={2}&pm={3}&v={4}&q={5}&p={6}&s={7}", App.appUrl, App.versionStr, App.appId, App.platForm, App.version, keyword, nextPageIndex, pageSize);

            this.SearchResultVM.LoadDataAysnc(url);
        }

        #endregion
    }
}
