using System;
using System.IO.IsolatedStorage;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Model;
using Newtonsoft.Json.Linq;
using ViewModels.Handler;
using System.Net;
using System.Windows;
using Model.Me;
using ViewModels.Me;
using System.Linq;

namespace AutoWP7.View.Forum
{
    /// <summary>
    /// 论坛回复页
    /// </summary>
    public partial class ReplyCommentPage : PhoneApplicationPage
    {
        public ReplyCommentPage()
        {
            InitializeComponent();

        }

        DraftModel sharedModel;
        string topicTitle = string.Empty;
        //论坛id
        string bbsId = string.Empty;
        //论坛板块 (c车系论坛 a地区论坛 o主题论坛)
        string bbsType = string.Empty;
        //帖子id
        string topicId = string.Empty;
        //回复目标楼层id
        string targetReplyId = string.Empty;
        //要请求的URL地址
        string url = string.Empty;
        //string replyId = "0";
        string pageindex = "0";

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {
                        UmengSDK.UmengAnalytics.onEvent("ForumActivity", "回贴");
                        bbsId = this.NavigationContext.QueryString["bbsId"];
                        bbsType = this.NavigationContext.QueryString["bbsType"];
                        topicId = this.NavigationContext.QueryString["topicId"];
                        topicTitle = this.NavigationContext.QueryString["title"];
                        targetReplyId = this.NavigationContext.QueryString["targetReplyId"];
                        url = this.NavigationContext.QueryString["url"];
                        pageindex = this.NavigationContext.QueryString["pageindex"];
                        //第一次进入页面验证是否用户登录
                        if (!Common.isLogin())
                        {
                            this.NavigationService.Navigate(new Uri("/View/More/LoginPage.xaml", UriKind.Relative));
                        }

                        //this is a draft
                        if (this.NavigationContext.QueryString.ContainsKey("savedTime"))
                        {
                            string savedTime = this.NavigationContext.QueryString["savedTime"];
                            var draftList = DraftViewModel.SingleInstance.DraftList;
                            sharedModel = draftList.FirstOrDefault(item => item.SavedTime.ToString() == savedTime);
                            if (sharedModel != null)
                            {
                                this.replyContent.Text = sharedModel.Content;
                            }
                        }

                        break;
                    }
                case System.Windows.Navigation.NavigationMode.Back:
                    {

                    }
                    break;
            }
        }

        //标识是否正在发送中
        bool isSending = false;
        //发送回复
        private void sendReply_Click(object sender, EventArgs e)
        {
            var setting = IsolatedStorageSettings.ApplicationSettings;
            string key = "userInfo";
            if (setting.Contains(key))
            {
                if (isSending == false)
                {
                    isSending = true;
                    //检查是否已经登录
                    string content = replyContent.Text;
                    MyForumModel userInfoModel = null;
                    if (string.IsNullOrEmpty(content))
                    {
                        Common.showMsg("您还没有输入要回复的内容哦~~");
                    }
                    else
                    {
                        userInfoModel = setting[key] as MyForumModel;
                        string sign = string.Empty;
                        string strData = "_appid=app"
                            + "&_timestamp=" + (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000//时间戳
                            + "&autohomeua=" + Common.GetAutoHomeUA()//设备类型\t系统版本号\tautohome\t客户端版本号
                            + "&bbsid=" + bbsId
                            + "&content=" + content
                            + "&imei=" + Common.GetDeviceID()//设备唯一标识
                            + "&targetreplyid=" + targetReplyId
                            + "&topicid=" + topicId
                            + "&uc_ticket=" + userInfoModel.Authorization;

                        //生成_sign
                        sign = Common.GetSignStr(strData);

                        strData += "&_sign=" + sign;

                        SendData(strData);
                    }
                }
                else
                { }
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/View/More/LoginPage.xaml", UriKind.Relative));
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="userKey">用户key</param>
        /// <param name="strData">数据</param>
        private void SendData(string strData)
        {
            try
            {
                GlobalIndicator.Instance.Text = "正在发送中...";
                GlobalIndicator.Instance.IsBusy = true;

                var wc = new WebClient();
                wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
                wc.Headers["Accept-Charset"] = "utf-8";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                wc.Headers["User-Agent"] = App.UserAgent;
                string url = App.clubUrl + "/api/reply/appadd";
                Uri urlSource = new Uri(url, UriKind.Absolute);
                wc.UploadStringAsync(urlSource, "POST", strData);

                wc.UploadStringCompleted += new UploadStringCompletedEventHandler((ss, ee) =>
                {
                    APIEventArgs<string> apiArgs = new APIEventArgs<string>();

                    if (ee.Error != null)
                    {
                        apiArgs.Error = ee.Error;
                        Common.showMsg("发送失败");
                        GlobalIndicator.Instance.Text = "评论失败";
                    }
                    else
                    {
                        JObject json = JObject.Parse(ee.Result);
                        int returnCode = (int)json.SelectToken("returncode");

                        if (returnCode != 0)
                        {
                            string strMsg = (string)json.SelectToken("message");
                            Common.showMsg(strMsg);
                            GlobalIndicator.Instance.Text = "评论失败";
                        }
                        else
                        {
                            int topicId = (int)json.SelectToken("result").SelectToken("topicid");
                            int replyid = (int)json.SelectToken("result").SelectToken("replyid");
                            int pageIndex = (int)json.SelectToken("result").SelectToken("pageindex");
                            int isSmallImageMode = Utils.MeHelper.GetIsSmallImageMode() ? 1 : 0;
                            App.TopicUrl = AppUrlMgr.TopicWebViewUrl(Convert.ToInt64(topicId), 0, pageIndex, 20, 1, 0, 0, isSmallImageMode, replyid, 480, 0);
                            App.pageIndex = pageIndex;
                            if (pageindex != pageIndex.ToString())
                                App.IsLoadTag = false;
                            else
                                App.IsLoadTag = true;

                            if (sharedModel != null)
                            {
                                DraftViewModel.SingleInstance.RemoveDraft(new DateTime[] { sharedModel.SavedTime });
                            }

                            this.NavigationService.GoBack();
                            Common.showMsg("发送成功");
                            //this.NavigationService.Navigate(new Uri("/View/Forum/TopicDetailPage.xaml?from=0&bbsId=" + bbsId + "&topicId=" + topicId + "&bbsType=" + bbsType + "&floor=" + floor, UriKind.Relative));
                        }
                    }
                    isSending = false;
                });
            }
            catch
            { }
        }

        #region 取消发帖时询问是否保存到草稿箱

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (sharedModel != null)//更新草稿
            {
                sharedModel.Content = replyContent.Text;
                sharedModel.SavedTime = DateTime.Now;
                DraftViewModel.SingleInstance.SaveDraft();
            }
            else
            {
                if (MessageBox.Show("尚未发送，是否保存到草稿箱？", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    DraftModel model = new DraftModel();
                    model.Title = topicTitle;
                    model.BBSID = bbsId;
                    model.BBSType = bbsType;
                    model.Content = replyContent.Text;
                    model.SavedTime = DateTime.Now;
                    model.TopicID = topicId;
                    model.TargetReplyID = targetReplyId;
                    DraftViewModel.SingleInstance.AddDraft(model);
                }
            }
            base.OnBackKeyPress(e);
        }

        #endregion
    }
}