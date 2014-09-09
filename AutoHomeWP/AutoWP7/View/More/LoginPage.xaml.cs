using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using ViewModels;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using ViewModels.Handler;
using Newtonsoft.Json;


namespace AutoWP7.View.More
{
    /// <summary>
    /// 用户登录页
    /// </summary>
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                UmengSDK.UmengAnalytics.onEvent("MoreActivity", "登录点击量");
            }
        }

        //登录状态
        bool isLogining = false;
        Thread tr;
        public ObservableCollection<MyForumModel> UserInfoDataSource = new ObservableCollection<MyForumModel>();
        private void login_Click(object sender, EventArgs e)
        {
            if (isLogining == false)
            {


                string userName = account.Text;
                string pwd = password.Password;
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(pwd))
                {

                    Common.showMsg("请输入完整~~");
                }
                else
                {
                    isLogining = true;
                    IApplicationBarIconButton Icon = this.ApplicationBar.Buttons[0] as IApplicationBarIconButton;
                    Icon.IsEnabled = false;

                    string postData = userName + "|" + AutoWP7.Handler.MD5.GetMd5String(pwd);
                  
                    GlobalIndicator.Instance.Text = "正在登录中...";
                    GlobalIndicator.Instance.IsBusy = true;

                    tr = new Thread(new ThreadStart(delegate()
                   {
                       UserInfoDataSource.Clear();

                       LoadData(App.loginUrl + "/login/applogin?userInfo=1&isAutho=0&imei=wp&encoding=gb2312", postData);
                   }));
                    tr.IsBackground = true;
                    tr.Start();


                }
            }
        }

        private string Encrypt(string pwd)
        {
            throw new NotImplementedException();
        }


        
        //登录验证
        LoginViewModel loginVM = null;
        public void LoadData(string url, string postData)
        {
            try
            {
                
                loginVM = new LoginViewModel();

                loginVM.LoadDataAsync(url, postData);

                loginVM.LoadDataCompleted += new EventHandler<ViewModels.Handler.APIEventArgs<IEnumerable<MyForumModel>>>((ss, ee) =>
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        GlobalIndicator.Instance.Text = "";
                        GlobalIndicator.Instance.IsBusy = false;
                        IApplicationBarIconButton Icon = this.ApplicationBar.Buttons[0] as IApplicationBarIconButton; 
                        Icon.IsEnabled = true;
                        isLogining = false;
                        if (ee.Error != null)
                        {
                            Common.showMsg("登录失败啦~~请重新登录");
                        }
                        else
                        {
                            UserInfoDataSource = (ObservableCollection<MyForumModel>)ee.Result;
                            foreach (MyForumModel model in UserInfoDataSource)
                            {
                                if (model.Success == 1)
                                {
                                    CleanUserInfoViewModel cleanM = new CleanUserInfoViewModel();

                                    string cleanUrl = string.Format("{0}/applogin/getmemberid?_appid=user&pcpopclub={1}", App.loginUrl, model.Authorization);
                                    cleanM.LoadDataAsync(cleanUrl);
                                    this.NavigationService.GoBack();
                                }
                                else
                                {
                                    Common.showMsg(model.Message);
                                }
                            }
                            UserInfoDataSource.Clear();
                        }
                    });


                });
            }
            catch (Exception ex)
            {

            }
        }





    }
}