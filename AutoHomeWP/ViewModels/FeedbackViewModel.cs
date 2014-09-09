using System;
using System.Net;
using ViewModels.Handler;

namespace ViewModels
{
    public class FeedbackViewModel
    {

        //事件通知
        public event EventHandler<APIEventArgs<string>> LoadDataCompleted;

        WebClient wc = null;
        public void sendData(string url, string strData)
        {
            if (wc == null)
            {
                wc = new WebClient();
            }
            // wc.Encoding = DBCSEncoding.GetDBCSEncoding("gb2312");
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            //wc.Headers["Accept-Charset"] = "RequestCodeType";
            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

            Uri urlSource = new Uri(url, UriKind.Absolute);
            wc.UploadStringAsync(urlSource, "POST", strData);
            wc.UploadStringCompleted += new UploadStringCompletedEventHandler((ss, ee) =>
            {
                APIEventArgs<string> apiArgs = new APIEventArgs<string>();

                if (ee.Error != null)
                {
                    apiArgs.Error = ee.Error;
                }
                else
                {
                    //返回的json数据
                    string json = HttpUtility.UrlDecode(ee.Result);

                    apiArgs.Result = json;
                }

                if (LoadDataCompleted != null)
                {
                    LoadDataCompleted(this, apiArgs);
                }
            });
        }
    }
}
