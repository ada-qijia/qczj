using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Model.Search
{
    [DataContract]
    public class RelatedBBSModel
    {
        [DataMember(Name = "bbsid")]
        public int ID { get; set; }

        [DataMember(Name = "bbsname")]
        public string Name { get; set; }
    }

    [DataContract]
    public class BBSModel
    {
        [DataMember(Name = "bbsid")]
        public int ID { get; set; }

        [DataMember(Name = "bbsname")]
        public string Name { get; set; }

        [DataMember(Name = "imgurl")]
        public string Img { get; set; }

        //[DataMember(Name="bbstype")]
        //public string Type { get; set; }

        [DataMember(Name = "topiccount")]
        public int TopicCount { get; set; }
    }

    [DataContract]
    public class TopicModel
    {
        [DataMember(Name = "topicid")]
        public int ID { get; set; }

        [DataMember(Name = "bbsid")]
        public int BBSID { get; set; }

        [DataMember(Name = "bbsname")]
        public string BBSName { get; set; }

        [DataMember(Name = "replycount")]
        public int ReplyCount { get; set; }

        [DataMember(Name = "postdate")]
        public string PostDate { get; set; }

        [DataMember(Name = "topictype")]
        public string TopicType { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "ispic")]
        public bool IsPic { get; set; }

        public bool IsJinghua { get { return this.TopicType == "精"; } }
    }
}
