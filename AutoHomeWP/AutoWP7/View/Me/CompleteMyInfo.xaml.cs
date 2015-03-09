using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using AutoWP7.Handler;
using ViewModels.Handler;
using Newtonsoft.Json.Linq;
using Model;
using System.IO.IsolatedStorage;

namespace AutoWP7.View.Me
{
    public partial class CompleteMyInfo : PhoneApplicationPage
    {
        private string platformId;
        private Model.Me.ThirdPartyAccountModel thirdPartyModel;

        public CompleteMyInfo()
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
                        var settings = IsolatedStorageSettings.ApplicationSettings;
                        if (settings.Contains(Utils.MeHelper.weiboAccountKey))
                        {
                            thirdPartyModel = settings[Utils.MeHelper.weiboAccountKey] as Model.Me.ThirdPartyAccountModel;
                        }
                        break;
                }
            }
        }

        private void ConnectAccountButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/View/Me/ConnectAccount.xaml?platformId=" + this.platformId, UriKind.Relative));
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            Common.showMsg("完善资料失败");
            base.OnBackKeyPress(e);
        }

        private void Submit_Click(object sender, EventArgs e)
        {
            this.Focus();

            bool checkSuccess = this.ThirdPartyRegisterPanel.CheckData();
            if (checkSuccess && thirdPartyModel != null)
            {
                string username = HttpUtility.UrlEncode(this.ThirdPartyRegisterPanel.UsernameTextbox.Text);
                string validcode = this.ThirdPartyRegisterPanel.CodeTextbox.Text;
                string mobile = this.ThirdPartyRegisterPanel.PhoneNoTextBox.Text;
                int expiresin = thirdPartyModel.ExpiresIn;
                string ua = AutoWP7.Handler.Common.GetAutoHomeUA();

                string format = "_appid={0}&autohomeua={1}&_timeStamp={2}&token={3}&openId={4}&username={5}&validcode={6}&mobile={7}&platformid={8}&expiresin={9}";//&refreshtoken={10}&area={11}
                string data = string.Format(format, Utils.MeHelper.appIDWp, ua, Common.GetTimeStamp(), thirdPartyModel.AccessToken, thirdPartyModel.OpenId, username, validcode, mobile, this.platformId, expiresin);
                data = Common.SortURLParamAsc(data);
                string sign = Common.GetSignStr(data);
                data += "&_sign=" + sign;

                GlobalIndicator.Instance.Text = "正在提交...";
                GlobalIndicator.Instance.IsBusy = true;

                string url = Utils.MeHelper.ThirdPartyRegisterUrl;
                var upstreamVM = ViewModels.Me.UpStreamViewModel.SingleInstance;
                upstreamVM.UploadAsync(url, data, wc_UploadStringCompleted);
            }
        }

        private void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs ee)
        {
            GlobalIndicator.Instance.Text = null;
            GlobalIndicator.Instance.IsBusy = false;

            try
            {
                if (ee.Error is WebException)
                {
                    Common.showMsg("当前网络不可用，请检查网络设置");
                }
                else if (ee.Error == null && ee.Cancelled == false)
                {
                    JObject json = JObject.Parse(ee.Result);
                    int resultCode = (int)json.SelectToken("returncode");
                    string message = json.SelectToken("message").ToString();
                    switch (resultCode)
                    {
                        case 0://成功
                            JToken resultToken = json.SelectToken("result");
                            if (resultToken != null)
                            {
                                int userID = resultToken.SelectToken("userid").Value<int>();
                                string mobile = resultToken.SelectToken("mobilephone").ToString();
                                string auth = resultToken.SelectToken("pcpopclub").ToString();
                                string nickName = resultToken.SelectToken("nickname").ToString();

                                //存入文件
                                var model = new MyForumModel();
                                model.Success = 1;
                                model.Message = message;
                                model.UserID = userID;
                                model.UserName = nickName;
                                model.Authorization = auth;

                                var setting = IsolatedStorageSettings.ApplicationSettings;
                                setting["userInfo"] = model;
                                setting.Save();

                                //返回登录页
                                Common.showMsg("完善资料成功");
                                NavigationService.RemoveBackEntry();
                                NavigationService.RemoveBackEntry();
                            }
                            break;
                        default://其他情况...
                            if (!string.IsNullOrEmpty(message))
                            {
                                Common.showMsg(message);
                            }
                            break;
                    }
                }
            }
            catch
            {

            }
        }
    }
}