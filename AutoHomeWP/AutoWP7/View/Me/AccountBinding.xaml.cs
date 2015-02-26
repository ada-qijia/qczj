using Microsoft.Phone.Controls;
using QConnectSDK.Exceptions;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using ViewModels.Handler;
using ViewModels.Me;

namespace AutoWP7.View.Me
{
    public partial class AccountBinding : PhoneApplicationPage
    {
        private ThirdPartyBindingViewModel BindingVM;

        public AccountBinding()
        {
            InitializeComponent();
            this.BindingVM = new ThirdPartyBindingViewModel();
            this.BindingVM.LoadDataCompleted += BindingVM_LoadDataCompleted;
            this.BindingVM.WeiboCheckTokenExpiredCompleted += BindingVM_WeiboCheckTokenExpiredCompleted;
            this.DataContext = this.BindingVM;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            string url = Utils.MeHelper.GetThirdPartyBindingUrl();
            this.BindingVM.LoadDataAysnc(url);
        }

        void BindingVM_LoadDataCompleted(object sender, EventArgs e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            if (this.BindingVM.ReturnCode == 10002)
            {
                CustomMessageBox messageBox = new CustomMessageBox()
                {
                    Caption = "获取失败",
                    Message = "密码检验失败",
                    LeftButtonContent = "取消",
                    RightButtonContent = "重新登录",
                    IsFullScreen = false
                };

                messageBox.Dismissed += (s1, e1) =>
                 {
                     switch (e1.Result)
                     {
                         case CustomMessageBoxResult.RightButton:
                             NavigationService.Navigate(new Uri("/View/More/LoginPage.xaml", UriKind.RelativeOrAbsolute));
                             break;
                         case CustomMessageBoxResult.LeftButton:
                             //go back to me page.
                             NavigationService.RemoveBackEntry();
                             NavigationService.RemoveBackEntry();
                             break;
                         default:
                             break;
                     }
                 };

                messageBox.Show();
            }

            //检查账户是否过期
            this.CheckTokenExpired();
        }

        #region 检查账户是否过期

        private void CheckTokenExpired()
        {
            //weibo
            this.BindingVM.WeiboCheckTokenExpired();

            //qq
            var qq = this.BindingVM.BindingList.FirstOrDefault(item => item.ThirdPartyID == 2);
            if (qq != null && qq.Token != null && qq.ThirdPartyID > 0)
            {
                var qqAuthVM = View.Me.QQ.AuthenticationViewModel.SingleInstance;
                qqAuthVM.LoadProfile(qq.Token, qq.ThirdPartyID.ToString(), QQLoadProfileCompleted);
            }
        }

        void BindingVM_WeiboCheckTokenExpiredCompleted(object sender, APIEventArgs<bool> e)
        {
            //授权状态,更新按钮文本
        }

        void QQLoadProfileCompleted(object sender, QzoneException e)
        {
            var qqAuthVM = sender as View.Me.QQ.AuthenticationViewModel;
            var qq = this.BindingVM.BindingList.FirstOrDefault(item => item.ThirdPartyID == 2);
            if (e == null && qqAuthVM.Profile != null)
            {
                if (qqAuthVM.Profile.Ret == 0)//未过期
                {
                    qq.IsExpired = false;
                }
                else if (qqAuthVM.Profile.Ret == 100014 || qqAuthVM.Profile.Ret == 100015)//过期，需重新注册
                {
                    qq.IsExpired = true;
                }
                else//其他错误
                {
                }
            }
        }

        #endregion

        #region UI交互

        private void WeiboBinding_Click(object sender, RoutedEventArgs e)
        {

        }

        private void QQBinding_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion
    }
}