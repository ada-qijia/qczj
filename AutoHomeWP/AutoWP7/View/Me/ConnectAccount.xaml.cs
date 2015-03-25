using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Windows.Navigation;
using ViewModels.Me;

namespace AutoWP7.View.Me
{
    public partial class ConnectAccount : PhoneApplicationPage
    {
        private string platformId;
        private Model.Me.ThirdPartyAccountModel thirdPartyModel;

        public ConnectAccount()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationContext.QueryString.ContainsKey("platformId"))
            {
                platformId = NavigationContext.QueryString["platformId"];
                switch (platformId)
                {
                    //weibo
                    case "16":
                        var settings = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;
                        if (settings.Contains(Utils.MeHelper.weiboAccountKey))
                        {
                            thirdPartyModel = settings[Utils.MeHelper.weiboAccountKey] as Model.Me.ThirdPartyAccountModel;
                        }
                        break;
                }
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            bool passwordEmpty = string.IsNullOrEmpty(this.PasswordTextbox.Password);
            if (string.IsNullOrEmpty(this.UsernameTextbox.Text))
            {
                string message = passwordEmpty ? "请您输入帐号密码" : "请您输入帐户名";
                Common.showMsg(message);
            }
            else if (passwordEmpty)
            {
                Common.showMsg("请您输入密码");
            }
            else
            {
                string username = this.UsernameTextbox.Text;
                string password = Handler.MD5.GetMd5String(this.PasswordTextbox.Password);

                //获取昵称
                EventHandler<string> getNicknameCompleted = (object s, string nickname) =>
                {
                    //关联帐号
                    string format = "_appid={0}&_timeStamp={1}&autohomeua={2}&logincode={3}&userPwd={4}&openId={5}&platformid={6}&token={7}&tokenSecret={8}&orginalName={9}";
                    string secret = thirdPartyModel.PlatformId == Utils.MeHelper.WeiboPlatformID ? Utils.MeHelper.WeiboAppSecret : Utils.MeHelper.QQAppSecret;
                    string data = string.Format(format, Utils.MeHelper.appID, Common.GetTimeStamp(), AutoWP7.Handler.Common.GetAutoHomeUA(), username, password, thirdPartyModel.OpenId, thirdPartyModel.PlatformId, thirdPartyModel.AccessToken, secret, nickname);
                    data = Common.SortURLParamAsc(data);
                    string sign = Common.GetSignStr(data);
                    data += "&_sign=" + sign;

                    UpStreamViewModel upstreamVM = UpStreamViewModel.SingleInstance;
                    upstreamVM.UploadAsync(Utils.MeHelper.ConnectAccountUrl, data, wc_UploadStringCompleted);
                };
                Utils.MeHelper.GetWeiboUserNickname(thirdPartyModel.AccessToken, thirdPartyModel.OpenId, getNicknameCompleted);
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
                            int userID = resultToken.SelectToken("UserId").Value<int>();
                            int userState = resultToken.SelectToken("UserState").Value<int>();
                            string auth = resultToken.SelectToken("PcpopClub").ToString();
                            string nickName = resultToken.SelectToken("LoginCode").ToString();

                            //返回登录前页面
                            for (int i = 0; i < 3; i++)
                            {
                                NavigationService.RemoveBackEntry();
                            }
                        }
                    }
                    else
                    {
                        string msg = string.IsNullOrEmpty(message) ? "关联失败" : message;
                        Common.showMsg(msg);
                    }
                }
            }
            catch
            { }
        }
    }
}