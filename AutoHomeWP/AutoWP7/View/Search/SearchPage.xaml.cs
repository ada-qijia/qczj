using AutoWP7.Utils;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using ViewModels.Search;

namespace AutoWP7.View.Search
{
    public partial class SearchPage : PhoneApplicationPage
    {
        const int MaxHistoryCnt = 10;

        private ObservableCollection<string> searchHistory;

        private string lastKeywordText;

        //论坛参数
        private int bbsID;
        private string bbsName;

        public SearchPage()
        {
            InitializeComponent();

            this.searchTypeListPicker.DataContext = Enum.GetValues(typeof(SearchType));

            this.searchHistory = new ObservableCollection<string>();
            this.historyListBox.DataContext = this.searchHistory;

            LoadSearchHistory();
        }

        /// <summary>
        /// 获取带参数的URl
        /// </summary>
        /// <param name="type">搜索类型</param>
        /// <param name="param1">论坛内搜索时为论坛ID</param>
        /// <param name="param2">论坛内搜索时为论坛名称</param>
        /// <returns></returns>
        public static string GetSearchPageUrlWithParams(SearchType type, string param1 = null, string param2 = null)
        {
            string param1Part = string.IsNullOrEmpty(param1) ? string.Empty : "&param1=" + param1;
            string param2Part = string.IsNullOrEmpty(param2) ? string.Empty : "&param2=" + param2;
            return string.Format("/View/Search/SearchPage.xaml?type={0}{1}{2}", (int)type, param1Part, param2Part);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //set the selected search type.
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                if (NavigationContext.QueryString.ContainsKey("type"))
                {
                    int typeIndex;
                    if (int.TryParse(NavigationContext.QueryString["type"], out typeIndex))
                    {
                        this.searchTypeListPicker.SelectedIndex = typeIndex;
                    }

                    //论坛
                    if (typeIndex == 3)
                    {
                        if (NavigationContext.QueryString.ContainsKey("param1"))
                        {
                            int.TryParse(NavigationContext.QueryString["param1"], out bbsID);
                        }
                        else
                        {
                            bbsID = 0;
                        }

                        bbsName = NavigationContext.QueryString.ContainsKey("param2") ? NavigationContext.QueryString["param2"] : string.Empty;
                    }
                }

                this.keywordTextBox.Loaded += keywordTB_Loaded;
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (this.ResultGrid.Visibility == Visibility.Visible)
            {
                this.ResultGrid.Children.Clear();
                this.ResultGrid.Visibility = Visibility.Collapsed;
                this.KeywordsGrid.Visibility = Visibility.Visible;
                e.Cancel = true;
            }
            else
            {
                base.OnBackKeyPress(e);
            }
        }

        #region UI交互

        //激活搜索框
        private void keywordTB_Loaded(object sender, RoutedEventArgs e)
        {
            this.keywordTextBox.Focus();
            this.keywordTextBox.Loaded -= keywordTB_Loaded;
        }

        //列出最多10个推荐联想词
        private void keyboardTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.keywordTextBox.Text == lastKeywordText)
            {
                return;
            }

            lastKeywordText = this.keywordTextBox.Text;
            //如果输入为空，显示历史记录
            if (string.IsNullOrWhiteSpace(this.keywordTextBox.Text))
            {
                this.ShowKeywords(true);
            }
            else
            {
                SuggestWordType suggestType = SuggestWordType.Cars;
                switch (this.searchTypeListPicker.SelectedIndex)
                {
                    //综合，文章搜索推荐车系联想词
                    case 0:
                    case 1:
                        suggestType = SuggestWordType.Cars;
                        break;
                    //视频搜索推荐视频联想词
                    case 2:
                        suggestType = SuggestWordType.Video;
                        break;
                    //论坛搜索推荐论坛联想词
                    case 3:
                        suggestType = SuggestWordType.Forum;
                        break;
                    //车系搜索推荐车系联想词
                    case 4:
                        suggestType = SuggestWordType.Cars;
                        break;
                    default:
                        break;
                }

                this.LoadSuggestWords(this.keywordTextBox.Text, suggestType);
            }
        }

        private void keywordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
                this.LoadSearchResult(this.keywordTextBox.Text);
            }
        }

        //更新搜索输入框提示内容
        private void searchTypeListPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (this.searchTypeListPicker.SelectedIndex)
            {
                case 0:
                    this.keywordTextBox.Hint = "搜索关键词";
                    break;
                case 1:
                    this.keywordTextBox.Hint = "搜索文章标题";
                    break;
                case 2:
                    this.keywordTextBox.Hint = "搜索视频标题";
                    break;
                case 3:
                    this.keywordTextBox.Hint = bbsID == 0 ? "搜索论坛或帖子标题" : "搜索帖子标题";
                    break;
                case 4:
                    this.keywordTextBox.Hint = "搜索车系";
                    break;
                default:
                    break;
            }

            //重新搜索
            string keyword = this.keywordTextBox.Text;
            if(!string.IsNullOrEmpty(keyword))
            {
                this.AddSearchHistory(keyword);
                this.LoadSearchResult(keyword);
            }
        }

        private void cleanHistory_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.ClearSearchHistory();
        }

        //滑动列表时隐藏键盘
        private void historyListBox_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            this.Focus();
        }

        private void historyItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string keyword = ((TextBlock)sender).Text;
            this.selectKeyword(keyword);
        }

        //滑动列表时隐藏键盘
        private void suggestWordsListBox_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            this.Focus();
        }

        private void suggestItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string keyword = ((TextBlock)sender).Text;
            this.selectKeyword(keyword);
        }

        private void selectKeyword(string keyword)
        {
            this.keywordTextBox.TextChanged -= this.keyboardTB_TextChanged;
            this.keywordTextBox.Text = keyword;
            this.keywordTextBox.TextChanged += this.keyboardTB_TextChanged;
            this.AddSearchHistory(keyword);
            this.LoadSearchResult(keyword);
        }

        private void ShowKeywords(bool historyThanSuggested)
        {
            this.KeywordsGrid.Visibility = Visibility.Visible;
            this.ResultGrid.Visibility = Visibility.Collapsed;
            this.historyGrid.Visibility = historyThanSuggested ? Visibility.Visible : Visibility.Collapsed;
            this.suggestWordsListBox.Visibility = historyThanSuggested ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion

        #region 历史记录
        private void LoadSearchHistory()
        {
            List<string> historyList = SearchHelper.GetSearchHistory();
            if (historyList != null)
            {
                foreach (string item in historyList)
                {
                    this.searchHistory.Add(item);
                }
            }
        }

        //如果不存在， 添加一条记录，最多10条，时间逆序
        private void AddSearchHistory(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim();
                if (this.searchHistory.Contains(keyword))
                {
                    this.searchHistory.Remove(keyword);
                    this.searchHistory.Insert(0, keyword);
                }
                else
                {
                    this.searchHistory.Insert(0, keyword.Trim());
                    if (this.searchHistory.Count > MaxHistoryCnt)
                    {
                        this.searchHistory.RemoveAt(MaxHistoryCnt - 1);
                    }
                }

                SearchHelper.UpdateSearchHistory(this.searchHistory);
            }
        }

        private void ClearSearchHistory()
        {
            this.searchHistory.Clear();
            SearchHelper.UpdateSearchHistory(null);
        }

        #endregion

        #region 通过接口下载联想词

        SuggestWordsViewModel suggestWordsVM = null;

        public void LoadSuggestWords(string keyword, SuggestWordType type)
        {
            if (suggestWordsVM == null)
            {
                suggestWordsVM = new SuggestWordsViewModel();
                suggestWordsVM.LoadDataCompleted += suggestWordsVM_LoadDataCompleted;
                this.suggestWordsListBox.DataContext = suggestWordsVM;
            }

            suggestWordsVM.ClearData();
            string url = string.Format("{0}{1}/sou/suggestwords.ashx?a={2}&pm={3}&v={4}&k={5}&t={6}", App.appUrl, App.versionStr, App.appId, App.platForm, App.version, keyword, (int)type);
            suggestWordsVM.LoadDataAysnc(url);
        }

        void suggestWordsVM_LoadDataCompleted(object sender, EventArgs e)
        {
            this.ShowKeywords(false);
        }

        #endregion

        #region 通过接口搜索

        private void LoadSearchResult(string keyword)
        {
            //每次搜索在历史记录中添加
            this.AddSearchHistory(keyword);

            //load data
            switch (this.searchTypeListPicker.SelectedIndex)
            {
                //综合
                case 0:
                    var generalSearchResultUC = new UcControl.SearchResult.GeneralSearchResult(keyword);
                    this.ShowResultUC(generalSearchResultUC);
                    generalSearchResultUC.LoadMore(true);
                    break;
                //文章
                case 1:
                    var articleSearchResultUC = new UcControl.SearchResult.ArticleSearchResult(keyword);
                    this.ShowResultUC(articleSearchResultUC);
                    articleSearchResultUC.LoadMore(true);
                    break;
                //视频
                case 2:
                    var videoSearchResultUC = new UcControl.SearchResult.VideoSearchResult(keyword);
                    this.ShowResultUC(videoSearchResultUC);
                    videoSearchResultUC.LoadMore(true);
                    break;
                //论坛
                case 3:
                    Model.Search.RelatedBBSModel relatedBBSModel = null;
                    if (bbsID != 0 && !string.IsNullOrEmpty(bbsName))
                    {
                        relatedBBSModel = new Model.Search.RelatedBBSModel() { ID = bbsID, Name = bbsName };
                    }
                    var forumSearchResultUC = new UcControl.SearchResult.ForumSearchResult(keyword, relatedBBSModel);
                    this.ShowResultUC(forumSearchResultUC);
                    forumSearchResultUC.LoadMore(true);
                    break;
                //车系
                case 4:
                    var carSeriesSearchResultUC = new UcControl.SearchResult.CarSeriesSearchResult(keyword);
                    this.ShowResultUC(carSeriesSearchResultUC);
                    carSeriesSearchResultUC.ReLoad();
                    break;
                default:
                    break;
            }
        }

        private void ShowResultUC(UIElement child)
        {
            if (child != null)
            {
                this.ResultGrid.Children.Clear();
                this.ResultGrid.Children.Add(child);
                this.KeywordsGrid.Visibility = Visibility.Collapsed;
                this.ResultGrid.Visibility = Visibility.Visible;
            }
        }

        #endregion
    }
}