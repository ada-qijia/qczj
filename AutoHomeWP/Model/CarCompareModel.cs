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
    public class CarCompareModelT : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// 车型ID
        /// </summary>
        private int _specid;
        [Column(IsPrimaryKey = true, CanBeNull = false)]
        public int SpecId
        {
            get {
                return _specid;
            }
            set {
                if (value != _specid)
                {
                    OnPropertyChanging("SpecId");
                    _specid = value;
                    OnPropertyChanged("SpecId");
                }
            }
        }
        /// <summary>
        /// 车型名称
        /// </summary>
        private string _specname;
        [Column]
        public string SpecName
        {
            get {
                return _specname;
            }
            set {
                if (value != _specname)
                {
                    OnPropertyChanging("SpecName");
                    _specname = value;
                    OnPropertyChanged("SpecName");
                }
            }
        }
        /// <summary>
        /// 所属车系id
        /// </summary>
        private int _seriesid;
        [Column]
        public int SeriesId
        {
            get
            {
                return _seriesid;
            }
            set
            {
                if (value != _seriesid)
                {
                    OnPropertyChanging("SeriesId");
                    _seriesid = value;
                    OnPropertyChanged("SeriesId");
                }
            }
        }
        /// <summary>
        /// 是否被选中
        /// </summary>
        private bool _ischoosed;
        [Column]
        public bool IsChoosed
        {
            get
            {
                return _ischoosed;
            }
            set
            {
                if (value != _ischoosed)
                {
                    OnPropertyChanging("IsChoosed");
                    _ischoosed = value;
                    OnPropertyChanged("IsChoosed");
                }
            }
        }
        /// <summary>
        /// 所属车系名称
        /// </summary>
        private string _seriesname;
        [Column(Name="SeriesName")]
        public string SeriesName
        {
            get
            {
                return _seriesname;
            }
            set
            {
                if (value != _seriesname)
                {
                    OnPropertyChanging("SeriesName");
                    _seriesname = value;
                    OnPropertyChanged("SeriesName");
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
