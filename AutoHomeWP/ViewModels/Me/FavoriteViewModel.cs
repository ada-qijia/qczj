using CommonLayer;
using Model;
using Model.Me;
using Newtonsoft.Json.Linq;
using System;
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
        }

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

        public ObservableCollection<FavoriteCarSeriesModel> CarSeriesList { get; private set; }

        public ObservableCollection<FavoriteCarSpecModel> CarSpecList { get; private set; }

        public ObservableCollection<FavoriteArticleModel> ArticleList { get; private set; }

        public ObservableCollection<FavoriteForumModel> ForumList { get; private set; }

        public ObservableCollection<FavoriteTopicModel> TopicList { get; private set; }

        #endregion

        #region public methods

        /// <summary>
        /// Not support FavoriteType.All
        /// </summary>
        /// <param name="type"></param>
        public void LoadMore(FavoriteType type)
        {
            switch (type)
            {
                case FavoriteType.CarSeries:
                    this.loadMore(this.CarSeriesList, this.model.CarSeriesList);
                    break;
                case FavoriteType.CarSpec:
                    this.loadMore(this.CarSpecList, this.model.CarSpecList);
                    break;
                case FavoriteType.Article:
                    this.loadMore(this.ArticleList, this.model.ArticleList);
                    break;
                case FavoriteType.Forum:
                    this.loadMore(this.ForumList, this.model.ForumList);
                    break;
                case FavoriteType.Topic:
                    this.loadMore(this.TopicList, this.model.TopicList);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// load the first page. Support FavoriteType.All
        /// </summary>
        public void Reload(FavoriteType type)
        {
            switch (type)
            {
                case FavoriteType.CarSeries:
                    this.CarSeriesList.Clear();
                    this.LoadMore(type);
                    break;
                case FavoriteType.CarSpec:
                    this.CarSpecList.Clear();
                    this.LoadMore(type);
                    break;
                case FavoriteType.Article:
                    this.ArticleList.Clear();
                    this.LoadMore(type);
                    break;
                case FavoriteType.Forum:
                    this.ForumList.Clear();
                    this.LoadMore(type);
                    break;
                case FavoriteType.Topic:
                    this.TopicList.Clear();
                    this.LoadMore(type);
                    break;
                case FavoriteType.All:
                    this.CarSeriesList.Clear();
                    this.LoadMore(FavoriteType.CarSeries);
                    this.CarSpecList.Clear();
                    this.LoadMore(FavoriteType.CarSpec);
                    this.ArticleList.Clear();
                    this.LoadMore(FavoriteType.Article);
                    this.ForumList.Clear();
                    this.LoadMore(FavoriteType.Forum);
                    this.TopicList.Clear();
                    this.LoadMore(FavoriteType.Topic);
                    break;
                default:
                    break;
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
                this.Reload(type);
            }

            return changed ? this.SaveLocally() : false;
        }

        public void Remove(FavoriteType type, IList models)
        {
            if (models != null)
            {
                switch (type)
                {
                    case FavoriteType.CarSeries:
                        foreach (FavoriteCarSeriesModel model in models)
                        {
                            this.model.CarSeriesList.Remove(model);
                        }
                        break;
                    case FavoriteType.CarSpec:
                        foreach (FavoriteCarSpecModel model in models)
                        {
                            this.model.CarSpecList.Remove(model);
                        }
                        break;
                    case FavoriteType.Article:
                        foreach (FavoriteArticleModel model in models)
                        {
                            this.model.ArticleList.Remove(model);
                        }
                        break;
                    case FavoriteType.Forum:
                        foreach (FavoriteForumModel model in models)
                        {
                            this.model.ForumList.Remove(model);
                        }
                        break;
                    case FavoriteType.Topic:
                        foreach (FavoriteTopicModel model in models)
                        {
                            this.model.TopicList.Remove(model);
                        }
                        break;
                    default:
                        break;
                }

                this.Reload(type);
                this.SaveLocally();
            }
        }

        public void RemoveAll()
        {
            this.model.CarSeriesList.Clear();
            this.model.CarSpecList.Clear();
            this.model.ArticleList.Clear();
            this.model.ForumList.Clear();
            this.model.TopicList.Clear();

            this.Reload(FavoriteType.All);
            this.SaveLocally();
        }

        #endregion

        #region private methods

        private void LoadLocally()
        {
            string content = IsolatedStorageFileHelper.ReadIsoFile(FilePath);
            this.model = JsonHelper.DeserializeOrDefault<MyFavoriteModel>(content);
            if (model == null)
            {
                model = new MyFavoriteModel();
                model.CarSeriesList = new List<FavoriteCarSeriesModel>();
                model.CarSpecList = new List<FavoriteCarSpecModel>();
                model.ArticleList = new List<FavoriteArticleModel>();
                model.ForumList = new List<FavoriteForumModel>();
                model.TopicList = new List<FavoriteTopicModel>();
            }
        }

        private bool SaveLocally()
        {
            string content = JsonHelper.Serialize(this.model);
            bool result = IsolatedStorageFileHelper.WriteIsoFile(FilePath, content, System.IO.FileMode.Create);
            return result;
        }

        #endregion

        #region web request

        public void LoadFromServer(string url)
        {
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
                    JToken resultToken = json.SelectToken("result");
                }
                catch
                {
                }
            }
        }
        #endregion

        #region 多页

        public LoadMoreItem LoadMoreButtonItem { get; set; }

        private void loadMore(IList toList, IList fromList)
        {
            if (fromList.Count > toList.Count)
            {
                this.TryRemoveMoreButton(toList);
                int leftCnt = Math.Min(pageCount, fromList.Count - toList.Count);
                for (int i = toList.Count; i < leftCnt; i++)
                {
                    toList.Add(fromList[i]);
                }
                this.EnsureMoreButton(toList, fromList);
            }
        }

        private void EnsureMoreButton(IList inList, IList sourceList)
        {
            if (inList.Count < sourceList.Count && (!inList.Contains(this.LoadMoreButtonItem)))
            {
                inList.Add(this.LoadMoreButtonItem);
            }
        }

        private void TryRemoveMoreButton(IList fromList)
        {
            if (fromList != null && fromList.Contains(this.LoadMoreButtonItem))
            {
                fromList.Remove(this.LoadMoreButtonItem);
            }
        }

        #endregion
    }
}
