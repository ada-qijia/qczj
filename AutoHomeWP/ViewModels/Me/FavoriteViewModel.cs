using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Me;

namespace ViewModels.Me
{
    public class FavoriteViewModel
    {
        public FavoriteViewModel()
        {
            this.CarSeriesList = new ObservableCollection<FavoriteCarSeriesModel>();
            this.CarSpecList = new ObservableCollection<FavoriteCarSpecModel>();
            this.ArticleList = new ObservableCollection<FavoriteArticleModel>();
            this.ForumList = new ObservableCollection<FavoriteForumModel>();
            this.TopicList = new ObservableCollection<FavoriteTopicModel>();
        }

        #region properties

        public ObservableCollection<FavoriteCarSeriesModel> CarSeriesList { get; private set; }

        public ObservableCollection<FavoriteCarSpecModel> CarSpecList { get; private set; }

        public ObservableCollection<FavoriteArticleModel> ArticleList { get; private set; }

        public ObservableCollection<FavoriteForumModel> ForumList { get; private set; }

        public ObservableCollection<FavoriteTopicModel> TopicList { get; private set; }

        #endregion

        #region methods

        public void LoadLocally(string path)
        { }

        public void SaveLocally(string path)
        { }

        #endregion
    }
}
