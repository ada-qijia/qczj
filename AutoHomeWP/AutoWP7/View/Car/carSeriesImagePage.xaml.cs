using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using ViewModels;

namespace AutoWP7.View.Car
{
    /// <summary>
    /// 车系 & 车型 图片列表页
    /// </summary>
    public partial class carSeriesImagePage : PhoneApplicationPage
    {
        public carSeriesImagePage()
        {
            InitializeComponent();
        }
        //车系id
        string carId = string.Empty;
        //页尺寸
        int loadPageSize = 15;
        //类型标识（1-车系  2-车型）
        int type;
        //加载数据类型（seriesid-车系  specid-车型）
        string loadType = string.Empty;
        //车身页码
        int loadPageFacadeIndex = 1;
        //中控页码
        int loadPageWheelIndex = 1;
        //车厢页码
        int loadPageComparmentIndex = 1;
        //其他页码
        int loadPageElseDetailIndex = 1;
        int seriesid = 0;
        int spectid = 0;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                //判断是车系图片列表请求还是车型图片列表请求
                type = int.Parse(NavigationContext.QueryString["type"]);
                if (type == 1)
                {
                    //车系
                    seriesid = Convert.ToInt32(NavigationContext.QueryString["carId"]);
                    spectid = 0;

                }
                else
                {
                    //车型
                    seriesid = 0;
                    spectid = Convert.ToInt32(NavigationContext.QueryString["carId"]);
                }
                piv.SelectedIndex = int.Parse(NavigationContext.QueryString["indexId"]);
            }

        }

        //用于存放服务器返回的数据的集合
        public ObservableCollection<ImageModel> imgFacadeDataSource = null;
        public ObservableCollection<ImageModel> imgWheelDataSoure = null;
        public ObservableCollection<ImageModel> imgComparmentDataSource = null;
        public ObservableCollection<ImageModel> imgElseDetailDataDSource = null;

        bool isfacadeLoaded = false;
        bool isCarComparmenteLoaded = false;
        bool isWheelLoaded = false;
        bool isElseDetailLoaded = false;
        private void piv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            switch (piv.SelectedIndex)
            {
                case 0: //车身外观
                    {
                        UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车身外观图片点击量");
                        if (!isfacadeLoaded)
                        {
                            imgFacadeDataSource = new ObservableCollection<ImageModel>();
                            facadeLoadData(1, loadPageSize);
                            isfacadeLoaded = true;
                        }
                    }
                    break;
                case 1: //中控方向盘
                    {
                        UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "中控方向盘图片点击量");
                        if (!isWheelLoaded)
                        {
                            imgWheelDataSoure = new ObservableCollection<ImageModel>();
                            WheelLoadData(1, loadPageSize);
                            isWheelLoaded = true;
                        }
                    }
                    break;
                case 2:  //车厢座椅
                    {
                        UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "车厢外观图片点击量");
                        if (!isCarComparmenteLoaded)
                        {
                            imgComparmentDataSource = new ObservableCollection<ImageModel>();
                            CarComparmentLoadData(1, loadPageSize);
                            isCarComparmenteLoaded = true;
                        }
                    }
                    break;
                case 3:  //其他细节
                    {
                        UmengSDK.UmengAnalytics.onEvent("SeriesActivity", "其他细节图片点击量");
                        if (!isElseDetailLoaded)
                        {
                            imgElseDetailDataDSource = new ObservableCollection<ImageModel>();
                            ElseDetailLoadData(1, loadPageSize);
                            isElseDetailLoaded = true;
                        }
                    }
                    break;
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            //清除缓存图片

            // Common.DeleteDirectory("CachedImages");
        }



        #region 车身外观数据加载

        ImageViewModel imgVM = null;
        public void facadeLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中......";
            GlobalIndicator.Instance.IsBusy = true;
            imgVM = new ImageViewModel();

            imgVM.LoadDataAysnc(CreatePicAppUrl(1, pageIndex, pageSize));
            imgVM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<ImageModel>>>(imgVM_LoadDataCompleted);

        }


        void imgVM_LoadDataCompleted(object sender, ViewModels.Handler.APIEventArgs<IEnumerable<ImageModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                if (e.Result.Count() <= 1 && loadPageFacadeIndex > 1)
                {
                    Common.showMsg("已经是最后一张了");
                }
                else
                {
                    if (e.Result.Count() < 1)
                    {
                        Common.showMsg("暂无图片");
                    }
                    else
                    {
                        foreach (ImageModel model in e.Result)
                        {
                            App.AllImgNum = model.Total;
                            imgFacadeDataSource.Add(model);
                        }
                        if (imgFacadeDataSource.Count >= (App.AllImgNum / 3))
                        {
                            imgFacadeDataSource.RemoveAt(imgFacadeDataSource.Count - 1);
                        }
                        facadeImgListbox.ItemsSource = imgFacadeDataSource;
                    }
                }
            }
        }

        private void loadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            imgFacadeDataSource.RemoveAt(imgFacadeDataSource.Count - 1);

            loadPageFacadeIndex++;
            facadeLoadData(loadPageFacadeIndex, loadPageSize);

        }

        #endregion

        #region 中控方向盘数据加载
        ImageViewModel wheelVM = null;
        //开始加载数据
        public void WheelLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中......";
            GlobalIndicator.Instance.IsBusy = true;
            wheelVM = new ImageViewModel();

            wheelVM.LoadDataAysnc(CreatePicAppUrl(10, pageIndex, pageSize));
            wheelVM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<ImageModel>>>(wheelVM_LoadDataCompleted);
        }
        //加载完成
        void wheelVM_LoadDataCompleted(object sender, ViewModels.Handler.APIEventArgs<IEnumerable<ImageModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            try
            {
                if (e.Error != null)
                {
                    Common.NetworkAvailablePrompt();
                }
                else
                {
                    if (e.Result.Count() <= 1 && loadPageFacadeIndex > 1)
                    {
                        Common.showMsg("已经是最后一张了");
                    }
                    else
                    {
                        if (e.Result.Count() < 1)
                        {
                            Common.showMsg("暂无图片");
                        }
                        else
                        {
                            foreach (ImageModel model in e.Result)
                            {
                                App.AllImgNum = model.Total;
                                imgWheelDataSoure.Add(model);
                            }
                            //如果没有多余的数据  不显示分页加载按钮
                            if (imgWheelDataSoure.Count >= (App.AllImgNum / 3))
                            {
                                imgWheelDataSoure.RemoveAt(imgWheelDataSoure.Count - 1);
                            }
                            wheelImgListbox.ItemsSource = imgWheelDataSoure;
                        }
                    }
                }
            }
            catch
            { }
        }
        //分页加载
        private void loadMoreWheel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            imgWheelDataSoure.RemoveAt(imgWheelDataSoure.Count - 1);
            loadPageWheelIndex++;
            WheelLoadData(loadPageWheelIndex, loadPageSize);
        }

        #endregion

        #region 车厢座椅数据加载
        ImageViewModel carComparmentVM = null;
        public void CarComparmentLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中......";
            GlobalIndicator.Instance.IsBusy = true;

            carComparmentVM = new ImageViewModel();

            carComparmentVM.LoadDataAysnc(CreatePicAppUrl(3, pageIndex, pageSize));
            carComparmentVM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<ImageModel>>>(carComparmentVM_LoadDataCompleted);
        }

        void carComparmentVM_LoadDataCompleted(object sender, ViewModels.Handler.APIEventArgs<IEnumerable<ImageModel>> e)
        {

            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            try
            {
                if (e.Error != null)
                {
                    Common.NetworkAvailablePrompt();
                }
                else
                {
                    if (loadPageComparmentIndex > 1 && e.Result.Count() <= 1)
                    {
                        Common.showMsg("已经是最后一张了");
                    }
                    else
                    {
                        if (e.Result.Count() < 1)
                        {
                            Common.showMsg("暂无图片");
                        }
                        else
                        {
                            foreach (ImageModel model in e.Result)
                            {
                                App.AllImgNum = model.Total;
                                imgComparmentDataSource.Add(model);
                            }
                            //如果没有下页数据  影藏其分页加载按钮
                            if (imgComparmentDataSource.Count >= (App.AllImgNum / 3))
                            {
                                imgComparmentDataSource.RemoveAt(imgComparmentDataSource.Count - 1);
                            }
                            carComparmentImgListbox.ItemsSource = imgComparmentDataSource;
                        }

                    }
                }
            }
            catch 
            {
            }
        }

        private void loadMoreComparment_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            imgComparmentDataSource.RemoveAt(imgComparmentDataSource.Count - 1);
            loadPageComparmentIndex++;
            CarComparmentLoadData(loadPageComparmentIndex, loadPageSize);
        }

        #endregion

        #region 其他细节数据加载
        ImageViewModel elsedetailVM = null;
        public void ElseDetailLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中......";
            GlobalIndicator.Instance.IsBusy = true;

            elsedetailVM = new ImageViewModel();

            elsedetailVM.LoadDataAysnc(CreatePicAppUrl(12, pageIndex, pageSize));
            elsedetailVM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<ImageModel>>>(elsedetailVM_LoadDataCompleted);
        }

        void elsedetailVM_LoadDataCompleted(object sender, ViewModels.Handler.APIEventArgs<IEnumerable<ImageModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            try
            {
                if (e.Error != null)
                {
                    Common.NetworkAvailablePrompt();
                }
                else
                {
                    if (e.Result.Count() <= 1 && loadPageElseDetailIndex > 1)
                    {
                        Common.showMsg("已经是最后一页了");
                    }
                    else
                    {

                        if (e.Result.Count() < 1)
                        {
                            Common.showMsg("暂无图片");
                        }
                        else
                        {
                            foreach (ImageModel model in e.Result)
                            {
                                App.AllImgNum = model.Total;
                                imgElseDetailDataDSource.Add(model);
                            }
                            //如果没有下页数据  不显示其分页加载按钮
                            if (imgElseDetailDataDSource.Count >= (App.AllImgNum / 3))
                            {
                                imgElseDetailDataDSource.RemoveAt(imgElseDetailDataDSource.Count - 1);
                            }
                            elseDetailImgListbox.ItemsSource = imgElseDetailDataDSource;
                        }
                    }
                }
            }
            catch
            { }
        }

        private void loadMoreElseDetail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            imgElseDetailDataDSource.RemoveAt(imgElseDetailDataDSource.Count - 1);
            loadPageElseDetailIndex++;
            ElseDetailLoadData(loadPageElseDetailIndex, loadPageSize);
        }

        #endregion


        //车身导航
        private void img1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image mm = (Image)sender;
            this.NavigationService.Navigate(CreateCarBigPicUrl(mm.Tag.ToString(), 1));
        }

        //中控导航
        private void img2_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image mm = (Image)sender;
            this.NavigationService.Navigate(CreateCarBigPicUrl(mm.Tag.ToString(), 10));
        }

        //车厢导航
        private void img3_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image mm = (Image)sender;

            this.NavigationService.Navigate(CreateCarBigPicUrl(mm.Tag.ToString(), 3));
        }

        //其他导航
        private void img4_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Image mm = (Image)sender;
            this.NavigationService.Navigate(CreateCarBigPicUrl(mm.Tag.ToString(), 12));
        }
        /// <summary>
        /// 车型图片大图
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pictype"></param>
        /// <returns></returns>
        private Uri CreateCarBigPicUrl(string url, int pictype)
        {
            return new Uri(string.Format("/View/Car/ShowBigImage.xaml?urlSource={0}&seriesid={1}&spectid={2}&pictype={3}", url, seriesid, spectid, pictype), UriKind.Relative);
        }
        /// <summary>
        /// 车系车型图片接口url
        /// </summary>
        /// <param name="picType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private string CreatePicAppUrl(int picType, int pageIndex, int pageSize)
        {
            return string.Format(AppUrlMgr.CarPicsUrl, seriesid, spectid, picType, 0, pageIndex, pageSize);
        }
    }
}