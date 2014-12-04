using Model.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Net;
using Newtonsoft.Json.Linq;
using CommonLayer;

namespace ViewModels.Search
{
    public class VideoSearchResultViewModel : SearchResultViewModelBase, ISearchViewModel
    {
        public VideoSearchResultViewModel()
        {
            this.VideoList = new ObservableCollection<VideoSearchModel>();
            this.LoadMoreButtonItem = new VideoSearchModel() { IsLoadMore = true };
            this.DownloadStringCompleted += VideoSearchResultViewModel_DownloadStringCompleted;
        }

        #region properties

        public ObservableCollection<VideoSearchModel> VideoList { get; private set; }

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

        private void VideoSearchResultViewModel_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
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
                    //视频列表
                    blockToken = resultToken.SelectToken("list");
                    if (blockToken.HasValues)
                    {
                        var modelList = JsonHelper.DeserializeOrDefault<List<VideoSearchModel>>(blockToken.ToString());
                        if (modelList != null)
                        {
                            foreach (var model in modelList)
                            {
                                this.VideoList.Add(model);
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
            this.PageCount = 0;
            this.PageIndex = 0;
            this.VideoList.Clear();
        }

        #endregion

        #region base class override
        protected override void EnsureMoreButton()
        {
            if (!this.IsEndPage && !this.VideoList.Contains(this.LoadMoreButtonItem))
            {
                this.VideoList.Add((VideoSearchModel)this.LoadMoreButtonItem);
            }
        }

        protected override void TryRemoveMoreButton()
        {
            if (this.VideoList.Contains(this.LoadMoreButtonItem))
            {
                this.VideoList.Remove((VideoSearchModel)this.LoadMoreButtonItem);
            }
        }
        #endregion
    }
}
