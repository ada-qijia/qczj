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

       

            string video = "http://video.ch9.ms/ch9/0420/5ad1f148-cad7-4d82-a7e4-898e009c0420/20140905TWC9_mid.mp4";
            string cover = "http://static.youku.com/index/img/header/yklogo.png";
            videoPlayer.SetCover(cover);
            videoPlayer.SetSource(video);

        }

        private void videoPlayer_FullScreen(object sender, bool fullScreen)
        {
            if (fullScreen)
            {
                this.contentsOtherThanVideo.Visibility = System.Windows.Visibility.Collapsed;
                this.SupportedOrientations = SupportedPageOrientation.Landscape;
                this.Orientation = PageOrientation.Landscape;
                videoRow.Height = new GridLength(1, GridUnitType.Star);
                otherRow.Height = new GridLength(0, GridUnitType.Star);
            }
            else
            {
                this.contentsOtherThanVideo.Visibility = System.Windows.Visibility.Visible;
                this.SupportedOrientations = SupportedPageOrientation.Portrait;
                this.Orientation = PageOrientation.Portrait;
                videoRow.Height = new GridLength(1, GridUnitType.Auto);
                otherRow.Height = new GridLength(1, GridUnitType.Star);
            }
        }

    }
}

