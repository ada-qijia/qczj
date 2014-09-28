using System.Data.Linq.Mapping;
using System.Collections.Generic;

namespace Model
{
    [Table]
    public class AlibiDetailModel
    {
        [Column]
        public int id { get; set; }

        [Column]
        public int memberid { get; set; }

        [Column]
        public string membername { get; set; }

        [Column]
        public string membericon { get; set; }

        [Column]
        public int isauth { get; set; }

        [Column]
        public int specid { get; set; }

        [Column]
        public string specname { get; set; }

        [Column]
        public string reportdate { get; set; }

        [Column]
        public string koubeitypes { get; set; }

        [Column]
        public int commentcount { get; set; }

        [Column]
        public int helpfulcount { get; set; }

        [Column]
        public int viewcount { get; set; }

        [Column]
        public string content { get; set; }

        [Column]
        public List<AlibiDetailPicModel> piclist { get; set; }

        public AlibiDetailCarInfoModel carinfo { get; set; }
    }

    [Table]
    public class AlibiDetailPicModel
    {
        [Column]
        public string bigurl { get; set; }

        [Column]
        public string smallurl { get; set; }
    }

    [Table]
    public class AlibiDetailCarInfoModel
    {
        [Column]
        public double boughtprice { get; set; }

        [Column]
        public string boughtdate { get; set; }

        [Column]
        public string boughtaddress { get; set; }

        [Column]
        public double oilconsumption { get; set; }

        [Column]
        public string drivenkiloms { get; set; }

        [Column]
        public string purposes { get; set; }

        [Column]
        public double space { get; set; }

        [Column]
        public double power { get; set; }

        [Column]
        public double maneuverability { get; set; }

        [Column]
        public double actualoilcomsumption { get; set; }

        [Column]
        public double comfortabelness { get; set; }

        [Column]
        public double apperance { get; set; }

        [Column]
        public double inside { get; set; }

        [Column]
        public double costefficient { get; set; }
    }

}
