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
using System.Data.Linq.Mapping;

namespace Model
{
    /// <summary>
    /// 某车系论坛model
    /// </summary>
    [Table]
    public class ForumModel : INotifyPropertyChanged, INotifyPropertyChanging
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
        //属性更改事件
        public event PropertyChangingEventHandler PropertyChanging;
        public void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }


        //标题
        private string _title;
        [Column]
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    OnPropertyChanging("Title");
                    _title = value;
                    OnPropertyChanged("Title");
                }
            }
        }

        //发帖用户名
        private string _postUserName;
        [Column]
        public string PostUserName
        {
            get
            {
                return _postUserName;
            }
            set
            {
                if (value != _postUserName)
                {
                    OnPropertyChanging("PostUserName");
                    _postUserName = value;
                    OnPropertyChanged("PostUserName");
                }
            }
        }

        //回复数
        private int _replyCounts;
        [Column]
        public int ReplyCounts
        {
            get
            {
                return _replyCounts;
            }
            set
            {
                if (value != _replyCounts)
                {
                    OnPropertyChanging("ReplyCounts");
                    _replyCounts = value;
                    OnPropertyChanged("ReplyCounts");
                }
            }
        }

        //最新回复日期
        private string _lastReplyDate;
        [Column]
        public string LastReplyDate
        {
            get
            {
                return _lastReplyDate;
            }
            set
            {
                if (value != _lastReplyDate)
                {
                    OnPropertyChanging("LastReplyDate");
                    _lastReplyDate = value;
                    OnPropertyChanged("LastReplyDate");
                }
            }
        }

        //贴子属性
        private string _topicType;
        [Column]
        public string TopicType
        {
            get
            {
                return _topicType;
            }
            set
            {
                if (value != _topicType)
                {
                    OnPropertyChanging("TopicType");
                    _topicType = value;
                    OnPropertyChanged("TopicType");
                }
            }
        }

        //贴子ID 
        private int _topicId;
        [Column]
        public int TopicId
        {
            get
            {
                return _topicId;
            }
            set
            {
                if (value != _topicId)
                {
                    OnPropertyChanging("TopicId");
                    _topicId = value;
                    OnPropertyChanged("TopicId");
                }
            }
        }

        //bbs类型
        private string _bbsType;
        [Column]
        public string bbsType
        {
            get
            {
                return _bbsType;
            }
            set
            {
                if (value != _bbsType)
                {
                    OnPropertyChanging("bbsType");
                    _bbsType = value;
                    OnPropertyChanged("bbsType");
                }
            }
        }

        //bbsID
        private int _bbsId;
        [Column]
        public int bbsId
        {
            get
            {
                return _bbsId;
            }
            set
            {
                if (value != _bbsId)
                {
                    OnPropertyChanging("bbsId");
                    _bbsId = value;
                    OnPropertyChanged("bbsId");
                }
            }
        }

        //评论数
        private int _toatalCount;
        [Column]
        public int TotalCount
        {
            get
            {
                return _toatalCount;
            }
            set
            {
                if (value != _toatalCount)
                {
                    OnPropertyChanging("TotalCount");
                    _toatalCount = value;
                    OnPropertyChanged("TotalCount");
                }
            }
        }

        //是否查看过（0 未看过 1看过）
        private int _isView;
        [Column]
        public int IsView
        {
            get
            {
                return _isView;
            }
            set
            {
                if (value != _isView)
                {
                    OnPropertyChanging("IsView");
                    _isView = value;
                    OnPropertyChanged("IsView");
                }
            }
        }
        //贴子是否被关闭
        private int _isClosed;
        [Column]
        public int IsClosed
        {
            get
            {
                return _isClosed;
            }
            set
            {
                if (value != _isClosed)
                {
                    OnPropertyChanging("IsClosed");
                    _isClosed = value;
                    OnPropertyChanged("IsClosed");
                }
            }
        }

        private string _showData;
        public string ShowData
        {
            get
            {
                return _showData;
            }
            set
            {
                if (value != _showData)
                {
                    OnPropertyChanging("ShowData");
                    _showData = value;
                    OnPropertyChanged("ShowData");
                }
            }
        }

        private string _loadMore;
        public string LoadMore
        {
            get
            {
                return _loadMore;
            }
            set
            {
                if (value != _loadMore)
                {
                    OnPropertyChanging("LoadMore");
                    _loadMore = value;
                    OnPropertyChanged("LoadMore");
                }
            }
        }

    }
}
