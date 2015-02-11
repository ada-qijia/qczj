using CommonLayer;
using Model;
using Model.Me;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
            this.CarSeriesList = new ObservableCollection<FavoriteCarSeriesModel>();
            this.CarSpecList = new ObservableCollection<FavoriteCarSpecModel>();
            this.ArticleList = new ObservableCollection<FavoriteArticleModel>();
            this.ForumList = new ObservableCollection<FavoriteForumModel>();
            this.TopicList = new ObservableCollection<FavoriteTopicModel>();
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
                    _singleInstance.LoadLocally();
                }
                return _singleInstance;
            }
        }

        #endregion

        #region properties

        public ObservableCollection<FavoriteCarSeriesModel> CarSeriesList { get; private set; }

        public ObservableCollection<FavoriteCarSpecModel> CarSpecList { get; private set; }

        public ObservableCollection<FavoriteArticleModel> ArticleList { get; private set; }

        public ObservableCollection<FavoriteForumModel> ForumList { get; private set; }

        public ObservableCollection<FavoriteTopicModel> TopicList { get; private set; }

        #endregion

        #region public methods

        //Save change locally, update related view to first page.
        public bool AddItem(FavoriteType type, object historyModel)
        {
            bool changed = false;
            switch (type)
            {
                case FavoriteType.CarSeries:
                    var carSeriesModel = historyModel as FavoriteCarSeriesModel;
                    if (carSeriesModel != null)
                    {
                        var find = this.viewHistoryModel.CarSeriesList.FirstOrDefault(item => item.ID == carSeriesModel.ID);
                        if (find != null)
                        {
                            this.viewHistoryModel.CarSeriesList.Remove(find);
                        }
                        else if (this.viewHistoryModel.CarSeriesList.Count == maxCount)
                        {
                            this.viewHistoryModel.CarSeriesList.RemoveAt(maxCount - 1);
                        }

                        this.viewHistoryModel.CarSeriesList.Insert(0, carSeriesModel);
                        changed = true;
                    }
                    break;
                case FavoriteType.CarSpec:
                    var carSpecModel = historyModel as FavoriteCarSpecModel;
                    if (carSpecModel != null)
                    {
                        var find = this.viewHistoryModel.CarSpecList.FirstOrDefault(item => item.ID == carSpecModel.ID);
                        if (find != null)
                        {
                            this.viewHistoryModel.CarSpecList.Remove(find);
                        }
                        else if (this.viewHistoryModel.CarSpecList.Count == maxCount)
                        {
                            this.viewHistoryModel.CarSpecList.RemoveAt(maxCount - 1);
                        }
                        this.viewHistoryModel.CarSpecList.Insert(0, carSpecModel);
                        changed = true;
                    }
                    break;
                case FavoriteType.Article:
                    var articleModel = historyModel as FavoriteArticleModel;
                    if (articleModel != null)
                    {
                        var find = this.viewHistoryModel.ArticleList.FirstOrDefault(item => item.ID == articleModel.ID);
                        if (find != null)
                        {
                            this.viewHistoryModel.ArticleList.Remove(find);
                        }
                        else if (this.viewHistoryModel.ArticleList.Count == maxCount)
                        {
                            this.viewHistoryModel.ArticleList.RemoveAt(maxCount - 1);
                        }
                        this.viewHistoryModel.ArticleList.Insert(0, articleModel);
                        changed = true;
                    }
                    break;
                case FavoriteType.Forum:
                    var forumModel = historyModel as FavoriteForumModel;
                    if (forumModel != null)
                    {
                        var find = this.viewHistoryModel.ForumList.FirstOrDefault(item => item.ID == forumModel.ID);
                        if (find != null)
                        {
                            this.viewHistoryModel.ForumList.Remove(find);
                        }
                        else if (this.viewHistoryModel.ForumList.Count == maxCount)
                        {
                            this.viewHistoryModel.ForumList.RemoveAt(maxCount - 1);
                        }
                        this.viewHistoryModel.ForumList.Insert(0, forumModel);
                        changed = true;
                    }
                    break;
                case FavoriteType.Topic:
                    var topicModel = historyModel as FavoriteTopicModel;
                    if (topicModel != null)
                    {
                        var find = this.viewHistoryModel.TopicList.FirstOrDefault(item => item.ID == topicModel.ID);
                        if (find != null)
                        {
                            this.viewHistoryModel.TopicList.Remove(find);
                        }
                        else if (this.viewHistoryModel.TopicList.Count == maxCount)
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

            return changed ? this.SaveLocally() : false;
        }

        public bool DeleteAll()
        {
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

        private void LoadLocally()
        {
            string content = IsolatedStorageFileHelper.ReadIsoFile(FilePath);
            viewHistoryModel = JsonHelper.DeserializeOrDefault<ViewHistoryModel>(content);

            viewHistoryModel = viewHistoryModel ?? new ViewHistoryModel();
            viewHistoryModel.CarSeriesList = viewHistoryModel.CarSeriesList ?? new List<FavoriteCarSeriesModel>();
            viewHistoryModel.CarSpecList = viewHistoryModel.CarSpecList ?? new List<FavoriteCarSpecModel>();
            viewHistoryModel.ForumList = viewHistoryModel.ForumList ?? new List<FavoriteForumModel>();
            viewHistoryModel.ArticleList = viewHistoryModel.ArticleList ?? new List<FavoriteArticleModel>();
            viewHistoryModel.TopicList = viewHistoryModel.TopicList ?? new List<FavoriteTopicModel>();
        }

        private bool SaveLocally()
        {
            string content = JsonHelper.Serialize(viewHistoryModel);
            bool result = IsolatedStorageFileHelper.WriteIsoFile(FilePath, content, System.IO.FileMode.Create);
            return result;
        }

        /// <summary>
        /// Reload the first page. support all.
        /// </summary>
        /// <param name="type">support all.</param>
        public void Refresh(FavoriteType type)
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

        /// <summary>
        /// Not support ViewHistoryType.All
        /// </summary>
        /// <param name="type"></param>
        /// <param name="toPageIndex"></param>
        public void LoadMore(FavoriteType type)
        {
            switch (type)
            {
                case FavoriteType.CarSeries:
                    this.loadMore(this.CarSeriesList, this.viewHistoryModel.CarSeriesList, LoadMoreCarSeriesItem);
                    break;
                case FavoriteType.CarSpec:
                    this.loadMore(this.CarSpecList, this.viewHistoryModel.CarSpecList, LoadMoreCarSpecItem);
                    break;
                case FavoriteType.Article:
                    this.loadMore(this.ArticleList, this.viewHistoryModel.ArticleList, LoadMoreArticleItem);
                    break;
                case FavoriteType.Forum:
                    this.loadMore(this.ForumList, this.viewHistoryModel.ForumList, LoadMoreForumItem);
                    break;
                case FavoriteType.Topic:
                    this.loadMore(this.TopicList, this.viewHistoryModel.TopicList, LoadMoreTopicItem);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 多页

        public FavoriteForumModel LoadMoreForumItem = new FavoriteForumModel { IsLoadMore = true };

        public FavoriteCarSeriesModel LoadMoreCarSeriesItem = new FavoriteCarSeriesModel { IsLoadMore = true };

        public FavoriteCarSpecModel LoadMoreCarSpecItem = new FavoriteCarSpecModel { IsLoadMore = true };

        public FavoriteArticleModel LoadMoreArticleItem = new FavoriteArticleModel { IsLoadMore = true };

        public FavoriteTopicModel LoadMoreTopicItem = new FavoriteTopicModel { IsLoadMore = true };

        private void loadMore(IList toList, IList fromList, LoadMoreItem loadMoreItem)
        {
            this.TryRemoveMoreButton(toList, loadMoreItem);
            int endIndex = Math.Min(toList.Count + pageCount, fromList.Count);
            for (int i = toList.Count; i < endIndex; i++)
            {
                toList.Add(fromList[i]);
            }
            this.EnsureMoreButton(toList, fromList, loadMoreItem);
        }

        private void EnsureMoreButton(IList inList, IList sourceList, LoadMoreItem loadMoreItem)
        {
            if (!inList.Contains(loadMoreItem))
            {
                bool isEndPage = inList.Count >= sourceList.Count;
                if (!isEndPage)
                {
                    inList.Add(loadMoreItem);
                }
            }
        }

        private void TryRemoveMoreButton(IList fromList, LoadMoreItem loadMoreItem)
        {
            if (fromList != null && fromList.Contains(loadMoreItem))
            {
                fromList.Remove(loadMoreItem);
            }
        }

        #endregion
    }
}
