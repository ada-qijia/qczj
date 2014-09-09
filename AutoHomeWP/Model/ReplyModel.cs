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

namespace Model
{
    public class ReplyModel
    {
        //手机型号
        private string _phoneType;
        public string phoneType
        {
            get
            {
                return _phoneType;
            }
            set
            {
                if (value != _phoneType)
                {
                    _phoneType = value;
                }
            }
        }

        //论坛版块id
        private int _bbsId;
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
                    _bbsId =value;
                }
            }

        }

        //论坛类型
        private string _bbsType;
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
                    _bbsType = value;
                }
            }
        }

        //帖子Id
        private int _topicId;
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
                    if (value != _topicId)
                    {
                        _topicId = value;
                    }
                }
            }
        }

        //回复Id
        private int _replyId;
        public int replyId
        {
            get
            {
                return _replyId;
            }
            set
            {
                if (value != _replyId)
                {
                    _replyId = value;
                }
            }
        }

        //回复目标楼层id
        private int _targetReplyId;
        public int targetReplyId
        {
            get
            {
                return _targetReplyId;
            }
            set
            {
                if (value != _targetReplyId)
                {
                    _targetReplyId = value;
                }
            }
        }

        //回复内容
        private string _content;
        public string content
        {
            get
            {
                return _content;
            }
            set
            {
                if (value != _content)
                {
                    _content = value;
                }
            }
        }

        //图片个数
        private int _picCount;
        public int picCount
        {
            get
            {
                return _picCount;
            }
            set
            {
                if (value != _picCount)
                {
                    _picCount = value;
                }
            }
        }

        //图片二进制流
        private byte[] _image1;
        public byte[] image1
        {
            get
            {
                return _image1;
            }
            set
            {
                if (value != _image1)
                {
                    _image1 = value;
                }
            }
        }
    }
}
