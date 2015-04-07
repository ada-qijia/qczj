using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Model;
using ViewModels;
using Microsoft.Phone.Tasks;

namespace AutoWP7.View.Channel.News
{
    public partial class VideoEndPage : PhoneApplicationPage
    {
        private string pageType = "3";

        public VideoEndPage()
        {
            InitializeComponent();
        }

        string videoId = string.Empty;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            videoId = NavigationContext.QueryString["videoid"];
            //pageType = NavigationContext.QueryString["pageType"];

            LoadData();

            var lastPage = NavigationService.BackStack.First();
            if (lastPage.Source.ToString().Contains("VideoEndPage"))
            {
                NavigationService.RemoveBackEntry();
            }

            //switch (e.NavigationMode)
            //{
            //    case System.Windows.Navigation.NavigationMode.New:
            //        {
            //        }
            //        break;
            //    case System.Windows.Navigation.NavigationMode.Back:
            //        {
            //        }
            //        break;
            //}
        }

        //protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        //{
        //    if ((App.Current as App).newsPartPageMessage.IsOpen == true)
        //    {
        //        (App.Current as App).newsPartPageMessage.Hide();
        //        e.Cancel = true;
        //    }
        //    else
        //    {
        //        //获得返回队列数
        //        int sum = NavigationService.BackStack.Count();
        //        for (int i = 2; i < sum; i++)
        //        {
        //            NavigationService.RemoveBackEntry();
        //        }
        //        base.OnBackKeyPress(e);
        //    }
        //}

        //分页集合

        #region 数据加载

        VideoDetailViewModel VM = null;
        VideoDetailModel videoData = null;
        private void LoadData()
        {
            ProgBar.Visibility = Visibility.Visible;

            VM = new VideoDetailViewModel();

            //http://221.192.136.99:804/wpv1.5/news/videopagejson-a2-pm3-v1.6.2-vid29509.html
            string url = string.Format("{0}{1}/news/videopagejson-{2}-vid{3}.html", App.appUrl, App.versionStr, App.AppInfo, videoId);
            //string url = string.Format("{0}{1}/news/videopagejson-{2}-vid{3}.html", "http://221.192.136.99:804", "/wpv1.5", App.AppInfo, videoId);
            VM.LoadDataAysnc(url);
            VM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<VideoDetailModel>>((ss, ee) =>
            {
                if (ee.Error != null)
                {

                }
                else
                {
                    videoData = ee.Result;
                    this.DataContext = videoData;
                    videoPlayer.SetCover(videoData.PicUrl);
                    videoPlayer.SetSource(videoData.VideoAddress);

                    videoImage.DataContext = videoData.PicUrl;
                    playButton.Opacity = 1;

                    timePanel.Visibility = Visibility.Visible;
                    if (videoData.RelationVideoList != null && videoData.RelationVideoList.Count() > 0)
                    {
                        relationVideoTitle.Visibility = Visibility.Visible;
                    }
                }
                ProgBar.Visibility = Visibility.Collapsed;
            });

        }

        #endregion

        // 查看评论
        private void checkComment_Click(object sender, EventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("VideoEndPageActivity", "视频评论页的访问量");
            this.NavigationService.Navigate(new Uri("/View/Channel/News/NewsCommentListPage.xaml?newsid=" + videoId + "&pageType=" + pageType, UriKind.Relative));
        }

        //刷新
        private void refreshButton_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //string newsPageUrl = CreateNewsViewUrl(pageIndex);
            //wb.Navigate(new Uri(newsPageUrl, UriKind.Absolute));
            ////wb.Navigate(new Uri(App.headUrl + "/news/news.aspx?newsid=" + newsId + "&pageIndex=" + pageIndex, UriKind.Absolute));
            //refreshButton.Visibility = Visibility.Collapsed;
            //wb.Visibility = Visibility.Visible;
            //prompt.Visibility = Visibility.Collapsed;
        }

        private void videoPlayer_FullScreen(object sender, bool e)
        {

        }

        private void videoImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            WebBrowserTask webBrowserTask = new WebBrowserTask();
            //http://221.192.136.99:804/wpv1.5/content/news/wpvideopage-a2-pm3-v1.6.2-vidXNzkyMzczMjI4.html
            string url = string.Format("{0}{1}/content/news/wpvideopage-{2}-vid{3}.html", App.newsPageDomain, App.versionStr, App.AppInfo, videoData.YoukuVideoKey);
            //string url = string.Format("http://player.youku.com/embed/{0}", videoData.YoukuVideoKey);

            webBrowserTask.Uri = new Uri(url, UriKind.RelativeOrAbsolute);
            webBrowserTask.Show();
        }

        private void relationVideo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            string vid = (sender as FrameworkElement).Tag.ToString();
            this.NavigationService.Navigate(new Uri("/View/Channel/News/VideoEndPage.xaml?videoid=" + vid, UriKind.Relative));
        }

    }
}