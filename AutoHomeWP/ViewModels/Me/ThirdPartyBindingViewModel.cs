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

namespace ViewModels.Me
{
    public class ThirdPartyBindingViewModel: BindableBase, Search.ISearchViewModel
    {
        public ThirdPartyBindingViewModel()
        {
            this.BindingList = new ObservableCollection<ThirdPartyBindingModel>();
        }

        #region properties

        public ObservableCollection<ThirdPartyBindingModel> BindingList { get; private set; }

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
    }
}
