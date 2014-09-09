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
    /// 车系列表
    /// </summary>
    [Table]
    public class CarSeriesModel : INotifyPropertyChanged, INotifyPropertyChanging
    {

        

        /// <summary>
        /// 车系id
        /// </summary>
        private int _id;
        [Column(IsPrimaryKey=true,CanBeNull=true)]
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

        /// <summary>
        /// 品牌名
        /// </summary>
        private string _fctName;
        [Column]
        public string FctName
        {
            get
            {
                return _fctName;
            }
            set
            {
                if (value != _fctName)
                {
                    OnPropertyChanging("FctName");
                    _fctName = value;
                    OnPropertyChanged("FctName");
                }
            }
        }

        //图片
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
        /// <summary>
        /// 车系名
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
        /// 车系图片
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
        /// 品牌名
        /// </summary>
        private string _brandName;
        [Column]
        public string BrandName
        {
            get
            {
                return _brandName;
            }
            set
            {
                if (value != _brandName)
                {
                    OnPropertyChanging("BrandName");
                    _brandName = value;
                    OnPropertyChanged("BrandName");
                }
            }
        }

        /// <summary>
        /// 级别
        /// </summary>
        private string _level;
        [Column]
        public string Level
        {
            get
            {
                return _level;
            }
            set
            {
                if (value != _level)
                {
                    OnPropertyChanging("Level");
                    _level = value;
                    OnPropertyChanged("Level");
                }
            }
        }

        /// <summary>
        /// 图片数量
        /// </summary>
        private int _imgNum;
        [Column]
        public int ImgNum
        {
            get
            {
                return _imgNum;
            }
            set
            {
                if (value != _imgNum)
                {
                    OnPropertyChanging("ImgNum");
                    _imgNum = value;
                    OnPropertyChanged("ImgNum");
                }
            }
        }

        /// <summary>
        /// 报价
        /// </summary>
        private string _priceBetween;
        [Column]
        public string PriceBetween
        {
            get
            {
                return _priceBetween;
            }
            set
            {
                if (value != _priceBetween)
                {
                    OnPropertyChanging("PriceBetween");
                    _priceBetween = value;
                    OnPropertyChanged("PriceBetween");
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
