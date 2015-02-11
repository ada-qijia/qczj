using CommonLayer;
using Model;
using Model.Me;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Handler;

namespace ViewModels.Me
{
    public class ThirdPartyBindingViewModel : BindableBase, Search.ISearchViewModel
    {
        public ThirdPartyBindingViewModel()
        {
            this.BindingList = new ObservableCollection<ThirdPartyBindingModel>();
        }

        #region properties

        public ObservableCollection<ThirdPartyBindingModel> BindingList { get; private set; }

        private int _returnCode;
        public int ReturnCode
        {
            get { return _returnCode; }
            set { SetProperty<int>(ref _returnCode, value); }
        }

        #endregion

        #region interface implementation

        public event EventHandler LoadDataCompleted;

        private bool isLoading = false;
        public void LoadDataAysnc(string url)
        {
            if (!isLoading)
            {
                isLoading = true;

                //开始下载
                CommonLayer.CommonHelper.DownloadStringAsync(url, ViewModel_DownloadStringCompleted);
            }
        }

        public void ClearData()
        {
            this.BindingList.Clear();
        }

        #endregion

        private void ViewModel_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null)
            {
                try
                {
                    this.ClearData();

                    //返回的json数据
                    JObject json = JObject.Parse(e.Result);
                    JToken resultToken = json.SelectToken("result");
                    this.ReturnCode = json.SelectToken("returncode").Value<int>();

                    JToken blockToken = resultToken.SelectToken("list");
                    if (blockToken.HasValues)
                    {
                        var bindingList = JsonHelper.DeserializeOrDefault<List<ThirdPartyBindingModel>>(blockToken.ToString());
                        if (bindingList != null)
                        {
                            foreach (var model in bindingList)
                            {
                                this.BindingList.Add(model);
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            isLoading = false;

            //触发完成事件
            if (LoadDataCompleted != null)
            {
                LoadDataCompleted(this, null);
            }
        }

        #region 检测授权是否过期

        public event EventHandler<APIEventArgs<bool>> WeiboCheckTokenExpiredCompleted;

        WebClient weiboClient;

        //http://open.weibo.com/wiki/Oauth2/get_token_info?sudaref=open.weibo.com
        public void WeiboCheckTokenExpired()
        {
            if (weiboClient == null)
            {
                weiboClient = new WebClient();
                weiboClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                weiboClient.UploadStringCompleted += weiboClient_UploadStringCompleted;
            }

            var weibo = this.BindingList.FirstOrDefault(item => item.ThirdPartyID == 8);
            if (weibo != null && !string.IsNullOrEmpty(weibo.Token) && !weiboClient.IsBusy)
            {
                string url = "https://api.weibo.com/oauth2/get_token_info";
                weiboClient.UploadStringAsync(new Uri(url, UriKind.Absolute),"access_token=" + weibo.Token);
            }
        }

        private void weiboClient_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            APIEventArgs<bool> args = new APIEventArgs<bool>();
            try
            {
                if (e.Error == null && e.Result != null)
                {
                    JObject json = JObject.Parse(e.Result);
                    //string uid = json.SelectToken("uid").ToString();
                    var expireIn = json.SelectToken("expire_in").Value<int>();

                    args.Result =expireIn<0;
                    var weibo = this.BindingList.FirstOrDefault(item => item.ThirdPartyID == 8);
                    if (weibo != null)
                    {
                        weibo.IsExpired = args.Result;
                    }
                }
                else
                {
                    args.Error = e.Error;
                }
            }
            catch (Exception ex)
            {
                args.Error = ex;
            }

            if (WeiboCheckTokenExpiredCompleted != null)
            {
                WeiboCheckTokenExpiredCompleted(this, args);
            }
        }

        #endregion
    }
}
