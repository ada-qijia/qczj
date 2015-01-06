﻿using CommonLayer;
using Model.Me;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Me
{
    /// <summary>
    /// pageIndex=50
    /// </summary>
    public class PrivateMessageViewModel:Search.SearchResultViewModelBase
    {
        public PrivateMessageViewModel()
        {
            this.MessageList = new ObservableCollection<PrivateMessageModel>();

            this.LoadMoreButtonItem = new PrivateMessageModel() { IsLoadMore = true };
            this.DownloadStringCompleted += ViewModel_DownloadStringCompleted;
        }

        #region properties

        public ObservableCollection<PrivateMessageModel> MessageList { get; private set; }

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

        private void ViewModel_DownloadStringCompleted(object sender, System.Net.DownloadStringCompletedEventArgs e)
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
                    this.PageIndex = resultToken.SelectToken("pageindex").Value<int>();
                    this.PageCount = resultToken.SelectToken("pagecount").Value<int>();

                    JToken blockToken;

                    //文章列表
                    blockToken = resultToken.SelectToken("list");
                    if (blockToken.HasValues)
                    {
                        var topicList = JsonHelper.DeserializeOrDefault<List<PrivateMessageModel>>(blockToken.ToString());
                        if (topicList != null)
                        {
                            foreach (var model in topicList)
                            {
                                this.MessageList.Add(model);
                            }
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

            this.MessageList.Clear();
        }

        #endregion

        #region base class override
        protected override void EnsureMoreButton()
        {
            if (!this.IsEndPage && !this.MessageList.Contains(this.LoadMoreButtonItem))
            {
                this.MessageList.Add((PrivateMessageModel)this.LoadMoreButtonItem);
            }
        }

        protected override void TryRemoveMoreButton()
        {
            if (this.MessageList.Contains(this.LoadMoreButtonItem))
            {
                this.MessageList.Remove((PrivateMessageModel)this.LoadMoreButtonItem);
            }
        }
        #endregion
    }
}
