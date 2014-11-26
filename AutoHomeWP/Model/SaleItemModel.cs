using System.Data.Linq.Mapping;
using System.Collections.Generic;

namespace Model
{
    [Table]
    public class SaleItemModel : LoadMoreItem
    {
        public int seriesid { get; set; }
        public string seriesname { get; set; }
        public int specid { get; set; }
        public string specname { get; set; }
        public string specpic { get; set; }
        public int inventorystate { get; set; }
        public string dealerprice { get; set; }
        public string fctprice { get; set; }
        public int articleid { get; set; }
        public int articletype { get; set; }

        public SaleDealer dealer { get; set; }

        //for ui only
        public string series_spec_name
        {
            get
            {
                return seriesname + " " + specname;
            }
        }

        public string inventorystate_name
        {
            get
            {
                string displayName = string.Empty;
                switch (inventorystate)
                {
                    case 0:
                        displayName = "现车充足";
                        break;
                    case 1:
                        displayName = "少量现车";
                        break;
                    case 2:
                        displayName = "";
                        break;
                    default:
                        break;
                }
                return displayName;
            }
        }

        public string city_dealer_name
        {
            get
            {
                return dealer.city + "|" + dealer.shortname;
            }
        }

        public string dealerprice_display
        {
            get
            {
                return dealerprice + "万";
            }
        }

        public string discount_price
        {
            get
            {
                return "降" + (double.Parse(fctprice) - double.Parse(dealerprice)).ToString() + "万";
            }
        }

    }

    public class SaleDealer
    {
        public int id { get; set; }
        public string name { get; set; }
        public string shortname { get; set; }
        public string city { get; set; }
        public string phone { get; set; }
    }

}
