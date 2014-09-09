﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AutoWP7.Handler;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Model;
using Newtonsoft.Json.Linq;
using ViewModels.Handler;
using System.Net;

namespace AutoWP7.View.Forum
{
    /// <summary>
    /// 发送评论页
    /// </summary>
    public partial class SendLetterPage : PhoneApplicationPage
    {
        public SendLetterPage()
        {
            InitializeComponent();
        }


        string bbsId = string.Empty;
        // c车系论坛 a地区论坛 o主题论坛
        string bbsType = string.Empty;
        int topicId;
        string title = string.Empty;
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            switch (e.NavigationMode)
            {
                case System.Windows.Navigation.NavigationMode.New:
                    {
                        UmengSDK.UmengAnalytics.onEvent("ForumActivity", "发帖");
                        bbsId = this.NavigationContext.QueryString["bbsId"];
                        bbsType = this.NavigationContext.QueryString["bbsType"];
                        title = this.NavigationContext.QueryString["title"];
                        PageTitle.Text = title;
                    }
                    break;
            }

        }

        //标识是否正在发送中
        bool isSending = false;
        //发送
        private void sendLetter_Click(object sender, EventArgs e)
        {
            if (isSending == false)
            {
                

                //检查是否已经登录
                var setting = IsolatedStorageSettings.ApplicationSettings;
                string key = "userInfo";
                MyForumModel userInfoModel = null;
                if (setting.Contains(key))//已经登录
                {
                    //TODU取消注释
                    isSending = true;
                    string lettertitle = letterTitle.Text;
                    string lettercontent = letterContent.Text;
                    userInfoModel = setting[key] as MyForumModel;
                    Version version = new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName).Version;
                    string sign = string.Empty;
                    //名值对按key正序排列
                    string strData = "_appid=app"
                        + "&_timestamp=" + DateTime.Now.Ticks//时间戳
                        + "&album_id=1"
                        + "&autohomeua=" + Common.GetAutoHomeUA() //设备类型\t系统版本号\tautohome\t客户端版本号
                        + "&bbsid="+ bbsId
                        + "&content=" + lettercontent
                        + "&imei=" + Common.GetDeviceID()//设备唯一标识
                        + "&informfriends=0"
                        + "&reply_notify_me=1"
                        + "&title=" + lettertitle
                        + "&uc_ticket=" + userInfoModel.Authorization;
                        //生成_sign
                        sign=Common.GetSignStr(strData);

                        strData += "&_sign=" + sign;
                    //发送
                    SendData(strData);
                }
                else 
                {
                    //未登录，跳转到登录页
                    this.NavigationService.Navigate(new Uri("/View/More/LoginPage.xaml",UriKind.Relative));
                }
            }
        }

        WebClient wc = null;
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="userKey">用户key</param>
        /// <param name="strData">数据</param>
        private void SendData( string strData)
        {
            try
            {
                GlobalIndicator.Instance.Text = "正在发送中...";
                GlobalIndicator.Instance.IsBusy = true;
                
                if (wc == null)
                {
                    wc = new WebClient();
                }
                //wc.Encoding = DBCSEncoding.GetDBCSEncoding("utf-8");
                wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
                wc.Headers["Accept-Charset"] = "utf-8";
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                //TODU 改成统一格式
                wc.Headers["User-Agent"] = Common.GetAutoHomeUA();
                string url = App.clubUrl + "/api/topic/appadd";
                Uri urlSource = new Uri(url, UriKind.Absolute);
                wc.UploadStringAsync(urlSource, "POST", strData);
                
                wc.UploadStringCompleted += new UploadStringCompletedEventHandler((ss, ee) =>
                {
                    APIEventArgs<string> apiArgs = new APIEventArgs<string>();


                    if (ee.Error != null)
                    {
                        apiArgs.Error = ee.Error;
                        Common.showMsg("发送失败");
                    }
                    else
                    {
                        JObject json = JObject.Parse(ee.Result);
                        int returnCode = (int)json.SelectToken("returncode");
                        
                        if (returnCode != 0)
                        {
                            string strMsg = (string)json.SelectToken("message");
                            Common.showMsg(strMsg);
                        }else
                        {
                            topicId = (int)json.SelectToken("result").SelectToken("topicid");
                            this.NavigationService.Navigate(new Uri("/View/Forum/TopicDetailPage.xaml?bbsId=" + bbsId + "&topicId=" + topicId + "&bbsType=" + bbsType+"&issend="+1, UriKind.Relative));
                        }
                    }
                    isSending = false;
                   
                });
            }
            catch (Exception ex)
            {

            }


        }



        private ObservableCollection<byte[]> imgDataSource = new ObservableCollection<byte[]>();
        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始   
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        private int picCount = 0;
        //读取媒体库图片
        private void picture_Click(object sender, EventArgs e)
        {
            PhotoChooserTask photoTask = new PhotoChooserTask();
            photoTask.Completed += new EventHandler<PhotoResult>((ss, ee) =>
            {
                if (ee.TaskResult == TaskResult.OK)
                {
                    BitmapImage bmpImage = new BitmapImage();

                    bmpImage.SetSource(ee.ChosenPhoto); //获取数据流



                    Image img = new Image();
                    img.Height = 100;
                    img.Width = 100;
                    picCount++;//图片数
                    img.Source = bmpImage;

                    imgStack.Children.Add(img);

                    imgDataSource.Add(StreamToBytes(ee.ChosenPhoto));

                    string strSource = ee.OriginalFileName;


                }
            });
            photoTask.Show();//打开相册
        }

        //拍照
        private void Camera_Click(object sender, EventArgs e)
        {
            CameraCaptureTask cameraTask = new CameraCaptureTask();
            cameraTask.Completed += new EventHandler<PhotoResult>((ss, ee) =>
            {
                if (ee.Error == null && ee.TaskResult == TaskResult.OK)
                {
                    BitmapImage bmpImage = new BitmapImage();
                    bmpImage.SetSource(ee.ChosenPhoto);//获取的数据流
                    Image img = new Image();
                    img.Height = 100;
                    img.Width = 100;
                    img.Source = bmpImage;
                    imgStack.Children.Add(img);

                    picCount++;
                    imgDataSource.Add(StreamToBytes(ee.ChosenPhoto));


                }
            });
            cameraTask.Show();//打开相机
        }




    }
}