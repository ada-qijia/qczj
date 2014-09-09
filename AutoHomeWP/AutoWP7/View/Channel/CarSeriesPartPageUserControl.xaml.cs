using System;
using System.Windows;
using System.Windows.Controls;
using AutoWP7.View.Car;
using Microsoft.Phone.Controls;

namespace AutoWP7.View.Channel
{
    public partial class CarSeriesPartPageUserControl : UserControl
    {
        public CarSeriesPartPageUserControl()
        {
            InitializeComponent();
        }

        private void partPagelist_Loaded(object sender, RoutedEventArgs e)
        {

            partPagelist.ItemsSource = CarSeriesArticleEndPage.newsDataSource;
     
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            (App.Current as App).carSeriesPartPageMessage.Hide();
        }

        /// <summary>
        /// 导航到具体某页面
        /// </summary>
        private void newsDetailIndex_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid gg = (Grid)sender;
            (App.Current as App).carSeriesPartPageMessage.Hide();

            PhoneApplicationFrame rootFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            Uri urlSource = new Uri("/View/Car/CarSeriesArticleEndPage.xaml?newsid=" + App.newsid + "&pageIndex=" + gg.Tag, UriKind.Relative);
            rootFrame.Navigate(urlSource);
        }

    }
}
