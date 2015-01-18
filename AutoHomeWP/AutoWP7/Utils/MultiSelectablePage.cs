using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            UpdateApplicationBar();
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
                    ApplicationBar = new ApplicationBar();
                    CreateApplicationBarItems();
                }

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
        }

        private void UpdateApplicationBar()
        {
            if (CurrentList.IsSelectionEnabled)
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
                select.IsEnabled = CurrentList.ItemsSource.Count > 0;
                delete.IsEnabled = hasSelection;
                selectAll.IsEnabled = !allSelected;
                cancelSelectAll.IsEnabled = allSelected;
            }
            else
            {
                SetupApplicationBar(MultiSelectionState.Ready);
            }
        }

        void OnSelectClick(object sender, EventArgs e)
        {
            CurrentList.IsSelectionEnabled = true;
        }

        void OnDeleteClick(object sender, EventArgs e)
        {
            IList source = CurrentList.ItemsSource as IList;
            foreach(var item in CurrentList.SelectedItems)
            //while (CurrentList.SelectedItems.Count > 0)
            {
                //source.Remove(CurrentList.SelectedItems[0]);
                source.Remove(item);
            }

            CurrentList.SelectedItems.Clear();
        }

        void OnSelectAllClick(object sender, EventArgs e)
        {
            CurrentList.SelectedItems.Clear();
            foreach (var item in CurrentList.ItemsSource)
            {
                CurrentList.SelectedItems.Add(item);
            }
        }

        void OnCancelSelectAllClick(object sender, EventArgs e)
        {
            CurrentList.SelectedItems.Clear();
            CurrentList.IsSelectionEnabled = false;
        }

        #endregion
    }
}
