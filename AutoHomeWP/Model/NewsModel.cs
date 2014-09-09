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
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Windows.Media.Imaging;


namespace Model
{
    /// <summary>
    /// 最新文章Model
    /// </summary>
    [Table]
    public class NewsModel : INotifyPropertyChanged, INotifyPropertyChanging
    {


        private int _localID;
        //自动编号，不可为空，自动增长
        [Column(IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false)]
        public int LocalID
        {
            get
            {
                return _localID;
            }
            set
            {
                if (value != _localID)
                {
                    OnPropertyChanging("LocalID");
                    _localID = value;
                    OnPropertyChanged("LocalID");
                }
            }
        }


        //是否显示数据
        private string _showData;
        [Column]
        public string showData
        {
            get
            {
                return _showData;
            }
            set
            {
                if (value != null)
                {
                    _showData = value;
                    OnPropertyChanged("showData");
                }
            }
        }

        //文章总数
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
        /// 加载更多
        /// </summary>
        private string _loadMore;
        [Column]
        public string loadMore
        {
            get
            {
                return _loadMore;
            }
            set
            {
                if (value != _loadMore)
                {
                    OnPropertyChanging("loadMore");
                    _loadMore = value;
                    OnPropertyChanged("loadMore");
                }
            }
        }

        private string _imgurl;
        [Column]
        public string imgurl
        {
            get
            {
                return _imgurl;
            }
            set
            {
                if (value != _imgurl)
                {
                    _imgurl = value;
                    OnPropertyChanged("imgurl");
                }
            }
        }

        //新闻编号
        private int _id;
        [Column]
        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != id)
                {
                    OnPropertyChanging("id");
                    _id = value;
                    OnPropertyChanged("id");

                }
            }
        }

        //小图
        private BitmapSource _bitmap;

        public BitmapSource bitmap
        {
            get
            {
                return _bitmap;
            }
            set
            {
                if (value != _bitmap)
                {
                    OnPropertyChanging("bitmap");
                    _bitmap = value;
                    OnPropertyChanged("bitmap");
                }
            }
        }

        //头图
        private BitmapSource _topImage;

        public BitmapSource topImage
        {
            get
            {
                return _topImage;
            }
            set
            {
                if (value != _topImage)
                {
                    OnPropertyChanging("topImage");
                    _topImage = value;
                    OnPropertyChanged("topImage");
                }
            }
        }

        //标题
        private string _title;
        [Column]
        public string title
        {
            get
            {
                return _title;
            }

            set
            {
                if (value != _title)
                {
                    OnPropertyChanging("title");
                    _title = value;
                    OnPropertyChanged("title");
                }
            }
        }

        //类型
        private string _type;
        [Column]
        public string type
        {
            get
            {
                return _type;
            }
            set
            {
                if (value != _type)
                {
                    OnPropertyChanging("type");
                    _type = value;
                    OnPropertyChanged("type");
                }
            }
        }

        //页码
        private string _pageIndex;
        [Column(CanBeNull=true)]
        public string pageIndex
        {
            get
            {        
                return _pageIndex;
            }
            set
            {
                if (value != _pageIndex)
                {
                    OnPropertyChanging("pageIndex");
                    _pageIndex = value;
                    OnPropertyChanged("pageIndex");
                }
            }
        }
       

        //时间
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


        //详情
        private string _indexdetail;
        [Column]
        public string indexdetail
        {
            get
            {
                return _indexdetail;
            }
            set
            {
                if (value != _indexdetail)
                {
                    OnPropertyChanging("indexdetail");
                    _indexdetail = value;
                    OnPropertyChanged("indextail");
                }
            }
        }



        //图片url
        private string _smallpic;
        [Column]
        public string smallpic
        {
            get
            {
                return _smallpic;
            }
            set
            {
                if (value != _indexdetail)
                {
                    OnPropertyChanging("smallpic");
                    _smallpic = value;
                    OnPropertyChanged("smallpic");
                }
            }
        }


        ///回复数
        private string _replycount;
        [Column]
        public string replycount
        {
            get
            {
                return _replycount;
            }
            set
            {
                if (value != _replycount)
                {
                    OnPropertyChanging("replycount");
                    _replycount = value;
                    OnPropertyChanged("replycount");
                }
            }
        }



        //页数
        private string _pagecount;
        [Column]
        public string pagecount
        {
            get
            {
                return _pagecount;
            }
            set
            {
                if (value != _pagecount)
                {
                    OnPropertyChanging("pagecount");
                    _pagecount = value;
                    OnPropertyChanged("pagecount");
                }
            }
        }

        //lasttime
        private string _lasttimeandid;
        [Column(CanBeNull = true)]
        public string lasttimeandid
        {
            get
            {
                return _lasttimeandid;
            }
            set
            {
                if (value != _lasttimeandid)
                {
                    OnPropertyChanging("lasttimeandid");
                    _lasttimeandid = value;
                    OnPropertyChanged("lasttimeandid");
                }
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
        //属性更改事件
        public event PropertyChangingEventHandler PropertyChanging;
        public void OnPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }
    }
}
