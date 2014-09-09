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
    /// 经销商Model
    /// </summary>
    [Table]
    public class DealerModel : INotifyPropertyChanged, INotifyPropertyChanging
    {


        /// <summary>
        /// 经销商id
        /// </summary>
        private int _id;
        [Column(IsPrimaryKey = true,CanBeNull = false)]
        public int id
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

        /// <summary>
        /// 车型Id
        /// </summary>
        private int _carId;
        [Column]
        public int CarId
        {
            get
            {
                return _carId;
            }
            set
            {
                if (value != _carId)
                {
                    OnPropertyChanging("CarId");
                    _carId = value;
                    OnPropertyChanged("CarId");
                }
            }
        }


        /// <summary>
        /// 城市Id
        /// </summary>
        private int _cityId;
        [Column]
        public int CityId
        {
            get
            {
                return _cityId;
            }
            set
            {
                if (value != _cityId)
                {
                    OnPropertyChanging("CityId");
                    _cityId = value;
                    OnPropertyChanged("CityId");
                }
            }
        }
        /// <summary>
        /// 经销商名称
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
        /// 商家报价
        /// </summary>
        private string _dealerPrice;
        [Column]
        public string DealerPrice
        {
            get
            {
                return _dealerPrice;
            }
            set
            {
                if (value != _dealerPrice)
                {
                    OnPropertyChanging("DealerPrice");
                    _dealerPrice = value;
                    OnPropertyChanged("DealerPrice");
                }
            }
        }


        /// <summary>
        /// 厂家报价
        /// </summary>
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

        /// <summary>
        /// 经营厂商
        /// </summary>
        private string _scopeFactory;
        [Column]
        public string ScopeFactory
        {
            get
            {
                return _scopeFactory;
            }
            set
            {
                if (value != _scopeFactory)
                {
                    OnPropertyChanging("ScopeFactory");
                    _scopeFactory = value;
                    OnPropertyChanged("ScopeFactory");
                }
            }
        }

        /// <summary>
        /// 商家性质
        /// </summary>
        private string _scope;
        [Column]
        public string Scope
        {
            get
            {
                return _scope;
            }
            set
            {
                if (value != _scope)
                {
                    OnPropertyChanging("Scope");
                    _scope = value;
                    OnPropertyChanged("Scope");
                }
            }
        }

        /// <summary>
        /// 公司地址
        /// </summary>
        private string _address;
        [Column]
        public string Address
        {
            get
            {
                return _address;
            }
            set
            {
                if (value != _address)
                {
                    OnPropertyChanging("Address");
                    _address = value;
                    OnPropertyChanged("Address");
                }
            }
        }

        /// <summary>
        ///联系人
        /// </summary>
        private string _linkPeople;
        [Column]
        public string LinkPeople
        {
            get
            {
                return _linkPeople;
            }
            set
            {
                if (value != _linkPeople)
                {
                    OnPropertyChanging("LinkPeople");
                    _linkPeople = value;
                    OnPropertyChanged("LinkPeople");
                }
            }
        }

        /// <summary>
        /// 电话
        /// </summary>
        private string _tel;
        [Column]
        public string Tel
        {
            get
            {
                return _tel;
            }
            set
            {
                if (value != _tel)
                {
                    OnPropertyChanging("Tel");
                    _tel = value;
                    OnPropertyChanged("Tel");
                }
            }
        }


        private string _styledTel;
        [Column]
        public string StyledTel
        {
            get
            {
                return _styledTel;
            }
            set
            {
                if (value != _styledTel)
                {
                    OnPropertyChanging("StyledTel");
                    _styledTel = value;
                    OnPropertyChanged("StyledTel");
                }
            }
        }

        /// <summary>
        /// 经度
        /// </summary>
        private string _mapLon;
        [Column]
        public string MapLon
        {
            get
            {
                return _mapLon;
            }
            set
            {
                if (value != _mapLon)
                {
                    OnPropertyChanging("MapLon");
                    _mapLon = value;
                    OnPropertyChanged("MapLon");
                }
            }
        }

        /// <summary>
        /// 纬度
        /// </summary>
        private string _mapXon;
        [Column]
        public string MapXon
        {
            get
            {
                return _mapXon;
            }
            set
            {
                if (value != _mapXon)
                {
                    OnPropertyChanging("MapXon");
                    _mapXon = value;
                    OnPropertyChanged("MapXon");
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
