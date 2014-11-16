﻿using CommonLayer;
using Model.Search;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Handler;

namespace ViewModels.Search
{
    public class ArticleSearchResultViewModel : SearchResultViewModelBase, ISearchViewModel
    {
        public ArticleSearchResultViewModel()
        {
            this.ArticleList = new ObservableCollection<ArticleModel>();
        }

        #region properties

        public ObservableCollection<ArticleModel> ArticleList { get; private set; }

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
                            this.PageIndex = resultToken.SelectToken("pageindex").Value<int>();

                            //文章列表
                            JArray blockToken = (JArray)resultToken.SelectToken("hits");
                            foreach(JToken itemToken in blockToken)
                                {
                                    string data = itemToken.SelectToken("data").ToString();
                                    var model= JsonHelper.DeserializeOrDefault<ArticleModel>(data);
                                    if(model!=null)
                                    {
                                        this.ArticleList.Add(model);
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
            this.ArticleList.Clear();
        }

        #endregion
    }
}
