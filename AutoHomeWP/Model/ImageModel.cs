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
    /// 图片Model
    /// </summary>
    [Table]
    public class ImageModel : INotifyPropertyChanged, INotifyPropertyChanging
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

        /// <summary>
        /// id
        /// </summary>
        private int _ID;
        [Column(IsPrimaryKey=true,CanBeNull=false)]
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                if (value != _ID)
                {
                    OnPropertyChanging("ID");
                    _ID = value;
                    OnPropertyChanged("ID");
                }
            }
        }

        /// <summary>
        /// 大图前缀
        /// </summary>
        private string _bigPicPrefix;
        [Column]
        public string BigPicPrefix
        {

            get
            {
                return _bigPicPrefix;
            }
            set
            {
                if (value != _bigPicPrefix)
                {
                    OnPropertyChanging("BigPicPrefix");
                    _bigPicPrefix = value;
                    OnPropertyChanged("BigPicPrefix");
                }
            }

        }

        /// <summary>
        /// 小图前缀
        /// </summary>
        private string _smallPifcPrefix;
        [Column]
        public string SmallPicPrefix
        {
            get
            {
                return _smallPifcPrefix;
            }
            set
            {
                if (value != _smallPifcPrefix)
                {
                    OnPropertyChanging("SmallPicPrefix");
                    _smallPifcPrefix = value;
                    OnPropertyChanged("SmallPicPrefix");
                }
            }
        }

        /// <summary>
        /// 图片总数
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
        /// 总页数
        /// </summary>
        private int _pages;
        [Column]
        public int Pages
        {
            get
            {
                return _pages;
            }
            set
            {
                if (value != _pages)
                {
                    OnPropertyChanging("pages");
                    _pages = value;
                    OnPropertyChanged("Pages");
                }
            }
        }

        /// <summary>
        /// 小图地址
        /// </summary>
        private string _smallPic;
        [Column]
        public string SmallPic
        {
            get
            {
                return _smallPic;
            }
            set
            {
                if (value != _smallPic)
                {
                    OnPropertyChanging("SmallPic");
                    _smallPic = value;
                    OnPropertyChanged("SmallPic");
                }
            }
        }


        private string _smallPicTwo;
        [Column]
        public string SmallPicTwo
        {
            get
            {
                return _smallPicTwo;
            }
            set
            {
                if (value != _smallPicTwo)
                {
                    OnPropertyChanging("SmallPicTwo");
                    _smallPicTwo = value;
                    OnPropertyChanged("SmallPicTwo");
                }
            }
        }

        private string _smallPicThree;
        [Column]
        public string SmallPicThree
        {
            get
            {
                return _smallPicThree;
            }
            set
            {
                if (value != _smallPicThree)
                {
                    OnPropertyChanging("SmallPicThree");
                    _smallPicThree = value;
                    OnPropertyChanged("SmallPicThree");
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

        /// <summary>
        /// 车型名字
        /// </summary>
        private string _specName;
        [Column]
        public string SpecName
        {
            get
            {
                return _specName;
            }
            set
            {
                if (value != _specName)
                {
                    OnPropertyChanging("SpecName");
                    _specName = value;
                    OnPropertyChanged("SpecName");
                }
            }
        }


        /// <summary>
        /// 图片1
        /// </summary>
        private ImageSource _bitmap;

        public ImageSource bitmap
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


        /// <summary>
        /// 图片2
        /// </summary>
        private ImageSource _bitmapTwo;

        public ImageSource bitmapTwo
        {
            get
            {
                return _bitmapTwo;
            }
            set
            {
                if (value != _bitmapTwo)
                {
                    OnPropertyChanging("bitmapTwo");
                    _bitmapTwo = value;
                    OnPropertyChanged("bitmapTwo");
                }
            }
        }


        /// <summary>
        /// 图片3
        /// </summary>
        private ImageSource _bitmapThree;

        public ImageSource bitmapThree
        {
            get
            {
                return _bitmapThree;
            }
            set
            {
                if (value != _bitmapThree)
                {
                    OnPropertyChanging("bitmapThree");
                    _bitmapThree = value;
                    OnPropertyChanged("bitmapThree");
                }
            }
        }



    }

}
