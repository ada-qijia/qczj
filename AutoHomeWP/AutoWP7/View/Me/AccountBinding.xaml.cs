using Hammock.Web;
using Microsoft.Phone.Controls;
using Model.Me;
using Newtonsoft.Json.Linq;
using QConnectSDK.Exceptions;
using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ViewModels.Me;
using WeiboSdk;
using WeiboSdk.PageViews;

namespace AutoWP7.View.Me
{
    public partial class AccountBinding : PhoneApplicationPage
    {
        private ThirdPartyBindingViewModel BindingVM;

        public AccountBinding()
        {
            InitializeComponent();
            this.BindingVM = new ThirdPartyBindingViewModel();
            this.BindingVM.LoadDataCompleted += BindingVM_LoadDataCompleted;
            this.DataContext = this.BindingVM;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var userInfoModel = Utils.MeHelper.GetMyInfoModel();
            if (userInfoModel != null)
            {
                GlobalIndicator.Instance.Text = "正在获取中...";
                GlobalIndicator.Instance.IsBusy = true;

                //qq 15, weibo 16
                string data = string.Format("a=2&pm=3&v={0}&au={1}&pfids={2}", Handler.Common.GetAssemblyVersion(), userInfoModel.Authorization, Utils.MeHelper.WeiboPlatformID);// + "," + Utils.MeHelper.QQPlatformID);
                string url = Utils.MeHelper.ThirdPartyBindingStateUrl;
                this.BindingVM.LoadDataAysnc(url, data);
            }
        }

        void BindingVM_LoadDataCompleted(object sender, EventArgs e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            if (this.BindingVM.ReturnCode == 0)
            {
                //检查账户是否过期
                this.CheckTokenExpired();
            }
            else if (this.BindingVM.ReturnCode == 10002)
            {
                CustomMessageBox messageBox = new CustomMessageBox()
                {
                    Caption = "获取失败",
                    Message = "密码检验失败",
                    LeftButtonContent = "取消",
                    RightButtonContent = "重新登录",
                    IsFullScreen = false
                };

                messageBox.Dismissed += (s1, e1) =>
                 {
                     switch (e1.Result)
                     {
                         case CustomMessageBoxResult.RightButton:
                             NavigationService.Navigate(new Uri("/View/More/LoginPage.xaml", UriKind.RelativeOrAbsolute));
                             break;
                         case CustomMessageBoxResult.LeftButton:
                             //go back to me page.
                             NavigationService.RemoveBackEntry();
                             NavigationService.RemoveBackEntry();
                             break;
                         default:
                             break;
                     }
                 };

                messageBox.Show();
            }
        }

        #region 检查账户是否过期

        private void CheckTokenExpired()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;

            //weibo, http://open.weibo.com/wiki/Oauth2/get_token_info?sudaref=open.weibo.com
            var weibo = this.BindingVM.BindingList.FirstOrDefault(item => item.ThirdPartyID == Utils.MeHelper.WeiboPlatformID);
            if (weibo != null)
            {
                if (settings.Contains(Utils.MeHelper.weiboAccountKey) && settings[Utils.MeHelper.weiboAccountKey] is ThirdPartyAccountModel)
                {
                    weibo.RelationType = 1;
                    weibo.IsExpired = false;
                }
                else if (!string.IsNullOrEmpty(weibo.Token))
                {
                    var client = new Hammock.RestClient();
                    client.Authority = "https://api.weibo.com";
                    client.HasElevatedPermissions = true;

                    var request = new Hammock.RestRequest();
                    request.Path = "/oauth2/get_token_info";
                    request.Method = WebMethod.Post;
                    request.Encoding = System.Text.Encoding.UTF8;

                    request.AddParameter("access_token", weibo.Token);

                    client.BeginRequest(request, (e1, e2, e3) => GetWeiboInfoCallback(e1, e2, e3), weibo);
                }
            }

            //qq
            if (settings.Contains(Utils.MeHelper.QQAuthResultKey) && settings[Utils.MeHelper.QQAuthResultKey] is ThirdPartyAccountModel)
            {
                //使用第三方帐号登录
            }
            else
            {
                var qq = this.BindingVM.BindingList.FirstOrDefault(item => item.ThirdPartyID == Utils.MeHelper.QQPlatformID);
                if (qq != null && !string.IsNullOrEmpty(qq.Token))
                {
                    var qqAuthVM = View.Me.QQ.AuthenticationViewModel.SingleInstance;
                    qqAuthVM.LoadProfile(qq.Token, qq.OriginalID, QQLoadProfileCompleted);
                }
            }
        }

        private void GetWeiboInfoCallback(Hammock.RestRequest request, Hammock.RestResponse response, object userState)
        {
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        var weibo = userState as Model.Me.ThirdPartyBindingModel;
                        if (weibo != null)
                        {
                            JObject json = JObject.Parse(response.Content);
                            JToken errorCodeToken = json.SelectToken("error_code");
                            if (errorCodeToken != null)
                            {
                                int code = errorCodeToken.Value<int>();
                                if (code > 21314 && code < 21318)
                                {
                                    weibo.IsExpired = true;
                                    return;
                                }
                            }
                            else
                            {
                                int expireIn = json.SelectToken("expire_in").Value<int>();
                                if (expireIn < 0)
                                {
                                    weibo.IsExpired = true;
                                    return;
                                }
                            }
                        }
                    });
            }
            catch
            { }
        }

        void QQLoadProfileCompleted(object sender, QzoneException e)
        {
            var qqAuthVM = sender as View.Me.QQ.AuthenticationViewModel;
            var qq = this.BindingVM.BindingList.FirstOrDefault(item => item.ThirdPartyID == Utils.MeHelper.QQPlatformID);
            if (e == null && qqAuthVM.Profile != null)
            {
                if (qqAuthVM.Profile.Ret == 0)//未过期
                {
                    qq.IsExpired = false;
                }
                else if (qqAuthVM.Profile.Ret == 100014 || qqAuthVM.Profile.Ret == 100015)//过期，需重新注册
                {
                    qq.IsExpired = true;
                }
                else//其他错误
                {
                }

                //update binding
                this.DataContext = null;
                this.DataContext = this.BindingVM;
            }
        }

        #endregion

        #region UI交互

        private void ThirdPartyBinding_Click(object sender, RoutedEventArgs e)
        {
            var thirdPartyModel = (sender as FrameworkElement).DataContext as ThirdPartyBindingModel;
            if (thirdPartyModel.ThirdPartyID == Utils.MeHelper.WeiboPlatformID)
            {
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
        }

        private void VerifyBack(bool isSucess, SdkAuthError errCode, SdkAuth2Res response)
        {
            if (errCode.errCode == SdkErrCode.SUCCESS && response != null)
            {
                var weiboModel = new Model.Me.ThirdPartyAccountModel();
                weiboModel.PlatformId = Utils.MeHelper.WeiboPlatformID;
                weiboModel.AccessToken = response.accesssToken;
                weiboModel.RefreshToken = response.refleshToken;
                weiboModel.ExpiresIn = int.Parse(response.expriesIn);
                weiboModel.OpenId = response.UserId;
                IsolatedStorageSettings.ApplicationSettings[Utils.MeHelper.weiboAccountKey] = weiboModel;

                var weibo = this.BindingVM.BindingList.FirstOrDefault(item => item.ThirdPartyID == Utils.MeHelper.WeiboPlatformID);
                //过期绑定，更新Token
                if (weibo.IsExpired)
                {
                    var meInfo = Utils.MeHelper.GetMyInfoModel();
                    string data = string.Format("token={0}&userid={1}&platformid={2}&_appid={3}&autohomeua={4}&_timestamp={5}", response.accesssToken, meInfo.UserID, weiboModel.PlatformId, Utils.MeHelper.appIDWp, Handler.Common.GetAutoHomeUA(), Handler.Common.GetTimeStamp());
                    data = Handler.Common.SortURLParamAsc(data);
                    string sign = Handler.Common.GetSignStr(data);
                    data += "&_sign=" + sign;
                    ViewModels.Me.UpStreamViewModel.SingleInstance.UploadAsync(Utils.MeHelper.ThirdPartyUpdateTokenUrl, data, updateTokenCompleted, weiboModel);
                }
                else//首次绑定,关联帐号
                {
                    EventHandler<string> getNicknameCompleted = (object s, string e) =>
                        {
                            var meInfo = Utils.MeHelper.GetMyInfoModel();
                            if (meInfo != null)
                            {
                                string format = "_appid={0}&_timeStamp={1}&autohomeua={2}&pcpopclub={3}&platformid={4}&openId={5}&token={6}&tokenSecret={7}&orginalName={8}";
                                string data = string.Format(format, Utils.MeHelper.appID, Handler.Common.GetTimeStamp(), AutoWP7.Handler.Common.GetAutoHomeUA(), meInfo.Authorization, weiboModel.PlatformId, weiboModel.OpenId, weiboModel.AccessToken, Utils.MeHelper.WeiboAppSecret, e);
                                data = Handler.Common.SortURLParamAsc(data);
                                string sign = Handler.Common.GetSignStr(data);
                                data += "&_sign=" + sign;
                                UpStreamViewModel.SingleInstance.UploadAsync(Utils.MeHelper.ConnectAccountUrl, data, connectAccountCompleted, weiboModel);
                            }
                        };
                    Utils.MeHelper.GetWeiboUserNickname(weiboModel.AccessToken, weiboModel.OpenId, getNicknameCompleted);
                }
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings[Utils.MeHelper.weiboAccountKey] = null;
            }
        }

        private void updateTokenCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && e.Cancelled == false)
                {
                    JObject json = JObject.Parse(e.Result);
                    var returnCode = json.SelectToken("returncode").Value<int>();
                    if (returnCode == 0 && e.UserState is Model.Me.ThirdPartyAccountModel)
                    {
                        var weiboModel = e.UserState as Model.Me.ThirdPartyAccountModel;
                        var weibo = this.BindingVM.BindingList.FirstOrDefault(item => item.ThirdPartyID == weiboModel.PlatformId);
                        if (weibo != null)
                        {
                            weibo.IsExpired = false;
                            weibo.RelationType = 1;
                            var result = json.SelectToken("result");
                            if (result != null)
                            {
                                weibo.UserID = result.SelectToken("UserId").Value<int>();
                                weibo.Token = result.SelectToken("Token").ToString();
                                weibo.OriginalID = result.SelectToken("OpenId").ToString();
                            }
                        }
                    }
                }
            }
            catch
            { }
        }

        private void connectAccountCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && e.Cancelled == false)
                {
                    JObject json = JObject.Parse(e.Result);
                    var returnCode = json.SelectToken("returncode").Value<int>();
                    if (returnCode == 0 && e.UserState is Model.Me.ThirdPartyAccountModel)
                    {
                        var weiboModel = e.UserState as Model.Me.ThirdPartyAccountModel;
                        var weibo = this.BindingVM.BindingList.FirstOrDefault(item => item.ThirdPartyID == weiboModel.PlatformId);
                        if (weibo != null)
                        {
                            weibo.IsExpired = false;
                            weibo.RelationType = 1;
                            var result = json.SelectToken("result");
                            if (result != null)
                            {
                                weibo.UserID = result.SelectToken("UserId").Value<int>();
                                weibo.Token = weiboModel.AccessToken;
                                weibo.OriginalID = weiboModel.OpenId;
                            }
                        }
                    }
                }
            }
            catch
            { }
        }

        private void cancelEvent(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.GoBack();
                Handler.Common.showMsg("授权失败");
            });
        }

        #endregion
    }
}