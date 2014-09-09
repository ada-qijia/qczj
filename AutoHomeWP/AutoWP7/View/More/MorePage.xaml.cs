using System;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using AutoWP7.Handler;
using ViewModels.Handler;
using System.Linq;
using Model;

namespace AutoWP7.View.More
{
    /// <summary>
    /// 更多页
    /// </summary>
    public partial class MorePage : PhoneApplicationPage
    {
        public MorePage()
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
                       if( Common.isLogin())
                        {
                            loginStats.Text = "退出登录";
                        }
                    }
                    break;
                case System.Windows.Navigation.NavigationMode.Back:
                    {
                        //验证是否已经登录
                        if (Common.isLogin())
                        {
                            loginStats.Text = "退出登录";
                        }
                    }
                    break;
            }
        }

        //登录
        private void login_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
            //如果木有登录，跳转到登录页
            if (!Common.isLogin())
            {
                this.NavigationService.Navigate(new Uri("/View/More/LoginPage.xaml", UriKind.Relative));
            }
            if (loginStats.Text == "退出登录")
            {
                var setting = IsolatedStorageSettings.ApplicationSettings;
                string key = "userInfo";
                // UserInfoModel userInfoModel = null;
                if (setting.Contains(key))//已经登录
                {
                    setting.Remove(key);
                    setting.Clear();
                }

             

                //清除我的论坛
                using (LocalDataContext ldc = new LocalDataContext())
                {
                    try
                    {
                        var deleteAllItem = from d in ldc.myForum where d.Id > 0 select d;
                        ldc.GetTable<MyForumModel>().DeleteAllOnSubmit(deleteAllItem);
                        ldc.SubmitChanges();
                    }
                    catch (Exception ex)
                    {

                    }
                }

                Common.showMsg("退出登录成功");
                loginStats.Text = "登录";
            }
           
        }

        //精选
        private void choiceness_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        //反馈
        private void feedback_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("MoreActivity", "反馈点击量");
            this.NavigationService.Navigate(new Uri("/View/More/FeedbackPage.xaml", UriKind.Relative));
        }

        //汽车之家
        private void autohomeMobile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("MoreActivity", "M站点击量");
            WebBrowserTask task = new WebBrowserTask();
            task.Uri = new Uri("http://m.autohome.com.cn/", UriKind.Absolute);
            task.Show();
        }

        //关于
        private void about_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("关于", "反馈点击量");
            this.NavigationService.Navigate(new Uri("/View/More/AboutPage.xaml", UriKind.Relative));
        }
        //对比
        private void carcompare_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UmengSDK.UmengAnalytics.onEvent("对比", "对比点击量");
            this.NavigationService.Navigate(new Uri("/View/Car/CarCompareListPage.xaml?action=0", UriKind.Relative));
        }
    }
}