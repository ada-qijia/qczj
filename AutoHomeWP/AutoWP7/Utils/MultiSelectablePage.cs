using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

namespace AutoWP7.Utils
{
    public class MultiSelectablePage : PhoneApplicationPage
    {
        private MultiSelectionState currentSelctionState;

        private LongListMultiSelector _currentList;
        public LongListMultiSelector CurrentList
        {
            get { return _currentList; }
            set
            {
                if (_currentList != null)
                {
                    _currentList.SelectionChanged -= _currentList_SelectionChanged;
                    _currentList.SelectedItems.Clear();
                    _currentList.IsSelectionEnabled = false;
                }

                if (_currentList != value && value != null)
                {
                    _currentList = value;
                    _currentList.SelectionChanged += _currentList_SelectionChanged;
                    SetupApplicationBar(MultiSelectionState.Ready);
                }
            }
        }

        private void _currentList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            bool hasSelection = ((CurrentList.SelectedItems != null) && (CurrentList.SelectedItems.Count > 0));
            bool allSelected = CurrentList.SelectedItems.Count == CurrentList.ItemsSource.Count;

            if (hasSelection && allSelected)
            {
                SetupApplicationBar(MultiSelectionState.SelectedAll);
            }
            else
            {
                SetupApplicationBar(MultiSelectionState.Selecting);
            }
        }

        public MultiSelectablePage()
        {
            CreateApplicationBarItems();
        }

        #region mutliSelect

        public enum MultiSelectionState
        {
            NotReady,
            Ready,
            Selecting,
            SelectedAll,
        }

        #region appbar

        ApplicationBarIconButton select;
        ApplicationBarIconButton delete;
        ApplicationBarMenuItem selectAll;
        ApplicationBarMenuItem cancelSelectAll;

        private void CreateApplicationBarItems()
        {
            select = new ApplicationBarIconButton();
            select.IconUri = new Uri("/Images/bar_select.png", UriKind.RelativeOrAbsolute);
            select.Text = "选择";
            select.Click += OnSelectClick;

            delete = new ApplicationBarIconButton();
            delete.IconUri = new Uri("/Images/car_del.png", UriKind.RelativeOrAbsolute);
            delete.Text = "删除";
            delete.Click += OnDeleteClick;

            selectAll = new ApplicationBarMenuItem();
            selectAll.Text = "全部选中";
            selectAll.Click += OnSelectAllClick;

            cancelSelectAll = new ApplicationBarMenuItem();
            cancelSelectAll.Text = "取消全选";
            cancelSelectAll.Click += OnCancelSelectAllClick;
        }

        /// <summary>
        /// Set which item visible.
        /// </summary>
        /// <param name="state"></param>
        private void SetupApplicationBar(MultiSelectionState state)
        {
            if (currentSelctionState != state)
            {
                currentSelctionState = state;

                if (ApplicationBar == null)
                {
                    ApplicationBar = new ApplicationBar() {BackgroundColor = Colors.Black, Opacity = 0.8};
                }

                ApplicationBar.IsVisible = true;
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

                if (this.CurrentList.IsSelectionEnabled)
                {
                    bool hasSelection = ((CurrentList.SelectedItems != null) && (CurrentList.SelectedItems.Count > 0));
                    bool allSelected = CurrentList.SelectedItems.Count == CurrentList.ItemsSource.Count;
                    select.IsEnabled = CurrentList.ItemsSource.Count > 0;
                    delete.IsEnabled = hasSelection;
                    selectAll.IsEnabled = !allSelected;
                    cancelSelectAll.IsEnabled = allSelected;
                }
                else
                {
                 
                }
            }
        }

        #endregion

        void OnSelectClick(object sender, EventArgs e)
        {
            CurrentList.IsSelectionEnabled = true;
            SetupApplicationBar(MultiSelectionState.Selecting);
        }


        /// <summary>
        /// Remove selected item from ItemsSource.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnDeleteClick(object sender, EventArgs e)
        {
            this.AfterDeleteItems(CurrentList.SelectedItems);
        }

        public virtual void AfterDeleteItems(IList selectedItems)
        {
            if (selectedItems != null)
            {
                while (selectedItems.Count > 0)
                {
                    CurrentList.ItemsSource.Remove(CurrentList.SelectedItems[0]);
                }
            }
        }

        void OnSelectAllClick(object sender, EventArgs e)
        {
            foreach (var item in CurrentList.ItemsSource)
            {
                if (!CurrentList.SelectedItems.Contains(item))
                { CurrentList.SelectedItems.Add(item); }
            }
        }

        void OnCancelSelectAllClick(object sender, EventArgs e)
        {
            CurrentList.SelectedItems.Clear();
            CurrentList.IsSelectionEnabled = false;
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (CurrentList != null && CurrentList.IsSelectionEnabled)
            {
                CurrentList.SelectedItems.Clear();
                this.CurrentList.IsSelectionEnabled = false;
                SetupApplicationBar(MultiSelectionState.Ready);

                e.Cancel = true;
            }

            base.OnBackKeyPress(e);
        }

        #endregion
    }
}
