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

namespace Model
{
    
    [Table]
    public class CarSeriesQuoteModel: INotifyPropertyChanged, INotifyPropertyChanging
    {
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

        /// <summary>
        /// 分组名
        /// </summary>
        private string _groupName;
        [Column]
        public string GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                if (value != _groupName)
                {
                    OnPropertyChanging("GroupName");
                    _groupName = value;
                    OnPropertyChanged("GroupName");
                }
            }
        }

        /// <summary>
        /// 车型全称
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

        private string _price;
        [Column]
        public string Price
        {
            get
            {
                return _price;
            }
            set
            {
                if (value != _price)
                {
                    OnPropertyChanging("Price");
                    _price = value;
                    OnPropertyChanged("Price");
                }
            }
        }

        private string _gearBox;
        [Column]
        public string GearBox
        {
            get
            {
                return _gearBox;
            }
            set
            {
                if (value != _gearBox)
                {
                    OnPropertyChanging("GearBox");
                    _gearBox = value;
                    OnPropertyChanged("GearBox");
                }
            }
        }

        private string _structure;
        [Column]
        public string Structure
        {
            get
            {
                return _structure;
            }
            set
            {
                if (value != _structure)
                {
                    OnPropertyChanging("Structure");
                    _structure = value;
                    OnPropertyChanged("Structure");
                }
            }
        }

        private string _transmission;
        [Column]
        public string Transmission
        {
            get
            {
                return _transmission;
            }
            set
            {
                if (value != _transmission)
                {
                    OnPropertyChanging("Transmission");
                    _transmission = value;
                    OnPropertyChanged("Transmission");
                }
            }
        }
        /// <summary>
        /// 是否显示参数配置
        /// </summary>
        private int _paramisshow;
        [Column(CanBeNull=true)]
        public int ParamIsShow
        {
            get
            {
                return _paramisshow;
            }
            set
            {
                if (value != _paramisshow)
                {
                    OnPropertyChanging("ParamIsShow");
                    _paramisshow = value;
                    OnPropertyChanged("ParamIsShow");
                }
            }
        }

        /// <summary>
        /// 对比
        /// </summary>
        private string _compare;
        public string Compare
        {
            get
            {
                return _compare;
            }
            set
            {
                if (value != _compare)
                {
                    OnPropertyChanging("Compare");
                    _compare = value;
                    OnPropertyChanged("Compare");
                }
            }
        }
        /// <summary>
        /// 对比显示文本
        /// </summary>
        private string _comparetext;
        public string CompareText
        {
            get
            {
                return _comparetext;
            }
            set
            {
                if (value != _comparetext)
                {
                    OnPropertyChanging("CompareText");
                    _comparetext = value;
                    OnPropertyChanged("CompareText");
                }
            }
        }
        /// <summary>
        /// 组排序
        /// </summary>
        private int _grouporder;
        [Column(CanBeNull = true)]
        public int GroupOrder
        {
            get
            {
                return _grouporder;
            }
            set
            {
                if (value != _grouporder)
                {
                    OnPropertyChanging("GroupOrder");
                    _grouporder = value;
                    OnPropertyChanged("GroupOrder");
                }
            }
        }
        /// <summary>
        /// 子排序
        /// </summary>
        private int _order;
        [Column(CanBeNull = true)]
        public int COrder
        {
            get
            {
                return _order;
            }
            set
            {
                if (value != _order)
                {
                    OnPropertyChanging("COrder");
                    _order = value;
                    OnPropertyChanged("COrder");
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
