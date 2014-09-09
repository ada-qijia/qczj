using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Model
{
    public class CommentReplyModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        //属性更改时的事件
        public event PropertyChangingEventHandler PropertyChanging;
        public void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        //属性更改完毕事件
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _newsid;
        public string NewsId
        {
            get
            {
                return _newsid;
            }
            set
            {
                if (value != _newsid)
                {
                    OnPropertyChanging("NewsId");
                    _newsid = value;
                    OnPropertyChanged("NewsId");
                }
            }
        }

        private string _replyid;
        public string ReplyId
        {
            get
            {
                return _replyid;
            }
            set
            {
                if (value != _replyid)
                {
                    OnPropertyChanging("ReplyId");
                    _replyid = value;
                    OnPropertyChanged("ReplyId");
                }
            }
        }

        private string _userid;
        public string UserId
        {
            get
            {
                return _userid;
            }
            set
            {
                if (value != _userid)
                {
                    OnPropertyChanging("UserId");
                    _userid = value;
                    OnPropertyChanged("UserId");
                }
            }
        }

        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (value != _content)
                {
                    OnPropertyChanging("Content");
                    _content = value;
                    OnPropertyChanged("Content");
                }
            }
        }


        private string _authorization;
        public string Authorization
        {
            get
            {
                return _authorization;
            }
            set
            {
                if (value != _authorization)
                {
                    OnPropertyChanging("Authorization");
                    _authorization = value;
                    OnPropertyChanged("Authorization");
                }
            }
        }
    }
}
