using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{

        [Table]
        public class CacheVarModel : INotifyPropertyChanged, INotifyPropertyChanging
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


            //自动增长id
            private int _id;
            [Column(IsPrimaryKey = true, IsDbGenerated = true, CanBeNull = false)]
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
            //消息提示
            private string _key;
            [Column]
            public string Key
            {
                get
                {
                    return _key;
                }
                set
                {
                    if (value != _key)
                    {
                        _key = value;
                    }
                }
            }
            //消息提示
            private string _value;
            [Column]
            public string Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    if (value != _value)
                    {
                        _value = value;
                    }
                }
            }
        }
    }

