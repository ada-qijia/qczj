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
    /// 文章最终详情页
    /// </summary>
    [Table]
    public class NewsDetailModel : INotifyPropertyChanged, INotifyPropertyChanging
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

        private string _type;
        /// <summary>
        /// 文章类型
        /// </summary>
        [Column]
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (value != _type)
                {
                    OnPropertyChanging("Type");
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }
        }




        private string _title;
        /// <summary>
        /// 主标题
        /// </summary>
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

        private int _replyCount;
        /// <summary>
        /// 回复数
        /// </summary>
        [Column]
        public int ReplyCount
        {
            get
            {
                return _replyCount;
            }
            set
            {
                if (value != _replyCount)
                {
                    OnPropertyChanging("ReplyCount");
                    _replyCount = value;
                    OnPropertyChanged("ReplyCount");
                }
            }
        }

        private string _pageList;
        /// <summary>
        /// 标题列表
        /// </summary>
        [Column]
        public string PageList
        {
            get
            {
                return _pageList;
            }
            set
            {
                if (value != _pageList)
                {
                    OnPropertyChanging("PageList");
                    _pageList = value;
                    OnPropertyChanged("PageList");
                }
            }
        }


        private int _id;
        /// <summary>
        /// id
        /// </summary>
        [Column]
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    OnPropertyChanging("Id");
                    _id = value;
                    OnPropertyChanged("Id");
                }
            }
        }


        private string _pageTitle;
        /// <summary>
        /// 页标题
        /// </summary>
        [Column]
        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }
            set
            {
                if (value != _pageTitle)
                {
                    OnPropertyChanging("PageTitle");
                    _pageTitle = value;
                    OnPropertyChanged("PageTitle");
                }
            }
        }


        private int _pageCount;
        /// <summary>
        /// 分页数
        /// </summary>
        [Column]
        public int PageCount
        {
            get
            {
                return _pageCount;
            }
            set
            {
                if (value != _pageCount)
                {
                    OnPropertyChanging("PageCount");
                    _pageCount = value;
                    OnPropertyChanged("PageCount");
                }
            }
        }

        private string _webURL;
        [Column]
        public string WebURL
        {
            get
            {
                return _webURL;
            }
            set
            {
                if (value != _webURL)
                {
                    OnPropertyChanging("WebURL");
                    _webURL = value;
                    OnPropertyChanged("WebURL");
                }
            }
        }
    }
}
