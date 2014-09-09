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
    /// 车系配置model
    /// </summary>
    [Table]
    public class CarSeriesConfigurationModel : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private string _groupame;
        [Column]
        ///分组名
        public string GroupName
        {
            get
            {
                return _groupame;
            }
            set
            {
                if (value != _groupame)
                {
                    OnPropertyChanging("GroupName");
                    _groupame = value;
                    OnPropertyChanged("GroupName");
                }
            }
        }


        /// <summary>
        /// 配置名称
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
        /// 配置的值
        /// </summary>
        private string _val;
        [Column]
        public string Val
        {
            get
            {
                return _val;
            }
            set
            {
                if (value != _val)
                {
                    OnPropertyChanging("Val");
                    _val = value;
                    OnPropertyChanged("Val");
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
