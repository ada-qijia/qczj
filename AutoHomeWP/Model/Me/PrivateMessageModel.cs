using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Model.Me
{
    [DataContract]
    public class PrivateMessageFriendModel:LoadMoreItem
    {
        [DataMember(Name = "userid")]
        public int ID { get; set; }

        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "img")]
        public string Img { get; set; }

        [DataMember(Name = "unread")]
        public int UnRead { get; set; }

        [DataMember(Name = "lastmsg")]
        public string LastMessage { get; set; }

        [DataMember(Name = "lastpostdate")]
        public DateTime LastPostDate { get; set; }

        [DataMember(Name = "isbusinessauth")]
        public bool IsBusinessAuth { get; set; }
    }

    [DataContract]
    public class PrivateMessageModel:LoadMoreItem
    {
        [DataMember(Name = "userid")]
        public int UserID { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "messageid")]
        public int ID { get; set; }

        [DataMember(Name = "lastpostdate")]
        public DateTime LastPostDate { get; set; }
    }

    [DataContract]
    public class PrivateMessageCacheModel
    {
        [DataMember]
        public List<PrivateMessageFriendModel> Friends { get; set; }

        [DataMember]
        public Dictionary<int, PrivateMessageModel> Messages { get; set; }
    }
}
