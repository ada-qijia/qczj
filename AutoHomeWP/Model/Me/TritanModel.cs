using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Me
{
    [DataContract]
    public class TritanModel: LoadMoreItem
    {
        [DataMember(Name = "topicid")]
        public int ID { get; set; }

        [DataMember(Name = "bbsid")]
        public int BBSID { get; set; }

        [DataMember(Name = "bbsname")]
        public string BBSName { get; set; }

        [DataMember(Name = "bbstype")]
        public string BBSType { get; set; }

        [DataMember(Name = "replynum")]
        public int ReplyCount { get; set; }

        [DataMember(Name = "posttime")]
        public string PostDate { get; set; }

        [DataMember(Name = "topictype")]
        public string TopicType { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }
    }
}
