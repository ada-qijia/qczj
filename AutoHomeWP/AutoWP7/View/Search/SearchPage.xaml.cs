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

namespace AutoWP7.View.Search
{
    public partial class SearchPage : PhoneApplicationPage
    {
        const int MaxHistoryCnt = 10;

        private ObservableCollection<string> searchHistory;

        public SearchPage()
        {
            InitializeComponent();

            LoadData();
        }

        private void LoadData()
        {
            this.searchTypeListPicker.DataContext = Enum.GetValues(typeof(SearchType));

            this.searchHistory = new ObservableCollection<string>();
            foreach (string item in SearchHelper.GetSearchHistory())
            {
                this.searchHistory.Add(item);
            }
            this.historyListBox.DataContext = this.searchHistory;
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

        private void keyboardTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.historyGrid.Visibility = string.IsNullOrEmpty(this.keywordTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void search_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string keyword = this.keywordTextBox.Text;
            if (!string.IsNullOrEmpty(keyword) && this.searchHistory.Contains(keyword))
            {
                this.searchHistory.Insert(0, keyword);
                if (this.searchHistory.Count > MaxHistoryCnt)
                {
                    this.searchHistory.RemoveAt(MaxHistoryCnt - 1);
                }
            }
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

        private void historyItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.keywordTextBox.Text = ((TextBlock)sender).Text;
            this.Focus();
        }

        private void cleanHistory_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SearchHelper.UpdateSearchHistory(null);
        }
    }
}