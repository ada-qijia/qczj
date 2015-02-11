using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using System;
using System.Windows.Navigation;

namespace AutoWP7.View.Me
{
    public partial class ServiceProtocol : PhoneApplicationPage
    {
        public ServiceProtocol()
        {
            InitializeComponent();

            wb.Navigate(new Uri(Utils.MeHelper.ServiceProtocolUrl, UriKind.Absolute));
        }

        #region web navigation events handler

        private void wb_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Common.showMsg("您的网络不可用~请检查连接设置");
        }

        private void wb_Navigating(object sender, NavigatingEventArgs e)
        {
            GlobalIndicator.Instance.Text = "加载中";
            GlobalIndicator.Instance.IsBusy = true;
        }

        private void wb_Navigated(object sender, NavigationEventArgs e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
        }

        #endregion
    }
}