using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Net;
using System.Threading;
using System.Windows;
using ViewModels;
using ViewModels.Me;
using WeiboSdk;
using WeiboSdk.PageViews;

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

            thirdPartyLoginVM = new ThirdPartyLoginViewModel();
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
            if (this.LoginPivot.SelectedIndex == 0)
            {
                this.Login();
            }
            else
            {
                this.Register();
            }
        }

        private void Login()
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

        //登录验证
        LoginViewModel loginVM = null;
        public void LoadData(string url, string postData)
        {
            try
            {
                loginVM = new LoginViewModel();

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
                                    Utils.MeHelper.SaveMyInfo(model);
                                    Utils.PushNotificationHelper.RegisterNewDevice();

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

                            //获取推送设置
                            Utils.PushNotificationHelper.GetUserSetting();
                        }
                    });
                });

                loginVM.LoadDataAsync(url, postData);
            }
            catch
            { }
        }

        #region 注册

        private void Register()
        {
            this.Focus();

            bool checkSuccess = this.UpdateSelfInfoPanel.CheckData();
            if (checkSuccess)
            {
                //接口注册
                string nickname = HttpUtility.UrlEncode(this.UpdateSelfInfoPanel.UsernameTextbox.Text);
                string password = Handler.MD5.GetMd5String(this.UpdateSelfInfoPanel.PasswordPanel.Password);
                string mobile = this.UpdateSelfInfoPanel.PhoneNoTextBox.Text;
                string validcode = this.UpdateSelfInfoPanel.CodeTextbox.Text;
                string ua = AutoWP7.Handler.Common.GetAutoHomeUA();
                string format = "_appid={0}&autohomeua={1}&nickname={2}&userpwd={3}&mobile={4}&validcode={5}&_timestamp={6}";
                string data = string.Format(format, Utils.MeHelper.appIDWp, ua, nickname, password, mobile, validcode, Common.GetTimeStamp());
                data = Common.SortURLParamAsc(data);
                string sign = Common.GetSignStr(data);
                data += "&_sign=" + sign;

                UpStreamViewModel upstreamVM = UpStreamViewModel.SingleInstance;
                upstreamVM.UploadAsync(Utils.MeHelper.UserRegisterUrl, data, wc_UploadStringCompleted);
            }
        }

        void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error is WebException)
                {
                    Common.showMsg("当前网络不可用，请检查网络设置");
                }
                else if (e.Error == null && e.Cancelled == false)
                {
                    JObject json = JObject.Parse(e.Result);
                    int resultCode = (int)json.SelectToken("returncode");
                    string message = json.SelectToken("message").ToString();
                    //成功
                    if (resultCode == 0)
                    {
                        JToken resultToken = json.SelectToken("result");
                        if (resultToken != null)
                        {
                            Model.Me.RegisterResultModel resultVM = JsonHelper.Deserialise<Model.Me.RegisterResultModel>(resultToken.ToString());

                            if (resultVM != null)
                            {
                                //存入文件
                                var model = new MyForumModel();
                                model.Success = 1;
                                model.Message = message;
                                model.UserID = resultVM.ID;
                                model.UserName = resultVM.UserName;
                                model.Authorization = resultVM.Auth;

                                Utils.MeHelper.SaveMyInfo(model);
                                Utils.PushNotificationHelper.RegisterNewDevice();
                                Utils.PushNotificationHelper.GetUserSetting();
                            }

                            //返回登录前页面
                            Common.showMsg("注册成功");
                            this.NavigationService.GoBack();
                        }
                    }
                    else
                    {
                        string msg = string.IsNullOrEmpty(message) ? "注册失败" : message;
                        Common.showMsg(msg);
                    }
                }
            }
            catch
            { }
        }

        #endregion

        #region 第三方登录

        ThirdPartyLoginViewModel thirdPartyLoginVM;

        //weibo
        void thirdPartyLoginVM_ThirdPartyLoginCompleted(object sender, int e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                //绑定成功，判断帐号授权情况
                if (e == 0)
                {
                    var result = this.thirdPartyLoginVM.LoginResult;
                    if (result != null)
                    {
                        //存入文件
                        var model = new MyForumModel();
                        model.Success = 1;

                        model.UserID = result.ID;
                        model.UserName = result.UserName;
                        model.Authorization = result.Auth;
                        model.WeiWang = result.Prestige.ToString();
                        model.UserPic = result.Img;

                        Utils.MeHelper.SaveMyInfo(model);
                        Utils.PushNotificationHelper.RegisterNewDevice();
                        Utils.PushNotificationHelper.GetUserSetting();
                    }

                    this.NavigationService.GoBack();
                }
                //未绑定车网帐号
                else if (e == 2013022)
                {
                    this.NavigationService.Navigate(new Uri("/View/Me/CompleteMyInfo.xaml?platformId=16", UriKind.Relative));
                }
                else//其他错误
                {
                    Common.showMsg("登录失败");
                }
            });
        }

        #region weibo login

        private void Weibo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //http://open.weibo.com/wiki/Oauth2/authorize
            //set account info
            SdkData.AppKey = Utils.MeHelper.WeiboAppKey;
            SdkData.AppSecret = Utils.MeHelper.WeiboAppSecret;
            SdkData.RedirectUri = Utils.MeHelper.WeiboRedirectUri;

            AuthenticationView.OAuth2VerifyCompleted = (e1, e2, e3) => VerifyBack(e1, e2, e3);
            AuthenticationView.OBrowserCancelled = new EventHandler(cancelEvent);

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.Navigate(new Uri("/WeiboSdk;component/PageViews/AuthenticationView.xaml"
                    , UriKind.Relative));
            });
        }

        private void VerifyBack(bool isSucess, SdkAuthError errCode, SdkAuth2Res response)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (errCode.errCode == SdkErrCode.SUCCESS && response != null)
            {
                var weiboModel = new Model.Me.ThirdPartyAccountModel();
                weiboModel.PlatformId = Utils.MeHelper.WeiboPlatformID;
                weiboModel.AccessToken = response.accesssToken;
                weiboModel.RefreshToken = response.refleshToken;
                weiboModel.ExpiresIn = int.Parse(response.expriesIn);
                weiboModel.OpenId = response.UserId;
                settings[Utils.MeHelper.weiboAccountKey] = weiboModel;

                //汽车之家第三方登录接口
                string dataFormat = "_appid={0}&autohomeua={1}&openid={2}&plantFormId={3}&token={4}&position={5}&_timeStamp={6}";
                string data = string.Format(dataFormat, Utils.MeHelper.appIDWp, AutoWP7.Handler.Common.GetAutoHomeUA(), response.UserId, Utils.MeHelper.WeiboPlatformID, response.accesssToken, App.CityId, Common.GetTimeStamp());
                data = Common.SortURLParamAsc(data);
                string sign = Common.GetSignStr(data);
                data += "&_sign=" + sign;
                thirdPartyLoginVM.ThirdPartyLoginAsync(Utils.MeHelper.ThirdPartyLoginUrl, data, thirdPartyLoginVM_ThirdPartyLoginCompleted);
            }
            else
            {
                settings[Utils.MeHelper.weiboAccountKey] = null;
                Common.showMsg("授权失败");
            }

            settings.Save();
        }

        private void cancelEvent(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.GoBack();
                Common.showMsg("授权失败");
            });
        }

        #endregion

        #region QQ login

        private void QQ_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var qqAuthVM = Me.QQ.AuthenticationViewModel.SingleInstance;
            qqAuthVM.Login();
        }

        #endregion

        #endregion
    }
}