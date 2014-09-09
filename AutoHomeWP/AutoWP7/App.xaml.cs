using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MessageControl;
using AutoWP7.View.Channel;
using Microsoft.Phone.Data.Linq;
using Model;
using System.IO.IsolatedStorage;
using System.Reflection;
using ViewModels;
using AutoWP7.Handler;
namespace AutoWP7
{
    public partial class App : Application
    {
        public static ScrollViewer _ScrollViewer = null;
        public static string AskName { get; set; }
        public static string AskPhone { get; set; }
        public static string AskSpec { get; set; }
        //最新新闻列表最后一条新闻的lasttime、lastid值
        public static string newsLastTime { get; set; }
        private static string _carTypeId;
        public static string UserAgent = "WindowsPhone\t8\tautohome\t1.4.0";
        /// <summary>
        /// 车型ID
        /// </summary>
        public static string CarTypeId
        {
            get
            {
                return _carTypeId;
            }
            set
            {
                if (value != _carTypeId)
                {
                    _carTypeId = value;
                }
            }
        }

        private static string _carSeriesId;
        /// <summary>
        ///车系ID
        /// </summary>
        public static string CarSeriesId
        {
            get
            {
                return _carSeriesId;
            }
            set
            {
                if (value != _carSeriesId)
                {
                    _carSeriesId = value;
                }
            }

        }
        private static string _carSeriesName;
        /// <summary>
        ///车系名称
        /// </summary>
        public static string CarSeriesName
        {
            get
            {
                return _carSeriesName;
            }
            set
            {
                if (value != _carSeriesName)
                {
                    _carSeriesName = value;
                }
            }

        }

        public static int timerId = 0;

        private static string _cityId;
        /// <summary>
        /// 城市ID
        /// </summary>
        public static string CityId
        {
            get
            {
                return _cityId;
            }
            set
            {
                if (value != _cityId)
                {
                    _cityId = value;
                }
            }
        }

        /// <summary>
        /// 图片总数
        /// </summary>
        private static int _allImgNum;
        public static int AllImgNum
        {
            get
            {
                return _allImgNum;
            }
            set
            {
                if (value != _allImgNum)
                {
                    _allImgNum = value;
                }
            }
        }

        //帖子最终页地址
        private static string _topicUrl;
        public static string TopicUrl
        {
            get
            {
                return _topicUrl;
            }
            set
            {
                if (value != _topicUrl)
                {
                    _topicUrl = value;
                }
            }
        }

        /// <summary>
        /// 贴子最终页是否重新加载（true-加载 false不加载）
        /// </summary>
        private static bool _isLoadTag;
        public static bool IsLoadTag
        {
            get
            {
                return _isLoadTag;
            }
            set
            {
                if (value != _isLoadTag)
                {
                    _isLoadTag = value;
                }
            }
        }

        //appbar的状态
        public static bool[] barStatus = new bool[4];
        //帖子页码
        private static int _pageIndex;
        public static int pageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                if (value != _pageIndex)
                {
                    _pageIndex = value;
                }
            }
        }

        //帖子数
        private static int _replyCount;
        public static int replyCount
        {
            get
            {
                return _replyCount;
            }
            set
            {
                if (value != _replyCount)
                {
                    replyCount = value;
                }
            }
        }

        //帖子数


        #region 向服务器请求数据url地址头部
        //向服务器请求数据url地址头部
        public static string headUrl = "http://sp.autohome.com.cn";
        public static string appUrl = "http://app.api.autohome.com.cn";

        public static string loginUrl = "http://account.autohome.com.cn";       

        public static string replyUrl = "http://reply.autohome.com.cn";
        public static string newsPageDomain = " http://cont.app.autohome.com.cn";

        public static string clubUrl = "http://club.api.autohome.com.cn";
        public static string topicPageDomain = "http://forum.app.autohome.com.cn";
        public static string versionStr = "/wpv1.4";

        #endregion
        /// <summary>
        /// 渠道号
        /// </summary>
        public static string ChannelId = "Marketplace";
        //产品id(appid)
        public static int appId = 2;
        //平台id
        public static int platForm = 3;
        //当前版本号
        public static string version = "1.4.0";
        //app相关信息
        public static string AppInfo
        {
            get
            {
                return string.Format("a{0}-pm{1}-v{2}", appId, platForm, version);
            }
        }

        public static string appKey = "@7U$aPOE@$";
        //最新分页消息控件
        public MyMessage newestPartPageMessage = new MyMessage { MessageContent = new ArticlePartPageUserControl() };
        //新闻分页消息控件
        public MyMessage newsPartPageMessage = new MyMessage { MessageContent = new NewsPartPageUserControl() };
        //车系文章分页消息控件
        public MyMessage carSeriesPartPageMessage = new MyMessage { MessageContent = new CarSeriesPartPageUserControl() };

        //文章id
        public static string newsid = string.Empty;



        /// <summary>
        /// 提供对电话应用程序的根框架的轻松访问。
        /// </summary>
        /// <returns>电话应用程序的根框架。</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Application 对象的构造函数。
        /// </summary>
        public App()
        {

            //内存消耗情况
            //if (System.Diagnostics.Debugger.IsAttached)
            //{
            //    AutoWP7.Handler.MemoryDiagnosticsHelper.Start(TimeSpan.FromMilliseconds(500), true);
            //}

            // 未捕获的异常的全局处理程序。 
            UnhandledException += Application_UnhandledException;

            // 标准 Silverlight 初始化
            InitializeComponent();

            // 特定于电话的初始化
            InitializePhoneApplication();

            // 调试时显示图形分析信息。
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 显示当前帧速率计数器
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // 显示在每个帧中重绘的应用程序区域。
                //Application.Current.Host.Settings.EnableRedrawRegions = true；

                // 启用非生产分析可视化模式， 
                // 该模式显示递交给 GPU 的包含彩色重叠区的页面区域。
                //Application.Current.Host.Settings.EnableCacheVisualization = true；

                // 通过将应用程序的 PhoneApplicationService 对象的 UserIdleDetectionMode 属性
                // 设置为 Disabled 来禁用应用程序空闲检测。
                //  注意: 仅在调试模式下使用此设置。禁用用户空闲检测的应用程序在用户不使用电话时将继续运行
                // 并且消耗电池电量。
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        // 应用程序启动(例如，从“开始”菜单启动)时执行的代码
        // 此代码在重新激活应用程序时不执行
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            #region 清洗auth
            var setting = IsolatedStorageSettings.ApplicationSettings;
            string key = "userInfo";
            string cleanKey = "isclean";
            CleanUserInfoViewModel cleanM = null;

            if (setting.Contains(cleanKey))
            {
                //非首次启动
                int isClean = (int)setting[cleanKey];
                if (isClean == 0)
                {
                    if (setting.Contains(key))
                    {
                        // -->已登录
                        //清洗
                        //清洗状态置为1（已清洗）
                        MyForumModel model = (MyForumModel)setting[key];
                        string oldAuth = model.Authorization;
                        string url = string.Format("{0}/applogin/getmemberid?_appid=user&pcpopclub={1}", loginUrl, oldAuth);
                        if (cleanM == null)
                        {
                            cleanM = new CleanUserInfoViewModel();
                        }
                        cleanM.LoadDataAsync(url);
                    }
                    else
                    {
                        //-->未登录
                        //清洗状态置为1（已清洗）
                        setting[cleanKey] = 1;
                        setting.Save();
                    }
                }
                else
                {
                    //不需要清洗
                    //不需要更改清洗状态
                }
            }
            else
            {
                //首次启动
                setting.Add(cleanKey, 0);
                setting.Save();
                if (setting.Contains(key))
                {
                    //1、覆盖安装-->已登录
                    //清洗
                    //清洗状态置为1（已清洗）
                    MyForumModel model = (MyForumModel)setting[key];
                    string oldAuth = model.Authorization;
                    string url = string.Format("{0}/applogin/getmemberid?_appid=user&pcpopclub={1}", loginUrl, oldAuth);
                    if (cleanM == null)
                    {
                        cleanM = new CleanUserInfoViewModel();
                    }
                    cleanM.LoadDataAsync(url);
                }
                else
                {
                    //2、正常（卸载）安装-->未登录
                    //不需要清洗
                    //清洗状态置为1（已清洗）
                    setting[cleanKey] = 1;
                    setting.Save();
                }
            }
            #endregion

            using (ViewModels.Handler.LocalDataContext db = new ViewModels.Handler.LocalDataContext())
            {

                if (!db.DatabaseExists())
                {
                    db.CreateDatabase();

                    //设置数据库版本号   
                    DatabaseSchemaUpdater schemaUpdater = db.CreateDatabaseSchemaUpdater();
                    schemaUpdater.DatabaseSchemaVersion = 1;
                    schemaUpdater.Execute();
                }
                else
                {
                    DatabaseSchemaUpdater schemaUpdater = db.CreateDatabaseSchemaUpdater();
                    int version = schemaUpdater.DatabaseSchemaVersion;
                    if (version == 0)
                    {
                        try
                        {

                            schemaUpdater.AddColumn<NewsModel>("pageIndex");
                            schemaUpdater.AddTable<MyForumModel>();
                            schemaUpdater.DatabaseSchemaVersion = 1;
                            schemaUpdater.Execute();

                        }
                        catch (Exception ex)
                        {
                            DatabaseSchemaUpdater schema = db.CreateDatabaseSchemaUpdater();
                            schema.DatabaseSchemaVersion = 1;
                            schema.Execute();
                        }

                    }
                }
            }
            //调用全局方法，更改本地数据库数据结构
            UpdateDBHelper upHelper = new UpdateDBHelper();
            upHelper.update_14();
            UmengSDK.UmengAnalytics.setDebug(true);
            UmengSDK.UmengAnalytics.onLaunching("5056b77d5270155f88000125", ChannelId);
            UmengSDK.UmengAnalytics.update("5056b77d5270155f88000125");
        }



        // 激活应用程序(置于前台)时执行的代码
        // 此代码在首次启动应用程序时不执行
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            // 确保正确恢复应用程序状态
            //if (!App.ViewModel.IsDataLoaded)
            //{
            //    App.ViewModel.LoadData();
            //}

            UmengSDK.UmengAnalytics.onActivated("5056b77d5270155f88000125", ChannelId);
        }

        // 停用应用程序(发送到后台)时执行的代码
        // 此代码在应用程序关闭时不执行
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {

        }

        // 应用程序关闭(例如，用户点击“后退”)时执行的代码
        // 此代码在停用应用程序时不执行
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            // 确保所需的应用程序状态在此处保持不变。
        }

        // 导航失败时执行的代码
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 导航已失败；强行进入调试器
                System.Diagnostics.Debugger.Break();
            }
        }

        // 出现未处理的异常时执行的代码
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // 出现未处理的异常；强行进入调试器
                System.Diagnostics.Debugger.Break();
            }
        }

        #region 电话应用程序初始化

        // 避免双重初始化
        private bool phoneApplicationInitialized = false;

        // 请勿向此方法中添加任何其他代码
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // 创建框架但先不将它设置为 RootVisual；这允许初始
            // 屏幕保持活动状态，直到准备呈现应用程序时。
            RootFrame = new PhoneApplicationFrame();

            RootFrame.Navigated += CompleteInitializePhoneApplication;


            // 处理导航故障
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // 确保我们未再次初始化
            phoneApplicationInitialized = true;

            GlobalIndicator.Instance.Initialize(RootFrame);
        }

        // 请勿向此方法中添加任何其他代码
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // 设置根视觉效果以允许应用程序呈现
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // 删除此处理程序，因为不再需要它
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}