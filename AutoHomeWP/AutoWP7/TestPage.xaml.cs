using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace AutoWP7
{
    public partial class TestPage : PhoneApplicationPage
    {
        public TestPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //string urlStr = "http://autohometest.azurewebsites.net/3.html";
            //Uri uri = new Uri(urlStr, UriKind.Absolute);
            //this.webBrowser.Navigate(uri);



            string video = "http://video.ch9.ms/ch9/eafd/4f2a4d99-05fe-4ebd-97c2-709957a5eafd/EnableTestingInProductionInAzureWebsites_mid.mp4";
            string cover = "http://video.ch9.ms/ch9/eafd/4f2a4d99-05fe-4ebd-97c2-709957a5eafd/EnableTestingInProductionInAzureWebsites_512.jpg";
            //videoPlayer.SetCover(cover);
            //videoPlayer.SetSource(video);

            string url = "http://221.192.136.99:804/wpv1.5/content/news/wpvideopage-a2-pm3-v1.5.0-vidXNzkxNTM5MTMy.html";
            browser.Navigate(new Uri(url, UriKind.Absolute));
        }

        private void videoPlayer_FullScreen(object sender, bool fullScreen)
        {
            //if (fullScreen)
            //{
            //    this.contentsOtherThanVideo.Visibility = System.Windows.Visibility.Collapsed;
            //    this.SupportedOrientations = SupportedPageOrientation.Landscape;
            //    this.Orientation = PageOrientation.Landscape;
            //    videoRow.Height = new GridLength(1, GridUnitType.Star);
            //    otherRow.Height = new GridLength(0, GridUnitType.Star);
            //}
            //else
            //{
            //    this.contentsOtherThanVideo.Visibility = System.Windows.Visibility.Visible;
            //    this.SupportedOrientations = SupportedPageOrientation.Portrait;
            //    this.Orientation = PageOrientation.Portrait;
            //    videoRow.Height = new GridLength(1, GridUnitType.Auto);
            //    otherRow.Height = new GridLength(1, GridUnitType.Star);
            //}
        }

        private void browser_ScriptNotify(object sender, NotifyEventArgs e)
        {

        }

    }
}

