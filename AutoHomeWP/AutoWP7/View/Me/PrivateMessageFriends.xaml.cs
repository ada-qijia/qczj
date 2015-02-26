using AutoWP7.Handler;
using AutoWP7.Utils;
using Microsoft.Phone.Controls;
using Model.Me;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Navigation;
using ViewModels.Me;

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
            //this.FriendsListBox.Loaded += (sender, e) => { AddScrollEvent(); };
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var userInfo = Utils.MeHelper.GetMyInfoModel();

            if (userInfo == null && e.NavigationMode == NavigationMode.New)
            {
                //未登录，跳转到登录页
                this.NavigationService.Navigate(new Uri("/View/More/LoginPage.xaml", UriKind.Relative));
            }
            else if(userInfo!= null)
            {
                this.LoadLocally();
                this.LoadMore(true);
            }
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

            if (this.FriendsVM.ReturnCode == 10001 || this.FriendsVM.ReturnCode == 10002)
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
                            NavigationService.Navigate(new Uri("/View/More/LoginPage.xaml", UriKind.RelativeOrAbsolute));
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
            else if (this.FriendsVM.ReturnCode == 0)
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
            if (url != null)
            {
                this.FriendsVM.LoadDataAysnc(url);
            }
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
            //上传
            var userInfoModel = Utils.MeHelper.GetMyInfoModel();
            if (userInfoModel != null)
            {
                string url = Utils.MeHelper.SyncPrivateMessageFriendsUrl;
                foreach (PrivateMessageFriendModel item in selectedItems)
                {
                    string data = string.Format("_appid=app.wp&authorization={0}&targetuserid={1}&_timestamp={2}&autohomeua={3}", userInfoModel.Authorization, item.ID, Common.GetTimeStamp(), Common.GetAutoHomeUA());
                    data = Handler.Common.SortURLParamAsc(data);
                    string sign = Handler.Common.GetSignStr(data);
                    data += "&_sign=" + sign;

                    UpStreamViewModel.SingleInstance.UploadAsyncWithOneoffClient(url, data, null);
                }
            }

            base.AfterDeleteItems(selectedItems);
            this.SaveLocally();
        }

        #endregion

        #region 滚动到顶端自动加载，未启用,有问题

        private void AddScrollEvent()
        {
            var scrollbar = AutoWP7.Handler.Common.FindChildOfType<ScrollBar>(this.FriendsListBox);
            if (scrollbar != null)
            {
                scrollbar.ValueChanged += scrollbarV_ValueChanged;
            }
        }

        void scrollbarV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var scrollbar = sender as ScrollBar;
            if (scrollbar.Value <= scrollbar.Minimum)
            {
                //refresh
                this.LoadMore(true);
            }
        }
        #endregion
    }
}