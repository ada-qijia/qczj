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
    /// 品牌汽车
    /// </summary>
    [Table]
    public class CarBrandModel:INotifyPropertyChanged,INotifyPropertyChanging
    {
        /// <summary>
        /// 品牌id
        /// </summary>
        private int _id;
        [Column(IsPrimaryKey=true,CanBeNull=false)]
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


        //当前时间
        private DateTime _currentTime;
        [Column]
        public DateTime CurrentTime
        {
            get
            {
                return _currentTime;
            }
            set
            {
                if (value != _currentTime)
                {
                    OnPropertyChanging("CurrentTime");
                    _currentTime = value;
                    OnPropertyChanged("CurrentTime");
                }
            }
        }

        /// <summary>
        /// 品牌名
        /// </summary>
        private string _name;
        [Column]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    OnPropertyChanging("Name");
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// 首字母
        /// </summary>
        private string _letter;
        [Column]
        public string Letter
        {
            get
            {
                return _letter;
            }
            set
            {
                if (value != _letter)
                {
                    OnPropertyChanging("Letter");
                    _letter = value;
                    OnPropertyChanged("Letter");
                }
            }
        }

        /// <summary>
        /// 图片地址
        /// </summary>
        private string _imgUrl;
        [Column]
        public string ImgUrl
        {
            get
            {
                return _imgUrl;
            }
            set
            {
                if (value != _imgUrl)
                {
                    OnPropertyChanging("ImgUrl");
                    _imgUrl = value;
                    OnPropertyChanged("ImgUrl");
                }
            }
        }

        /// <summary>
        /// 图片
        /// </summary>
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
