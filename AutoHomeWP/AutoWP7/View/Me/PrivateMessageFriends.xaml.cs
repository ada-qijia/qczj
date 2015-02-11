using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ViewModels.Me;
using AutoWP7.Utils;
using Model.Me;
using System.Collections;

namespace AutoWP7.View.Me
{
    public partial class PrivateMessageFriends : MultiSelectablePage
    {
        private PrivateMessageFriendViewModel FriendsVM;

        public PrivateMessageFriends()
        {
            InitializeComponent();

            this.CurrentList = this.FriendsListBox;

            this.FriendsVM = new PrivateMessageFriendViewModel();
            this.FriendsVM.LoadDataCompleted += FriendsVM_LoadDataCompleted;
            this.DataContext = this.FriendsVM;
            this.LoadLocally();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.LoadMore(true);
        }

        #region load data

        /// <summary>
        /// Load cached data.
        /// </summary>
        private void LoadLocally()
        {
            this.FriendsVM.ClearData();

            var cacheModel = PrivateMessageCacheViewModel.SingleInstance.CacheModel;
            foreach (var item in cacheModel.Friends)
            {
                this.FriendsVM.FriendList.Add(item);
            }
        }

        private void SaveLocally()
        {
            var cacheVM = PrivateMessageCacheViewModel.SingleInstance;
            IEnumerable<PrivateMessageFriendModel> source = this.FriendsListBox.ItemsSource as IEnumerable<PrivateMessageFriendModel>;
            if (source != null)
            {
                List<PrivateMessageFriendModel> friends = new List<PrivateMessageFriendModel>(source);
                cacheVM.UpdateFriends(friends);
            }
        }

        private void FriendsVM_LoadDataCompleted(object sender, EventArgs e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            if (this.FriendsVM.ReturnCode != 0)
            {
                CustomMessageBox messageBox = new CustomMessageBox()
                {
                    Caption = "",
                    Message = "密码检验失败",
                    LeftButtonContent = "取消",
                    RightButtonContent = "重新登录",
                    IsFullScreen = false
                };

                messageBox.Dismissed += (s1, e1) =>
                {
                    switch (e1.Result)
                    {
                        case CustomMessageBoxResult.LeftButton:
                            NavigationService.Navigate(new Uri("View/More/LoginPage.xaml", UriKind.RelativeOrAbsolute));
                            break;
                        case CustomMessageBoxResult.RightButton:
                            //返回未登录下我的主页
                            NavigationService.GoBack();
                            break;
                        default:
                            break;
                    }
                };

                messageBox.Show();
            }
            else
            {
                bool noResult = this.FriendsVM.RowCount == 0;
                if (noResult)
                {
                    this.NoResultUC.SetContent("您还没有收到私信");
                }

                if (this.FriendsVM.PageIndex == 1)//update local cache.
                {
                    PrivateMessageCacheViewModel.SingleInstance.UpdateFriends(this.FriendsVM.FriendList.ToList());
                }

                this.NoResultUC.Visibility = noResult ? Visibility.Visible : Visibility.Collapsed;
                this.FriendsListBox.Visibility = noResult ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void LoadMore(bool restart)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            int nextPageIndex = this.FriendsVM.PageIndex + 1;
            if (restart)
            {
                nextPageIndex = 1;
            }

            string url = Utils.MeHelper.GetPrivateMessageFriendsUrl(nextPageIndex);
            this.FriendsVM.LoadDataAysnc(url);
        }

        #endregion

        #region UI interaction

        private void refresh_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LoadMore(true);
        }

        //导航到私信列表页
        private void Friend_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var element = sender as FrameworkElement;
            var model = element.DataContext as Model.Me.PrivateMessageFriendModel;
            if (model != null)
            {
                string url = string.Format("/View/Me/PrivateMessage.xaml?username={0}&userId={1}&userimg={2}", model.UserName, model.ID, model.Img);
                this.NavigationService.Navigate(new Uri(url, UriKind.Relative));

                element.DataContext = null;
                model.UnRead = 0;
                element.DataContext = model;
                //((Grid)sender).GetBindingExpression(Grid.DataContextProperty).UpdateSource();
            }
        }

        private void LoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LoadMore(false);
        }

        #endregion

        #region override MultiSelectablePage method

        public override void AfterDeleteItems(IList selectedItems)
        {
            base.AfterDeleteItems(selectedItems);

            this.SaveLocally();
        }

        #endregion
    }
}