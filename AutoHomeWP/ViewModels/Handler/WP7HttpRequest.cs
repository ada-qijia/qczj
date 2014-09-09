using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Text;

namespace ViewModels.Handler
{
    /// <summary>
    ///HTTP封装类
    /// </summary>
    public class WP7HttpRequest
    {

        #region 私有成员

        private string _request_url = null;
        private requestType _request_type;
        IDictionary<string, string> _parameter;

        #endregion

        /// <summary>
        /// Http请求指代
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">发送所带的参数</param>
        public delegate void httpResquestEventHandler(object sender, WP7HttpEventArgs e);

        /// <summary>
        /// Http请求完成事件
        /// </summary>
        public event httpResquestEventHandler httpCompleted;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <remarks>
        /// 默认的请求方式的GET
        /// </remarks>
        public WP7HttpRequest()
        {
            _request_url = "";
            _parameter = new Dictionary<string, string>();
            _request_type = requestType.GET; //默认请求方式为GET方式
        }

        /// <summary>
        /// 追加就参数
        /// </summary>
        /// <param name="key">进行追加的键</param>
        /// <param name="value">键对应的值</param>
        public void appendParameter(string key, string value)
        {
            _parameter.Add(key, value);
        }

        /// <summary>
        /// 触发HTTP请求完成方法
        /// </summary>
        /// <param name="e">事件参数</param>
        public void OnHttpCompleted(WP7HttpEventArgs e)
        {
            if (this.httpCompleted != null)
            {
                this.httpCompleted(this, e);
            }
        }

        /// <summary>
        /// 请求URL地址
        /// </summary>
        public string requestUrl
        {
            get { return _request_url; }
            set { _request_url = value; }
        }

        /// <summary>
        /// 请求方式
        /// </summary>
        public requestType requestMethod
        {
            get { return _request_type; }
            set { _request_type = value; }
        }

        /// <summary>
        /// 进行HTTP请求
        /// </summary>
        public void request()
        {
            if (this.requestMethod == requestType.GET)
            {
                this.getRequest();
            }
            else
            {
                this.postRequest();
            }
        }

        /// <summary>
        /// HTTP方式的GET请求
        /// </summary>
        /// <returns></returns>
        private void getRequest()
        {
            string strrequesturl = this.requestUrl;
            string parastring = this.getParemeterString();
            if (parastring.Length > 0)
            {
                strrequesturl += "?" + parastring;
            }
            Uri myurl = new Uri(strrequesturl);
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(myurl);
            webRequest.Method = "GET";
            webRequest.BeginGetResponse(new AsyncCallback(handleResponse), webRequest); //直接获取响应
            _parameter.Clear(); //清空参数列表

        }

        /// <summary>
        /// HTTP的POST请求
        /// </summary>
        /// <returns></returns>
        private void postRequest()
        {
            Uri myurl = new Uri(this.requestUrl,UriKind.Absolute);
           // string boundary = "--------boundary";
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(myurl);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            webRequest.Headers["Referer"] = "http://www.autohome.com.cn/china";
            webRequest.Headers["Accept-Charset"] = "RequestCodeType";
            webRequest.Headers["User-Agent"] ="WP\t8\tautohome\t1.4.0\tWP";
           // webRequest.ContentType = "multipart/form-data;boundary=" + boundary;
           // webRequest.ContentType = "image/pjpeg";
            webRequest.BeginGetRequestStream(new AsyncCallback(handlePostReady), webRequest);
          //  PictureDecoder
        
        }

        /// <summary>
        /// 获取传递参数的字符串
        /// </summary>
        /// <returns>字符串</returns>
        private string getParemeterString()
        {
            string result = "";
            StringBuilder sb = new StringBuilder();
            bool hasParameter = false;
            string value = "";
            foreach (var item in _parameter)
            {
                if (!hasParameter)
                    hasParameter = true;
                value = UrlEncoder.Encode(item.Value); //对传递的字符串进行编码操作
                sb.Append(string.Format("{0}={1}&", item.Key, value));
            }
            if (hasParameter)
            {
                result = sb.ToString();
                int len = result.Length;
                result = result.Substring(0, --len); //将字符串尾的‘&’去掉
            }
            return result;

        }

        /// <summary>
        /// 发送到服务器的数据
        /// </summary>
        public string postData;

        public ObservableCollection<byte[]> filePathSource;

        /// <summary>
        /// 异步请求回调函数
        /// </summary>
        /// <param name="asyncResult">异步请求参数</param>
        private void handlePostReady(IAsyncResult asyncResult)
        {
            HttpWebRequest webRequest = asyncResult.AsyncState as HttpWebRequest;

            using (Stream stream = webRequest.EndGetRequestStream(asyncResult))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {


                    //// 构造发送数据
                    //StringBuilder sb = new StringBuilder();
                    //string boundary = "--------boundary";
                    //// 文件域的数据
                    //sb.Append("--" + boundary);
                    //sb.Append("\r\n");
                    //sb.Append("Content-Disposition: form-data; name=\"upload_file\";filename=\"image.jpg\"");
                    //sb.Append("\r\n");

                    //sb.Append("Content-Type: ");
                    //sb.Append("image/jpeg");
                    //sb.Append("\r\n\r\n");

                    //string postHeader = sb.ToString();
                    //byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);
                    //stream.Write(postHeaderBytes, 0, postHeaderBytes.Length);


                    //post到服务器的数据
                    string parameterstring = this.postData;
                    byte[] b = DBCSEncoding.GetDBCSEncoding("gb2312").GetBytes(parameterstring);
                    stream.Write(b, 0, b.Length);

                   // writer.Write(parameterstring);
                    //post数据编码
                    // string data = HttpUtility.UrlEncode(parameterstring).ToLowerInvariant();
                    //文本数据
                  

                   // byte[] imgByte = filePathSource[0];
                   // string strImage = parameterstring + " \"image1\":\"" + imgByte + "\"";
                  //  byte[] imgData = DBCSEncoding.GetDBCSEncoding("gb2312").GetBytes(strImage);
                  //  stream.Write(imgData, 0, imgData.Length);
                    writer.Flush();

                }
            }
            webRequest.BeginGetResponse(new AsyncCallback(handleResponse), webRequest);
            _parameter.Clear();//清空参数列表
        }


        private byte[] GetFile(string filePath)
        {
            using (var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isolatedStorageFile.FileExists(filePath))
                {
                    var stream = isolatedStorageFile.OpenFile(filePath, FileMode.Open, FileAccess.Read);

                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);

                    // 设置当前流的位置为流的开始   
                    stream.Seek(0, SeekOrigin.Begin);
                    return bytes;
                }
                return null;
            }
        }


        /// <summary>
        /// 异步响应回调函数
        /// </summary>
        /// <param name="asyncResult">异步请求参数</param>
        private void handleResponse(IAsyncResult asyncResult)
        {
            string result = "";
            bool iserror = false;
            try
            {
                HttpWebRequest webRequest = asyncResult.AsyncState as HttpWebRequest;
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.EndGetResponse(asyncResult);
                Stream streamResult = webResponse.GetResponseStream(); //获取响应流
                StreamReader reader = new StreamReader(streamResult);


                byte[] b = new byte[streamResult.Length];

                streamResult.Read(b, 0, b.Length);
                streamResult.Seek(0, SeekOrigin.Begin);

                //post数据编码
                Gb2312Encoding encoding = new Gb2312Encoding();

                //string data = HttpUtility.UrlEncode(reader.ReadToEnd()).ToLowerInvariant();

                string data = DBCSEncoding.GetDBCSEncoding("gb2312").GetString(b, 0, b.Length);

                result = data;
            }
            catch (Exception ex)
            {
                iserror = true;
                result = ex.Message;
            }
            finally
            {
                WP7HttpEventArgs e = new WP7HttpEventArgs();
                e.isError = iserror;
                e.result = UrlDecoder.UrlDecode(result);
                //进行异步回调操作
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(delegate()
                {

                    OnHttpCompleted(e);

                });
            }
        }
    }

    /// <summary>
    /// 枚举请求类型
    /// </summary>
    public enum requestType
    {
        /// <summary>
        /// GET请求
        /// </summary>
        GET,

        /// <summary>
        /// POST请求
        /// </summary>
        POST
    }
}
