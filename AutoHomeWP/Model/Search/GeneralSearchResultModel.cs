using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Model.Search
{
    public enum GeneralSearchResultMatchType
    {
        Brand = 1,
        Firm = 2,
        CarSeries = 3,
        CarType = 4,
        FindCars = 5,
        Forum = 6,
        Others
    }

    /// <summary>
    /// 车系
    /// </summary>
    [DataContract]
    public class HotSeriesModel
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        /// <summary>
        /// 是否显示车系结果模块，true为是，false否
        /// </summary>
        [DataMember(Name = "isshowmodel")]
        public bool IsShowModel { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "pricebetween")]
        public string PriceBetween { get; set; }

        [DataMember(Name = "img")]
        public string Img { get; set; }

        [DataMember(Name = "imgcount")]
        public int ImgCount { get; set; }

        [DataMember(Name = "koubeiscore")]
        public float KoubeiScore { get; set; }

        [DataMember(Name = "koubeicount")]
        public int KoubeiCount { get; set; }

        //该车系下是否有口碑，1有，0无
        [DataMember(Name = "isshowkoubei")]
        public int IsShowKoubei { get; set; }

        [DataMember(Name = "bbsid")]
        public int BBSID { get; set; }

        //热门车型ID
        [DataMember(Name = "hotspecid")]
        public int HotSpecID { get; set; }
    }

    /// <summary>
    /// 车型
    /// </summary>
    [DataContract]
    public class SpecModel
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "lowprice")]
        public string LowPrice { get; set; }
    }

    /// <summary>
    ///品牌/厂商
    /// </summary>
    [DataContract]
    public class SeriesModel
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "pricebetween")]
        public string PriceBetween { get; set; }
    }

    /// <summary>
    /// 车系/车型图片
    /// </summary>
    [DataContract]
    public class ImgModel
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "img")]
        public string Img { get; set; }
    }

    /// <summary>
    /// 相关推荐
    /// </summary>
    [DataContract]
    public class RelatedSeriesModel
    {
        [DataMember(Name="id")]
        public int ID { get; set; }

        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="img")]
        public string Img { get; set; }

        [DataMember(Name="lowprice")]
        public string LowPrice { get; set; }
    }

    /// <summary>
    /// 经销商
    /// </summary>
    [DataContract]
    public class DealerSearchModel
    {
        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="address")]
        public string Address { get; set; }

        [DataMember(Name="tel")]
        public string Tel { get; set; }
    }

    /*
    /// <summary>
    /// 找车结果
    /// </summary>
    [DataContract]
    public class FindCarModel
    {
        [DataMember(Name="conditions")]
        public List<FindCarCondition> ConditionList { get; set; }

        [DataMember(Name="findserieslist")]
        public List<FindSeriesModel> FindSeriesList { get; set; }
    }
    */

    [DataContract]
    public class FindCarCondition
    {
        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="value")]
        public string Value { get; set; }

        //FindCarConditionType
        [DataMember(Name="type")]
        public int Type { get; set; }
    }

    public enum FindCarConditionType
    {
        Brand = 1,
        Country = 2,
        Displacement = 3,
        Fuel = 4,
        Level = 5,
        PriceRange = 6,
        Structure = 7,
        //变速箱
        Gearbox = 8,
    }

    /// <summary>
    /// 找车列表
    /// </summary>
    [DataContract]
    public class FindSeriesModel
    {
        [DataMember(Name="id")]
        public int ID { get; set; }

        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name = "pricebetween")]
        public string PriceBetween { get; set; }

        [DataMember(Name="level")]
        public string Level { get; set; }

        [DataMember(Name = "displacement")]
        public string Displacement { get; set; }
    }

    /// <summary>
    /// 论坛精选日报类别
    /// </summary>
    [DataContract]
    public class JingxuanModel:LoadMoreItem
    {
        [DataMember(Name="topicid")]
        public long TopicID { get; set; }
        
        [DataMember(Name="title")]
        public string Title { get; set; }

        //[DataMember(Name = "lastreplydate")]
        //public string LastReplyDate { get; set; }

        [DataMember(Name="smallpic")]
        public string SmallImg { get; set; }

        [DataMember(Name="replycounts")]
        public int ReplyCount { get; set; }

        [DataMember(Name="bbsid")]
        public int BBSID { get; set; }

        [DataMember(Name="bbsname")]
        public string BBSName { get; set; }
    }

    /// <summary>
    /// 其他自然搜索结果
    /// </summary>
    [DataContract]
    public class NaturalModel:LoadMoreItem
    {
        [DataMember(Name="id")]
        public int ID { get; set; }

        [DataMember(Name="title")]
        public string Title { get; set; }

        [DataMember(Name="mediatype")]
        //NaturalModelMediaType
        public int MediaType { get; set; }

        /// <summary>
        /// 当MediaType为说客时，代表作者
        /// </summary>
        [DataMember(Name="type")]
        public string Type { get; set; }

        [DataMember(Name="time")]
        public string Time { get; set; }

        [DataMember(Name = "indexdetail")]
        public string IndexDetail { get; set; }

        [DataMember(Name="smallpic")]
        public string SmallImg { get; set; }

        [DataMember(Name="replycount")]
        public int ReplyCount { get; set; }

        [DataMember(Name="pagecount")]
        public int PageCount { get; set; }

        /// <summary>
        /// 当MediaType为帖子时，代表BBSID
        /// </summary>
        [DataMember(Name="jumppage")]
        public int JumpPage { get; set; }
    }

    public enum NaturalModelMediaType
    {
        Article = 1,
        Lobbyist = 2,
        Video = 3,
        Post = 5,
        Others,
    }

}
