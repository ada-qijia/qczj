using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace AutoWP7.View.Me
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void AccountBinding_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void NotificationSetting_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        //小图模式开关 
        private void PictureMode_Click(object sender, RoutedEventArgs e)
        {
            //save the new value to appsetting
        }

        private void ClearCache_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Feedback_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("MoreActivity", "反馈点击量");
            this.NavigationService.Navigate(new Uri("/View/More/FeedbackPage.xaml", UriKind.Relative));
        }

        private void AutohomeMobile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("MoreActivity", "M站点击量");
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://m.autohome.com.cn/", UriKind.Absolute);
            task.Show();
        }

        private void About_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("关于", "反馈点击量");
            this.NavigationService.Navigate(new Uri("/View/More/AboutPage.xaml", UriKind.Relative));
        }
    }
}