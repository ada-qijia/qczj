using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table]
    public class ImageDuTuModel : BaseModel
    {
        public int _index;
        [Column]
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (value != _index)
                {
                    OnPropertyChanging("Index");
                    _index = value;
                    OnPropertyChanged("Index");
                }
            }
        }

        public string _url;
        [Column]
        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                if (value != _url)
                {
                    OnPropertyChanging("Url");
                    _url = value;
                    OnPropertyChanged("Url");
                }
            }
        }

        public string _bigurl;
        [Column]
        public string BigUrl
        {
            get
            {
                return _bigurl;
            }
            set
            {
                if (value != _bigurl)
                {
                    OnPropertyChanging("BigUrl");
                    _bigurl = value;
                    OnPropertyChanged("BigUrl");
                }
            }
        }

        public string _smallgurl;
        [Column]
        public string SmallUrl
        {
            get
            {
                return _smallgurl;
            }
            set
            {
                if (value != _smallgurl)
                {
                    OnPropertyChanging("SmallUrl");
                    _smallgurl = value;
                    OnPropertyChanged("SmallUrl");
                }
            }
        }
    }
}
