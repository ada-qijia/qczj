using System;
using System.Runtime.Serialization;

namespace Model.Me
{
    [DataContract]
    public class DraftModel
    {
        [DataMember]
        public string BBSID { get; set; }

        /// <summary>
        /// if null, this draft is a post, otherwise a reply.
        /// </summary>
        [DataMember]
        public string TopicID { get; set; }

        [DataMember]
        public string TargetReplyID { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Content { get; set; }

        /// <summary>
        /// use this as the model ID.
        /// </summary>
        [DataMember]
        public DateTime SavedTime { get; set; }

        [DataMember]
        public bool read { get; set; }
    }
}
