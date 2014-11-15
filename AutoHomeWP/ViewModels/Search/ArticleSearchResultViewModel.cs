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

        public int ArticleCount { get; set; }

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

                            this.ArticleCount = resultToken.SelectToken("rowcount").Value<int>();

                            JToken blockToken;
                            //文章列表
                            blockToken = resultToken.SelectToken("hits");
                            if (blockToken.HasValues)
                            {
                                var modelList = JsonHelper.DeserializeOrDefault<List<ArticleModel>>(blockToken.ToString());
                                if (modelList != null)
                                {
                                    foreach (var model in modelList)
                                    {
                                        this.ArticleList.Add(model);
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
            this.ArticleCount = 0;
            this.ArticleList.Clear();
        }

        #endregion
    }
}
