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
    [Table]
    public class ProvinceModel : INotifyPropertyChanged, INotifyPropertyChanging  
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

        //省市id
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

        //城市名称
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

        //省份名字
        private string _fatherName;
        [Column]
        public string FatherName
        {
            get
            {
                return _fatherName;
            }
            set
            {
                if (value != _fatherName)
                {
                    OnPropertyChanging("FatherName");
                    _fatherName = value;
                    OnPropertyChanged("FatherName");
                }
            }
        }

        //父亲ID
        private int _father;
        [Column]
        public int Father
        {
            get
            {
                return _father;
            }
            set
            {
                if (value != _father)
                {
                    OnPropertyChanging("Father");
                    _father = value;
                    OnPropertyChanged("Father");
                }
            }
        }

        /// <summary>
        /// 首字母
        /// </summary>
        private  string _firstLetter;
        [Column]
        public string FirstLetter
        {
            get
            {
                return _firstLetter;
            }
            set
            {
                if (value != _firstLetter)
                {
                    OnPropertyChanging("FirstLetter");
                    _firstLetter = value;
                    OnPropertyChanged("FirstLetter");
                }
            }
        }
    }
}
