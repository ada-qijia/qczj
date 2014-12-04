using CommonLayer;
using Model.Search;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace ViewModels.Search
{
    public class ForumSearchResultViewModel : SearchResultViewModelBase, ISearchViewModel
    {
        public ForumSearchResultViewModel()
        {
            this.RelatedBBSList = new ObservableCollection<RelatedBBSModel>();
            this.BBSList = new ObservableCollection<BBSModel>();
            this.TopicList = new ObservableCollection<TopicModel>();

            this.LoadMoreButtonItem = new TopicModel() { IsLoadMore = true };
            this.DownloadStringCompleted += ForumSearchResultViewModel_DownloadStringCompleted;
        }

        #region properties

        private RelatedBBSModel _defaultRelatedBBS;
        public RelatedBBSModel DefaultRelatedBBS
        {
            get { return this._defaultRelatedBBS; }
            set
            {
                this._defaultRelatedBBS = value;
                var equalItem = this.RelatedBBSList.FirstOrDefault(item => item.ID == value.ID);
                if (equalItem == null)
                {
                    this.RelatedBBSList.Insert(0, value);
                }
            }
        }

        public ObservableCollection<RelatedBBSModel> RelatedBBSList { get; private set; }

        public ObservableCollection<BBSModel> BBSList { get; private set; }

        public ObservableCollection<TopicModel> TopicList { get; private set; }

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

        private void ForumSearchResultViewModel_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
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

                    //相关论坛列表
                    blockToken = resultToken.SelectToken("relatedclubs");
                    if (blockToken.HasValues && this.RelatedBBSList.Count <= 1)//首次加载,以后同样关键词返回结果一样
                    {
                        var relatedClubList = JsonHelper.DeserializeOrDefault<List<RelatedBBSModel>>(blockToken.ToString());
                        if (relatedClubList != null && relatedClubList.Count > 0)
                        {
                            this.RelatedBBSList.Clear();
                            foreach (var model in relatedClubList)
                            {
                                this.RelatedBBSList.Add(model);
                            }
                        }
                    }

                    if (this.RelatedBBSList.Count == 0 && this.DefaultRelatedBBS != null)
                    {
                        this.RelatedBBSList.Add(DefaultRelatedBBS);
                    }

                    //论坛列表
                    blockToken = resultToken.SelectToken("clublist");
                    if (blockToken.HasValues)
                    {
                        var clubList = JsonHelper.DeserializeOrDefault<List<BBSModel>>(blockToken.ToString());
                        if (clubList != null)
                        {
                            foreach (var model in clubList)
                            {
                                this.BBSList.Add(model);
                            }
                        }
                    }

                    //文章列表
                    blockToken = resultToken.SelectToken("topiclist");
                    if (blockToken.HasValues)
                    {
                        var topicList = JsonHelper.DeserializeOrDefault<List<TopicModel>>(blockToken.ToString());
                        if (topicList != null)
                        {
                            foreach (var model in topicList)
                            {
                                this.TopicList.Add(model);
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

            this.BBSList.Clear();
            this.TopicList.Clear();
        }

        #endregion

        #region base class override
        protected override void EnsureMoreButton()
        {
            if (!this.IsEndPage && !this.TopicList.Contains(this.LoadMoreButtonItem))
            {
                this.TopicList.Add((TopicModel)this.LoadMoreButtonItem);
            }
        }

        protected override void TryRemoveMoreButton()
        {
            if (this.TopicList.Contains(this.LoadMoreButtonItem))
            {
                this.TopicList.Remove((TopicModel)this.LoadMoreButtonItem);
            }
        }
        #endregion

    }
}
