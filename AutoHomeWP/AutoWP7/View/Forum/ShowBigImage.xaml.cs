using System;
using Microsoft.Phone.Controls;
using ViewModels.Handler;
using MyPhoneControls;
using Microsoft.Xna.Framework;
using System.Windows;
using ViewModels;
using AutoWP7.Handler;
using Model;
using System.Collections.Generic;

namespace AutoWP7.View.Forum
{
    public partial class ShowBigImage : PhoneApplicationPage
    {

        double initialScaleX;
        double initialScaleY;
        double centerX;
        double centerY;
        public long Id = 0;
        public string imgUrl = string.Empty;
        public UseTypeEnum useType = UseTypeEnum.Club;
        public int curIndex = 0;
        public Dictionary<int, string> img = new Dictionary<int, string>();
        public bool isF = false;

        public ShowBigImage()
        {
            InitializeComponent();
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (this.NavigationContext.QueryString.ContainsKey("url"))
                imgUrl = this.NavigationContext.QueryString["url"];
            if (this.NavigationContext.QueryString.ContainsKey("id"))
                Id = Convert.ToInt64(this.NavigationContext.QueryString["id"]);
            if (this.NavigationContext.QueryString.ContainsKey("type"))
                useType = (UseTypeEnum)Convert.ToInt16(this.NavigationContext.QueryString["type"]);
            LoadImage(imgUrl);
        }

        public void LoadImage(string url)
        {
            if (Id > 0)
            {
                InitDate();
            }
        }

        ImageDuTuViewModel imageDuTuViewModel;
        public void InitDate()
        {
            GlobalIndicator.Instance.Text = "正在获取中......";
            GlobalIndicator.Instance.IsBusy = true;
            imageDuTuViewModel = new ImageDuTuViewModel();
            string url = string.Empty;
            if (useType == UseTypeEnum.Club)
            {
                url = string.Format("{0}{3}/forum/club/appdutu-a2-pm3-v{4}-t{1}-r0-h{2}.html", App.topicPageDomain, Id, imgUrl, App.versionStr, App.version);
            }
            else if (useType == UseTypeEnum.News)
            {
                url = string.Format("{0}{3}/content/news/newsimgall-a1-pm1-v{4}-n{1}-h{2}.html", App.newsPageDomain, Id, imgUrl, App.versionStr, App.version);
            }
            imageDuTuViewModel.LoadDataAysnc(url, (int)useType);
            imageDuTuViewModel.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<Model.ImageDuTuModel>>>(imageDuTuViewModel_LoadDataCompleted);
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
                catch
                { }
            }
            if (img.Count > 0)
            {
                if (img.ContainsValue(imgUrl))
                {
                    bigImg.Source = new StorageCachedImage(new Uri(imgUrl, UriKind.Absolute));
                }
                else
                {
                    bigImg.Source = new StorageCachedImage(new Uri(img[0], UriKind.Absolute));
                }
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
            if (this.transform.ScaleX != 1.0)
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

        private void GestureListener_Flick(object sender, MyPhoneControls.FlickGestureEventArgs e)
        {
            if (isF)
            {
                if (e.Angle > 135 && e.Angle < 225)  //向左增加图片 index++
                {
                    if (curIndex >= img.Count - 1)
                    {
                        MessageBox.Show("已经到最后一张了");
                    }
                    else
                    {
                        curIndex += 1;
                        bigImg.Source = new StorageCachedImage(new Uri(img[curIndex], UriKind.Absolute));
                    }
                }
                else if (e.Angle > 315 || e.Angle < 45)//向右增加图片 index--
                {
                    if (curIndex <= 0)
                    {
                        MessageBox.Show("已经到第一张了");
                    }
                    else
                    {
                        curIndex -= 1;
                        bigImg.Source = new StorageCachedImage(new Uri(img[curIndex], UriKind.Absolute));
                    }
                }
            }
            else
            { }
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