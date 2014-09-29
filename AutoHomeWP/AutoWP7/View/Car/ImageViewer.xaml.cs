using System;
using System.Collections.ObjectModel;
using System.Net;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Model;
using Newtonsoft.Json.Linq;
using ViewModels.Handler;
using MyPhoneControls;
using System.Windows;

namespace AutoWP7.View.Car
{
    /// <summary>
    /// 显示汽车大图
    /// </summary>
    public partial class ImageViewer : PhoneApplicationPage
    {
        string pageTitle = string.Empty;
        int imageIndex = 0;

        double initialScaleX;
        double initialScaleY;
        private Point point;
        double centerX;
        double centerY;

        public ImageViewer()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            imageIndex = int.Parse(this.NavigationContext.QueryString["imageIndex"]);

            pageTitle = this.NavigationContext.QueryString["pageTitle"];
            carName.Text = pageTitle;

            pageSize.Text = App.BigImageList.Count.ToString();

            LoadImage();
        }

        private void GestureListener_Flick(object sender, MyPhoneControls.FlickGestureEventArgs e)
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


        // 加载前一张
        private bool HasPrev()
        {
            if (imageIndex == 0)
            {
                Common.showMsg("已经是第一张了");
            }
            else
            {
                imageIndex--;
                LoadImage();
                return true;
            }
            return false;
        }

        //加载后一张
        private bool HasNext()
        {
            if (imageIndex >= (App.BigImageList.Count - 1))
            {
                Common.showMsg("已经是最后一张了");
            }
            else
            {
                imageIndex++;
                LoadImage();
                return true;
            }
            return false;
        }

        //加载图片
        public void LoadImage()
        {
            pageIndex.Text = (imageIndex + 1).ToString();
            string url = App.BigImageList[imageIndex];
            bigImg.DataContext = url;
        }
    }
}