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
    /// 最新评论列表 
    /// </summary>
    [Table]
    public class CommentModel : INotifyPropertyChanged, INotifyPropertyChanging
    {

        //评论id
        private string _id;
        [Column]
        public string id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    OnPropertyChanging("id");
                    _id = value;
                    OnPropertyChanged("id");
                }
            }
        }

        //楼数
        private string _floor;
        [Column]
        public string floor
        {
            get
            {
                return _floor;
            }
            set
            {
                if (value != _floor)
                {
                    OnPropertyChanging("floor");
                    _floor = value;
                    OnPropertyChanged("floor");
                }
            }
        }

        /// <summary>
        ///评论人姓名
        /// </summary>
        private string _name;
        [Column]
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    OnPropertyChanging("name");
                    _name = value;
                    OnPropertyChanged("name");
                }
            }
        }

        /// <summary>
        /// 评论时间
        /// </summary>
        private string _time;
        [Column]
        public string time
        {
            get
            {
                return _time;
            }
            set
            {
                if (value != _time)
                {
                    OnPropertyChanging("time");
                    _time = value;
                    OnPropertyChanged("time");
                }
            }
        }

        /// <summary>
        /// 评论内容
        /// </summary>
        private string _content;
        [Column]
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
                    OnPropertyChanging("content");
                    _content = value;
                    OnPropertyChanged("content");
                }
            }
        }
        
        /// <summary>
        /// 总评论数
        /// </summary>
        private int _total;
        [Column]
        public int Total
        {
            get
            {
                return _total;
            }
            set
            {
                if (value != _total)
                {
                    OnPropertyChanging("Total");
                    _total = value;
                    OnPropertyChanged("Total");
                }
            }
        }

        /// <summary>
        /// 是否追加回帖
        /// </summary>
        private string _isadd;
        [Column]
        public string isadd
        {
            get
            {
                return _isadd;
            }
            set
            {
                if (value != _isadd)
                {
                    OnPropertyChanging("isadd");
                    _isadd = value;
                    OnPropertyChanged("isadd");
                }
            }
        }

        //原帖id
        private string _sourceId;
        [Column]
        public string SourceId
        {
            get
            {
                return _sourceId;
            }
            set
            {
                if (value != _sourceId)
                {
                    OnPropertyChanged("SourceId");
                    _sourceId = value;
                    OnPropertyChanging("SourceId");
                }
            }
        }

        /// <summary>
        /// 追加原帖的姓名
        /// </summary>
        private string _sourcename;
        [Column]
        public string sourcename
        {
            get
            {
                return _sourcename;
            }
            set
            {
                if (value != _sourcename)
                {
                    OnPropertyChanging("sourcename");
                    _sourcename = value;
                    OnPropertyChanged("sourcename");
                }
            }
        }


        /// <summary>
        /// 追加原帖的内容
        /// </summary>
        private string _sourcecontent;
        [Column]
        public string sourcecontent
        {
            get
            {
                return _sourcecontent;
            }
            set
            {
                if (value != _sourcecontent)
                {
                    OnPropertyChanging("sourcecontent");
                    _sourcecontent = value;
                    OnPropertyChanged("sourcecontent");
                }
            }
        }

        //加载更多
        private string _loadMore;
        [Column]
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

        //是否显示数据
        private string _showData;
        [Column]
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

    }
}
