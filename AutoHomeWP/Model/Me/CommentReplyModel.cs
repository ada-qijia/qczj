using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Model.Me
{
    [DataContract]
    public class CommentReplyModel:LoadMoreItem
    {
        [DataMember(Name="id")]
        public int ID { get; set; }

        [DataMember(Name="title")]
        public string Title { get; set; }

        [DataMember(Name = "replyuserid")]
        public int ReplyUserID { get; set; }

        [DataMember(Name = "replyusername")]
        public string ReplyUserName { get; set; }

        [DataMember(Name = "replyuserimg")]
        public string ReplyUserImg { get; set; }

        [DataMember(Name = "replycontent")]
        public string ReplyContent { get; set; }

        [DataMember(Name = "replytime")]
        public string ReplyTime { get; set; }

        [DataMember(Name="replyid")]
        public int ReplyID { get;set;}

        [DataMember(Name="replytype")]
        public int ReplyType { get; set; }

        [DataMember(Name="mycomment")]
        public string MyComment { get; set; }

        [DataMember(Name = "imgid")]
        public int ImgID { get; set; }

        [DataMember(Name = "isbusinessauth")]
        public bool IsBusinessAuth { get; set; }

        [DataMember(Name = "fromtitle")]
        public string From { get; set; }

        [DataMember(Name = "notallowcom")]
        public bool NotAllowCom { get; set; }
    }
}
