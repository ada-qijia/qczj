using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using MyPhoneControls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ViewModels.Handler;
using ViewModels;
using Model;

namespace AutoWP7.View.Channel.Newest
{
    public partial class ShowBigImage : PhoneApplicationPage
    {
        public ShowBigImage()
        {
            InitializeComponent();

            //Touch.FrameReported += Touch_FrameReported;
        }


        double initialAngle;
        double initialScaleX;
        double initialScaleY;
        private Point point;
        double centerX;
        double centerY;

        //void Touch_FrameReported(object sender, TouchFrameEventArgs e)
        //{
        //     // 判是否以上的Touch
        //    if (e.GetTouchPoints(this.LayoutRoot).Count() > 1)
        //    {
        //        // Touch事件是Down的候，Touch的初始距
        //        if (e.GetTouchPoints(this.LayoutRoot)[0].Action == TouchAction.Down || e.GetTouchPoints(this.LayoutRoot)[1].Action == TouchAction.Down)
        //        {
        //            initialAngle = CalLength(e.GetTouchPoints(this.LayoutRoot)[0].Position, e.GetTouchPoints(this.LayoutRoot)[1].Position);
        //            initialScaleX = transform.ScaleX;
        //            initialScaleY = transform.ScaleY;
        //        }

        //        // 依照Touch倍率改片的大小
        //        if (initialAngle > 0)
        //        {
        //            transform.ScaleX = initialScaleX * CalLength(e.GetTouchPoints(this.LayoutRoot)[0].Position, e.GetTouchPoints(this.LayoutRoot)[1].Position) / initialAngle;
        //            transform.ScaleY = initialScaleY * CalLength(e.GetTouchPoints(this.LayoutRoot)[0].Position, e.GetTouchPoints(this.LayoutRoot)[1].Position) / initialAngle;
        //        }
        //    }
        //}

        string Id = string.Empty;
        string imgUrl = string.Empty;
        public UseTypeEnum useType = UseTypeEnum.News;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (this.NavigationContext.QueryString.ContainsKey("type"))
                useType = (UseTypeEnum)Convert.ToInt16(this.NavigationContext.QueryString["type"]);
            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {
                        if (useType == UseTypeEnum.News)
                            UmengSDK.UmengAnalytics.onEvent("ArticleEndPageActivity", "文章最终页大图点击量");
                        Id = NavigationContext.QueryString["id"];
                        imgUrl = NavigationContext.QueryString["url"];
                        LoadAllImage();
                    }
                    break;
            }

        }

        bool isF = false;
        int curIndex = 0;
        Dictionary<int, string> img = new Dictionary<int, string>();
        ImageDuTuViewModel imageDuTuViewModel;
        public void LoadAllImage()
        {
            GlobalIndicator.Instance.Text = "正在获取中......";
            GlobalIndicator.Instance.IsBusy = true;
            imageDuTuViewModel = new ImageDuTuViewModel();
            imageDuTuViewModel.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<Model.ImageDuTuModel>>>(imageDuTuViewModel_LoadDataCompleted);

            string url = string.Empty;
            if (useType == UseTypeEnum.Club)
            {
                url = string.Format("{0}{3}/forum/club/appdutu-a2-pm3-v1.6.2-t{1}-r0-h{2}.html", App.topicPageDomain, Id, imgUrl, App.versionStr);
            }
            else if (useType == UseTypeEnum.News)
            {
                url = string.Format("{0}{3}/content/news/newsimgall-a1-pm1-v1.6.2-n{1}-h{2}.html", App.newsPageDomain, Id, imgUrl, App.versionStr);
            }
            imageDuTuViewModel.LoadDataAysnc(url, (int)useType);
        }

        void imageDuTuViewModel_LoadDataCompleted(object sender, ViewModels.Handler.APIEventArgs<IEnumerable<Model.ImageDuTuModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                try
                {
                    foreach (ImageDuTuModel model in e.Result)
                    {
                        if (model.Url == imgUrl)
                            curIndex = model.Index;
                        img.Add(model.Index, model.Url);
                    }
                    isF = true;
                }
                catch (Exception ex)
                {

                }
            }
            if (img.Count > 0)
            {
                if (img.ContainsValue(imgUrl))
                {
                    //显示点击的图片
                    LoadImage(imgUrl);
                }
                else
                {
                    bigImg.Source = new StorageCachedImage(new Uri(img[0], UriKind.Absolute));
                }
            }
        }
        //图片数组
        //string[] imgArr;
        //int curIndex = 0;
        //WebClient wc = null;
        //public void LoadAllImage(string newsId, string imgUrl)
        //{
        //    if (wc == null)
        //    {
        //        wc = new WebClient();
        //    }
        //    string url = string.Format("{0}/autoV4.0/news/newsimgall-a1-pm3-V4.0.0-n{1}-h{2}.html", App.appUrl, newsId, imgUrl);

        //    Uri urlSource = new Uri(url, UriKind.Absolute);
        //    wc.DownloadStringAsync(urlSource);
        //    wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
        //    {
        //        try
        //        {
        //            if (ee.Result != null)
        //            {
        //                //返回的json数据
        //                JObject json = JObject.Parse(ee.Result);

        //                int total = 0;
        //                if (json != null)
        //                {
        //                    total = (int)json.SelectToken("result").SelectToken("rowcount");
        //                    curIndex = (int)json.SelectToken("result").SelectToken("index");

        //                    imgArr = new string[total];
        //                    JArray imgJson = (JArray)json.SelectToken("result").SelectToken("list");

        //                    for (int i = 0; i < imgJson.Count; i++)
        //                    {
        //                        imgArr[i] = (string)imgJson[i].SelectToken("img");
        //                    }
        //                }
        //            }

        //            //显示点击的图片
        //            LoadImage(currentImgUrl);
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    });
        //}

        private void GestureListener_Flick(object sender, MyPhoneControls.FlickGestureEventArgs e)
        {
            if (transform.ScaleX <= 1 || transform.ScaleY <= 1)
            {
                //后
                if (e.HorizontalVelocity < 0)
                {
                    HasNext();

                }
                //前
                if (e.HorizontalVelocity > 0)
                {
                    HasPrev();
                }
            }
        }

        // 加载前一张
        private bool HasPrev()
        {
            if (img.Count > 0)
            {
                if (curIndex <= 0)
                {
                    Common.showMsg("已经是第一张了");
                }
                else
                {
                    curIndex -= 1;
                    imgUrl = img[curIndex];
                    move_right.Begin();
                    LoadImage(img[curIndex]);
                    return true;
                }
            }
            return false;
        }

        //加载后一张
        private bool HasNext()
        {
            if (img.Count > 0)
            {
                if (curIndex >= img.Count - 1)
                {
                    Common.showMsg("已经是最后一张了");
                }
                else
                {
                    curIndex += 1;
                    imgUrl = img[curIndex];
                    move_right.Begin();
                    LoadImage(imgUrl);
                    return true;
                }
            }

            return false;

        }

        // 加载图片
        public void LoadImage(string urlSource)
        {
            if (urlSource != null)
            {   // fadeOut.Begin();
                bigImg.Source = new StorageCachedImage(new Uri(urlSource));
                // fadeIn.Begin();
            }
        }


        private void OnDragStarted(object sender, MyPhoneControls.DragStartedGestureEventArgs e)
        {
            //border.Background = greenBrush;
        }

        private void OnDragDelta(object sender, MyPhoneControls.DragDeltaGestureEventArgs e)
        {

            //图片放大的情况下才可拖拽
            if (transform.ScaleX > 1 || transform.ScaleY > 1)
            {
                transform.TranslateX += e.HorizontalChange;
                transform.TranslateY += e.VerticalChange;
            }
        }

        private void OnDragCompleted(object sender, MyPhoneControls.DragCompletedGestureEventArgs e)
        {
            if (this.transform.TranslateX <= -300)
            {
                reset();
            }

        }

        private void reset()
        {
            if (transform.ScaleX != 1.0)
            {
                this.rest_s_x.To = 1.0;
                this.rest_s_y.To = 1.0;
                this.rest_s.Begin();
                this.transform.ScaleX = 1.0;
                this.transform.ScaleY = 1.0;
            }
            else if (this.transform.ScaleX > 3.0)
            {

                this.rest_s_x.To = 3.0;
                this.rest_s_y.To = 3.0;
                this.rest_s.Begin();
                this.transform.ScaleX = 3.0;
                this.transform.ScaleY = 3.0;
            }

            double num = (this.transform.ScaleX - 1.0) * 240.0;
            double num2 = (this.transform.ScaleX - 1.0) * 400.0;
            this.rest_x_t.To = this.transform.TranslateX;
            double num3 = this.transform.TranslateX - num;
            if (num3 > 0.0)
            {
                this.rest_x_t.To = num;
            }
            double num4 = this.transform.TranslateX + num;
            if (num4 < 0.0)
            {
                this.rest_x_t.To = -num;
            }
            this.rest_y_t.To = transform.TranslateY;
            double num5 = this.transform.TranslateY - num2;
            if (num5 > 0.0)
            {
                this.rest_y_t.To = num2;
            }
            double num6 = this.transform.TranslateY + num2;
            if (num6 < 0.0)
            {
                this.rest_y_t.To = -num2;
            }
            if (((num3 > 0.0) || (num4 < 0.0)) || ((num5 > 0.0) || (num6 < 0.0)))
            {
                this.rest_t.Begin();
                this.transform.TranslateX = num;
                this.transform.TranslateY = num2;
            }
        }

        private void OnPinchStarted(object sender, MyPhoneControls.PinchStartedGestureEventArgs e)
        {
            //border.Background = redBrush;

            // initialAngle = transform.Rotation;
            //transform.p

            //设置图片缩放的基准点
            bigImg.RenderTransformOrigin = new Point(0.5, 0.5);

            initialScaleX = transform.ScaleX;
            initialScaleY = transform.ScaleY;
            centerX = transform.CenterX;
            centerY = transform.CenterY;
        }

        private void OnPinchDelta(object sender, MyPhoneControls.PinchGestureEventArgs e)
        {

            //  transform.Rotation = initialAngle + e.TotalAngleDelta;
            transform.ScaleX = initialScaleX * e.DistanceRatio;
            transform.ScaleY = initialScaleY * e.DistanceRatio;

            transform.CenterX = centerX;
            transform.CenterY = centerY;
        }

        private void OnPinchCompleted(object sender, MyPhoneControls.PinchGestureEventArgs e)
        {
            // _image.Background = normalBrush;

            if (transform.ScaleX < 1 || transform.ScaleY < 1)
            {
                backLocation.Begin();
            }
        }


    }
    /// <summary>
    /// 此读图默认被调用的业务
    /// </summary>
    public enum UseTypeEnum
    {
        Default = 0,
        News = 1,
        Club = 2
    }
}