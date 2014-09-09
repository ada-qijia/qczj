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
    /// 地区论坛列表model
    /// </summary>
    [Table]
    public class AreaForumModel : INotifyPropertyChanged, INotifyPropertyChanging
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
        /// 版块名称
        /// </summary>
        private string _bbsName;
        [Column]
        public string bbsName
        {
            get
            {
                return _bbsName;
            }
            set
            {
                if (value != _bbsName)
                {
                    OnPropertyChanging("bbsName");
                    _bbsName = value;
                    OnPropertyChanged("bbsName");
                }
            }
        }

        /// <summary>
        /// 板块id
        /// </summary>
        private int _bbsId;
        [Column]
        public int bbsId
        {
            get
            {
                return _bbsId;
            }
            set
            {
                if (value != _bbsId)
                {
                    OnPropertyChanging("bbsId");
                    _bbsId = value;
                    OnPropertyChanged("bbsId");
                }
            }
        }

        private int _Id;
        [Column(IsDbGenerated=true,IsPrimaryKey = true, CanBeNull = false)]
        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (value != _Id)
                {
                    OnPropertyChanging("Id");
                    _Id = value;
                    OnPropertyChanged("Id");
                }
            }
        }
        /// <summary>
        /// 板块类型
        /// </summary>
        private string _bbsType;
        [Column]
        public string bbsType
        {
            get
            {
                return _bbsType;
            }
            set
            {
                if (value != _bbsType)
                {
                    OnPropertyChanging("bbsType");
                    _bbsType = value;
                    OnPropertyChanged("bbsType");
                }
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        private DateTime _createTime;
        [Column]
        public DateTime CreateTime
        {
            get
            {
                return _createTime;
            }
            set
            {
                if (value != _createTime)
                {
                    OnPropertyChanging("CreateTime");
                    _createTime = value;
                    OnPropertyChanged("CreateTime");
                }
            }
        }

        /// <summary>
        /// 首字母
        /// </summary>
        private string _firstLetter;
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
