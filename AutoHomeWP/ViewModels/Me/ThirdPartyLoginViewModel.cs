using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Me;
using System.Net;
using ViewModels.Handler;
using Newtonsoft.Json.Linq;
using CommonLayer;

namespace ViewModels.Me
{
    public class ThirdPartyLoginViewModel : Model.BindableBase
    {
        #region properties

        private ThirdPartyLoginResultModel _loginResult;
        public ThirdPartyLoginResultModel LoginResult
        {
            get { return _loginResult; }
            set { SetProperty<ThirdPartyLoginResultModel>(ref _loginResult, value); }
        }

        private int _returnCode;
        public int ReturnCode
        {
            get { return _returnCode; }
            set { SetProperty<int>(ref _returnCode, value); }
        }

        #endregion

        WebClient wc = new WebClient();

        public event EventHandler<int> ThirdPartyLoginCompleted;
        public void ThirdPartyLoginAsync(string url, string postData)
        {
            if (wc == null)
            {
                wc = new WebClient();
            }

            if (!wc.IsBusy)
            {
                wc.Encoding = DBCSEncoding.GetDBCSEncoding("gb2312");
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                wc.UploadStringAsync(new Uri(url), "POST", postData);
                wc.UploadStringCompleted += new UploadStringCompletedEventHandler((ss, ee) =>
                {
                    int returnCode = int.MinValue;
                    try
                    {
                        if (ee.Error == null && ee.Cancelled == false)
                        {
                            JObject json = JObject.Parse(ee.Result);
                            returnCode = json.SelectToken("returncode").Value<int>();
                            JToken resultToken = json.SelectToken("result");

                            this.LoginResult = JsonHelper.DeserializeOrDefault<ThirdPartyLoginResultModel>(resultToken.ToString());
                        }
                    }
                    catch { }

                    if (this.ThirdPartyLoginCompleted != null)
                    {
                        this.ThirdPartyLoginCompleted(this, returnCode);
                    }
                });
            }
        }
    }
}
