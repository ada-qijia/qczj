using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AutoWP7.View.Me.QQ
{
    public partial class AuthenticationPage : PhoneApplicationPage
    {
        public AuthenticationPage()
        {
            InitializeComponent();

            this.DataContext = Me.QQ.AuthenticationViewModel.SingleInstance;
        }

        private void webBrowser1_Navigating(object sender, NavigatingEventArgs e)
        {
            if (e.Uri.Host.Equals("qzs.qq.com"))
            {
                webBrowser1.Visibility = Visibility.Collapsed;
                e.Cancel = true;
                //e.Uri.Fragment.Replace("#", "");
                // setting this text will bind it back to the view model
                codeBlock.Text = e.Uri.Fragment.Replace("#", "");
            }
        }

        private void webBrowser1_Navigated(object sender, NavigationEventArgs e)
        {
            webBrowser1.Visibility = Visibility.Visible;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (HasAuthenticatedCheckbox.IsChecked.Value && NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}