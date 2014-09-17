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

namespace AutoWP7.View.Channel.News
{
    public partial class VideoEndPage : PhoneApplicationPage
    {
        public VideoEndPage()
        {
            InitializeComponent();
        }

        string videoId = string.Empty;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            videoId = NavigationContext.QueryString["videoid"];
            LoadData();

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
        private void LoadData()
        {
            VM = new VideoDetailViewModel();

            //http://221.192.136.99:804/wpv1.5/news/videopagejson-a2-pm3-v1.5.0-vid29509.html
            //string url = string.Format("{0}{1}/news/videopagejson-{2}-vid{3}.html", App.appUrl, App.versionStr, App.AppInfo, videoId);
            string url = string.Format("{0}{1}/news/videopagejson-{2}-vid{3}.html", "http://221.192.136.99:804", "/wpv1.5", App.AppInfo, videoId);
            VM.LoadDataAysnc(url);
            VM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<VideoDetailModel>>((ss, ee) =>
            {
                if (ee.Error != null)
                {

                }
                else
                {
                    var videoData = ee.Result;
                    this.DataContext = videoData;
                }

            });

        }

        #endregion

        // 查看评论
        private void checkComment_Click(object sender, EventArgs e)
        {
            //UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "评论页的访问量");

            //for (int i = 0; i < buttonCount; i++)
            //{
            //    App.barStatus[i] = (this.ApplicationBar.Buttons[i] as IApplicationBarIconButton).IsEnabled;

            //}
            //(App.Current as App).newsPartPageMessage.Hide();
            //this.NavigationService.Navigate(new Uri("/View/Channel/News/NewsCommentListPage.xaml?newsid=" + newsId + "&pageType=" + pageType, UriKind.Relative));

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

    }
}