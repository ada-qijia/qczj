using System;
using System.Windows;
using System.Windows.Controls;
using AutoWP7.View.Channel.News;
using Microsoft.Phone.Controls;

namespace AutoWP7.View.Channel
{
    public partial class NewsPartPageUserControl : UserControl
    {
        public NewsPartPageUserControl()
        {
            InitializeComponent();
        }

        private void partPagelist_Loaded(object sender, RoutedEventArgs e)
        {
            partPagelist.ItemsSource = NewsEndPage.newsDataSource;
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            (App.Current as App).newsPartPageMessage.Hide();
        }

        /// <summary>
        /// 导航到具体某页面
        /// </summary>
        private void newsDetailIndex_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid gg = (Grid)sender;
            //影藏控件
            (App.Current as App).newsPartPageMessage.Hide();
            //获取控件的主页面（主应用程序用户页面）
            PhoneApplicationFrame rootFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            ((NewsEndPage)rootFrame.Content).wb.InvokeScript("GetALocation", "news", gg.Tag.ToString());
            //Uri urlSource = new Uri("/View/Channel/News/NewsEndPage.xaml?newsid=" + App.newsid + "&pageIndex=" + gg.Tag, UriKind.Relative);
            //rootFrame.Navigate(urlSource);
        }

    }
}
