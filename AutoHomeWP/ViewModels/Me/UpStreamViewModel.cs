using System;
using System.Net;

namespace ViewModels.Me
{
    public class UpStreamViewModel
    {
        private UpStreamViewModel()
        { }

        private static UpStreamViewModel _singleInstance;
        public static UpStreamViewModel SingleInstance
        {
            get
            {
                if (_singleInstance == null)
                {
                    _singleInstance = new UpStreamViewModel();
                }

                return _singleInstance;
            }
        }

        #region 网络实现

        public void UploadAsync(string url, string data, UploadStringCompletedEventHandler uploadCompleted, object userState = null)
        {
            WebClient client = new WebClient();
            client.Headers["Accept-Charset"] = "utf-8";
            client.Headers["Referer"] = "http://www.autohome.com.cn/china";
            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

            client.UploadStringCompleted += (object sender, UploadStringCompletedEventArgs e) =>
            {
                if (uploadCompleted != null)
                {
                    uploadCompleted(this, e);
                }
            };
            client.UploadStringAsync(new Uri(url,UriKind.Absolute), "POST", data, userState);
        }

        #endregion
    }
}
