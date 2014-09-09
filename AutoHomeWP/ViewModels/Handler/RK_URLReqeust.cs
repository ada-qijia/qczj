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
using System.IO;
using System.Threading;
using System.Text;

namespace ViewModels.Handler
{
    public class RK_URLReqeust
    {
        /// <summary>
        /// Delegate declaration for server callbacks.
        /// </summary>
        /// <param name="response">The server response.</param>
        public delegate void Callback(string response);

        /// <summary>
        /// The callback for the server response.
        /// </summary>
        Callback callback;

        /// <summary>
        /// The actual URL.
        /// </summary>
        string url;
        string poststr;

        public Stream Image { get; set; }

        /// <summary>
        /// The server response to this URL request.
        /// </summary>
        public string Response { get; private set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public RK_URLReqeust(string url, string PostString, Callback callback)
        {
            // Initialize members
            this.url = url;
            this.callback = callback;
            poststr = PostString;
            //State = URLRequestState.Idle;
        }


        /// <summary>
        /// Send the URL request off!
        /// </summary>
        /// <returns>The server response.</returns>
        public void SendPost()
        {
            // Create a background thread to run the web request
            Thread t = new Thread(new ThreadStart(SendPostThreadFunc));
            t.Name = "URLRequest_For_" + url;
            t.IsBackground = true;
            t.Start();
        }

        /// <summary>
        ///用后台线程来发生请求
        /// </summary>
        void SendPostThreadFunc()
        {
            string boundary = "--------boundary";
            // Create the web request object
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

            // Start the request
            webRequest.BeginGetRequestStream(new AsyncCallback(GetReqeustStreamCallback), webRequest);

            // Update our state
            //State = URLRequestState.Working;
        }

        /// <summary>
        /// 开始请求
        /// </summary>
        /// <param name="asynchronousResult">The asynchronous result object.</param>
        void GetReqeustStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            string boundary = "--------boundary";
            // End the stream request operation
            Stream postStream = webRequest.EndGetRequestStream(asynchronousResult);

            // 构造发送数据
            StringBuilder sb = new StringBuilder();

            // 文件域的数据
            sb.Append("--" + boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"upload_file\";filename=\"image.jpg\"");
            sb.Append("\r\n");

            sb.Append("Content-Type: ");
            sb.Append("image/jpeg");
            sb.Append("\r\n\r\n");

            string postHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);
            postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

            // 输入文件流数据 
            byte[] buffer = new Byte[checked((uint)Math.Min(4096, (int)Image.Length))];
            int bytesRead = 0;
            while ((bytesRead = Image.Read(buffer, 0, buffer.Length)) != 0)
                postStream.Write(buffer, 0, bytesRead);


            // 构造发送数据
            StringBuilder sb2 = new StringBuilder();
            // 文本域的数据，将user=eking&pass=123456  格式的文本域拆分 ，然后构造 
            foreach (string c in poststr.Split('&'))
            {
                string[] item = c.Split('=');
                if (item.Length != 2)
                {
                    break;
                }
                string name = item[0];
                string value = item[1];
                sb2.Append("--" + boundary);
                sb2.Append("\r\n");
                sb2.Append("Content-Disposition: form-data; name=\"" + name + "\"");
                sb2.Append("\r\n\r\n");
                sb2.Append(value);
                sb2.Append("\r\n");
            }

            byte[] postHeaderBytes2 = Encoding.UTF8.GetBytes("\r\n" + sb2.ToString());

            // Add the post data to the web request
            postStream.Write(postHeaderBytes2, 0, postHeaderBytes2.Length);
            postStream.Close();

            // Start the web request
            webRequest.BeginGetResponse(new AsyncCallback(GetResponseCallback), webRequest);
        }

        /// <summary>
        /// Start the URL request.
        /// </summary>
        /// <param name="asynchronousResult">The asynchronous result object.</param>
        void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest webRequest = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the get response operation
            HttpWebResponse response = (HttpWebResponse)webRequest.EndGetResponse(asynchronousResult);
            Stream streamResponse = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(streamResponse);
            Response = streamReader.ReadToEnd();
            streamResponse.Close();
            streamReader.Close();
            response.Close();

            // Call the response callback
            if (callback != null)
            {
                callback(Response);
            }

            // Update state
            //State = URLRequestState.Done;
        }

        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始   
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

    }
}
