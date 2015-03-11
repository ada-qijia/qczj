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
using Coding4Fun.Phone.Controls;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Info;
using System.Linq;

namespace AutoWP7.Handler
{
    public class Common
    {
        /// <summary>
        /// 打印消息提示
        /// </summary>
        /// <param name="msg"></param>
        public static void showMsg(string msg)
        {
            ToastPrompt prompt = new ToastPrompt();
            prompt.Foreground = new SolidColorBrush(Color.FromArgb(200, 255, 255, 255));
            prompt.Message = msg;
            prompt.MillisecondsUntilHidden = 1000;
            prompt.Show();
        }


        /// <summary>
        /// 网络是否可用
        /// </summary>
        /// <returns></returns>
        public static void NetworkAvailablePrompt()
        {
            if (DeviceNetworkInformation.IsNetworkAvailable) //检查网络是否可用
            {
                Common.showMsg("获取数据失败");
            }
            else
            {
                Common.showMsg("网络连接失败,请检查连接设置");
            }
        }

        /// <summary>
        /// 清空缓存图片
        /// </summary>
        /// <param name="directoryPath">文件目录</param>
        public static void DeleteDirectory(string directoryPath)
        {
            var myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            if (myIsolatedStorage.DirectoryExists(directoryPath))
            {
                string[] fileNames = myIsolatedStorage.GetFileNames(directoryPath + "\\*");
                string[] dirctoryNames = myIsolatedStorage.GetDirectoryNames(directoryPath + "\\*");

                if (fileNames.Length > 0)
                {
                    foreach (var item in fileNames)
                    {
                        myIsolatedStorage.DeleteFile(System.IO.Path.Combine(directoryPath, item));
                    }
                }
                if (dirctoryNames.Length > 0)
                {
                    foreach (var item in dirctoryNames)
                    {
                        DeleteDirectory(System.IO.Path.Combine(directoryPath, item));
                    }
                }
                //  myIsolatedStorage.DeleteDirectory(directoryPath);
            }
        }


        /// <summary>
        /// 验证是否已经登录
        /// </summary>
        /// <returns>true(已登录) false(未登录)</returns>
        public static bool isLogin()
        {
            var setting = IsolatedStorageSettings.ApplicationSettings;
            string key = "userInfo";
            // UserInfoModel userInfoModel = null;
            if (setting.Contains(key))//已经登录
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 获取设备唯一标识
        /// </summary>
        /// <returns></returns>
        public static string GetDeviceID()
        {
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
            return strDeviceUniqueID;
        }
        /// <summary>
        /// 获取当前系统版本
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyVersion()
        {

            Version version = new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName).Version;

            return version.ToString();
        }
        /// <summary>
        /// 获取_sign串
        /// </summary>
        /// <param name="sign">已排序的(按key字母顺序正序）名值串，例如：a=1&b=2</param>
        /// <returns></returns>
        public static string GetSignStr(string sign)
        {
            string _sign = sign.Replace("&", "").Replace("=", "");
            _sign = App.appKey + _sign + App.appKey;
            _sign = MD5.GetMd5String(_sign);
            _sign = _sign.ToUpper();
            return _sign;
        }

        public static string GetSysVersion()
        {
            return System.Environment.OSVersion.Version.ToString();
        }

        /// <summary>
        /// 获取User-Agent（设备类型\t系统版本号\tautohome\t客户端版本号）
        /// </summary>
        /// <returns></returns>
        public static string GetAutoHomeUA()
        {
            //设备类型\t系统版本号\tautohome\t客户端版本号
            return "WP\t" + Common.GetSysVersion() + "\tautohome\t" + GetAssemblyVersion();
        }

        /// <summary>
        /// 对上传参数列表进行字母升序排序
        /// </summary>
        /// <param name="urlParam">参数名值串，例如：a=1&b=2</param>
        public static string SortURLParamAsc(string urlParam)
        {
            if (urlParam != null)
            {
                var urlParams = urlParam.Split(new char[] { '&' }).OrderBy(p => p).ToArray();
                return string.Join("&", urlParams);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取上行接口用到的时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            var stamp = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            return stamp.ToString();
        }

        /// <summary>
        /// 获取子控件
        /// </summary>
        public static T FindChildOfType<T>(DependencyObject root) where T : class
        {
            var queue = new System.Collections.Generic.Queue<DependencyObject>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                DependencyObject current = queue.Dequeue();
                for (int i = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= i; i--)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        return typedChild;
                    }
                    queue.Enqueue(child);
                }
            }
            return null;
        }
    }
}
