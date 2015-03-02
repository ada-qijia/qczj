using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CommonLayer
{
    public class CommonHelper
    {
        /// <summary>
        /// 获取第一个符合类型的控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parentElement"></param>
        /// <returns></returns>
        public static T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parentElement);
            if (count == 0)
                return null;
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    var result = FindFirstElementInVisualTree<T>(child);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        public static void DownloadStringAsync(string url,DownloadStringCompletedEventHandler completedHandler)
        {
            if (!string.IsNullOrEmpty(url))
            {
                WebClient wc = new WebClient();

                wc.Encoding = System.Text.Encoding.UTF8;
                wc.Headers["Referer"] = "http://www.autohome.com.cn/china";

                var downloadStringCompleted = completedHandler;
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler((obj, e) =>
                {
                    if (downloadStringCompleted != null)
                    {
                        downloadStringCompleted(null, e);
                    }
                });

                Uri urlSource = new Uri(url + "&" + Guid.NewGuid().ToString(), UriKind.Absolute);
                wc.DownloadStringAsync(urlSource);
            }
        }

        public static void UploadStringAsync(string url,string data, UploadStringCompletedEventHandler completedHandler)
        {

        }

    }
}
