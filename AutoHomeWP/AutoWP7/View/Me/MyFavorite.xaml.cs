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
using ViewModels.Me;
using Model.Me;

namespace AutoWP7.View.Me
{
    public partial class MyFavorite : Utils.MultiSelectablePage
    {
        private FavoriteViewModel FavoriteVM;

        public MyFavorite()
        {
            InitializeComponent();

            this.FavoriteVM = FavoriteViewModel.SingleInstance;
            this.DataContext = this.FavoriteVM;
            this.FavoriteVM.Reload(FavoriteType.All);
        }

        private void FavoritePivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCurrentList();
        }

        private void UpdateCurrentList()
        {
            var selectPivot = FavoritePivot.SelectedItem as PivotItem;
            this.CurrentList = selectPivot.Content as LongListMultiSelector;
        }

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