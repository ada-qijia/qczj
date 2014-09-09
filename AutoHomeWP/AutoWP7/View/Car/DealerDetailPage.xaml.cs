using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Tasks;
using Model;
using System;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ViewModels.Handler;

namespace AutoWP7.View.Car
{
    /// <summary>
    /// 经销商详情页
    /// </summary>
    public partial class DealerDetailPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public DealerDetailPage()
        {
          
            InitializeComponent();
          //  InitializeDefaults();
            DataContext = this;
        }

        //经销商id
        int id;
        //新建图层
        private MapLayer pushpinLayer;
        //图钉上显示的文本
        private string pushpinLabel;
        //图钉
        private Pushpin locationMark;
        //图层
        MapTileLayer customTileLayer;

        const string BingMapsId = "Ap6prRFv3pCWTulmksa8tWoJGvRx8Lm5-ouTPruqqyYaLq_iEk6pQiTD0zSCPxiw";
        //bingmap app key
        private readonly CredentialsProvider credentialsProvider = new ApplicationIdCredentialsProvider(BingMapsId);

        public CredentialsProvider CredentialsProvider
        {
            get { return credentialsProvider; }
        }



        void DealerDetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            NotifyPropertyChanged("Zoom");
            NotifyPropertyChanged("Center");
            pushpinLayer = new MapLayer();


            //将图层添加到地图中
            Map.Children.Add(pushpinLayer);
            //在图层中添加商家图钉信息
            pushpinLayer.AddChild(      //在图钉里绘制边框  在边框里绘制文本
                new Pushpin() { Background = new SolidColorBrush(Colors.LightGray), Content = new Border() { Child = new TextBlock() { Text = pushpinLabel, Foreground = new SolidColorBrush(Colors.Black) }, Background = new SolidColorBrush(Colors.White) } },
                Center);

            //我的位置图钉信息
            locationMark = new Pushpin() { Background = new SolidColorBrush(Colors.LightGray), BorderThickness = new Thickness(1, 1, 1, 1), Content = new Border() { Child = new TextBlock() { Text = "我在这里", Foreground = new SolidColorBrush(Colors.Black) }, Background = new SolidColorBrush(Colors.White) } };
            pushpinLayer.AddChild(locationMark, new GeoCoordinate(0, 0));
        }

        
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            id = int.Parse(NavigationContext.QueryString["id"]);

            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "经销商详情点击量");
                LoadData();
                this.Loaded += new RoutedEventHandler(DealerDetailPage_Loaded);
            }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            
        }


        /// <summary>
        /// 加载经销商详情信息
        /// </summary>
        public void LoadData()
        {
            using (LocalDataContext ldc = new LocalDataContext())
            {
                //本地查询经销商详细信息
                var queryResult = from d in ldc.dealerModels where d.id == id select d;

                dealerDetailListbox.ItemsSource = queryResult;

                DealerModel mm = null;
                foreach (DealerModel model in queryResult)
                {
                    mm = model;
                }

                double latitude;
                double longitude;

                if (Double.TryParse(mm.MapLon, out latitude) && Double.TryParse(mm.MapXon, out longitude))
                {
                    //商家位置
                    Center = new GeoCoordinate(latitude, longitude);
                    //我的位置
                    Venue = new GeoCoordinate(latitude, longitude);
                    //缩放大小
                    Zoom = DefaultZoomLevel;
                }

                if (mm.name != null)
                {
                    pushpinLabel = mm.name;
                }
                else
                {
                    pushpinLabel = "";
                }

                customTileLayer = new MapTileLayer();

                GoogleTileSource gts = new GoogleTileSource();
                customTileLayer.TileSources.Add(gts);

                Map.ZoomLevel = Zoom;
                Map.Center = Center;
            }

        }


        public void AddPushpin(GeoCoordinate location)
        {
            Pushpin pushpin = new Pushpin();
            pushpin.Content = "Test";
            pushpinLayer.AddChild(pushpin, location);
        }

        ///<summary>
        /// Gets or sets the map zoom level. 缩放大小设置
        ///</summary>
        private const double DefaultZoomLevel = 10.0;
        private const double MaxZoomLevel = 21.0;
        private const double MinZoomLevel = 1.0;

        private double _zoom = DefaultZoomLevel;
        public double Zoom
        {
            get { return _zoom; }
            set
            {
                var coercedZoom = Math.Max(MinZoomLevel, Math.Min(MaxZoomLevel, value));
                if (_zoom != coercedZoom)
                {
                    _zoom = value;
                    NotifyPropertyChanged("Zoom");
                }
            }
        }

        /// <summary>
        /// 我的位置
        /// </summary>
        private GeoCoordinateWatcher loc = null;
        private void CurrentLocation_Click(object sender, EventArgs e)
        {
            //Zoom += 1;
            if (loc == null)
            {
                loc = new GeoCoordinateWatcher(GeoPositionAccuracy.Default);
                loc.StatusChanged += loc_StatusChanged;
            }
            if (loc.Status == GeoPositionStatus.Disabled)
            {
                loc.StatusChanged -= loc_StatusChanged;
                Common.showMsg("请开启GPS卫星定位服务");
                return;
            }
            loc.Start();
        }

        //状态改变
        void loc_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {

            if (e.Status == GeoPositionStatus.Ready)
            {
                locationMark.Location = loc.Position.Location;
                Map.SetView(loc.Position.Location, 10.0);
                loc.Stop();
            }
        }


        //定位商家位置
        private void VenueLocation_Click(object sender, EventArgs e)
        {
            Map.SetView(Venue, 10.0);
        }


        //商家位置
        private GeoCoordinate _center;
        public GeoCoordinate Center
        {
            get { return _center; }
            set
            {
                if (_center != value)
                {
                    _center = value;
                    NotifyPropertyChanged("Center");
                }
            }
        }

        private GeoCoordinate Venue;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool HasDirections
        {
            get
            {
                // TODO : Return true only if has directions.

                return true;
            }
        }

        #region Tasks

        private void InitializeDefaults()
        {
            // TODO : Initialize default properties.
        }

        private void ChangeMapMode()
        {
            // TODO : Change map view mode.
        }

        private void IncreaseZoomLevel()
        {
            // TODO : Increases zoom level.
        }

        private void DecreaseZoomLevel()
        {
            // TODO : Decreases zoom level.
        }

        private void CenterLocation()
        {
            // TODO : Center current location.
        }

        private void CenterPushpinsPopup(Point touchPoint)
        {
            // TODO : Center pushpins popup to the touch point.
        }

        private void CreateNewPushpin(object selectedItem, Point point)
        {
            // TODO : Create a new pushpin.
        }

        private void CalculateRoute()
        {
            // TODO : Calculate a route.
        }

        #endregion

        /// <summary>
        /// 拨打电话
        /// </summary>
        private void callDealer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image bb = (Image)sender;
            PhoneCallTask phoneCall = new PhoneCallTask();
            phoneCall.PhoneNumber = bb.Tag.ToString();
            phoneCall.Show();
        }
    }
}