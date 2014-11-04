using System.Data.Linq.Mapping;
using System.Collections.Generic;

namespace Model
{
    [Table]
    public class SaleItemModel
    {
        public int seriesid{get;set;}
        public string seriesname{get;set;}
        public int specid{get;set;}
        public string specname{get;set;}
        public string specpic{get;set;}
        public int inventorystate{get;set;}
        public string dealerprice{get;set;}
        public string fctprice{get;set;}

        public SaleItemDealerModel dealer { get; set; }
    }

    public class SaleItemDealerModel
    {
        public int id{get;set;}
        public string name{get;set;}
        public string shortname{get;set;}
        public string city{get;set;}
        public string phone{get;set;}
    }

}
