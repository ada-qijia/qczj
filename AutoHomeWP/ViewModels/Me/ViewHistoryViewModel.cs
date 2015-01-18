using CommonLayer;
using Model;
using Model.Me;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ViewModels.Me
{
    public class ViewHistoryViewModel
    {
        private const int maxCount = 50;
        private const int pageCount = 20;

        public static string FilePath;

        private ViewHistoryModel viewHistoryModel;

        private ViewHistoryViewModel()
        {
            this.CarSeriesList = new ObservableCollection<ViewHistoryCarSeriesModel>();
            this.CarSpecList = new ObservableCollection<ViewHistoryCarSpecModel>();
            this.ArticleList = new ObservableCollection<ViewHistoryArticleModel>();
            this.ForumList = new ObservableCollection<ViewHistoryForumModel>();
            this.TopicList = new ObservableCollection<ViewHistoryTopicModel>();

            this.currentPageIndex = new Dictionary<FavoriteType, int>(5);
            currentPageIndex.Add(FavoriteType.CarSeries, 0);
            currentPageIndex.Add(FavoriteType.CarSpec, 0);
            currentPageIndex.Add(FavoriteType.Article, 0);
            currentPageIndex.Add(FavoriteType.Forum, 0);
            currentPageIndex.Add(FavoriteType.Topic, 0);
        }

        #region single instance

        private static ViewHistoryViewModel _singleInstance;
        public static ViewHistoryViewModel SingleInstance
        {
            get
            {
                if (_singleInstance == null)
                {
                    _singleInstance = new ViewHistoryViewModel();
                    _singleInstance.Initialize();
                }
                return _singleInstance;
            }
        }

        #endregion

        #region properties

        public ObservableCollection<ViewHistoryCarSeriesModel> CarSeriesList { get; private set; }

        public ObservableCollection<ViewHistoryCarSpecModel> CarSpecList { get; private set; }

        public ObservableCollection<ViewHistoryArticleModel> ArticleList { get; private set; }

        public ObservableCollection<ViewHistoryForumModel> ForumList { get; private set; }

        public ObservableCollection<ViewHistoryTopicModel> TopicList { get; private set; }

        public Dictionary<FavoriteType, int> currentPageIndex { get; private set; }

        #endregion

        #region public methods

        public void Initialize()
        {
            string content = IsolatedStorageFileHelper.ReadIsoFile(FilePath);
            viewHistoryModel = JsonHelper.DeserializeOrDefault<ViewHistoryModel>(content);

            viewHistoryModel = viewHistoryModel ?? new ViewHistoryModel();
            viewHistoryModel.CarSeriesList = viewHistoryModel.CarSeriesList ?? new List<ViewHistoryCarSeriesModel>();
            viewHistoryModel.CarSpecList = viewHistoryModel.CarSpecList ?? new List<ViewHistoryCarSpecModel>();
            viewHistoryModel.ForumList = viewHistoryModel.ForumList ?? new List<ViewHistoryForumModel>();
            viewHistoryModel.ArticleList = viewHistoryModel.ArticleList ?? new List<ViewHistoryArticleModel>();
            viewHistoryModel.TopicList = viewHistoryModel.TopicList ?? new List<ViewHistoryTopicModel>();
        }

        //Save change locally, update related view to first page.
        public bool AddItem(FavoriteType type, object historyModel)
        {
            bool changed = false;
            switch (type)
            {
                case FavoriteType.CarSeries:
                    var carSeriesModel = historyModel as ViewHistoryCarSeriesModel;
                    if (carSeriesModel != null)
                    {
                        if (this.viewHistoryModel.CarSeriesList.Count == maxCount)
                        {
                            this.viewHistoryModel.CarSeriesList.RemoveAt(maxCount - 1);
                        }
                        this.viewHistoryModel.CarSeriesList.Insert(0, carSeriesModel);
                        changed = true;
                    }
                    break;
                case FavoriteType.CarSpec:
                    var carSpecModel = historyModel as ViewHistoryCarSpecModel;
                    if (carSpecModel != null)
                    {
                        if (this.viewHistoryModel.CarSpecList.Count == maxCount)
                        {
                            this.viewHistoryModel.CarSpecList.RemoveAt(maxCount - 1);
                        }
                        this.viewHistoryModel.CarSpecList.Insert(0, carSpecModel);
                        changed = true;
                    }
                    break;
                case FavoriteType.Article:
                    var articleModel = historyModel as ViewHistoryArticleModel;
                    if (articleModel != null)
                    {
                        if (this.viewHistoryModel.ArticleList.Count == maxCount)
                        {
                            this.viewHistoryModel.ArticleList.RemoveAt(maxCount - 1);
                        }
                        this.viewHistoryModel.ArticleList.Insert(0, articleModel);
                        changed = true;
                    }
                    break;
                case FavoriteType.Forum:
                    var forumModel = historyModel as ViewHistoryForumModel;
                    if (forumModel != null)
                    {
                        if (this.viewHistoryModel.ForumList.Count == maxCount)
                        {
                            this.viewHistoryModel.ForumList.RemoveAt(maxCount - 1);
                        }
                        this.viewHistoryModel.ForumList.Insert(0, forumModel);
                        changed = true;
                    }
                    break;
                case FavoriteType.Topic:
                    var topicModel = historyModel as ViewHistoryTopicModel;
                    if (topicModel != null)
                    {
                        if (this.viewHistoryModel.TopicList.Count == maxCount)
                        {
                            this.viewHistoryModel.TopicList.RemoveAt(maxCount - 1);
                        }
                        this.viewHistoryModel.TopicList.Insert(0, topicModel);
                        changed = true;
                    }
                    break;
                default:
                    break;
            }

            if (changed)
            {
                this.Refresh(type);
            }

            return changed ? this.SaveLocally() : false;
        }

        public bool DeleteAll()
        {
            foreach(var key in this.currentPageIndex.Keys)
            {
                this.currentPageIndex[key] = 0;
            }

            this.CarSeriesList.Clear();
            this.CarSpecList.Clear();
            this.ArticleList.Clear();
            this.ForumList.Clear();
            this.TopicList.Clear();

            this.viewHistoryModel.CarSeriesList.Clear();
            this.viewHistoryModel.CarSpecList.Clear();
            this.viewHistoryModel.ArticleList.Clear();
            this.viewHistoryModel.ForumList.Clear();
            this.viewHistoryModel.TopicList.Clear();

            return this.SaveLocally();
        }

        public bool SaveLocally()
        {
            string content = JsonHelper.Serialize(viewHistoryModel);
            bool result = IsolatedStorageFileHelper.WriteIsoFile(FilePath, content, System.IO.FileMode.Create);
            return result;
        }

        /// <summary>
        /// Reload the first page.
        /// </summary>
        /// <param name="type"></param>
        public void Refresh(FavoriteType type)
        {
            switch (type)
            {
                case FavoriteType.CarSeries:
                    this.CarSeriesList.Clear();
                    this.LoadMore(type, 0);
                    break;
                case FavoriteType.CarSpec:
                    this.CarSpecList.Clear();
                    this.LoadMore(type, 0);
                    break;
                case FavoriteType.Article:
                    this.ArticleList.Clear();
                    this.LoadMore(type, 0);
                    break;
                case FavoriteType.Forum:
                    this.ForumList.Clear();
                    this.LoadMore(type, 0);
                    break;
                case FavoriteType.Topic:
                    this.TopicList.Clear();
                    this.LoadMore(type, 0);
                    break;
                case FavoriteType.All:
                    this.CarSeriesList.Clear();
                    this.LoadMore(FavoriteType.CarSeries, 0);
                    this.CarSpecList.Clear();
                    this.LoadMore(FavoriteType.CarSpec, 0);
                    this.ArticleList.Clear();
                    this.LoadMore(FavoriteType.Article, 0);
                    this.ForumList.Clear();
                    this.LoadMore(FavoriteType.Forum, 0);
                    this.TopicList.Clear();
                    this.LoadMore(FavoriteType.Topic, 0);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Not support ViewHistoryType.All
        /// </summary>
        /// <param name="type"></param>
        /// <param name="toPageIndex"></param>
        public void LoadMore(FavoriteType type, int toPageIndex)
        {
            switch (type)
            {
                case FavoriteType.CarSeries:
                    this.loadMore(this.CarSeriesList, this.viewHistoryModel.CarSeriesList, toPageIndex);
                    break;
                case FavoriteType.CarSpec:
                    this.loadMore(this.CarSpecList, this.viewHistoryModel.CarSpecList, toPageIndex);
                    break;
                case FavoriteType.Article:
                    this.loadMore(this.ArticleList, this.viewHistoryModel.ArticleList, toPageIndex);
                    break;
                case FavoriteType.Forum:
                    this.loadMore(this.ForumList, this.viewHistoryModel.ForumList, toPageIndex);
                    break;
                case FavoriteType.Topic:
                    this.loadMore(this.TopicList, this.viewHistoryModel.TopicList, toPageIndex);
                    break;
                default:
                    break;
            }

            if (type > FavoriteType.All)
            {
                this.currentPageIndex[type] = toPageIndex;
            }
        }

        #endregion

        #region 多页

        public LoadMoreItem LoadMoreButtonItem { get; set; }

        private void loadMore(IList toList, IList fromList, int toPageIndex)
        {
            this.TryRemoveMoreButton(toList);
            int endIndex = Math.Min((toPageIndex + 1) * pageCount, fromList.Count);
            for (int i = toList.Count; i < endIndex; i++)
            {
                toList.Add(fromList[i]);
            }
            this.EnsureMoreButton(toList, fromList);
        }

        private void EnsureMoreButton(IList inList, IList sourceList)
        {
            if (!inList.Contains(this.LoadMoreButtonItem))
            {
                bool isEndPage = inList.Count >= sourceList.Count;
                if (!isEndPage)
                {
                    inList.Add(this.LoadMoreButtonItem);
                }
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
