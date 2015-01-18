using CommonLayer;
using Model;
using Model.Me;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Me
{
    public class MeViewModel : BindableBase, Search.ISearchViewModel
    {
        public MeViewModel()
        {
        }

        private MeModel _model;
        public MeModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                if (value != _model)
                {
                    _model = value;
                    OnPropertyChanged("Model");
                }
            }
        }

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
            this.Model = null;
        }

        #endregion

        private void ViewModel_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null)
            {
                try
                {
                    //返回的json数据
                    JObject json = JObject.Parse(e.Result);
                    JToken resultToken = json.SelectToken("result");

                    this.Model = JsonHelper.DeserializeOrDefault<MeModel>(resultToken.ToString());
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
    }
}
