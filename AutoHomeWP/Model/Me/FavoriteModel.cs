using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Model.Me
{
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
    public class FavoriteCarSeriesModel
    {
        [DataMember(Name="id")]
        public int ID { get; set; }

        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="imgurl")]
        public string Img { get; set; }

        [DataMember(Name="level")]
        public string Level { get; set; }

        [DataMember(Name="pricebetween")]
        public string PriceBetween { get; set; }
    }

    [DataContract]
    public class FavoriteCarSpecModel
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "imgurl")]
        public string Img { get; set; }

        [DataMember]
        public string SeriesName { get; set; }

        [DataMember(Name = "lowprice")]
        public string LowPrice { get; set; }
    }

    [DataContract]
    public class FavoriteArticleModel
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "newstype")]
        public string Type { get; set; }

        [DataMember(Name = "imgurl")]
        public string Img { get; set; }

        [DataMember(Name = "date")]
        public string Date { get; set; }

        [DataMember(Name = "replycount")]
        public int ReplyCount { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }

    [DataContract]
    public class FavoriteForumModel
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class FavoriteTopicModel
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name="url")]
        public string url { get; set; }
    }
}
