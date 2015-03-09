using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Model.Me
{
    [DataContract]
    public class ThirdPartyBindingModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [DataMember(Name = "id")]
        public int ThirdPartyID { get; set; }

        [DataMember(Name = "userid")]
        public int UserID { get; set; }

        private int _relationType;

        [DataMember(Name = "relationtype")]
        public int RelationType
        {
            get { return _relationType; }
            set
            {
                if (value != _relationType)
                {
                    _relationType = value;
                    OnPropertyChanged("RelationType");
                    OnPropertyChanged("ShowName");
                    OnPropertyChanged("StatusName");
                }
            }
        }

        private string _userName;
        [DataMember(Name = "username")]
        public string UserName
        {
            get { return _userName; }
            set
            {
                if (value != _userName)
                {
                    _userName = value;
                    OnPropertyChanged("UserName");
                    OnPropertyChanged("ShowName");
                }
            }
        }

        [DataMember(Name = "orginalid")]
        public string OriginalID { get; set; }

        [DataMember(Name = "token")]
        public string Token { get; set; }

        #region for UI presentation

        private bool _isExpired;
        public bool IsExpired
        {
            get { return _isExpired; }
            set
            {
                if (value != _isExpired)
                {
                    _isExpired = value;
                    OnPropertyChanged("IsExpired");
                    OnPropertyChanged("StatusName");
                }
            }
        }

        public string ShowName
        {
            get { return RelationType == 0 ? "未绑定" : UserName; }
        }

        public string StatusName
        {
            get
            {
                switch (RelationType)
                {
                    case 0:
                        return "立即绑定";
                    case 1:
                        return IsExpired ? "重新绑定" : "已经绑定";
                    case 2:
                        return "重新绑定";
                    default:
                        return "";
                }
            }
        }

        #endregion
    }
}
