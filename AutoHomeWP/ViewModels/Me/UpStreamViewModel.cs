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

        private WebClient uploadWC;
        private UploadStringCompletedEventHandler UploadCompletedHandler;

        public void UploadAsyncWithSharedClient(string url, string data, UploadStringCompletedEventHandler uploadCompleted)
        {
            if (uploadWC == null)
            {
                uploadWC = new WebClient();
                uploadWC.UploadStringCompleted += wc_UploadStringCompleted;
                uploadWC.Headers["Accept-Charset"] = "utf-8";
                uploadWC.Headers["Referer"] = "http://www.autohome.com.cn/china";
                uploadWC.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                //registerWC.Headers["User-Agent"] = AutoWP7.Handler.Common.GetAutoHomeUA();
            }

            if (!uploadWC.IsBusy)
            {
                this.UploadCompletedHandler = uploadCompleted;
                uploadWC.UploadStringAsync(new Uri(url), "POST", data);
            }
        }

        private void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (UploadCompletedHandler != null)
            {
                UploadCompletedHandler(sender, e);
            }
        }

        public void UploadAsyncWithOneoffClient(string url, string data, UploadStringCompletedEventHandler uploadCompleted, object userState = null)
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
            client.UploadStringAsync(new Uri(url), "POST", data, userState);
        }

        #endregion
    }
}
