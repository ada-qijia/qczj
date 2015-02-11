using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Me
{

    [DataContract]
    public class ViewHistoryModel
    {
        [DataMember]
        public List<FavoriteCarSeriesModel> CarSeriesList { get; set; }

        [DataMember]
        public List<FavoriteCarSpecModel> CarSpecList { get; set; }

        [DataMember]
        public List<FavoriteArticleModel> ArticleList { get; set; }

        [DataMember]
        public List<FavoriteForumModel> ForumList { get; set; }

        [DataMember]
        public List<FavoriteTopicModel> TopicList { get; set; }
    }
}
