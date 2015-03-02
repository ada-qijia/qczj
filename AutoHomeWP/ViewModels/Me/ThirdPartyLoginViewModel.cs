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
        public void ThirdPartyLoginAsync(string url, string postData,EventHandler<int> loginCompleted)
        {
            var completed=new UploadStringCompletedEventHandler((ss, ee) =>
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

                if (loginCompleted != null)
                {
                    loginCompleted(this, returnCode);
                }
            });

            UpStreamViewModel.SingleInstance.UploadAsync(url, postData, completed);
        }
    }
}
