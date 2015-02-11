using CommonLayer;
using Model.Me;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ViewModels.Me
{
    //他的主页
    public class OthersViewModel:Search.SearchResultViewModelBase
    {
        public OthersViewModel()
        {
            this.TopicList = new ObservableCollection<TritanModel>();

            this.LoadMoreButtonItem = new TritanModel() { IsLoadMore = true };
            this.DownloadStringCompleted += ViewModel_DownloadStringCompleted;
        }

        #region properties

        private MeModel _mainInfoModel;
        public MeModel MainInfo
        {
            get { return _mainInfoModel; }
            set { SetProperty<MeModel>(ref _mainInfoModel, value); }
        }

        public ObservableCollection<TritanModel> TopicList { get; private set; }

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

                    //个人信息
                    if(this.MainInfo==null)
                    {
                        this.MainInfo = JsonHelper.DeserializeOrDefault<MeModel>(resultToken.ToString());
                    }

                    //帖子列表
                     JToken blockToken = resultToken.SelectToken("topiclist");
                    if (blockToken.HasValues)
                    {
                        this.RowCount = blockToken.SelectToken("rowcount").Value<int>();
                        this.PageIndex = blockToken.SelectToken("pageindex").Value<int>();
                        this.PageCount = blockToken.SelectToken("pagecount").Value<int>();

                        JToken topicToken = blockToken.SelectToken("list");
                        if (topicToken.HasValues)
                        {
                            var topicList = JsonHelper.DeserializeOrDefault<List<TritanModel>>(topicToken.ToString());
                            if (topicList != null)
                            {
                                foreach (var model in topicList)
                                {
                                    this.TopicList.Add(model);
                                }
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

            this.MainInfo = null;
            this.TopicList.Clear();
        }

        #endregion

        #region base class override

        protected override void EnsureMoreButton()
        {
            if (!this.IsEndPage && !this.TopicList.Contains(this.LoadMoreButtonItem))
            {
                this.TopicList.Add((TritanModel)this.LoadMoreButtonItem);
            }
        }

        protected override void TryRemoveMoreButton()
        {
            if (this.TopicList.Contains(this.LoadMoreButtonItem))
            {
                this.TopicList.Remove((TritanModel)this.LoadMoreButtonItem);
            }
        }

        #endregion
    }
}
