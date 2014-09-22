using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Model;
using ViewModels;
using ViewModels.Handler;
using System.Windows;

namespace AutoWP7.View.Channel
{
    public partial class NewsListPage : PhoneApplicationPage
    {
        #region Property

        //页容量
        int loadPageSize = 20;
        int pageType = 1;

        //标示是否已加载数据
        bool isNewsLoaded = false;
        bool isVideoLoaded = false;
        bool isEvalLoaded = false;
        bool isQutoLoaded = false;
        bool isShoopingLoaded = false;
        bool isUsecarLoaded = false;
        bool isCultureLoaded = false;
        bool isChangeLoaded = false;
        bool isCompetionLoaded = false;
        bool isShuokeLoaded = false;
        bool isTravelsLoaded = false;
        bool isTechnologyLoaded = false;

        bool isFilterShown = false;

        #endregion

        #region Lifecycle

        public NewsListPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {
                        this.piv.SelectedIndex = int.Parse(NavigationContext.QueryString["index"]);
                    }
                    break;
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (isFilterShown)
            {
                e.Cancel = true;
                HideVideoFilter();
                return;
            }
            base.OnBackKeyPress(e);
        }

        #endregion

        #region Pivot

        private void piv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (piv.SelectedIndex)
            {

                case 0: //新闻
                    {
                        UmengSDK.UmengAnalytics.onEvent("ArticleActivity", "新闻点击量");
                        pageType = 1;
                        if (isNewsLoaded == false)
                        {
                            NewsLoadData(1, loadPageSize);
                            isNewsLoaded = true;

                        }
                    }
                    break;
                case 1: //视频
                    {
                        UmengSDK.UmengAnalytics.onEvent("ArticleActivity", "视频点击量");
                        pageType = 1;
                        if (isVideoLoaded == false)
                        {
                            VideoLoadData(1, loadPageSize);
                            isVideoLoaded = true;

                        }
                    }
                    break;
                case 2: //评测
                    {
                        UmengSDK.UmengAnalytics.onEvent("ArticleActivity", "评测点击量");
                        pageType = 1;
                        if (isEvalLoaded == false)
                        {
                            EvaluatingsLoadData(1, loadPageSize);
                            isEvalLoaded = true;
                        }
                    }
                    break;
                case 3: //行情
                    {
                        UmengSDK.UmengAnalytics.onEvent("ArticleActivity", "行情点击量");
                        pageType = 1;
                        if (isQutoLoaded == false)
                        {
                            if (string.IsNullOrEmpty(App.CityId))//默认城市加载
                            {
                                QutotationsLoadData(1, loadPageSize, "110100");
                            }
                            else//用户选择的城市加载
                            {
                                QutotationsLoadData(1, loadPageSize, App.CityId);
                            }

                            isQutoLoaded = true;
                        }
                    }
                    break;
                case 4: //导购
                    {
                        UmengSDK.UmengAnalytics.onEvent("ArticleActivity", "导购点击量");
                        pageType = 1;
                        if (isShoopingLoaded == false)
                        {
                            ShoppingLoadData(1, loadPageSize);
                            isShoopingLoaded = true;
                        }
                    }
                    break;
                case 5:  //用车
                    {
                        UmengSDK.UmengAnalytics.onEvent("ArticleActivity", "用车点击量");
                        pageType = 1;
                        if (isUsecarLoaded == false)
                        {
                            UseCarLoadData(1, loadPageSize);
                            isUsecarLoaded = true;
                        }
                    }
                    break;
                case 6:  //文化
                    {
                        UmengSDK.UmengAnalytics.onEvent("ArticleActivity", "文化点击量");
                        pageType = 1;
                        if (isCultureLoaded == false)
                        {
                            CultureLoadData(1, loadPageSize);
                            isCultureLoaded = true;
                        }
                    }
                    break;
                case 7: //改装
                    {
                        UmengSDK.UmengAnalytics.onEvent("ArticleActivity", "改装点击量");
                        pageType = 1;
                        if (isChangeLoaded == false)
                        {
                            ChangeLoadData(1, loadPageSize);
                            isChangeLoaded = true;
                        }
                    }
                    break;
                case 8: //说客
                    {
                        UmengSDK.UmengAnalytics.onEvent("ArticleActivity", "说客点击量");
                        pageType = 2;
                        if (isCompetionLoaded == false)
                        {
                            ShuokeLoadData(1, loadPageSize);
                            isShuokeLoaded = true;
                        }
                    }
                    break;
                case 9: //游记
                    {
                        UmengSDK.UmengAnalytics.onEvent("ArticleActivity", "游记点击量");
                        pageType = 1;
                        if (isTravelsLoaded == false)
                        {
                            TravelsLoadData(1, loadPageSize);
                            isTravelsLoaded = true;
                        }
                    }
                    break;
                case 10: //技术
                    {
                        UmengSDK.UmengAnalytics.onEvent("ArticleActivity", "技术点击量");
                        pageType = 1;
                        if (isTechnologyLoaded == false)
                        {
                            TechnologyLoadData(1, loadPageSize);
                            isTechnologyLoaded = true;
                        }
                    }
                    break;
            }
        }

        #endregion

        #region 新闻

        public ObservableCollection<NewsModel> newsDataSource = null;
        NewsViewModel comm = null;
        int newspageIndex = 1; //页码
        /// <summary>
        /// 新闻 - 第一次加载
        /// </summary>
        public void NewsLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            if (comm == null)
            {
                comm = new NewsViewModel();
            }
            string url = string.Format(AppUrlMgr.NewsListUrl, 0, 1, pageIndex, pageSize, 0);
            comm.LoadDataAysnc(url);
            comm.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<NewsModel>>>(comm_LoadDataCompleted);
        }

        /// <summary>
        /// 新闻-分页加载
        /// </summary>
        private void moreStack_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //清楚集合中的更多按钮
            newsDataSource.RemoveAt(newsDataSource.Count - 1);
            newspageIndex++;
            NewsLoadData(newspageIndex, loadPageSize);
        }

        /// <summary>
        /// 新闻—完成
        /// </summary>
        void comm_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<NewsModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                if (e.Result.Count() <= 1 && newspageIndex > 1)
                {
                    Common.showMsg("已经是最后一页了");
                }
                else
                {
                    //数据绑定到前台
                    newsDataSource = (ObservableCollection<NewsModel>)e.Result;
                    newsListbox.ItemsSource = newsDataSource;
                }
            }
        }
        #endregion

        #region 视频

        ObservableCollection<NewsModel> videoDataSource = null;
        NewsViewModel videoComm = null;
        int videoPageIndex = 1;
        string videoType = "0";//all

        public void VideoLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            if (videoComm == null)
            {
                videoComm = new NewsViewModel();
                videoComm.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<NewsModel>>>(videoComm_LoadDataCompleted);
            }

            //http://app.api.autohome.com.cn/wpv1.4/news/videos-a2-pm3-v1.5.0-vt0-p1-s20.html
            string format = App.appUrl + App.versionStr + "/news/videos-" + App.AppInfo + "-vt{0}-p{1}-s{2}.html";
            string url = string.Format(format, videoType, pageIndex, pageSize);
            videoComm.LoadDataAysnc(url, 3);
        }

        void videoComm_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<NewsModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                if (e.Result.Count() <= 1 && videoPageIndex > 1)
                {
                    Common.showMsg("已经是最后一页了");
                }
                else
                {
                    videoDataSource = (ObservableCollection<NewsModel>)e.Result;
                    videoListbox.ItemsSource = videoDataSource;
                }
            }
        }

        private void videoLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            videoDataSource.RemoveAt(videoDataSource.Count - 1);
            videoPageIndex++;
            VideoLoadData(videoPageIndex, loadPageSize);
        }

        private void videoFilterButton_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowVideoFilter();
        }

        protected void ShowVideoFilter()
        {
            VisualStateManager.GoToState(this, "VSVideoFilterShown", true);
            isFilterShown = true;
        }
        protected void HideVideoFilter()
        {
            VisualStateManager.GoToState(this, "VSVideoFilterHidden", true);
            isFilterShown = false;
        }

        private void videoFilter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string tag = ((FrameworkElement)sender).Tag.ToString();
            videoType = tag;

            //reload data
            videoPageIndex = 1;
            videoComm.newestDataSource.Clear();
            VideoLoadData(videoPageIndex, loadPageSize);

            //change filter title
            videoFilterButton.Content = ((Button)sender).Content;

            HideVideoFilter();
        }

        #endregion

        #region 评测

        //评测集合
        public ObservableCollection<NewsModel> evalDataSource = null;
        NewsViewModel evalComm = null;
        int evalpageIndex = 1; //页码
        /// <summary>
        /// 评测- 加载数据
        /// </summary>
        public void EvaluatingsLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            if (evalComm == null)
            {
                evalComm = new NewsViewModel();
            }
            string url = CreateNewsListUrl(0, 3, pageIndex, pageSize);

            evalComm.LoadDataAysnc(url);
            evalComm.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<NewsModel>>>(evalComm_LoadDataCompleted);
        }

        /// <summary>
        ///评测—加载完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void evalComm_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<NewsModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                if (e.Result.Count() <= 1 && evalpageIndex > 1)
                {
                    Common.showMsg("已经是最后一页了");
                }
                else
                {
                    //数据绑定到前台
                    evalDataSource = (ObservableCollection<NewsModel>)e.Result;
                    evaluatingListbox.ItemsSource = evalDataSource;
                }
            }

        }

        /// <summary>
        /// 评测-分页加载
        /// </summary>
        private void btnLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //清楚加载更多按钮
            evalDataSource.RemoveAt(evalDataSource.Count - 1);
            evalpageIndex++;
            EvaluatingsLoadData(evalpageIndex, loadPageSize);
        }
        #endregion

        #region 行情

        public ObservableCollection<NewsModel> qutoDataSource = null;
        NewsViewModel qutoComm = null;
        int qutopageIndex = 1; //页码
        /// <summary>
        /// 行情- 加载数据
        /// </summary>
        public void QutotationsLoadData(int pageIndex, int pageSize, string cityId)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            if (qutoComm == null)
            {
                qutoComm = new NewsViewModel();
            }
            string url = CreateNewsListUrl(Convert.ToInt32(cityId), 2, pageIndex, pageSize);

            qutoComm.LoadDataAysnc(url);
            qutoComm.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<NewsModel>>>(qutoComm_LoadDataCompleted);
        }

        /// <summary>
        ///行情—加载完成
        /// </summary>
        void qutoComm_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<NewsModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                if (e.Result.Count() <= 1 && qutopageIndex > 1)
                {
                    Common.showMsg("已经是最后一页了");
                }
                else
                {
                    //数据绑定到前台
                    //foreach (NewsModel model in e.Result)
                    //{
                    //    qutoDataSource.Add(model);
                    //}
                    qutoDataSource = (ObservableCollection<NewsModel>)e.Result;
                    quotationsListBox.ItemsSource = qutoDataSource;
                }
            }

        }

        /// <summary>
        /// 行情-分页加载
        /// </summary>
        private void btnQutoLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            qutoDataSource.RemoveAt(qutoDataSource.Count - 1);
            qutopageIndex++;

            if (string.IsNullOrEmpty(App.CityId))//默认城市加载
            {
                QutotationsLoadData(qutopageIndex, loadPageSize, "110100");
            }
            else//用户选择的城市加载
            {
                QutotationsLoadData(qutopageIndex, loadPageSize, App.CityId);
            }
        }
        #endregion

        #region 导购

        public ObservableCollection<NewsModel> shoppingDataSource = new ObservableCollection<NewsModel>();
        NewsViewModel shoppingComm = null;
        int shoppingPageIndex = 1; //页码
        /// <summary>
        /// 导购- 加载数据
        /// </summary>
        public void ShoppingLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            shoppingComm = new NewsViewModel();
            string url = CreateNewsListUrl(0, 60, pageIndex, pageSize);

            shoppingComm.LoadDataAysnc(url);
            shoppingComm.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<NewsModel>>>(shoppingComm_LoadDataCompleted);
        }

        /// <summary>
        ///导购—加载完成
        /// </summary>
        void shoppingComm_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<NewsModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }

            else
            {
                if (e.Result.Count() <= 1 && shoppingPageIndex > 1)
                {
                    Common.showMsg("已经是最后一页了");
                }
                else
                {
                    int total = 0;
                    //数据绑定到前台
                    foreach (NewsModel model in e.Result)
                    {
                        shoppingDataSource.Add(model);
                        total = model.Total;
                    }
                    if (shoppingDataSource.Count() - 1 == total)
                    {
                        shoppingDataSource.RemoveAt(shoppingDataSource.Count - 1);
                    }
                    shoppingGuidListBox.ItemsSource = shoppingDataSource;
                }
            }

        }

        /// <summary>
        /// 导购-分页加载
        /// </summary> 
        private void shoppingLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            shoppingDataSource.RemoveAt(shoppingDataSource.Count - 1);
            shoppingPageIndex++;
            ShoppingLoadData(shoppingPageIndex, loadPageSize);
        }

        #endregion

        #region 用车

        public ObservableCollection<NewsModel> useCarDataSource = null;
        NewsViewModel useCarComm = null;
        int useCarPageIndex = 1; //页码
        /// <summary>
        /// 用车- 加载数据
        /// </summary>
        public void UseCarLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;
            if (useCarComm == null)
            {
                useCarComm = new NewsViewModel();
            }
            string url = CreateNewsListUrl(0, 82, pageIndex, pageSize);

            useCarComm.LoadDataAysnc(url);
            useCarComm.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<NewsModel>>>(useCarComm_LoadDataCompleted);
        }

        /// <summary>
        ///用车—加载完成
        /// </summary>
        void useCarComm_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<NewsModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                if (e.Result.Count() <= 1 && useCarPageIndex > 1)
                {
                    Common.showMsg("已经是最后一页了");
                }
                else
                {
                    //数据绑定到前台
                    useCarDataSource = (ObservableCollection<NewsModel>)e.Result;
                    userCarListBox.ItemsSource = useCarDataSource;
                }
            }

        }
        /// <summary>
        /// 用车-分页加载
        /// </summary> 
        private void useCarLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            useCarDataSource.RemoveAt(useCarDataSource.Count - 1);
            useCarPageIndex++;
            UseCarLoadData(useCarPageIndex, loadPageSize);
        }
        #endregion

        #region 文化

        public ObservableCollection<NewsModel> acticleDataSource = null;
        NewsViewModel cultureComm = null;
        int culturePageIndex = 1; //页码
        /// <summary>
        /// 文化- 加载数据
        /// </summary>
        public void CultureLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;
            if (cultureComm == null)
            {
                cultureComm = new NewsViewModel();
            }
            string url = CreateNewsListUrl(0, 97, pageIndex, pageSize);

            cultureComm.LoadDataAysnc(url);
            cultureComm.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<NewsModel>>>(cultureComm_LoadDataCompleted);
        }

        /// <summary>
        ///文化—加载完成
        /// </summary>
        void cultureComm_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<NewsModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                if (e.Result.Count() <= 1 && culturePageIndex > 1)
                {
                    Common.showMsg("已经是最后一页了");
                }
                else
                {
                    //数据绑定到前台
                    acticleDataSource = (ObservableCollection<NewsModel>)e.Result;
                    cultureListBox.ItemsSource = acticleDataSource;
                }
            }

        }
        /// <summary>
        /// 文化-分页加载
        /// </summary>     
        private void cultureLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            acticleDataSource.RemoveAt(acticleDataSource.Count - 1);
            culturePageIndex++;
            CultureLoadData(culturePageIndex, loadPageSize);
        }

        #endregion

        #region 改装

        public ObservableCollection<NewsModel> changeDataSource = null;
        NewsViewModel changeComm = null;
        int ChangePageIndex = 1; //页码
        /// <summary>
        /// 改装- 加载数据
        /// </summary>
        public void ChangeLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;
            if (changeComm == null)
            {
                changeComm = new NewsViewModel();
            }
            string url = CreateNewsListUrl(0, 107, pageIndex, pageSize);

            changeComm.LoadDataAysnc(url);
            changeComm.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<NewsModel>>>(changeComm_LoadDataCompleted);
        }

        /// <summary>
        ///改装—加载完成
        /// </summary>
        void changeComm_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<NewsModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                if (e.Result.Count() <= 1 && ChangePageIndex > 1)
                {
                    Common.showMsg("已经是最后一页了");
                }
                else
                {
                    //数据绑定到前台
                    changeDataSource = (ObservableCollection<NewsModel>)e.Result;
                    changeListBox.ItemsSource = changeDataSource;
                }
            }

        }

        /// <summary>
        /// 改装-分页加载
        /// </summary>       
        private void changeLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            changeDataSource.RemoveAt(changeDataSource.Count - 1);
            ChangePageIndex++;
            ChangeLoadData(ChangePageIndex, loadPageSize);
        }
        #endregion

        #region 说客

        public ObservableCollection<NewsModel> shuokeDataSource = null;
        NewsViewModel shuokeComm = null;
        int shuokePageIndex = 1; //页码
        /// <summary>
        /// 说客- 加载数据
        /// </summary>
        public void ShuokeLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;
            if (shuokeComm == null)
            {
                shuokeComm = new NewsViewModel();
            }
            string url = string.Format("{0}{3}/news/shuokelist-a2-pm1-v1.4.0-p{1}-s{2}.html", App.appUrl, pageIndex, pageSize, App.versionStr);

            shuokeComm.LoadDataAysnc(url, 2);
            shuokeComm.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<NewsModel>>>(shuoke_LoadDataCompleted);
        }

        /// <summary>
        ///说客—加载完成
        /// </summary>
        void shuoke_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<NewsModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                if (e.Result.Count() <= 1 && shuokePageIndex > 1)
                {
                    Common.showMsg("已经是最后一页了");
                }
                else
                {
                    //数据绑定到前台
                    shuokeDataSource = (ObservableCollection<NewsModel>)e.Result;
                    shuokeCommListBox.ItemsSource = shuokeDataSource;

                }
            }

        }

        /// <summary>
        /// 说客-分页加载
        /// </summary>       
        private void shuokeLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            shuokeDataSource.RemoveAt(shuokeDataSource.Count - 1);
            shuokePageIndex++;
            ShuokeLoadData(shuokePageIndex, loadPageSize);
        }
        #endregion

        #region 游记

        public ObservableCollection<NewsModel> travelsDataSoure = null;
        NewsViewModel travelsComm = null;
        int travelsPageIndex = 1; //页码
        /// <summary>
        /// 游记- 加载数据
        /// </summary>
        public void TravelsLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;
            if (travelsComm == null)
            {
                travelsComm = new NewsViewModel();
            }
            string url = CreateNewsListUrl(0, 100, pageIndex, pageSize);

            travelsComm.LoadDataAysnc(url);
            travelsComm.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<NewsModel>>>(travelsComm_LoadDataCompleted);
        }

        /// <summary>
        ///游记—加载完成
        /// </summary>
        void travelsComm_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<NewsModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                if (e.Result.Count() <= 1 && travelsPageIndex > 1)
                {
                    Common.showMsg("已经是最后一页了");
                }
                else
                {
                    //数据绑定到前台
                    travelsDataSoure = (ObservableCollection<NewsModel>)e.Result;
                    travelsListBox.ItemsSource = travelsDataSoure;
                }
            }

        }
        /// <summary>
        /// 游记-分页加载
        /// </summary>       
        private void travelsLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            travelsDataSoure.RemoveAt(travelsDataSoure.Count - 1);
            travelsPageIndex++;
            TravelsLoadData(travelsPageIndex, loadPageSize);
        }
        #endregion

        #region  技术

        public ObservableCollection<NewsModel> technologyDataSource = null;
        NewsViewModel technologyComm = null;
        int technologyPageIndex = 1; //页码
        /// <summary>
        /// 技术- 加载数据
        /// </summary>
        public void TechnologyLoadData(int pageIndex, int pageSize)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;
            if (technologyComm == null)
            {
                technologyComm = new NewsViewModel();
            }
            string url = CreateNewsListUrl(0, 102, pageIndex, pageSize);

            technologyComm.LoadDataAysnc(url);
            technologyComm.LoadDataCompleted += new EventHandler<APIEventArgs<IEnumerable<NewsModel>>>(technologyComm_LoadDataCompleted);
        }

        /// <summary>
        ///技术—加载完成
        /// </summary>
        void technologyComm_LoadDataCompleted(object sender, APIEventArgs<IEnumerable<NewsModel>> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            if (e.Error != null)
            {
                Common.NetworkAvailablePrompt();
            }
            else
            {
                if (e.Result.Count() <= 1 && technologyPageIndex > 1)
                {
                    Common.showMsg("已经是最后一页了");
                }
                else
                {
                    //数据绑定到前台
                    technologyDataSource = (ObservableCollection<NewsModel>)e.Result;
                    technologyListBox.ItemsSource = technologyDataSource;
                }
            }

        }


        /// <summary>
        /// 技术-分页加载
        private void technologyLoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            technologyDataSource.RemoveAt(technologyDataSource.Count - 1);
            technologyPageIndex++;
            TechnologyLoadData(technologyPageIndex, loadPageSize);
        }
        #endregion

        #region News List & Item

        private static string CreateNewsListUrl(int cityid, int newsType, int pageIndex, int pageSize)
        {
            return string.Format(AppUrlMgr.NewsListUrl, cityid, newsType, pageIndex, pageSize, 0);
        }

        /// <summary>
        /// 导向新闻最终页
        /// </summary>
        private void newsGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Grid gg = (Grid)sender;
            string pageindex = "1";
            NewsModel news = null;
            switch (piv.SelectedIndex)
            {

                case 0: //新闻
                    if (newsDataSource != null)
                        news = newsDataSource.Where(w => w.id == (int)gg.Tag).FirstOrDefault();
                    break;
                case 1: //视频
                    if (videoDataSource != null)
                    {
                        news = videoDataSource.Where(w => w.id == (int)gg.Tag).FirstOrDefault();
                        this.NavigationService.Navigate(new Uri("/View/Channel/News/VideoEndPage.xaml?videoid=" + gg.Tag, UriKind.Relative));
                    }
                    break;
                case 2: //评测
                    if (evalDataSource != null)
                        news = evalDataSource.Where(w => w.id == (int)gg.Tag).FirstOrDefault();
                    break;
                case 3: //行情
                    if (qutoDataSource != null)
                        news = qutoDataSource.Where(w => w.id == (int)gg.Tag).FirstOrDefault();
                    break;
                case 4: //导购
                    if (shoppingDataSource != null)
                        news = shoppingDataSource.Where(w => w.id == (int)gg.Tag).FirstOrDefault();
                    break;
                case 5:  //用车
                    if (useCarDataSource != null)
                        news = useCarDataSource.Where(w => w.id == (int)gg.Tag).FirstOrDefault();
                    break;
                case 6:  //文化
                    if (acticleDataSource != null)
                        news = acticleDataSource.Where(w => w.id == (int)gg.Tag).FirstOrDefault();
                    break;
                case 7: //改装
                    if (changeDataSource != null)
                        news = changeDataSource.Where(w => w.id == (int)gg.Tag).FirstOrDefault();
                    break;
                case 8: //说客
                    if (shuokeDataSource != null)
                        news = shuokeDataSource.Where(w => w.id == (int)gg.Tag).FirstOrDefault();
                    break;
                case 9: //游记
                    if (travelsDataSoure != null)
                        news = travelsDataSoure.Where(w => w.id == (int)gg.Tag).FirstOrDefault();
                    break;
                case 10: //技术
                    if (technologyDataSource != null)
                        news = technologyDataSource.Where(w => w.id == (int)gg.Tag).FirstOrDefault();
                    break;
            }

            if (news != null)
            {
                pageindex = news.pageIndex;
                this.NavigationService.Navigate(new Uri("/View/Channel/News/NewsEndPage.xaml?newsid=" + gg.Tag + "&pageIndex=" + pageindex + "&pageType=" + pageType, UriKind.Relative));
            }
        }

        #endregion


    }

}