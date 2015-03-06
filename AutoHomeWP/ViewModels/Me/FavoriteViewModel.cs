using CommonLayer;
using Model;
using Model.Me;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace ViewModels.Me
{
    public class FavoriteViewModel
    {
        private const int pageCount = 20;

        public static string FilePath;

        private MyFavoriteModel model;

        private FavoriteViewModel()
        {
            this.CarSeriesList = new ObservableCollection<FavoriteCarSeriesModel>();
            this.CarSpecList = new ObservableCollection<FavoriteCarSpecModel>();
            this.ArticleList = new ObservableCollection<FavoriteArticleModel>();
            this.ForumList = new ObservableCollection<FavoriteForumModel>();
            this.TopicList = new ObservableCollection<FavoriteTopicModel>();

            this.FavoriteBlockHeaders = new Dictionary<FavoriteType, FavoriteBlockInfo>();
        }

        #region public methods

        public bool UploadFavoriteSuccess(UploadStringCompletedEventArgs e)
        {
            bool success = false;
            try
            {
                if (e.Error == null && e.Result != null)
                {
                    JObject json = JObject.Parse(e.Result);
                    int resultCode = (int)json.SelectToken("returncode");
                    success = resultCode == 0;
                }
            }
            catch
            { }

            return success;
        }

        #endregion

        #region properties

        private static FavoriteViewModel _singleInstance;
        public static FavoriteViewModel SingleInstance
        {
            get
            {
                if (_singleInstance == null)
                {
                    _singleInstance = new FavoriteViewModel();
                    _singleInstance.LoadLocally();
                }
                return _singleInstance;
            }
        }

        public Dictionary<FavoriteType, FavoriteBlockInfo> FavoriteBlockHeaders { get; private set; }

        public ObservableCollection<FavoriteCarSeriesModel> CarSeriesList { get; private set; }

        public ObservableCollection<FavoriteCarSpecModel> CarSpecList { get; private set; }

        public ObservableCollection<FavoriteArticleModel> ArticleList { get; private set; }

        public ObservableCollection<FavoriteForumModel> ForumList { get; private set; }

        public ObservableCollection<FavoriteTopicModel> TopicList { get; private set; }

        #endregion

        #region 本地方法

        /// <summary>
        /// load the from saved items. Support FavoriteType.All
        /// </summary>
        public void ReloadLocally(FavoriteType type)
        {
            switch (type)
            {
                case FavoriteType.CarSeries:
                    this.CarSeriesList.Clear();
                    this.ReloadBindingCollection(type);
                    break;
                case FavoriteType.CarSpec:
                    this.CarSpecList.Clear();
                    this.ReloadBindingCollection(type);
                    break;
                case FavoriteType.Article:
                    this.ArticleList.Clear();
                    this.ReloadBindingCollection(type);
                    break;
                case FavoriteType.Forum:
                    this.ForumList.Clear();
                    this.ReloadBindingCollection(type);
                    break;
                case FavoriteType.Topic:
                    this.TopicList.Clear();
                    this.ReloadBindingCollection(type);
                    break;
                case FavoriteType.All:
                    this.CarSeriesList.Clear();
                    this.ReloadBindingCollection(FavoriteType.CarSeries);
                    this.CarSpecList.Clear();
                    this.ReloadBindingCollection(FavoriteType.CarSpec);
                    this.ArticleList.Clear();
                    this.ReloadBindingCollection(FavoriteType.Article);
                    this.ForumList.Clear();
                    this.ReloadBindingCollection(FavoriteType.Forum);
                    this.TopicList.Clear();
                    this.ReloadBindingCollection(FavoriteType.Topic);
                    break;
                default:
                    break;
            }
        }

        public bool Exist(FavoriteType type, int id)
        {
            switch (type)
            {
                case FavoriteType.CarSeries:
                    return this.model.CarSeriesList.FirstOrDefault(m => m.ID == id) != null;
                case FavoriteType.CarSpec:
                    return this.model.CarSpecList.FirstOrDefault(m => m.ID == id) != null;
                case FavoriteType.Article:
                    return this.model.ArticleList.FirstOrDefault(m => m.ID == id) != null;
                case FavoriteType.Forum:
                    return this.model.ForumList.FirstOrDefault(m => m.ID == id) != null;
                case FavoriteType.Topic:
                    return this.model.TopicList.FirstOrDefault(m => m.ID == id) != null;
                default:
                    return false;
            }
        }

        public bool Add(FavoriteType type, object item)
        {
            bool changed = false;
            switch (type)
            {
                case FavoriteType.CarSeries:
                    var carSeriesModel = item as FavoriteCarSeriesModel;
                    if (carSeriesModel != null)
                    {
                        var find = this.model.CarSeriesList.FirstOrDefault(m => m.ID == carSeriesModel.ID);
                        if (find == null)
                        {
                            carSeriesModel.Action = 1;
                            this.model.CarSeriesList.Insert(0, carSeriesModel);
                            changed = true;
                        }
                    }
                    break;
                case FavoriteType.CarSpec:
                    var carSpecModel = item as FavoriteCarSpecModel;
                    if (carSpecModel != null)
                    {
                        var find = this.model.CarSpecList.FirstOrDefault(m => m.ID == carSpecModel.ID);
                        if (find == null)
                        {
                            carSpecModel.Action = 1;
                            this.model.CarSpecList.Insert(0, carSpecModel);
                            changed = true;
                        }
                    }
                    break;
                case FavoriteType.Article:
                    var articleModel = item as FavoriteArticleModel;
                    if (articleModel != null)
                    {
                        var find = this.model.ArticleList.FirstOrDefault(m => m.ID == articleModel.ID);
                        if (find == null)
                        {
                            articleModel.Action = 1;
                            this.model.ArticleList.Insert(0, articleModel);
                            changed = true;
                        }
                    }
                    break;
                case FavoriteType.Forum:
                    var forumModel = item as FavoriteForumModel;
                    if (forumModel != null)
                    {
                        var find = this.model.ForumList.FirstOrDefault(m => m.ID == forumModel.ID);
                        if (find == null)
                        {
                            forumModel.Action = 1;
                            this.model.ForumList.Insert(0, forumModel);
                            changed = true;
                        }
                    }
                    break;
                case FavoriteType.Topic:
                    var topicModel = item as FavoriteTopicModel;
                    if (topicModel != null)
                    {
                        var find = this.model.TopicList.FirstOrDefault(m => m.ID == topicModel.ID);
                        if (find == null)
                        {
                            topicModel.Action = 1;
                            this.model.TopicList.Insert(0, topicModel);
                            changed = true;
                        }
                    }
                    break;
                default:
                    break;
            }

            if (changed)
            {
                this.ReloadLocally(type);
            }

            return changed ? this.SaveLocally() : false;
        }

        public bool Remove(FavoriteType type, List<int> ids)
        {
            if (ids == null) return false;

            bool changed = false;
            switch (type)
            {
                case FavoriteType.CarSeries:
                    foreach (int id in ids)
                    {
                        var find = this.model.CarSeriesList.FirstOrDefault(m => m.ID == id);
                        if (find != null)
                        {
                            find.Action = 2;
                            // this.model.CarSeriesList.Remove(find);
                            changed = true;
                        }
                    }
                    break;
                case FavoriteType.CarSpec:
                    foreach (int id in ids)
                    {
                        var findSpec = this.model.CarSpecList.FirstOrDefault(m => m.ID == id);
                        if (findSpec != null)
                        {
                            findSpec.Action = 2;
                            //this.model.CarSpecList.Remove(findSpec);
                            changed = true;
                        }
                    }
                    break;
                case FavoriteType.Article:
                    foreach (int id in ids)
                    {
                        var findArticle = this.model.ArticleList.FirstOrDefault(m => m.ID == id);
                        if (findArticle != null)
                        {
                            findArticle.Action = 2;
                            // this.model.ArticleList.Remove(findArticle);
                            changed = true;
                        }
                    }
                    break;
                case FavoriteType.Forum:
                    foreach (int id in ids)
                    {
                        var findForum = this.model.ForumList.FirstOrDefault(m => m.ID == id);
                        if (findForum != null)
                        {
                            findForum.Action = 2;
                            //this.model.ForumList.Remove(findForum);
                            changed = true;
                        }
                    }
                    break;
                case FavoriteType.Topic:
                    foreach (int id in ids)
                    {
                        var findTopic = this.model.TopicList.FirstOrDefault(m => m.ID == id);
                        if (findTopic != null)
                        {
                            findTopic.Action = 2;
                            //this.model.TopicList.Remove(findTopic);
                            changed = true;
                        }
                    }
                    break;
                default:
                    break;
            }

            if (changed)
            {
                this.ReloadLocally(type);
            }

            return changed ? this.SaveLocally() : false;
        }

        //public void Remove(FavoriteType type, IList models)
        //{
        //    if (models != null)
        //    {
        //        switch (type)
        //        {
        //            case FavoriteType.CarSeries:
        //                foreach (FavoriteCarSeriesModel model in models)
        //                {
        //                    this.model.CarSeriesList.Remove(model);
        //                }
        //                break;
        //            case FavoriteType.CarSpec:
        //                foreach (FavoriteCarSpecModel model in models)
        //                {
        //                    this.model.CarSpecList.Remove(model);
        //                }
        //                break;
        //            case FavoriteType.Article:
        //                foreach (FavoriteArticleModel model in models)
        //                {
        //                    this.model.ArticleList.Remove(model);
        //                }
        //                break;
        //            case FavoriteType.Forum:
        //                foreach (FavoriteForumModel model in models)
        //                {
        //                    this.model.ForumList.Remove(model);
        //                }
        //                break;
        //            case FavoriteType.Topic:
        //                foreach (FavoriteTopicModel model in models)
        //                {
        //                    this.model.TopicList.Remove(model);
        //                }
        //                break;
        //            default:
        //                break;
        //        }

        //        this.ReloadLocally(type);
        //        this.SaveLocally();
        //    }
        //}

        public void RemoveAll()
        {
            this.model.CarSeriesList.Clear();
            this.model.CarSpecList.Clear();
            this.model.ArticleList.Clear();
            this.model.ForumList.Clear();
            this.model.TopicList.Clear();

            this.ReloadLocally(FavoriteType.All);
            this.SaveLocally();
        }

        #endregion

        #region private methods

        private void LoadLocally()
        {
            string content = IsolatedStorageFileHelper.ReadIsoFile(FilePath);
            this.model = JsonHelper.DeserializeOrDefault<MyFavoriteModel>(content);

            model = model ?? new MyFavoriteModel();
            model.CarSeriesList = model.CarSeriesList ?? new List<FavoriteCarSeriesModel>();
            model.CarSpecList = model.CarSpecList ?? new List<FavoriteCarSpecModel>();
            model.ArticleList = model.ArticleList ?? new List<FavoriteArticleModel>();
            model.ForumList = model.ForumList ?? new List<FavoriteForumModel>();
            model.TopicList = model.TopicList ?? new List<FavoriteTopicModel>();
        }

        private bool SaveLocally()
        {
            string content = JsonHelper.Serialize(this.model);
            bool result = IsolatedStorageFileHelper.WriteIsoFile(FilePath, content, System.IO.FileMode.Create);
            return result;
        }

        //只保留未同步的,不支持all
        private void ClearData(FavoriteType type)
        {
            switch (type)
            {
                case FavoriteType.CarSeries:
                    var syncedCarSeries = model.CarSeriesList.FindAll(item => item.Action == 0);
                    foreach (var item in syncedCarSeries)
                    {
                        model.CarSeriesList.Remove(item);
                    }
                    break;
                case FavoriteType.CarSpec:
                    var syncedCarSpecs = model.CarSpecList.FindAll(item => item.Action == 0);
                    foreach (var item in syncedCarSpecs)
                    {
                        model.CarSpecList.Remove(item);
                    }
                    break;
                case FavoriteType.Forum:
                    var syncedForum = model.ForumList.FindAll(item => item.Action == 0);
                    foreach (var item in syncedForum)
                    {
                        model.ForumList.Remove(item);
                    }
                    break;
                case FavoriteType.Topic:
                    var syncedTopic = model.TopicList.FindAll(item => item.Action == 0);
                    foreach (var item in syncedTopic)
                    {
                        model.TopicList.Remove(item);
                    }
                    break;
                case FavoriteType.Article:
                    var syncedArticle = model.ArticleList.FindAll(item => item.Action == 0);
                    foreach (var item in syncedArticle)
                    {
                        model.ArticleList.Remove(item);
                    }
                    break;
            }

            this.ReloadLocally(type);
        }

        /// <summary>
        /// Not support FavoriteType.All, from model lists to observable collections.
        /// </summary>
        private void ReloadBindingCollection(FavoriteType type)
        {
            switch (type)
            {
                case FavoriteType.CarSeries:
                    foreach (var item in this.model.CarSeriesList)
                    {
                        if (item.Action != 2)
                        {
                            this.CarSeriesList.Add(item);
                        }
                    }
                    break;
                case FavoriteType.CarSpec:
                    foreach (var item in this.model.CarSpecList)
                    {
                        if (item.Action != 2)
                        {
                            this.CarSpecList.Add(item);
                        }
                    }
                    break;
                case FavoriteType.Article:
                    foreach (var item in this.model.ArticleList)
                    {
                        if (item.Action != 2)
                        {
                            this.ArticleList.Add(item);
                        }
                    }
                    break;
                case FavoriteType.Forum:
                    foreach (var item in this.model.ForumList)
                    {
                        if (item.Action != 2)
                        {
                            this.ForumList.Add(item);
                        }
                    }
                    break;
                case FavoriteType.Topic:
                    foreach (var item in this.model.TopicList)
                    {
                        if (item.Action != 2)
                        {
                            this.TopicList.Add(item);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 网络请求

        #region 下载

        private DownloadStringCompletedEventHandler downloadCompleted;

        public void LoadFromServer(string url, DownloadStringCompletedEventHandler completed)
        {
            this.downloadCompleted = completed;
            CommonLayer.CommonHelper.DownloadStringAsync(url, ViewModel_DownloadStringCompleted);
        }

        private void ViewModel_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null && e.Result != null)
            {
                try
                {
                    //返回的json数据
                    JObject json = JObject.Parse(e.Result);
                    int returncode = json.SelectToken("returncode").Value<int>();
                    if (returncode == 0)
                    {
                        JToken resultToken = json.SelectToken("result");

                        //各收藏列表
                        JToken blockToken;

                        #region 论坛列表

                        blockToken = resultToken.SelectToken("clublist");
                        if (blockToken != null)
                        {
                            this.TryRemoveMoreButton(this.ForumList, LoadMoreForumItem);

                            this.HandleBlockHeader(blockToken, FavoriteType.Forum);
                            var clublist = JsonHelper.DeserializeOrDefault<List<FavoriteForumModel>>(blockToken.SelectToken("list").ToString());
                            foreach (var item in clublist)
                            {
                                if (this.model.ForumList.FirstOrDefault(m => m.ID == item.ID) == null)
                                {
                                    this.model.ForumList.Add(item);
                                }
                                if (this.ForumList.FirstOrDefault(m => m.ID == item.ID) == null)
                                {
                                    this.ForumList.Add(item);
                                }
                            }

                            this.EnsureMoreButton(FavoriteType.Forum, this.ForumList, LoadMoreForumItem);
                        }


                        #endregion

                        #region 帖子列表

                        blockToken = resultToken.SelectToken("topiclist");
                        if (blockToken != null)
                        {
                            this.TryRemoveMoreButton(this.TopicList, LoadMoreTopicItem);

                            this.HandleBlockHeader(blockToken, FavoriteType.Topic);
                            var itemlist = JsonHelper.DeserializeOrDefault<List<FavoriteTopicModel>>(blockToken.SelectToken("list").ToString());
                            foreach (var item in itemlist)
                            {
                                if (this.model.TopicList.FirstOrDefault(m => m.ID == item.ID) == null)
                                {
                                    this.model.TopicList.Add(item);
                                }
                                if (this.TopicList.FirstOrDefault(m => m.ID == item.ID) == null)
                                {
                                    this.TopicList.Add(item);
                                }
                            }

                            this.EnsureMoreButton(FavoriteType.Topic, this.TopicList, LoadMoreTopicItem);
                        }

                        //添加加载更多
                        #endregion

                        #region 文章列表

                        blockToken = resultToken.SelectToken("articlelist");
                        if (blockToken != null)
                        {
                            this.TryRemoveMoreButton(this.ArticleList, LoadMoreArticleItem);

                            this.HandleBlockHeader(blockToken, FavoriteType.Article);
                            var itemlist = JsonHelper.DeserializeOrDefault<List<FavoriteArticleModel>>(blockToken.SelectToken("list").ToString());
                            foreach (var item in itemlist)
                            {
                                if (this.model.ArticleList.FirstOrDefault(m => m.ID == item.ID) == null)
                                {
                                    this.model.ArticleList.Add(item);
                                }
                                if (this.ArticleList.FirstOrDefault(m => m.ID == item.ID) == null)
                                {
                                    this.ArticleList.Add(item);
                                }
                            }

                            this.EnsureMoreButton(FavoriteType.Article, this.ArticleList, LoadMoreArticleItem);
                        }

                        #endregion

                        #region 车系列表

                        blockToken = resultToken.SelectToken("serieslist");
                        if (blockToken != null)
                        {
                            this.TryRemoveMoreButton(this.CarSeriesList, LoadMoreCarSeriesItem);

                            this.HandleBlockHeader(blockToken, FavoriteType.CarSeries);
                            var itemlist = JsonHelper.DeserializeOrDefault<List<FavoriteCarSeriesModel>>(blockToken.SelectToken("list").ToString());
                            foreach (var item in itemlist)
                            {
                                if (this.model.CarSeriesList.FirstOrDefault(m => m.ID == item.ID) == null)
                                {
                                    this.model.CarSeriesList.Add(item);
                                }
                                if (this.CarSeriesList.FirstOrDefault(m => m.ID == item.ID) == null)
                                {
                                    this.CarSeriesList.Add(item);
                                }
                            }

                            this.EnsureMoreButton(FavoriteType.CarSeries, this.CarSeriesList, LoadMoreCarSeriesItem);
                        }
                        #endregion

                        #region 车型列表

                        blockToken = resultToken.SelectToken("speclist");
                        if (blockToken != null)
                        {
                            this.TryRemoveMoreButton(this.CarSpecList, LoadMoreCarSpecItem);

                            this.HandleBlockHeader(blockToken, FavoriteType.CarSpec);
                            var itemlist = JsonHelper.DeserializeOrDefault<List<FavoriteCarSpecModel>>(blockToken.SelectToken("list").ToString());
                            foreach (var item in itemlist)
                            {
                                if (this.model.CarSpecList.FirstOrDefault(m => m.ID == item.ID) == null)
                                {
                                    this.model.CarSpecList.Add(item);
                                }
                                if (this.CarSpecList.FirstOrDefault(m => m.ID == item.ID) == null)
                                {
                                    this.CarSpecList.Add(item);
                                }
                            }

                            this.EnsureMoreButton(FavoriteType.CarSpec, this.CarSpecList, LoadMoreCarSpecItem);
                        }
                        #endregion

                        this.SaveLocally();
                    }
                }
                catch
                {
                }
            }

            if (this.downloadCompleted != null)
            {
                this.downloadCompleted(this, e);
            }
        }

        private void HandleBlockHeader(JToken blockToken, FavoriteType type)
        {
            var blockHeader = GetBlockInfoFromToken(blockToken);
            this.FavoriteBlockHeaders[type] = blockHeader;
            if (blockHeader != null && blockHeader.PageIndex == 1)
            {
                ClearData(type);
            }
        }

        private FavoriteBlockInfo GetBlockInfoFromToken(JToken blockToken)
        {
            try
            {
                var rowCount = blockToken.SelectToken("rowcount").Value<int>();
                var pageIndex = blockToken.SelectToken("pageindex").Value<int>();
                var pageCount = blockToken.SelectToken("pagecount").Value<int>();
                var blockInfo = new FavoriteBlockInfo() { RowCount = rowCount, PageCount = pageCount, PageIndex = pageIndex };
                return blockInfo;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region 上传

        public List<FavoriteSyncItem> GetUnsyncedItems(FavoriteType type)
        {
            switch (type)
            {
                case FavoriteType.CarSeries:
                    return model.CarSeriesList.FindAll(item => item.Action != 0).Select(item => new FavoriteSyncItem() { id = item.ID, time = item.Time, action = item.Action - 1 }).ToList();
                case FavoriteType.CarSpec:
                    return model.CarSpecList.FindAll(item => item.Action != 0).Select(item => new FavoriteSyncItem() { id = item.ID, time = item.Time, action = item.Action - 1 }).ToList();
                case FavoriteType.Forum:
                    return model.ForumList.FindAll(item => item.Action != 0).Select(item => new FavoriteSyncItem() { id = item.ID, time = item.Time, action = item.Action - 1 }).ToList();
                case FavoriteType.Topic:
                    return model.TopicList.FindAll(item => item.Action != 0).Select(item => new FavoriteSyncItem() { id = item.ID, time = item.Time, action = item.Action - 1 }).ToList();
                case FavoriteType.Article:
                    return model.ArticleList.FindAll(item => item.Action != 0).Select(item => new FavoriteSyncItem() { id = item.ID, time = item.Time, action = item.Action - 1 }).ToList();
                default:
                    break;
            }
            return null;
        }

        public string GetUnsyncedItemsInJson(FavoriteType type)
        {
            switch (type)
            {
                case FavoriteType.CarSeries:
                    var unsyncedCarSeries = model.CarSeriesList.FindAll(item => item.Action != 0).Select(item => new FavoriteSyncItem() { id = item.ID, time = item.Time, action = item.Action - 1 }).ToList();
                    return unsyncedCarSeries.Count == 0 ? null : JsonHelper.Serialize(unsyncedCarSeries);
                case FavoriteType.CarSpec:
                    var unsyncedCarSpec = model.CarSpecList.FindAll(item => item.Action != 0).Select(item => new FavoriteSyncItem() { id = item.ID, time = item.Time, action = item.Action - 1 }).ToList();
                    return unsyncedCarSpec.Count == 0 ? null : JsonHelper.Serialize(unsyncedCarSpec);
                case FavoriteType.Forum:
                    var unsyncedForum = model.ForumList.FindAll(item => item.Action != 0).Select(item => new FavoriteSyncItem() { id = item.ID, time = item.Time, action = item.Action - 1 }).ToList();
                    return unsyncedForum.Count == 0 ? null : JsonHelper.Serialize(unsyncedForum);
                case FavoriteType.Topic:
                    var unsyncedTopic = model.TopicList.FindAll(item => item.Action != 0).Select(item => new FavoriteSyncItem() { id = item.ID, time = item.Time, action = item.Action - 1 }).ToList();
                    return unsyncedTopic.Count == 0 ? null : JsonHelper.Serialize(unsyncedTopic);
                case FavoriteType.Article:
                    var unsyncedArticle = model.ArticleList.FindAll(item => item.Action != 0).Select(item => new FavoriteSyncItem() { id = item.ID, time = item.Time, action = item.Action - 1 }).ToList();
                    return unsyncedArticle.Count == 0 ? null : JsonHelper.Serialize(unsyncedArticle);
                default:
                    break;
            }
            return null;
        }

        private UploadStringCompletedEventHandler uploadCarCompletedHandler;

        //车系，车型
        public void UploadCar(string url, string data, UploadStringCompletedEventHandler completedHandler, List<FavoriteSyncItem> carseries, List<FavoriteSyncItem> carspecs)
        {
            this.uploadCarCompletedHandler = completedHandler;
            var userState = new Dictionary<FavoriteType, List<FavoriteSyncItem>>();
            userState[FavoriteType.CarSeries] = carseries;
            userState[FavoriteType.CarSpec] = carspecs;
            UpStreamViewModel.SingleInstance.UploadAsync(url, data, uploadClient_UploadCarCompleted, userState);
        }

        private UploadStringCompletedEventHandler uploadOthersCompletedHandler;
        //论坛，帖子，文章
        public void UploadOthers(string url, string data, UploadStringCompletedEventHandler completedHandler, List<FavoriteSyncItem> forums, List<FavoriteSyncItem> topics, List<FavoriteSyncItem> articles)
        {
            this.uploadOthersCompletedHandler = completedHandler;
            var userState = new Dictionary<FavoriteType, List<FavoriteSyncItem>>();
            userState[FavoriteType.Forum] = forums;
            userState[FavoriteType.Topic] = topics;
            userState[FavoriteType.Article] = articles;
            UpStreamViewModel.SingleInstance.UploadAsync(url, data, uploadClient_UploadOthersCompleted, userState);
        }

        void uploadClient_UploadCarCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && e.Result != null)
                {
                    JObject json = JObject.Parse(e.Result);
                    int resultCode = (int)json.SelectToken("returncode");
                    string message = json.SelectToken("message").ToString();
                    switch (resultCode)
                    {
                        case 0://成功
                            var syncItems = e.UserState as Dictionary<FavoriteType, List<FavoriteSyncItem>>;
                            if (syncItems != null && syncItems.Count == 2)
                            {
                                //更新车系
                                var carSeries = syncItems[FavoriteType.CarSeries];
                                if (carSeries != null && carSeries.Count > 0)
                                {
                                    foreach (var item in carSeries)
                                    {
                                        var found = this.model.CarSeriesList.FirstOrDefault(m => m.ID == item.id);
                                        if (found != null)
                                        {
                                            if (item.action == 0)
                                            {
                                                found.Action = 0;
                                            }
                                            else if (item.action == 1)
                                            {
                                                this.model.CarSeriesList.Remove(found);
                                            }
                                        }
                                    }
                                }

                                //更新车型
                                var carSpecs = syncItems[FavoriteType.CarSpec];
                                if (carSpecs != null && carSpecs.Count > 0)
                                {
                                    foreach (var item in carSpecs)
                                    {
                                        var found = this.model.CarSpecList.FirstOrDefault(m => m.ID == item.id);
                                        if (found != null)
                                        {
                                            if (item.action == 0)
                                            {
                                                found.Action = 0;
                                            }
                                            else if (item.action == 1)
                                            {
                                                this.model.CarSpecList.Remove(found);
                                            }
                                        }
                                    }
                                }

                                this.SaveLocally();
                            }
                            break;
                        default://其他情况...
                            break;
                    }
                }
            }
            catch
            { }

            if (uploadCarCompletedHandler != null)
            {
                uploadCarCompletedHandler(this, e);
            }
        }

        void uploadClient_UploadOthersCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && e.Result != null)
                {
                    JObject json = JObject.Parse(e.Result);
                    int resultCode = (int)json.SelectToken("returncode");
                    string message = json.SelectToken("message").ToString();
                    switch (resultCode)
                    {
                        case 0://成功
                            var syncItems = e.UserState as Dictionary<FavoriteType, List<FavoriteSyncItem>>;
                            if (syncItems != null && syncItems.Count == 3)
                            {
                                var forums = syncItems[FavoriteType.Forum];
                                if (forums != null && forums.Count > 0)
                                {
                                    foreach (var item in forums)
                                    {
                                        var found = this.model.ForumList.FirstOrDefault(m => m.ID == item.id);
                                        if (found != null)
                                        {
                                            if (item.action == 0)
                                            {
                                                found.Action = 0;
                                            }
                                            else if (item.action == 1)
                                            {
                                                this.model.ForumList.Remove(found);
                                            }
                                        }
                                    }
                                }

                                var topics = syncItems[FavoriteType.Topic];
                                if (topics != null && topics.Count > 0)
                                {
                                    foreach (var item in topics)
                                    {
                                        var found = this.model.TopicList.FirstOrDefault(m => m.ID == item.id);
                                        if (found != null)
                                        {
                                            if (item.action == 0)
                                            {
                                                found.Action = 0;
                                            }
                                            else if (item.action == 1)
                                            {
                                                this.model.TopicList.Remove(found);
                                            }
                                        }
                                    }
                                }

                                var articles = syncItems[FavoriteType.Article];
                                if (articles != null && articles.Count > 0)
                                {
                                    foreach (var item in articles)
                                    {
                                        var found = this.model.ArticleList.FirstOrDefault(m => m.ID == item.id);
                                        if (found != null)
                                        {
                                            if (item.action == 0)
                                            {
                                                found.Action = 0;
                                            }
                                            else if (item.action == 1)
                                            {
                                                this.model.ArticleList.Remove(found);
                                            }
                                        }
                                    }
                                }
                            }

                            this.SaveLocally();
                            break;
                        default://其他情况...
                            break;
                    }
                }
            }
            catch
            { }

            if (this.uploadOthersCompletedHandler != null)
            {
                this.uploadOthersCompletedHandler(this, e);
            }
        }

        #endregion

        #endregion

        #region 多页

        public FavoriteForumModel LoadMoreForumItem = new FavoriteForumModel { IsLoadMore = true };

        public FavoriteCarSeriesModel LoadMoreCarSeriesItem = new FavoriteCarSeriesModel { IsLoadMore = true };

        public FavoriteCarSpecModel LoadMoreCarSpecItem = new FavoriteCarSpecModel { IsLoadMore = true };

        public FavoriteArticleModel LoadMoreArticleItem = new FavoriteArticleModel { IsLoadMore = true };

        public FavoriteTopicModel LoadMoreTopicItem = new FavoriteTopicModel { IsLoadMore = true };

        private void EnsureMoreButton(FavoriteType type, IList collection, LoadMoreItem item)
        {
            var blockInfo = this.FavoriteBlockHeaders[type];
            if (blockInfo != null && blockInfo.PageIndex < blockInfo.RowCount)
            {
                collection.Add(item);
            }
        }

        private void TryRemoveMoreButton(IList collection, LoadMoreItem item)
        {
            if (collection.Contains(item))
            {
                collection.Remove(item);
            }
        }

        #endregion

        #region 辅助类

        /// <summary>
        /// 用于上传
        /// </summary>
        public class FavoriteSyncItem
        {
            public int id { get; set; }

            /// <summary>
            /// 格式为YYYY-MM-dd HH:mm:ss
            /// </summary>
            public string time { get; set; }

            /// <summary>
            /// 0，添加收藏，1取消收藏
            /// </summary>
            public int action { get; set; }
        }

        public class FavoriteBlockInfo
        {
            public int RowCount;
            public int PageIndex;
            public int PageCount;
        }

        #endregion
    }
}
