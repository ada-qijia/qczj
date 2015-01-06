using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Me
{
    public enum ViewHistoryType
    {
        All,
        CarSeries,
        CarSpec,
        Article,
        Forum,
        Topic,
    }

    [DataContract]
    public class ViewHistoryModel
    {
        [DataMember]
        public List<ViewHistoryCarSeriesModel> CarSeriesList { get; set; }

        [DataMember]
        public List<ViewHistoryCarSpecModel> CarSpecList { get; set; }

        [DataMember]
        public List<ViewHistoryArticleModel> ArticleList { get; set; }

        [DataMember]
        public List<ViewHistoryForumModel> ForumList { get; set; }

        [DataMember]
        public List<ViewHistoryTopicModel> TopicList { get; set; }
    }

    [DataContract]
    public class ViewHistoryCarSeriesModel:FavoriteCarSeriesModel
    { }

    [DataContract]
    public class ViewHistoryCarSpecModel:FavoriteCarSpecModel
    { }

    [DataContract]
    public class ViewHistoryArticleModel : FavoriteArticleModel
    { }

    [DataContract]
    public class ViewHistoryForumModel : FavoriteForumModel
    { }

    [DataContract]
    public class ViewHistoryTopicModel:FavoriteTopicModel
    { }
}
