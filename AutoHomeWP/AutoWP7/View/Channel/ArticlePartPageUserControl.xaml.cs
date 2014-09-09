using System;
using System.Windows;
using System.Windows.Controls;
using AutoWP7.View.Channel.Newest;
using Microsoft.Phone.Controls;

namespace AutoWP7.View.Channel
{
    public partial class ArticlePartPageUserControl : UserControl
    {
        public ArticlePartPageUserControl()
        {
            InitializeComponent();

        }

        private void partPagelist_Loaded(object sender, RoutedEventArgs e)
        {

            partPagelist.ItemsSource = ArticleEndPage.newsDataSource;
        }


        /// <summary>
        /// 导航到具体某页面
        /// </summary>
        private void newsDetailIndex_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            Grid gg = (Grid)sender;
            (App.Current as App).newestPartPageMessage.Hide();
            PhoneApplicationFrame rootFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            ((ArticleEndPage)rootFrame.Content).wb.InvokeScript("GetALocation", "news", gg.Tag.ToString());
            //Uri urlSource= new Uri("/View/Channel/Newest/ArticleEndPage.xaml?newsid=" + App.newsid + "&pageIndex="+gg.Tag, UriKind.Relative);
            //rootFrame.Navigate(urlSource);  
        }

        private void LayoutRoot_LostFocus(object sender, RoutedEventArgs e)
        {
            (App.Current as App).newestPartPageMessage.Hide();
        }


    }
}
