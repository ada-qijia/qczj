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
    public class ForumSearchResultViewModel :SearchResultViewModelBase, ISearchViewModel
    {
        public ForumSearchResultViewModel()
        {
            this.BBSList = new ObservableCollection<BBSModel>();
            this.TopicList = new ObservableCollection<TopicModel>();
        }

        #region properties

        public int BBSCount { get; set; }

        public ObservableCollection<BBSModel> BBSList { get; private set; }

        public int TopicCount { get; set; }

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

                            JToken blockToken;
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
            this.BBSCount = 0;
            this.TopicCount = 0;
            this.BBSList.Clear();
            this.TopicList.Clear();
        }

        #endregion
    }
}
