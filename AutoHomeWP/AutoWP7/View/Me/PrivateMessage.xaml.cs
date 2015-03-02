using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Model;
using Model.Me;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using ViewModels.Me;

namespace AutoWP7.View.Me
{
    public partial class PrivateMessage : PhoneApplicationPage, INotifyPropertyChanged
    {
        private const int maxReplyLength = 300;

        private string friendName;

        #region bindable properties and INotifyPropertyChanged implementation

        private int _friendID;
        public int FriendID
        {
            get { return _friendID; }
            set
            {
                if (_friendID != value)
                {
                    _friendID = value;
                    this.OnPropertyChanged("FriendID");
                }
            }
        }

        private string _friendImg;
        public string FriendImg
        {
            get { return _friendImg; }
            set
            {
                if (_friendImg != value)
                {
                    _friendImg = value;
                    this.OnPropertyChanged("FriendImg");
                }
            }
        }

        private string _meImg;
        public string MeImg
        {
            get { return _meImg; }
            set
            {
                if (_meImg != value)
                {
                    _meImg = value;
                    this.OnPropertyChanged("MeImg");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private Timer updateTimer;

        private PrivateMessageViewModel MessageVM;

        public PrivateMessage()
        {
            InitializeComponent();

            this.MessageVM = new PrivateMessageViewModel();
            this.MessageVM.LoadDataCompleted += FriendsVM_LoadDataCompleted;
            this.ContentPanel.DataContext = this.MessageVM;
            this.LoadLocally();

            updateTimer = new Timer(UpdateMessage);

            this.MessageListBox.Loaded += (sender, e) => AddScrollEvent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.New)
            {
                this.friendName = this.NavigationContext.QueryString["username"];
                this.PageTitle.Text = this.friendName;

                this.FriendID = int.Parse(this.NavigationContext.QueryString["userId"]);
                this.FriendImg = this.NavigationContext.QueryString["userimg"];
                this.MeImg = Utils.MeHelper.GetMyInfoModel().UserPic;
            }

            this.LoadMore(true);
            SetTimerWithNetwork();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            updateTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            base.OnNavigatedFrom(e);
        }

        #region auto update

        //load latest
        private void UpdateMessage(object state)
        {
            SetTimerWithNetwork();

            int count = this.MessageVM.MessageList.Count;
            int baseMessageID = count > 0 ? this.MessageVM.MessageList[count - 1].ID : 0;
            string url = Utils.MeHelper.GetPrivateMessageUrl(this.FriendID, baseMessageID, 0, 1);
            this.MessageVM.LoadDataAysnc(url);
        }

        private void SetTimerWithNetwork()
        {
            //获取网络状态
            if (DeviceNetworkInformation.IsWiFiEnabled)
            {
                this.updateTimer.Change(0, 15000);
            }
            else if (DeviceNetworkInformation.IsCellularDataEnabled)
            {
                this.updateTimer.Change(0, 30000);
            }
        }

        #endregion

        #region load data

        bool isLoading = false;

        //确保加载新的后页面不滚动
        PrivateMessageModel visibleItem;

        /// <summary>
        /// Load cached data.
        /// </summary>
        private void LoadLocally()
        {
            this.MessageVM.ClearData();

            var cacheModel = PrivateMessageCacheViewModel.SingleInstance.CacheModel;
            if (cacheModel.Messages.ContainsKey(this.FriendID))
            {
                foreach (var item in cacheModel.Messages[this.FriendID])
                {
                    this.MessageVM.MessageList.Add(item);
                }
            }
        }

        private void FriendsVM_LoadDataCompleted(object sender, EventArgs e)
        {
            GlobalIndicator.Instance.Text = "";
            GlobalIndicator.Instance.IsBusy = false;

            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (visibleItem != null)
                {
                    this.MessageListBox.UpdateLayout();
                    this.MessageListBox.ScrollIntoView(visibleItem);
                }

                if (this.MessageVM.ReturnCode == 10001 || this.MessageVM.ReturnCode == 10002)
                {
                    CustomMessageBox messageBox = new CustomMessageBox()
                    {
                        Caption = "",
                        Message = "密码检验失败",
                        LeftButtonContent = "取消",
                        RightButtonContent = "重新登录",
                        IsFullScreen = false
                    };

                    messageBox.Dismissed += (s1, e1) =>
                    {
                        switch (e1.Result)
                        {
                            case CustomMessageBoxResult.LeftButton:
                                NavigationService.Navigate(new Uri("/View/More/LoginPage.xaml", UriKind.RelativeOrAbsolute));
                                break;
                            case CustomMessageBoxResult.RightButton:
                                //返回未登录下我的主页
                                NavigationService.GoBack();
                                break;
                            default:
                                break;
                        }
                    };

                    messageBox.Show();
                }
                else if (this.MessageVM.ReturnCode == 0)
                {
                    //update local cache.
                    int cnt = this.MessageVM.MessageList.Count;
                    if (cnt > 0)
                    {
                        var cacheModel = PrivateMessageCacheViewModel.SingleInstance.CacheModel;
                        if (cacheModel.Messages.ContainsKey(this.FriendID))
                        {
                            var messages = cacheModel.Messages[this.FriendID];
                            if (messages != null && messages.Count > 0 && this.MessageVM.MessageList[cnt - 1].ID <= messages.Last().ID)
                            {
                                isLoading = false;
                                return;
                            }
                        }

                        List<PrivateMessageModel> latest = this.MessageVM.MessageList.Where((model, index) => index >= cnt - 50).ToList();
                        PrivateMessageCacheViewModel.SingleInstance.UpdateMessages(this.FriendID, latest);
                    }
                }

                isLoading = false;
            });
        }

        //load history.
        private void LoadMore(bool restart)
        {
            if (this.MessageVM.RowCount > 0 && this.MessageVM.MessageList.Count >= this.MessageVM.RowCount)
            {
                Common.showMsg("没有更早的私信了。");
            }
            else if (!isLoading)
            {
                isLoading = true;

                GlobalIndicator.Instance.Text = "正在获取中...";
                GlobalIndicator.Instance.IsBusy = true;

                int baseMessageID = restart ? int.MaxValue : this.MessageVM.MessageList[0].ID;
                string url = Utils.MeHelper.GetPrivateMessageUrl(this.FriendID, baseMessageID, 1, 1);
                this.MessageVM.LoadDataAysnc(url);

                if (this.MessageVM.MessageList.Count > 0)
                {
                    visibleItem = this.MessageVM.MessageList[0];
                }
            }
        }

        #endregion

        #region UI interaction

        private void LoadMore_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.LoadMore(false);
        }

        //隐藏键盘
        private void MessageListBox_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            this.Focus();
        }

        #region 滚动到顶端自动加载

        private void AddScrollEvent()
        {
            var scrollviewer = AutoWP7.Handler.Common.FindChildOfType<ScrollViewer>(this.MessageListBox);
            if (scrollviewer != null)
            {
                scrollviewer.MouseMove += scrollviewer_MouseMove;
            }
        }

        //加载更多.
        void scrollviewer_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var scrollviewer = sender as ScrollViewer;
            if (scrollviewer.VerticalOffset <= 0.1)
            {
                this.LoadMore(false);
            }
        }

        #endregion

        //复制到剪贴板
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            var model = element.DataContext as PrivateMessageModel;
            if (model != null)
            {
                Clipboard.SetText(model.Content);
            }
        }

        //清空剪贴板,没用到
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var element = sender as FrameworkElement;
            var model = element.DataContext as PrivateMessageModel;
            if (model != null)
            {
                Clipboard.SetText(string.Empty);
            }
        }

        //超过一行时显示汉字数
        private void ReplyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int inputLength = (int)CommonLayer.StringHelper.GetStringChLength(this.ReplyTextBox.Text);
            int showLength = inputLength > maxReplyLength ? maxReplyLength - inputLength : inputLength;
            this.LetterNumTextBlock.Text = string.Format("{0}/{1}", showLength, maxReplyLength);
            this.LetterNumTextBlock.Foreground = showLength < 0 ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
            this.ReplyTextBlock.Foreground = showLength < 0 ? new SolidColorBrush(Colors.Gray) : (SolidColorBrush)App.Current.Resources["App_Theme_Color"];
            this.ReplyTextBlock.Tag = showLength >= 0;

            bool multiLine = this.ReplyTextBox.ActualHeight > 80;
            this.LetterNumTextBlock.Visibility = multiLine ? Visibility.Visible : Visibility.Collapsed;
        }

        #region 发送私信

        private bool isSending = false;

        //发送私信
        private void Reply_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.ReplyTextBox.Text) && this.ReplyTextBox.Text.Length <= maxReplyLength)
            {
                //发送文字
                if (!isSending)
                {
                    var userInfoModel = Utils.MeHelper.GetMyInfoModel();
                    if (userInfoModel != null)
                    {
                        var content = this.ReplyTextBox.Text;
                        var timeStamp = Common.GetTimeStamp();

                        //add new item to listbox
                        var model = new PrivateMessageNewModel();
                        model.Content = content;
                        model.LastPostDate = timeStamp;
                        model.UserID = userInfoModel.Id;
                        model.SendingState = 2;
                        this.MessageVM.MessageList.Add(model);

                        //发送
                        this.SendMessage(model);

                        this.ReplyTextBox.Text = string.Empty;
                        this.ReplyTextBox.Focus();

                        this.MessageListBox.UpdateLayout();
                        this.MessageListBox.ScrollIntoView(model);
                    }
                }
            }
        }

        private void SendData(string strData, object userState)
        {
            string url = Utils.MeHelper.SendPrivateMessageUrl;

            ViewModels.Me.UpStreamViewModel upstreamVM = ViewModels.Me.UpStreamViewModel.SingleInstance;
            upstreamVM.UploadAsync(url, strData, wc_UploadStringCompleted, userState);
        }

        private void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            isSending = false;

            var model = e.UserState as PrivateMessageNewModel;
            if (model != null)
            {
                if (e.Error == null && e.Result != null)
                {
                    try
                    {
                        //返回的json数据
                        JObject json = JObject.Parse(e.Result);
                        JToken resultToken = json.SelectToken("result");
                        var returnCode = json.SelectToken("returncode").Value<int>();

                        if (returnCode == 0)
                        {
                            //成功
                            int letterid = resultToken.SelectToken("letterid").Value<int>();
                            string postDate = resultToken.SelectToken("time").ToString();
                            model.ID = letterid;
                            model.LastPostDate = postDate;
                            model.SendingState = 0;
                            return;
                        }
                    }
                    catch
                    {
                    }
                }

                model.SendingState = 1;
            }


        }

        private void ReSend_Click(object sender, RoutedEventArgs e)
        {
            var model = (sender as FrameworkElement).DataContext as PrivateMessageNewModel;
            if (model != null && model.SendingState == 1)
            {
                var setting = IsolatedStorageSettings.ApplicationSettings;
                if (setting.Contains("userInfo"))
                {
                    model.SendingState = 2;
                    this.SendMessage(model);
                }
            }
        }

        private void SendMessage(PrivateMessageNewModel model)
        {
            var userInfoModel = Utils.MeHelper.GetMyInfoModel();
            if (userInfoModel != null)
            {
                model.SendingState = 2;
                string strData = "_appid=" + Utils.MeHelper.appID
                                   + "&authorization=" + userInfoModel.Authorization
                                   + "&acceptmemberid=" + FriendID
                                   + "&acceptname=" + friendName
                                   + "&lettercontent=" + Utils.MeHelper.UTF8ToGB2312(model.Content)
                                   + "&_timestamp=" + Common.GetTimeStamp()//时间戳
                                   + "&autohomeua=" + Common.GetAutoHomeUA();//设备类型\t系统版本号\tautohome\t客户端版本号
                strData = Common.SortURLParamAsc(strData);
                //生成_sign
                string sign = Common.GetSignStr(strData);
                strData += "&_sign=" + sign;
                this.SendData(strData, model);
            }
        }

        #endregion

        #endregion
    }
}