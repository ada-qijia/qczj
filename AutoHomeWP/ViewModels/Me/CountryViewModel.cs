using CommonLayer;
using Model;
using Model.Me;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace ViewModels.Me
{
    public class CountryViewModel : BindableBase, Search.ISearchViewModel
    {
        private readonly CountryModel chinaModel = new CountryModel() { Name = "中国", CountryCode = 86, PhoneLength = 11 };

        public CountryViewModel()
        {
            this.CountryList = new ObservableCollection<CountryModel>();
            this.CountryList.Add(chinaModel);
        }

        #region properties

        private static CountryViewModel _singleInstance;
        public static CountryViewModel SingleInstance
        {
            get
            {
                if (_singleInstance == null)
                {
                    _singleInstance = new CountryViewModel();
                }
                return _singleInstance;
            }
        }

        #endregion

        public ObservableCollection<CountryModel> CountryList { get; private set; }

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
            this.CountryList.Clear();
            this.CountryList.Add(chinaModel);
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
                    JToken resultToken = json.SelectToken("result").SelectToken("list");

                    if(resultToken.HasValues)
                    {
                        List<CountryModel> countries=JsonHelper.DeserializeOrDefault<List<CountryModel>>(resultToken.ToString());
                        if (countries != null)
                        {
                            this.CountryList.Clear();
                            foreach (var model in countries)
                            {
                                this.CountryList.Add(model);
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
