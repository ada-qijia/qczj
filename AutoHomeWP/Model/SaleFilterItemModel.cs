using System.Data.Linq.Mapping;
using System.Collections.Generic;

namespace Model
{
    public class SaleFilterModel
    {
        public Dictionary<string, SaleFilterGroup> FilterGroups { get; set; }

    }

    public class SaleFilterGroup
    {
        public string Key { get; set; }
        public IEnumerable<SaleFilterItemModel> Filters { get; set; }
    }

    public class SaleFilterItemModel
    {
        public string name { get; set; }
        public string value { get; set; }
        public string type { get; set; }
    }

}
