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
using System.Windows.Media.Imaging;


namespace Model
{
    [Table]
    public class VideoModel : NewsModel
    {
        private string _playcount;
        [Column]
        public string playcount
        {
            get
            {
                return _playcount;
            }
            set
            {
                if (value != _playcount)
                {
                    OnPropertyChanging("playcount");
                    _playcount = value;
                    OnPropertyChanged("playcount");
                }
            }
        }

        private string _nickname;
        [Column]
        public string nickname
        {
            get
            {
                return _nickname;
            }
            set
            {
                if (value != _nickname)
                {
                    OnPropertyChanging("nickname");
                    _nickname = value;
                    OnPropertyChanged("nickname");
                }
            }
        }

        private string _videoaddress;
        [Column]
        public string videoaddress
        {
            get
            {
                return _videoaddress;
            }
            set
            {
                if (value != _videoaddress)
                {
                    OnPropertyChanging("videoaddress");
                    _videoaddress = value;
                    OnPropertyChanged("videoaddress");
                }
            }
        }

        private string _shareaddress;
        [Column]
        public string shareaddress
        {
            get
            {
                return _shareaddress;
            }
            set
            {
                if (value != _shareaddress)
                {
                    OnPropertyChanging("shareaddress");
                    _shareaddress = value;
                    OnPropertyChanged("shareaddress");
                }
            }
        }

    }
}
