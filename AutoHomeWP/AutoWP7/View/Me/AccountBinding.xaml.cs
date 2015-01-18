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

namespace AutoWP7.View.Me
{
    public partial class AccountBinding : PhoneApplicationPage
    {
        private ThirdPartyBindingViewModel BindingVM;

        public AccountBinding()
        {
            InitializeComponent();
            this.BindingVM = new ThirdPartyBindingViewModel();
            this.DataContext = this.BindingVM;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string url = Utils.MeHelper.GetThirdPartyBindingUrl();
            this.BindingVM.LoadDataAysnc(url);
        }

        private void WeiboBinding_Click(object sender, RoutedEventArgs e)
        {

        }

        private void QQBinding_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}