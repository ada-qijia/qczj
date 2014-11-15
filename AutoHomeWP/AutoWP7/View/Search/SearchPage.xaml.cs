using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AutoWP7.Utils;
using System.Windows.Input;
using System.Collections.ObjectModel;
using ViewModels;
using ViewModels.Handler;

namespace AutoWP7.View.Search
{
    public partial class SearchPage : PhoneApplicationPage
    {
        const int MaxHistoryCnt = 10;

        private ObservableCollection<string> searchHistory;

        public SearchPage()
        {
            InitializeComponent();

            this.searchTypeListPicker.DataContext = Enum.GetValues(typeof(SearchType));
            LoadSearchHistory();
        }

        public static string GetSearchPageUrlWithParams(SearchType type)
        {
            return string.Format("/View/Search/SearchPage.xaml?type={0}", (int)type);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //set the selected search type.
            if (NavigationContext.QueryString.ContainsKey("type"))
            {
                int typeIndex;
                if (int.TryParse(NavigationContext.QueryString["type"], out typeIndex))
                {
                    this.searchTypeListPicker.SelectedIndex = typeIndex;
                }
            }
        }

        //激活搜索框
        private void keywordTB_Loaded(object sender, RoutedEventArgs e)
        {
            this.keywordTextBox.Focus();
        }

        //显示或隐藏搜索历史
        private void keyboardTB_TextInputUpdate(object sender, TextCompositionEventArgs e)
        {
            this.historyGrid.Visibility = string.IsNullOrEmpty(this.keywordTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        //列出最多10个推荐联想词
        private void keyboardTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.keywordTextBox.Text))
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

        private void search_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LoadSearchResult(this.keywordTextBox.Text);
        }

        private void cancel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

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
                    this.keywordTextBox.Hint = "搜索论坛或帖子标题";
                    break;
                case 4:
                    this.keywordTextBox.Hint = "搜索车系";
                    break;
                default:
                    break;
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
            this.keywordTextBox.Text = keyword;
            this.LoadSearchResult(keyword);
        }

        //滑动列表时隐藏键盘
        private void suggestWordsListBox_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            this.Focus();
        }

        private void suggestItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string keyword = ((TextBlock)sender).Text;
            this.keywordTextBox.Text = keyword;
            this.LoadSearchResult(keyword);
        }

        #region 历史记录
        private void LoadSearchHistory()
        {
            this.searchHistory = new ObservableCollection<string>();
            List<string> historyList = SearchHelper.GetSearchHistory();
            if (historyList != null)
            {
                foreach (string item in historyList)
                {
                    this.searchHistory.Add(item);
                }
            }
            this.historyListBox.DataContext = this.searchHistory;
        }

        //如果不存在， 添加一条记录，最多10条，时间逆序
        private void AddSearchHistory(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword) && this.searchHistory.Contains(keyword.Trim()))
            {
                this.searchHistory.Insert(0, keyword.Trim());
                if (this.searchHistory.Count > MaxHistoryCnt)
                {
                    this.searchHistory.RemoveAt(MaxHistoryCnt - 1);
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
            }

            //清空原有数据
            suggestWordsVM.WordsDataSource.Clear();

            //string url = "http://221.192.136.99:804/wpv1.6/sou/suggestwords.ashx?a=2&pm=3&v=1.6.0&k=bmw&t=1";
            string url = string.Format("{0}{1}/sou/suggestwords.ashx?a={2}&pm={3}&v={4}&k={5}&t={6}", App.appUrl, App.versionStr, App.appId, App.platForm, App.version, keyword, (int)type);
            suggestWordsVM.LoadDataAysnc(url);
        }

        private void suggestWordsVM_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<string>> e)
        {
            //set suggestwords as datasource
            if (this.suggestWordsListBox.DataContext == null)
            {
                this.suggestWordsListBox.DataContext = suggestWordsVM;
            }
        }

        #endregion

        #region 通过接口搜索

        private void LoadSearchResult(string keyword)
        {
            //每次搜索在历史记录中添加
            this.AddSearchHistory(keyword);

            //load data
        }

        #endregion
    }
}