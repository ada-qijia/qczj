using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ViewModels.Handler;

namespace ViewModels
{
    public class SendLetterViewModel
    {

        WebClient wc = null;
        public void SendData(string url, string data)
        {
            if (wc == null)
            {
                wc = new WebClient();
            }
            Uri urlSource = new Uri(url, UriKind.Absolute);
            wc.Encoding = DBCSEncoding.GetDBCSEncoding("gb2312");
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            wc.OpenWriteAsync(urlSource, "POST");
            wc.OpenWriteCompleted += new OpenWriteCompletedEventHandler((ss, ee) =>
            {

            });

        }
    }
}
