using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace Model
{
    [Table]
    public class MyForumModel : INotifyPropertyChanged, INotifyPropertyChanging
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

        //是否成功
        private int _success;
        [Column]
        public int Success
        {
            get
            {
                return _success;
            }
            set
            {
                if (value != _success)
                {
                    OnPropertyChanging("Success");
                    _success = value;
                    OnPropertyChanged("Success");
                }
            }
        }

        //消息提示
        private string _message;
        [Column]
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (value != _message)
                {
                    _message = value;
                }
            }
        }

        //密钥
        private string _authorization;
        [Column]
        public string Authorization
        {
            get
            {
                return _authorization;
            }
            set
            {
                if (value != _authorization)
                {
                    OnPropertyChanging("Authorization");
                    _authorization = value;
                    OnPropertyChanged("Authorization");
                }
            }
        }

        //用户头像
        private string _userPic;
        [Column]
        public string UserPic
        {
            get
            {
                return _userPic;
            }
            set
            {
                if (value != _userPic)
                {
                    OnPropertyChanging("UserPic");
                    _userPic = value;
                    OnPropertyChanged("UserPic");
                }
            }
        }

        //用户名
        private string _userName;
        [Column]
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (value != _userName)
                {
                    OnPropertyChanging("UserName");
                    _userName = value;
                    OnPropertyChanged("UserName");
                }
            }
        }

        //用户级别
        private string _grade;
        [Column]
        public string Grade
        {
            get
            {
                return _grade;
            }
            set
            {
                if (value != _grade)
                {
                    OnPropertyChanging("Grade");
                    _grade = value;
                    OnPropertyChanged("Grade");
                }
            }
        }

        //用户威望
        private string _weiWang;
        [Column]
        public string WeiWang
        {
            get
            {
                return _weiWang;
            }
            set
            {
                if (value != _weiWang)
                {
                    OnPropertyChanging("WeiWang");
                    _weiWang = value;
                    OnPropertyChanged("WeiWang");
                }
            }
        }

        //论坛名
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

        //论坛id
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

        //论坛类型
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

        //列出注册时选的车系
        private string _regCar;
        [Column]
        public string RegCar
        {
            get
            {
                return _regCar;
            }
            set
            {
                if (value != _regCar)
                {
                    OnPropertyChanging("RegCar");
                    _regCar = value;
                    OnPropertyChanged("RegCar");
                }
            }
        }

    }
}
