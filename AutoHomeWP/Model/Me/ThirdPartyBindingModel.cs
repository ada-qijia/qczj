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

        [DataMember(Name = "relationtype")]
        public int RelationType { get; set; }

        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "orginalid")]
        public string OriginalID { get; set; }

        [DataMember(Name = "token")]
        public string Token { get; set; }

        private bool _isExpired;
        public bool IsExpired { get { return _isExpired; }
            set
            {
               if(value!=_isExpired)
               {
                   _isExpired = value;
                   OnPropertyChanged("IsExpired");
               }
            }
        }
    }
}
