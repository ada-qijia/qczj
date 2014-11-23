using System.Data.Linq.Mapping;
using System.Collections.Generic;

namespace Model
{
    public class CarSearchResultSeriesItemModel : LoadMoreItem
    {
        public CarSearchResultSeriesItemModel()
        {
            specitemgroups = new List<CarSearchResultSpecItemGroupModel>();
        }

        public int id { get; set; }
        public string name { get; set; }
        public string img { get; set; }
        public string level { get; set; }
        public string price { get; set; }
        public int count { get; set; }

        public List<CarSearchResultSpecItemGroupModel> specitemgroups { get; set; }
    }

    public class CarSearchResultSpecItemGroupModel
    {
        public CarSearchResultSpecItemGroupModel()
        {
            specitems = new List<CarSearchResultSpecItemModel>();
        }

        public string groupname { get; set; }
        public List<CarSearchResultSpecItemModel> specitems { get; set; }
    }

    public class CarSearchResultSpecItemModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string price { get; set; }
        public string description { get; set; }
    }

}
