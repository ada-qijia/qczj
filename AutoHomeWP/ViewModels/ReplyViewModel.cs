using System;
using System.Net;
using Microsoft.Phone.Info;
using Model;
using ViewModels.Handler;

namespace ViewModels
{
    public class ReplyViewModel
    {


        //事件通知
        public event EventHandler<APIEventArgs<string>> LoadDataCompleted;

        WebClient wc = null;
        public void sendData(string url, CommentReplyModel model,int pageType,string UA, string contentType)
        {
            if (wc == null)
            {
                wc = new WebClient();
                wc.UploadStringCompleted += wc_UploadStringCompleted;
            }
            if (pageType == 2)
            {
                //说客
                pageType = 7;
            }
            //手机型号
            // string phoneType = "windows phone " + DeviceExtendedProperties.GetValue("DeviceName").ToString();
            //TODU  改为动态获取
            String phoneType = "WP\t8\tautohome\t1.6.2\tWP";
            //手机设备id
            string deviceid = DeviceExtendedProperties.GetValue("DeviceUniqueId").ToString();
            //post 数据
            string strData = "authorization=" + model.Authorization
                + "&_appid=app"
                + "&appid=" + contentType //pageType
                + "&objid=" + model.NewsId
                + "&txtcontent=" + model.Content
                + "&TargetReplyId=" + model.ReplyId
                + "&encoding=gb2312"
                + "&datatype=json"
                + "&phonetype=" + phoneType;

            #region 获得设备号
            byte[] byteArray = DeviceExtendedProperties.GetValue("DeviceUniqueId") as byte[];
            string strTemp = "";
            string strDeviceUniqueID = string.Empty;
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
            #endregion

            Uri urlSource = new Uri(url, UriKind.Absolute);

            
            wc.Encoding = DBCSEncoding.GetDBCSEncoding("gb2312");
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            wc.Headers["Accept-Charset"] = "gb2312";
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            //TODU 改成统一格式
            wc.Headers["User-Agent"] = UA;

            wc.UploadStringAsync(urlSource, "POST", strData);
        }

        void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            APIEventArgs<string> apiArgs = new APIEventArgs<string>();

            if (e.Error != null)
            {
                apiArgs.Error = e.Error;
            }
            else
            {
                //返回的json数据
                string json = e.Result;

                apiArgs.Result = json;
            }

            if (LoadDataCompleted != null)
            {
                LoadDataCompleted(this, apiArgs);
            }
        }

    }
}
