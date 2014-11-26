using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Model.Search
{
    [DataContract]
    public class ArticleFilterModel
    {
        [DataMember(Name="type")]
        public string Type { get; set; }

        [DataMember(Name="num")]
        public int Num { get; set; }

        [DataMember(Name="name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class ArticleModel:LoadMoreItem
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "newstype")]
        public string Type { get; set; }

        [DataMember(Name = "date")]
        public string Date { get; set; }

        [DataMember(Name = "replycount")]
        public int ReplyCount { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }

    public enum ArticleType
    {
        News,
        /// <summary>
        /// 评测
        /// </summary>
        Review,
        UsingCar,
        /// <summary>
        /// 行情
        /// </summary>
        Quotation,
        /// <summary>
        /// 导购
        /// </summary>
        Guide,
        /// <summary>
        /// 改装
        /// </summary>
        Refit,
        Culture,
        Technology,
    }
}
