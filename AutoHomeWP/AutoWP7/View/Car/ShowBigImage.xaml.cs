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
    public partial class ShowBigImage : PhoneApplicationPage
    {
        double initialScaleX;
        double initialScaleY;
        //private Point point;
        double centerX;
        double centerY;

        public ShowBigImage()
        {
            InitializeComponent();
            _bigImgDataSoure = new ObservableCollection<ImageModel>();
        }

        //加载数据（车身，车厢，中控，其他）
        int type;
        //汽车id
        string carId = string.Empty;
        //加载类型（seriesid-车系  specid-车型）
        string loadType = string.Empty;
        //当前对象
        ImageModel currentModel = new ImageModel();
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {
                        //图片url
                        currentModel.SmallPic = this.NavigationContext.QueryString["urlSource"];
                        type = int.Parse(this.NavigationContext.QueryString["pictype"]);
                        int seriesid = 0;
                        int spectid = 0;
                        //车系
                        seriesid = Convert.ToInt32(this.NavigationContext.QueryString["seriesid"]);
                        spectid = Convert.ToInt32(this.NavigationContext.QueryString["spectid"]);
                        switch (type)
                        {
                            case 1: //获取车身图片               
                                LoadData(CreatePicUrl(seriesid, spectid, 1));
                                break;
                            case 10://获取中控图片
                                LoadData(CreatePicUrl(seriesid, spectid, 10));
                                break;
                            case 3://车厢图片
                                LoadData(CreatePicUrl(seriesid, spectid, 3));
                                break;
                            case 12: //其它图片
                                LoadData(CreatePicUrl(seriesid, spectid, 12));
                                break;
                        }
                        //读取图片

                    }
                    break;
                case System.Windows.Navigation.NavigationMode.Back:
                    break;
            }
        }

        private string CreatePicUrl(int seriesid, int specid, int type)
        {
            return string.Format(AppUrlMgr.CarPicsUrl, seriesid, specid, type, 0, 1, App.AllImgNum);
        }

        //数据源
        private ObservableCollection<ImageModel> _bigImgDataSoure;
        private ObservableCollection<ImageModel> BigImgDataSource
        {
            get
            {
                return _bigImgDataSoure;
            }
            set
            {
                if (value != _bigImgDataSoure)
                {
                    BigImgDataSource = value;
                }
            }
        }


        WebClient wc = null;
        /// <summary>
        ///获取车身图片url
        /// </summary>
        /// <param name="url"></param>
        public void LoadData(string urlSource)
        {
            if (wc == null)
            {
                wc = new WebClient();
            }
            wc.Encoding = new Gb2312Encoding();
            Uri url = new Uri(urlSource, UriKind.Absolute);
            wc.DownloadStringAsync(url);

            wc.Encoding = System.Text.Encoding.UTF8;
            wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
            {
                try
                {
                    //返回的json数据
                    JObject json = JObject.Parse(ee.Result);
                    JArray ImgData = (JArray)json.SelectToken("result").SelectToken("piclist");
                    ImageModel model = null;
                    for (int i = 0; i < ImgData.Count; i++)
                    {
                        model = new ImageModel();
                        model.ID = i + 1;
                        model.SpecName = (string)ImgData[i].SelectToken("specname");
                        model.SmallPic = (string)ImgData[i].SelectToken("smallpic");
                        BigImgDataSource.Add(model);
                    }
                    //获取当前图片
                    currentModel = GetModelInfo();
                    //加载当前图片
                    LoadImage(currentModel);
                }
                catch
                { }
            });
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


        //获取图片

        public ImageModel GetModelInfo()
        {
            for (int i = 0; i < BigImgDataSource.Count; i++)
            {
                if (currentModel.SmallPic == BigImgDataSource[i].SmallPic)
                {
                    return BigImgDataSource[i];
                }
            }
            return null;
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
            for (int i = 0; i < BigImgDataSource.Count; i++)
            {
                if (currentModel.SmallPic == BigImgDataSource[i].SmallPic)
                {
                    if (i == 0)
                    {
                        Common.showMsg("已经是第一张了");
                    }
                    else
                    {
                        if (BigImgDataSource[i - 1] != null)
                        {
                            //重设当前model
                            currentModel = BigImgDataSource[i - 1];
                            move_right.Begin();
                            LoadImage(BigImgDataSource[i - 1]);
                            return true;
                        }
                    }
                }

            }
            return false;
        }

        //加载后一张
        private bool HasNext()
        {
            for (int i = 0; i < BigImgDataSource.Count; i++)
            {
                if (currentModel.SmallPic == BigImgDataSource[i].SmallPic)
                {
                    if (i == BigImgDataSource.Count - 1)
                    {
                        Common.showMsg("已经是最后一张了");
                    }
                    else
                    {
                        if (BigImgDataSource[i + 1] != null)
                        {
                            currentModel = BigImgDataSource[i + 1];
                            move_left.Begin();
                            LoadImage(BigImgDataSource[i + 1]);
                            return true;
                        }

                    }
                }

            }

            return false;

        }

        //加载图片
        public void LoadImage(ImageModel model)
        {
            carName.Text = model.SpecName;
            pageIndex.Text = model.ID.ToString();
            pageSize.Text = App.AllImgNum.ToString();
            string url = string.Empty;
            url = model.SmallPic.Replace("s_", "w_");
            bigImg.Source = new StorageCachedImage(new Uri(url));
        }
    }
}