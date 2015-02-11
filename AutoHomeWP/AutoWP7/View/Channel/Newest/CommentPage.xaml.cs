using Microsoft.Phone.Controls;
using Model;
using ViewModels;
using AutoWP7.Handler;
using Newtonsoft.Json.Linq;
using System;
using System.Windows;

namespace AutoWP7.View.Channel.Newest
{
    public partial class CommentPage : PhoneApplicationPage
    {

        public CommentPage()
        {
            InitializeComponent();
        }

        //文章id
        string newsId = string.Empty;
        //回复id
        string replyId = string.Empty;
        //用户id
        string userId = string.Empty;
        //密钥
        string authorization = string.Empty;
        //数据类型1-文章类，2-说客，3-视频
        int pageType = 1;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {

                        newsId = this.NavigationContext.QueryString["newsid"];
                        replyId = this.NavigationContext.QueryString["replyid"];
                        userId = this.NavigationContext.QueryString["userid"];
                        authorization = this.NavigationContext.QueryString["authorization"];
                        pageType = Convert.ToInt16(NavigationContext.QueryString["pageType"]);
                    }
                    break;
            }
        }

        bool isSend = false;
        //发送评论内容
        private void sendMsg_Click(object sender, System.EventArgs e)
        {
            string content = CommentContent.WaterContent.ToString();
            if (string.IsNullOrEmpty(content))
            {
                Common.showMsg("您还没有填写评论内容噢~~");
            }
            else
            {
                if (isSend == false)
                {
                    isSend = true;
                    SendData();
                }
            }

        }

        ReplyViewModel replyVM;
        public void SendData()
        {
            GlobalIndicator.Instance.Text = "正在发送中...";
            GlobalIndicator.Instance.IsBusy = true;
            CommentReplyModel model = new CommentReplyModel();
            model.NewsId = newsId;
            model.ReplyId = replyId;
            model.UserId = userId;
            model.Authorization = authorization;
            //文章评论，提交，对 \ " &字符的特殊兼容处理
            model.Content = CommentContent.Text.ToString().Replace("\\", "\\\\").Replace("\"", "'").Replace("&", " ");

            if (replyVM == null)
            {
                replyVM = new ReplyViewModel();
                replyVM.LoadDataCompleted += replyVM_LoadDataCompleted;
            }

            string contentType = "1";
            switch (pageType)
            {
                case 1:
                    contentType = "1";
                    break;
                case 2:
                    contentType = "7";
                    break;
                case 3:
                    contentType = "4";
                    break;
                default:
                    break;
            }
            replyVM.sendData(string.Format("{0}/api/create2.ashx", App.replyUrl), model, pageType, App.UserAgent, contentType);
        }

        void replyVM_LoadDataCompleted(object sender, ViewModels.Handler.APIEventArgs<string> e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;
            //重置标志位
            isSend = false;
            JObject json = JObject.Parse(e.Result);
            int isSuccess = (int)json.SelectToken("returncode");
            if (isSuccess == 0)
            {
                Common.showMsg("评论成功啦~~");
                //标志回复
                App.IsLoadTag = true;
                //返回
                this.NavigationService.GoBack();
            }
            else
            {
                Common.showMsg("发送失败");
            }
        }
    }
}