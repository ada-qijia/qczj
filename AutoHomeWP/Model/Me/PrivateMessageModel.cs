using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Model.Me
{
    [DataContract]
    public class PrivateMessageFriendModel : LoadMoreItem
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
        public string LastPostDate { get; set; }

        [DataMember(Name = "isbusinessauth")]
        public bool IsBusinessAuth { get; set; }
    }

    [DataContract]
    public class PrivateMessageModel : LoadMoreItem
    {
        [DataMember(Name = "userid")]
        public int UserID { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "messsageid")]
        public int ID { get; set; }

        [DataMember(Name = "lastpostdate")]
        public string LastPostDate { get; set; }
    }

    [DataContract]
    public class PrivateMessageNewModel : PrivateMessageModel, INotifyPropertyChanged
    {
        //属性更改完毕事件
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private int _sendingState;
        /// <summary>
        /// 0:发送成功, 1:发送失败, 2:发送中
        /// </summary>
        public int SendingState
        {
            get { return _sendingState; }
            set
            {
                if (value != _sendingState)
                {
                    _sendingState = value;
                    OnPropertyChanged("SendingState");
                }
            }
        }
    }

    [DataContract]
    public class PrivateMessageCacheModel
    {
        [DataMember]
        public List<PrivateMessageFriendModel> Friends { get; set; }

        [DataMember]
        public Dictionary<int, List<PrivateMessageModel>> Messages { get; set; }
    }
}
