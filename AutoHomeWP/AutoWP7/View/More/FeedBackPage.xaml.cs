using System;
using System.IO.IsolatedStorage;
using System.Net;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Info;
using Microsoft.Phone.Net.NetworkInformation;
using Model;
using Newtonsoft.Json.Linq;
using ViewModels.Handler;
using System.Text;

namespace AutoWP7.View.More
{
    /// <summary>
    /// 反馈页
    /// </summary>
    public partial class FeedBackPage : PhoneApplicationPage
    {
        public FeedBackPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //  wb.Navigate(new Uri("http://m.autohome.com.cn/bug/wp/index.aspx", UriKind.Absolute));
        }


        /// <summary>
        /// 获得网络类型
        /// </summary>
        /// <returns>返回字符串</returns>
        public static string GetNetStates()
        {
            var info = Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType;

            switch (info)
            {
                case NetworkInterfaceType.MobileBroadbandCdma:
                    return "CDMA";
                case NetworkInterfaceType.MobileBroadbandGsm:
                    return "CSM";
                case NetworkInterfaceType.Wireless80211:
                    return "WiFi";
                case NetworkInterfaceType.Ethernet:
                    return "Ethernet";
                case NetworkInterfaceType.None:
                    return "None";
                default:
                    return "Other";
            }
        }

        ///<summary>
        /// 获取设备唯一标识
        ///</summary>
        ///<returns>返回字符串</returns>
        public static string GetDeviceUniqueId()
        {
            byte[] byteArray = DeviceExtendedProperties.GetValue("DeviceUniqueId") as byte[];
            string strTemp = "";
            string strDeviceUniqueID = "";
            foreach (byte b in byteArray)
            {
                strTemp = b.ToString();
                if (1 == strTemp.Length)
                {
                    strTemp = "00" + strTemp;
                }
                else if (2 == strTemp.Length)
                {
                    strTemp = "0" + strTemp;
                }
                strDeviceUniqueID += strTemp;
            }
            return strDeviceUniqueID;
        }


        private bool isSending = false;
        //意见反馈
        private void sendAdvice_Click(object sender, EventArgs e)
        {
            if (isSending == false)
            {
                isSending = true;

                string strContent = adviceConent.Text;
                string strPhoneNumber = advicePhoneNumber.Text;
                if (!string.IsNullOrEmpty(strContent))
                {
                    Version version = new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName).Version;

                    //手机型号
                    string phoneType = DeviceExtendedProperties.GetValue("DeviceName").ToString();

                    OperatingSystem os = Environment.OSVersion;
                    Version phoneSystemVersion = os.Version;

                    //联网类型
                    string networkType = GetNetStates();

                    //用户名
                    string userName = string.Empty;
                    var setting = IsolatedStorageSettings.ApplicationSettings;
                    string key = "userInfo";
                    MyForumModel userInfoModel = null;
                    if (setting.Contains(key))//已经登录
                    {
                        userInfoModel = setting[key] as MyForumModel;
                        userName = userInfoModel.UserName;
                    }
                    strContent = strContent.Replace('"', '“').Replace(':', '：');


                    //设备唯一标识
                    string deviceId = GetDeviceUniqueId();

                    string strMsg = "fj={\"PID\":\"2\",\"PV\":\"" + version + "\",\"DT\":\"" +
                        phoneType + "\",\"SID\":\"3\",\"SV\":\"" + phoneSystemVersion + "\",\"NT\":\"" +
                        networkType + "\",\"CT\":\"" + strPhoneNumber + "\",\"C\":\"" + strContent + "\",\"UN\":\"" +
                        userName + "\",\"DID\":\"" + deviceId + "\"}";

                    SendData(strMsg);
                }
                else
                {
                    isSending = false;
                    Common.showMsg("请输入您要反馈的信息~~谢谢");
                }
            }
        }
        WebClient wc = null;
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="strData">数据</param>
        private void SendData(string data)
        {
            try
            {
                GlobalIndicator.Instance.Text = "正在发送中...";
                GlobalIndicator.Instance.IsBusy = true;

                if (wc == null)
                {
                    wc = new WebClient();
                }

                if (!wc.IsBusy)
                {
                    wc.Encoding = Encoding.UTF8;
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";


                    string url = string.Format("{0}{1}/mobile/feedback.ashx", App.appUrl, App.versionStr);
                    wc.UploadStringAsync(new Uri(url), "POST", data);
                    wc.UploadStringCompleted += new UploadStringCompletedEventHandler((ss, ee) =>
                    {

                        isSending = false;
                        GlobalIndicator.Instance.Text = "";
                        GlobalIndicator.Instance.IsBusy = false;

                        if (ee.Error != null)
                        {
                            Common.showMsg("发送失败");
                        }
                        else
                        {
                            JObject json = JObject.Parse(ee.Result);
                            int success = (int)json.SelectToken("returncode");
                            if (success == 0)
                            {

                                adviceConent.Text = "";
                                advicePhoneNumber.Text = "";
                                this.NavigationService.GoBack();
                                Common.showMsg("发送成功");
                            }
                            else
                            {
                                //提示错误消息
                                string strMsg = (string)json.SelectToken("message");
                                Common.showMsg(strMsg);
                            }
                        }
                    });

                }
            }
            catch
            { }
        }
    }


}