using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Model.Me
{
    [DataContract]
    public class DraftModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [DataMember]
        public string BBSID { get; set; }

        [DataMember]
        public string BBSType { get; set; }

        /// <summary>
        /// if null, this draft is a post, otherwise a reply.
        /// </summary>
        [DataMember]
        public string TopicID { get; set; }

        [DataMember]
        public string TargetReplyID { get; set; }

        private string _title;
        [DataMember]
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        private string _content;
        [DataMember]
        public string Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged("Content");
                }
            }
        }

        /// <summary>
        /// use this as the model ID.
        /// </summary>
        [DataMember]
        public DateTime SavedTime { get; set; }

        [DataMember]
        public bool read { get; set; }
    }
}
