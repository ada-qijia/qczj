using System.Data.Linq.Mapping;
using System.Collections.Generic;

namespace Model
{
    [Table]
    public class CarSeriesAlibiModel
    {
        [Column]
        public string Grade { get; set; }

        [Column]
        public int PeopleNum { get; set; }

        [Column]
        public int HasStopSellAlibi { get; set; }

        [Column]
        public IEnumerable<CarSeriesAlibiSpecGroupModel> SpecGroupList { get; set; }
    }

    [Table]
    public class CarSeriesAlibiSpecGroupModel
    {
        [Column]
        public string GroupName { get; set; }

        public IEnumerable<CarSeriesAlibiSpecModel> SpecList { get; set; }
    }

    [Table]
    public class CarSeriesAlibiSpecModel
    {
        [Column]
        public int ID { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public string Grade { get; set; }

        [Column]
        public int PeopleNum { get; set; }
        
        [Column]
        public string FlowModeName { get; set; }

        [Column]
        public double Displacement { get; set; }

        [Column]
        public string EnginePower { get; set; }

        [Column]
        public string Year { get; set; }
    }

}
