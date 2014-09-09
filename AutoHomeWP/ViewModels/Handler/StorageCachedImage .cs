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
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.IO;

namespace ViewModels.Handler
{
    public class StorageCachedImage : BitmapSource
    {

        private readonly Uri uriSource;
        private readonly string filePath;
        //临时缓存
        private const string CacheDirectory = "CachedImages";
        //周期缓存
        private const string CacheFindCarDiretory = "CycleCachedImages";

        static StorageCachedImage()
        {
            //创建缓存目录
            using (var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isolatedStorageFile.DirectoryExists(CacheDirectory))
                {
                    isolatedStorageFile.CreateDirectory(CacheDirectory);
                }

                if (!isolatedStorageFile.DirectoryExists(CacheFindCarDiretory))
                {
                    isolatedStorageFile.CreateDirectory(CacheFindCarDiretory);
                }

            }
        }

        /// <summary>
        /// 创建一个独立存储临时缓存的图片源
        /// </summary>
        /// <param name="uriSource"></param>
        public StorageCachedImage(Uri uriSource)
        {
            this.uriSource = uriSource;
            //文件路径
            filePath = System.IO.Path.Combine(CacheDirectory, uriSource.AbsolutePath.TrimStart('/').Replace('/', '_'));
            OpenCatchSource();
        }

        /// <summary>
        /// 周期缓存图片源
        /// </summary>
        public StorageCachedImage(Uri uriSource, int type)
        {
            this.uriSource = uriSource;
            //文件路径
            filePath = System.IO.Path.Combine(CacheFindCarDiretory, uriSource.AbsolutePath.TrimStart('/').Replace('/', '_'));
            OpenCatchSource();
        }

        /// <summary>
        /// 打开缓存源
        /// </summary>
        private void OpenCatchSource()
        {
            bool exist;
            using (var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                exist = isolatedStorageFile.FileExists(filePath);
            }
            if (exist)
            {
                SetCacheStreamSource();
            }
            else
            {
                SetWebStreamSource();
            }
        }
        /// <summary>
        /// 设置缓存流到图片
        /// </summary>
        private void SetCacheStreamSource()
        {
            try
            {
                using (var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var stream = isolatedStorageFile.OpenFile(filePath, FileMode.Open, FileAccess.Read))
                    {

                        SetSource(stream);
                        stream.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex.Message);
                //MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 下载Uri中的图片
        /// </summary>
        private void SetWebStreamSource()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uriSource);
            httpWebRequest.Headers["Referer"] = "http://www.autohome.com.cn/china";
            httpWebRequest.AllowReadStreamBuffering = true;
            httpWebRequest.BeginGetResponse(ResponseCallBack, httpWebRequest);
        }
        /// <summary>
        /// 下载回调
        /// </summary>
        /// <param name="asyncResult"></param>
        private void ResponseCallBack(IAsyncResult asyncResult)
        {
            var httpWebRequest = asyncResult.AsyncState as HttpWebRequest;
            if (httpWebRequest == null) return;
            try
            {
                var response = httpWebRequest.EndGetResponse(asyncResult);
                using (var stream = response.GetResponseStream())
                using (var isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication())
                using (var fileStream = isolatedStorageFile.OpenFile
                    (filePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                    stream.Close();
                    //SetSource(stream);
                }
                Dispatcher.BeginInvoke(SetCacheStreamSource);
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
            }
        }
    }

}

