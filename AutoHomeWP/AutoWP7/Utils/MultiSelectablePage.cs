using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
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
                if (_currentList != value)
                {
                    //clear old state
                    if (_currentList != null)
                    {
                        _currentList.SelectionChanged -= _currentList_SelectionChanged;
                        _currentList.IsSelectionEnabled = false;
                    }

                    _currentList = value;
                    if (_currentList != null)
                    {
                        _currentList.SelectionChanged += _currentList_SelectionChanged;
                        _currentList.SelectedItems.Clear();
                        SetupApplicationBar(MultiSelectionState.Ready);
                    }
                }
            }
        }

        private void _currentList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            bool hasSelection = ((CurrentList.SelectedItems != null) && (CurrentList.SelectedItems.Count > 0));
            bool allSelected = CurrentList.ItemsSource != null && CurrentList.SelectedItems.Count == CurrentList.ItemsSource.Count;

            if (hasSelection)
            {
                var newState = allSelected ? MultiSelectionState.SelectedAll : MultiSelectionState.Selecting;
                SetupApplicationBar(newState);
            }
            else
            {
                SetupApplicationBar(MultiSelectionState.Ready);
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
                    ApplicationBar = new ApplicationBar() { BackgroundColor = Colors.Black };
                }

                ApplicationBar.IsVisible = this.CurrentList != null;
                //clear applicationbar
                ApplicationBar.Buttons.Clear();
                ApplicationBar.MenuItems.Clear();

                switch (state)
                {
                    case MultiSelectionState.Ready:
                        this.CurrentList.IsSelectionEnabled = false;
                        ApplicationBar.Buttons.Add(select);
                        //select.IsEnabled = CurrentList.ItemsSource != null && CurrentList.ItemsSource.Count > 0;
                        break;
                    case MultiSelectionState.Selecting:
                        this.CurrentList.IsSelectionEnabled = true;
                        ApplicationBar.Buttons.Add(delete);
                        ApplicationBar.MenuItems.Add(selectAll);
                        break;
                    case MultiSelectionState.SelectedAll:
                        this.CurrentList.IsSelectionEnabled = true;
                        ApplicationBar.Buttons.Add(delete);
                        ApplicationBar.MenuItems.Add(cancelSelectAll);
                        break;
                    default:
                        break;
                }
            }

            delete.IsEnabled = CurrentList.SelectedItems.Count > 0;
            selectAll.IsEnabled = CurrentList.ItemsSource != null && CurrentList.SelectedItems.Count != CurrentList.ItemsSource.Count;
            cancelSelectAll.IsEnabled = CurrentList.ItemsSource != null && CurrentList.SelectedItems.Count == CurrentList.ItemsSource.Count;
        }

        #endregion

        void OnSelectClick(object sender, EventArgs e)
        {
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
            if (selectedItems != null && CurrentList.ItemsSource != null)
            {
                while (selectedItems.Count > 0)
                {
                    CurrentList.ItemsSource.Remove(CurrentList.SelectedItems[0]);
                }
            }
        }

        void OnSelectAllClick(object sender, EventArgs e)
        {
            if (CurrentList.ItemsSource != null)
            {
                foreach (var item in CurrentList.ItemsSource)
                {
                    if (!CurrentList.SelectedItems.Contains(item))
                    { CurrentList.SelectedItems.Add(item); }
                }
            }
        }

        void OnCancelSelectAllClick(object sender, EventArgs e)
        {
            CurrentList.SelectedItems.Clear();
            SetupApplicationBar(MultiSelectionState.Ready);
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (CurrentList != null && CurrentList.IsSelectionEnabled)
            {
                CurrentList.SelectedItems.Clear();
                SetupApplicationBar(MultiSelectionState.Ready);

                e.Cancel = true;
            }

            base.OnBackKeyPress(e);
        }

        #endregion
    }
}
