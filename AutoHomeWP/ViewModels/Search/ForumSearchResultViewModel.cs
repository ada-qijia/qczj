using CommonLayer;
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
    public class ForumSearchResultViewModel : SearchResultViewModelBase, ISearchViewModel
    {
        public ForumSearchResultViewModel()
        {
            this.RelatedBBSList = new ObservableCollection<RelatedBBSModel>();
            this.BBSList = new ObservableCollection<BBSModel>();
            this.TopicList = new ObservableCollection<TopicModel>();
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

                            JToken blockToken;

                            //相关论坛列表
                            blockToken = resultToken.SelectToken("relatedclubs");
                            this.RelatedBBSList.Clear();
                            if (blockToken.HasValues)
                            {
                                var relatedClubList = JsonHelper.DeserializeOrDefault<List<RelatedBBSModel>>(blockToken.ToString());
                                if (relatedClubList != null)
                                {
                                    foreach (var model in relatedClubList)
                                    {
                                        this.RelatedBBSList.Add(model);
                                    }
                                }
                            }
                            else if(this.DefaultRelatedBBS!=null)
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
            if (this.DefaultRelatedBBS != null)
            {
                foreach (var relatedBBS in this.RelatedBBSList)
                {
                    if (relatedBBS.ID != this.DefaultRelatedBBS.ID)
                    {
                        this.RelatedBBSList.Remove(relatedBBS);
                    }
                }
            }
            else
            {
                this.RelatedBBSList.Clear();
            }

            this.BBSList.Clear();
            this.TopicList.Clear();
        }

        #endregion
    }
}
