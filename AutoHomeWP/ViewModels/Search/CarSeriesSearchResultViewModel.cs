using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Model.Search;
using System.Net;
using ViewModels.Handler;
using Newtonsoft.Json.Linq;
using CommonLayer;

namespace ViewModels.Search
{
    public class CarSeriesSearchResultViewModel:SearchResultViewModelBase, ISearchViewModel
    {
        public CarSeriesSearchResultViewModel()
        {
            this.CarSeriesList = new ObservableCollection<CarSeriesSearchModel>();
        }

        #region properties

        public ObservableCollection<CarSeriesSearchModel> CarSeriesList { get; private set; }

        #endregion

        #region interface implementation

        public event EventHandler LoadDataCompleted;

        private bool isLoading = false;
        public void LoadDataAysnc(string url)
        {
            if (!isLoading)
            {
                isLoading = true;

                this.DownloadStringCompleted += new DownloadStringCompletedEventHandler((ss, ee) =>
                {
                    if (ee.Error == null && ee.Result != null)
                    {
                        try
                        {
                            //返回的json数据
                            JObject json = JObject.Parse(ee.Result);
                            JToken resultToken = json.SelectToken("result");

                            #region 用返回结果填充每个版块

                            this.RowCount = resultToken.SelectToken("rowcount").Value<int>();
                            //this.PageIndex = resultToken.SelectToken("pageindex").Value<int>();

                            JToken blockToken;
                            //车系列表
                            blockToken = resultToken.SelectToken("list");
                            if (blockToken.HasValues)
                            {
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
                });

                //开始下载
                this.DownloadStringAsync(url);
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
