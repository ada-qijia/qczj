using System.Data.Linq.Mapping;
using System.Collections.Generic;

namespace Model
{
    public class CarSearchFilterItemModel : BindableBase
    {
        public string name { get; set; }
        public string value { get; set; }

        private bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set
            {
                if (_Selected != value)
                {
                    _Selected = value;
                    RaisePropertyChanged("Selected");
                }
            }
        }
    }

    public class CarSearchFilterGroupModel
    {
        public CarSearchFilterGroupModel()
        {
            filters = new List<CarSearchFilterItemModel>();
        }

        public string key { get; set; }
        public List<CarSearchFilterItemModel> filters { get; set; }

        public string DisplayName
        {
            get
            {
                string displayName = string.Empty;
                switch (key)
                {
                    case "structure":
                        displayName = "结构";
                        break;
                    case "gearbox":
                        displayName = "变速箱";
                        break;
                    case "price":
                        displayName = "价格";
                        break;
                    case "level":
                        displayName = "级别";
                        break;
                    case "country":
                        displayName = "国别";
                        break;
                    case "findorder":
                        displayName = "排序";
                        break;
                    case "displacement":
                        displayName = "排量";
                        break;
                    case "configs":
                        displayName = "配置";
                        break;
                    case "fueltype":
                        displayName = "燃料";
                        break;
                    default:
                        break;
                }
                return displayName;
            }
        }
    }

}
