using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Net.NetworkInformation;
using Model;
using Model.Me;
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
                        return;
                    }
                }

                List<PrivateMessageModel> latest = this.MessageVM.MessageList.Where((model, index) => index >= cnt - 50).ToList();
                PrivateMessageCacheViewModel.SingleInstance.UpdateMessages(this.FriendID, latest);
            }
        }

        //load history.
        private void LoadMore(bool restart)
        {
            GlobalIndicator.Instance.Text = "正在获取中...";
            GlobalIndicator.Instance.IsBusy = true;

            int baseMessageID = restart ? int.MaxValue : this.MessageVM.MessageList[0].ID;
            string url = Utils.MeHelper.GetPrivateMessageUrl(this.FriendID, baseMessageID, 1, 1);
            this.MessageVM.LoadDataAysnc(url);
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

        //load more.
        private void MessageListBox_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (e.TotalManipulation.Translation.Y > e.TotalManipulation.Translation.X && e.TotalManipulation.Translation.Y > 5)
            {
                var scrollViewer = e.ManipulationContainer as ScrollViewer;
                if (scrollViewer != null && scrollViewer.VerticalOffset == 0)
                {
                    this.LoadMore(false);
                }
            }
        }

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

        //清空剪贴板
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
            int inputLength = this.LetterNumTextBlock.Text.Length;
            int showLength = inputLength > maxReplyLength ? maxReplyLength - inputLength : inputLength;
            this.LetterNumTextBlock.Text = string.Format("{0}/{1}", showLength, maxReplyLength);
            this.LetterNumTextBlock.Foreground = showLength < 0 ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Black);
            this.ReplyTextBlock.Foreground = showLength < 0 ? new SolidColorBrush(Colors.Gray) : (SolidColorBrush)App.Current.Resources["App_Theme_Color"];
            this.ReplyTextBlock.Tag = showLength >= 0;

            bool multiLine = this.ReplyTextBox.ActualHeight > 80;
            this.LetterNumTextBlock.Visibility = multiLine ? Visibility.Visible : Visibility.Collapsed;
        }

        private bool isSending = false;

        //发送私信
        private void Reply_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.ReplyTextBox.Text) && this.ReplyTextBlock.Text.Length <= maxReplyLength)
            {
                //发送文字
                if (!isSending)
                {
                    var setting = IsolatedStorageSettings.ApplicationSettings;
                    if (setting.Contains("userInfo"))
                    {
                        var content = this.ReplyTextBox.Text;
                        var userInfoModel = setting["userInfo"] as Model.MyForumModel;
                        var timeStamp = Common.GetTimeStamp();
                        string strData = "_appid=app.wp"
                                + "&authorization=" + userInfoModel.Authorization
                                + "&acceptmemberid=" + FriendID
                                + "&acceptname=" + friendName
                                + "&lettercontent=" + Utils.MeHelper.UTF8ToGB2312(content)
                                + "&_timestamp=" + timeStamp//时间戳
                                + "&autohomeua=" + Common.GetAutoHomeUA(); //设备类型\t系统版本号\tautohome\t客户端版本号
                        //生成_sign
                        string sign = Common.GetSignStr(strData);
                        strData += "&_sign=" + sign;
                        this.SendData(strData);

                        this.ReplyTextBox.Text = string.Empty;
                        this.ReplyTextBox.Focus();

                        //add new item to listbox
                        PrivateMessageModel model = new PrivateMessageModel();
                        model.Content = content;
                        model.LastPostDate = timeStamp;
                        model.UserID = FriendID;
                        this.MessageVM.MessageList.Add(model);
                    }
                }
            }
        }

        private void SendData(string strData)
        {
            string url = Utils.MeHelper.SendPrivateMessageUrl;

            ViewModels.Me.UpStreamViewModel upstreamVM = ViewModels.Me.UpStreamViewModel.SingleInstance;
            upstreamVM.UploadAsyncWithSharedClient(url, strData, wc_UploadStringCompleted);
        }

        private void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs ee)
        {
            if (ee.Error != null)
            {
                //红圆叹号
            }
            else
            {
                //图标消失
            }
            isSending = false;
        }

        #endregion
    }
}