﻿using CommonLayer;
using Model.Search;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace ViewModels.Search
{
    public class ArticleSearchResultViewModel : SearchResultViewModelBase, ISearchViewModel
    {
        private const int PageSize = 20;

        private ArticleFilterModel defaultFilter = new ArticleFilterModel() { Type = "0", Num = 0, Name = "全部" };

        public ArticleSearchResultViewModel()
        {
            this.ArticleList = new ObservableCollection<ArticleModel>();
            this.LoadMoreButtonItem = new ArticleModel() { IsLoadMore = true };
            this.ArticleFilterList = new ObservableCollection<ArticleFilterModel>() { defaultFilter };

            this.DownloadStringCompleted += ArticleSearchResultViewModel_DownloadStringCompleted;
        }

        #region properties

        public ObservableCollection<ArticleFilterModel> ArticleFilterList { get; private set; }

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
                //开始下载
                this.DownloadStringAsync(url);
            }
        }

        private void ArticleSearchResultViewModel_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null)
            {
                try
                {
                    this.TryRemoveMoreButton();

                    //返回的json数据
                    JObject json = JObject.Parse(e.Result);
                    JToken resultToken = json.SelectToken("result");

                    #region 用返回结果填充每个版块

                    this.RowCount = resultToken.SelectToken("rowcount").Value<int>();
                    this.PageIndex = resultToken.SelectToken("pageindex").Value<int>() / PageSize + 1;
                    this.PageCount = resultToken.SelectToken("pagecount").Value<int>();

                    //文章类别列表
                    JToken sortsToken = resultToken.SelectToken("facets.sorts");
                    if (sortsToken != null && this.ArticleFilterList.Count <= 1)
                    {
                        var filterList = JsonHelper.DeserializeOrDefault<List<ArticleFilterModel>>(sortsToken.ToString());
                        if (filterList != null && filterList.Count > 0)
                        {
                            this.ArticleFilterList.Clear();
                            foreach (var model in filterList)
                            {
                                this.ArticleFilterList.Add(model);
                            }
                        }

                        if (this.ArticleFilterList.Count == 0)
                        {
                            this.ArticleFilterList.Add(this.defaultFilter);
                        }
                    }

                    //文章列表
                    JArray blockToken = (JArray)resultToken.SelectToken("hits");
                    foreach (JToken itemToken in blockToken)
                    {
                        string data = itemToken.SelectToken("data").ToString();
                        var model = JsonHelper.DeserializeOrDefault<ArticleModel>(data);
                        if (model != null)
                        {
                            this.ArticleList.Add(model);
                        }
                    }

                    #endregion

                    this.EnsureMoreButton();
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
            this.PageIndex = 0;
            this.PageCount = 0;
            this.ArticleList.Clear();
        }

        #endregion

        #region base class override
        protected override void EnsureMoreButton()
        {
            if (!this.IsEndPage && !this.ArticleList.Contains(this.LoadMoreButtonItem))
            {
                this.ArticleList.Add((ArticleModel)this.LoadMoreButtonItem);
            }
        }

        protected override void TryRemoveMoreButton()
        {
            if (this.ArticleList.Contains(this.LoadMoreButtonItem))
            {
                this.ArticleList.Remove((ArticleModel)this.LoadMoreButtonItem);
            }
        }
        #endregion
    }
}
