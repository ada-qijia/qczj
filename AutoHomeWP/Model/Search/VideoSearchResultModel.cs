using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Model.Search
{
    [DataContract]
    public class VideoSearchModel:LoadMoreItem
    {
        [DataMember(Name = "id")]
        public int ID { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "time")]
        public string Time { get; set; }

        [DataMember(Name = "smallimg")]
        public string SmallImg { get; set; }

        [DataMember(Name = "playcount")]
        public int PlayCount { get; set; }
    }
}
