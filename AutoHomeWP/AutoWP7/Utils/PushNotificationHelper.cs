using Microsoft.Phone.Notification;
using Microsoft.Phone.Shell;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Net;

namespace AutoWP7.Utils
{
    public class PushNotificationHelper
    {
        public const string channelName = "autohomeChannel";
        public const string pushSettingKey = "pushSetting";

        const int AppID = 100010;
        const int DeviceType = 5;
        const string RegisterBaseUrl = "http://push.app.autohome.com.cn/user/reg";
        const string SaveSettingBaseUrl = "http://push.app.autohome.com.cn/user/clientsetting/save";
        const string GetSettingBaseUrl = "http://push.app.autohome.com.cn/user/clientsetting/get";
        const string UnRegisterBaseUrl = "http://push.app.autohome.com.cn/user/unreg";
        const string SampleBaseUrl = "http://push.app.autohome.com.cn/history/postmessage";

        public static event EventHandler<ToastNotificationEventArgs> ToastNotificationReceived;

        /// <summary>
        /// 如有变更，重新注册新设备
        /// </summary>
        public static string ChannelUrl
        {
            get
            {
                if (PhoneApplicationService.Current.State.ContainsKey(channelName))
                { return (string)PhoneApplicationService.Current.State[channelName]; }
                else
                {
                    return null;
                }
            }
            private set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var oldValue = PhoneApplicationService.Current.State.ContainsKey(channelName) ? (string)PhoneApplicationService.Current.State[channelName] : null;
                    PhoneApplicationService.Current.State[channelName] = value;
                    if (oldValue != value)
                    {
                        var myInfo = MeHelper.GetMyInfoModel();
                        string userId = myInfo == null ? null : myInfo.UserID.ToString();
                        RegisterNewDevice(userId);
                    }
                }
                else
                {
                    PhoneApplicationService.Current.State[channelName] = value;
                }
            }
        }

        #region 打开推动通道

        public static void OpenChannel()
        {
            HttpNotificationChannel pushChannel = HttpNotificationChannel.Find(channelName);
            if (pushChannel == null)
            {
                pushChannel = new HttpNotificationChannel(channelName);

                // Register for all the events before attempting to open the channel.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);
                pushChannel.ShellToastNotificationReceived += pushChannel_ShellToastNotificationReceived;

                pushChannel.Open();
                pushChannel.BindToShellToast();
            }
            else
            {
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);
                pushChannel.ShellToastNotificationReceived += pushChannel_ShellToastNotificationReceived;
                ChannelUrl = pushChannel.ChannelUri.AbsoluteUri;
            }
        }

        private static void pushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            ToastNotificationEventArgs args = new ToastNotificationEventArgs();
            foreach (string key in e.Collection.Keys)
            {
                if (key == "wp:Text1")
                {
                    args.Title = e.Collection[key];
                }
                else if (key == "wp:Text2")
                {
                    args.Subtitle = e.Collection[key];
                }
                else if (key == "wp:Param")
                {
                    args.CustomParam = e.Collection[key];
                }
            }

            if (ToastNotificationReceived != null)
            {
                ToastNotificationReceived(sender, args);
            }
        }

        private static void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            ChannelUrl = null;
        }

        private static void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            ChannelUrl = e.ChannelUri.AbsoluteUri;
        }

        #endregion

        #region 推送接口

        /// <summary>
        /// 注册推送通知服务
        /// </summary>
        public static void RegisterNewDevice(string userId)
        {
            if (!string.IsNullOrEmpty(ChannelUrl))
            {
                var deviceName = Handler.Common.GetAutoHomeUA();
                string regUrl = string.Format(RegisterBaseUrl + "?appId={0}&deviceType={1}&deviceToken={2}&deviceName={3}&userId={4}", AppID, DeviceType, ChannelUrl, deviceName, userId);

                WebClient wc = new WebClient();
                wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
                wc.Headers["Accept-Charset"] = "utf-8";
                wc.DownloadStringCompleted += wc_DownloadStringCompleted;
                wc.DownloadStringAsync(new Uri(regUrl, UriKind.Absolute));
            }
        }

        /// <summary>
        /// 注册新设备和获取推送设置回调
        /// </summary>
        private static void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && e.Result != null)
                {
                    JObject json = JObject.Parse(e.Result);
                    int returnCode = json.SelectToken("returncode").Value<int>();
                    if (returnCode == 0)//成功
                    {
                        JToken resultToken = json.SelectToken("result.list");
                        var pushSettingCollection = CommonLayer.JsonHelper.DeserializeOrDefault<List<Model.Me.PushNotificationSettingModel>>(resultToken.ToString());
                        if (pushSettingCollection != null && pushSettingCollection.Count > 0)//保存推送设置
                        {
                            IsolatedStorageSettings.ApplicationSettings[pushSettingKey] = pushSettingCollection[0];
                            IsolatedStorageSettings.ApplicationSettings.Save();
                        }
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 提交客户端的推送设置
        /// </summary>
        public static void SaveUserSetting(string userId, bool allowSystem, bool allowPerson, int startTime, int endTime, EventHandler<bool> saveCompleted = null)
        {
            if (!string.IsNullOrEmpty(ChannelUrl))
            {
                var deviceName = Handler.Common.GetAutoHomeUA();
                var allowSys = allowSystem ? 0 : 1;
                var allowPer = allowPerson ? 0 : 1;
                string saveUrl = string.Format("{0}?appId={1}&deviceType={2}&deviceToken={3}&deviceName={4}&userId={5}&allowSystem={6}&allowPerson={7}&startTime={8}&endTime={9}",
                    SaveSettingBaseUrl, AppID, DeviceType, ChannelUrl, deviceName, userId, allowSys, allowPer, startTime, endTime);

                WebClient wc = new WebClient();
                wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
                wc.Headers["Accept-Charset"] = "utf-8";
                wc.DownloadStringCompleted += (sender, e) =>
                    {
                        bool saveSuccess = false;
                        try
                        {
                            if (e.Error == null && e.Result != null)
                            {
                                JObject json = JObject.Parse(e.Result);
                                int returnCode = json.SelectToken("returncode").Value<int>();
                                if (returnCode == 0)//成功,保存到本地
                                {
                                    saveSuccess = true;

                                    var settings = IsolatedStorageSettings.ApplicationSettings;
                                    if (settings.Contains(pushSettingKey))
                                    {
                                        var pushSetting = settings[pushSettingKey] as Model.Me.PushNotificationSettingModel;
                                        if (pushSetting == null)
                                        {
                                            pushSetting = new Model.Me.PushNotificationSettingModel();
                                        }
                                        pushSetting.UserId = userId;
                                        pushSetting.NotAllowSystem = !allowSystem;
                                        pushSetting.NotAllowPerson = !allowPerson;
                                        pushSetting.StartTime = startTime;
                                        pushSetting.EndTime = endTime;
                                        settings[pushSettingKey] = pushSetting;
                                        settings.Save();
                                    }
                                }
                            }
                        }
                        catch
                        { }

                        if (saveCompleted != null)
                        {
                            saveCompleted(sender, saveSuccess);
                        }
                    };
                wc.DownloadStringAsync(new Uri(saveUrl, UriKind.Absolute));
            }
        }

        /// <summary>
        /// 获取客户端的推送设置
        /// </summary>
        public static void GetUserSetting(string userId)
        {
            if (!string.IsNullOrEmpty(ChannelUrl))
            {
                var deviceName = Handler.Common.GetAutoHomeUA();
                string getUrl = string.Format("{0}?appId={1}&deviceType={2}&deviceToken={3}&deviceName={4}&userId={5}", GetSettingBaseUrl, AppID, DeviceType, ChannelUrl, deviceName, userId);

                WebClient wcGetting = new WebClient();
                wcGetting.DownloadStringCompleted += wc_DownloadStringCompleted;
                wcGetting.DownloadStringAsync(new Uri(getUrl, UriKind.Absolute));
            }
        }

        /// <summary>
        /// 注销用户设备
        /// </summary>
        public static void UnRegisterDevice(string userId)
        {
            if (!string.IsNullOrEmpty(ChannelUrl))
            {
                var deviceName = Handler.Common.GetAutoHomeUA();
                string unregisterUrl = string.Format("{0}?appId={1}&deviceType={2}&deviceToken={3}&deviceName={4}&userId={5}", UnRegisterBaseUrl, AppID, DeviceType, ChannelUrl, deviceName, userId);

                WebClient wc = new WebClient();
                wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
                wc.Headers["Accept-Charset"] = "utf-8";
                wc.DownloadStringAsync(new Uri(unregisterUrl, UriKind.Absolute));

                var settings = IsolatedStorageSettings.ApplicationSettings;
                if (settings.Contains(pushSettingKey))
                {
                    settings.Remove(pushSettingKey);
                    settings.Save();
                }
            }
        }

        /// <summary>
        /// 采样日志
        /// </summary>
        /// <param name="result">9客户端接收到推送通知时，10客户端在通知中心通过点击或滑动操作进行处理时</param>
        public static void SampleLog(string categoryID, string messageID, string result, string userID)
        {
            string appVersion = Handler.Common.GetSysVersion();
            string deviceID = Handler.Common.GetDeviceID();
            string getUrl = string.Format("{0}?appId={1}&CategoryId={2}&OID={3}&DeviceType={4}&DeviceToken={5}&Result={6}&Version={7}&userId={8}&CityId={9}&DeviceId={10}", SampleBaseUrl, AppID, categoryID, messageID, DeviceType, ChannelUrl, result, appVersion, userID, App.CityId, deviceID);

            WebClient wcGetting = new WebClient();
            wcGetting.DownloadStringAsync(new Uri(getUrl, UriKind.Absolute));
        }

        #endregion
    }

    public class ToastNotificationEventArgs : EventArgs
    {
        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string CustomParam { get; set; }
    }
}
