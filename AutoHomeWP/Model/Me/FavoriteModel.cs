using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Model.Me
{
    /// <summary>
    /// 数值与获取收藏列表中参数对应
    /// </summary>
    public enum FavoriteType
    {
        All=0,
        CarSeries=3,
        CarSpec=4,
        Article=5,
        Forum=2,
        Topic=1,
    }

    [DataContract]
    public class MyFavoriteModel
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

    [DataContract]
    public class FavoriteModelBase : LoadMoreItem
    {
        /// <summary>
        /// 1 添加收藏，2 取消收藏
        /// </summary>
        public int Action { get; set; }
    }

    //this should be consistent with cloud data schema
    [DataContract]
    public class FavoriteCarSeriesModel : FavoriteModelBase
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "pic")]
        public string Img { get; set; }

        [DataMember(Name = "level")]
        public string Level { get; set; }

        [DataMember(Name = "price")]
        public string PriceBetween { get; set; }

        [DataMember(Name = "time")]
        public string Time { get; set; }
    }

    [DataContract]
    public class FavoriteCarSpecModel : FavoriteModelBase
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "price")]
        public string LowPrice { get; set; }

        [DataMember(Name = "seriesname")]
        public string SeriesName { get; set; }

        [DataMember(Name = "pic")]
        public string Img { get; set; }

        [DataMember(Name = "time")]
        public string Time { get; set; }
    }

    [DataContract]
    public class FavoriteArticleModel : FavoriteModelBase
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "pic")]
        public string Img { get; set; }

        [DataMember(Name = "isvideo")]
        public int IsVideo { get; set; }

        [DataMember(Name = "publishtime")]
        public string PublishTime { get; set; }

        //[DataMember(Name = "updatetime")]
        //public DateTime UpdateTime { get; set; }

        [DataMember(Name = "time")]
        public string Time { get; set; }

        [DataMember(Name = "replycount")]
        public int ReplyCount { get; set; }

        [DataMember]
        public bool IsShuoke { get; set; }
    }

    [DataContract]
    public class FavoriteForumModel : FavoriteModelBase
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "time")]
        public string Time { get; set; }
    }

    [DataContract]
    public class FavoriteTopicModel : FavoriteModelBase
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "name")]
        public string Title { get; set; }

        [DataMember(Name = "bbsid")]
        public string BBSID { get; set; }

        [DataMember(Name = "bbstype")]
        public string BBSType { get; set; }

        [DataMember(Name = "time")]
        public string Time { get; set; }
    }
}
