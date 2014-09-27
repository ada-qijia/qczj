using System.Data.Linq.Mapping;
using System.Collections.Generic;

namespace Model
{
    [Table]
    public class AlibiDetailModel
    {
        [Column]
        public int ID { get; set; }

        [Column]
        public int MemberID { get; set; }

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
        public string Content { get; set; }

        [Column]
        public IEnumerable<AlibiDetailPicModel> PicList { get; set; }

        public AlibiDetailCarInfoModel CarInfo { get; set; }
    }

    [Table]
    public class AlibiDetailPicModel
    {
        [Column]
        public string BigURL { get; set; }

        [Column]
        public string SmallURL { get; set; }
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
        public string oilconsumption { get; set; }

        [Column]
        public string drivenkiloms { get; set; }

        [Column]
        public string purposes { get; set; }

        [Column]
        public string space { get; set; }

        [Column]
        public string power { get; set; }

        [Column]
        public string maneuverability { get; set; }

        [Column]
        public string actualoilcomsumption { get; set; }

        [Column]
        public string comfortabelness { get; set; }

        [Column]
        public string apperance { get; set; }

        [Column]
        public string inside { get; set; }

        [Column]
        public string costefficient { get; set; }
    }

}
