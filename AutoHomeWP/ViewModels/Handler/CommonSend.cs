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

namespace ViewModels.Handler
{
    public class CommonSend
    {
        private string strData = string.Empty;
        public void sendData()
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(new Uri("", UriKind.Absolute));
            httpRequest.Method = "POST";
            httpRequest.ContentType = "multipart/form-data";
            IAsyncResult asyncResult = httpRequest.BeginGetRequestStream(new AsyncCallback(RepuestStreamCallback), httpRequest);
        }

        //发送数据
        public void RepuestStreamCallback(IAsyncResult result)
        {
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            Stream requestStream = request.EndGetRequestStream(result);
            //写入数据
           // requestStream.Write(new byte[10]());
            requestStream.Flush();
            requestStream.Close();
            request.BeginGetRequestStream(new AsyncCallback(ResponseCallback),request);
        }

        //接收返回的数据
        public void ResponseCallback(IAsyncResult result)
        {
            try
            {
                HttpWebRequest request = result.AsyncState as HttpWebRequest;
                WebResponse response = request.EndGetResponse(result) as HttpWebResponse;
                if (response != null)
                {
                    Stream responseStream = response.GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        //获取数据
                        string strResponse = reader.ReadToEnd();
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}
