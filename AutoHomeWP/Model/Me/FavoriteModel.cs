using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Model.Me
{
    
    public enum FavoriteType
    {
        All,
        CarSeries,
        CarSpec,
        Article,
        Forum,
        Topic,
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

    //this should be consistent with cloud data schema
    [DataContract]
    public class FavoriteCarSeriesModel:LoadMoreItem
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
    public class FavoriteCarSpecModel:LoadMoreItem
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "price")]
        public string LowPrice { get; set; }

        [DataMember(Name="seriesname")]
        public string SeriesName { get; set; }

        [DataMember(Name = "pic")]
        public string Img { get; set; }

        [DataMember(Name = "time")]
        public string Time { get; set; }
    }

    [DataContract]
    public class FavoriteArticleModel:LoadMoreItem
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

        [DataMember(Name = "updatetime")]
        public DateTime UpdateTime { get; set; }

        [DataMember(Name = "time")]
        public string Time { get; set; }

        [DataMember(Name = "replycount")]
        public int ReplyCount { get; set; }
    }

    [DataContract]
    public class FavoriteForumModel:LoadMoreItem
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
    public class FavoriteTopicModel:LoadMoreItem
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
