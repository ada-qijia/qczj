using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections;

namespace AutoWP7.View.Me
{
    public partial class MyFavorite : PhoneApplicationPage
    {
        private LongListMultiSelector currentList;

        public MyFavorite()
        {
            InitializeComponent();
            this.Loaded += MyFavorite_Loaded;
        }

        void MyFavorite_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCurrentList();
        }

        private void FavoritePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCurrentList();
            currentList.IsSelectionEnabled = false;
        }

        private void UpdateCurrentList()
        {
            var selectPivot = FavoritePivot.SelectedItem as PivotItem;
            currentList = selectPivot.Content as LongListMultiSelector;
        }

        #region mutliSelect
        public enum MultiSelectionState
        {
            Ready,
            Selecting,
            SelectedAll,
        }

        ApplicationBarIconButton select;
        ApplicationBarIconButton delete;
        ApplicationBarMenuItem selectAll;
        ApplicationBarMenuItem cancelSelectAll;

        private void CreateApplicationBarItems()
        {
            select = new ApplicationBarIconButton();
            select.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Select.png", UriKind.RelativeOrAbsolute);
            select.Text = "选择";
            select.Click += OnSelectClick;

            delete = new ApplicationBarIconButton();
            delete.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Delete.png", UriKind.RelativeOrAbsolute);
            delete.Text = "删除";
            delete.Click += OnDeleteClick;

            selectAll = new ApplicationBarMenuItem();
            selectAll.Text = "全部选中";
            selectAll.Click += OnSelectAllClick;

            cancelSelectAll = new ApplicationBarMenuItem();
            cancelSelectAll.Text = "取消全选";
            cancelSelectAll.Click += OnCancelSelectAllClick;
        }

        private void SetupApplicationBar(MultiSelectionState state)
        {
            //clear applicationbar
            ApplicationBar.Buttons.Clear();
            ApplicationBar.MenuItems.Clear();

            switch (state)
            {
                case MultiSelectionState.Ready:
                    ApplicationBar.Buttons.Add(select);
                    break;
                case MultiSelectionState.Selecting:
                    ApplicationBar.Buttons.Add(delete);
                    ApplicationBar.MenuItems.Add(selectAll);
                    break;
                case MultiSelectionState.SelectedAll:
                    ApplicationBar.Buttons.Add(delete);
                    ApplicationBar.MenuItems.Add(cancelSelectAll);
                    break;
                default:
                    break;
            }
        }

        private void UpdateEmailApplicationBar()
        {
            if (currentList.IsSelectionEnabled)
            {
                bool hasSelection = ((currentList.SelectedItems != null) && (currentList.SelectedItems.Count > 0));
                bool allSelected = currentList.SelectedItems.Count == currentList.ItemsSource.Count;
                select.IsEnabled = currentList.ItemsSource.Count > 0;
                delete.IsEnabled = hasSelection;
                selectAll.IsEnabled = !allSelected;
                cancelSelectAll.IsEnabled = allSelected;
            }
        }

        void OnSelectClick(object sender, EventArgs e)
        {
            currentList.IsSelectionEnabled = true;
        }

        void OnDeleteClick(object sender, EventArgs e)
        {
            IList source = currentList.ItemsSource as IList;
            while (currentList.SelectedItems.Count > 0)
            {
                source.Remove(currentList.SelectedItems[0]);
            }
        }

        void OnSelectAllClick(object sender, EventArgs e)
        {
            currentList.SelectedItems.Clear();
            foreach (var item in currentList.ItemsSource)
            {
                currentList.SelectedItems.Add(item);
            }
        }

        void OnCancelSelectAllClick(object sender, EventArgs e)
        {
            currentList.SelectedItems.Clear();
            currentList.IsSelectionEnabled = false;
        }

        #endregion

        #region item navigation

        private void CarSeriesItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void CarSpecItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void ArticleItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void ForumItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void TopicItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        #endregion
    }
}