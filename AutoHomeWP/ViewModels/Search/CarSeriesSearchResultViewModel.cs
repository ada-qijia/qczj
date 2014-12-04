using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Model.Search;
using System.Net;
using Newtonsoft.Json.Linq;
using CommonLayer;

namespace ViewModels.Search
{
    public class CarSeriesSearchResultViewModel : SearchResultViewModelBase, ISearchViewModel
    {
        public CarSeriesSearchResultViewModel()
        {
            this.CarSeriesList = new ObservableCollection<CarSeriesSearchModel>();
            this.DownloadStringCompleted += CarSeriesSearchResultViewModel_DownloadStringCompleted;
        }

        #region properties

        public ObservableCollection<CarSeriesSearchModel> CarSeriesList { get; private set; }

        #endregion

        #region interface implementation

        public event EventHandler LoadDataCompleted;

        private bool isLoading = false;
        /// <summary>
        /// 不分页
        /// </summary>
        /// <param name="url"></param>
        public void LoadDataAysnc(string url)
        {
            if (!isLoading)
            {
                isLoading = true;

                //开始下载
                this.DownloadStringAsync(url);
            }
        }

        void CarSeriesSearchResultViewModel_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null)
            {
                try
                {
                    //返回的json数据
                    JObject json = JObject.Parse(e.Result);
                    JToken resultToken = json.SelectToken("result");

                    #region 用返回结果填充每个版块

                    JArray blockToken;
                    //车系列表
                    blockToken = (JArray)resultToken.SelectToken("list");
                    if (blockToken.HasValues)
                    {
                        this.RowCount = blockToken.Count;

                        var modelList = JsonHelper.DeserializeOrDefault<List<CarSeriesSearchModel>>(blockToken.ToString());
                        if (modelList != null)
                        {
                            foreach (var model in modelList)
                            {
                                this.CarSeriesList.Add(model);
                            }
                        }
                    }

                    #endregion
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

        public void ClearData()
        {
            this.RowCount = 0;
            this.CarSeriesList.Clear();
        }

        #endregion
    }
}
