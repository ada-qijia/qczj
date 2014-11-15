using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Model.Search
{
    [DataContract]
    public class CarSeriesSearchModel
    {
        [DataMember(Name="id")]
        public int ID { get; set; }

        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="imgurl")]
        public string Img { get; set; }

        [DataMember(Name="fctname")]
        public string FactoryName { get; set; }

        [DataMember(Name="level")]
        public string Level { get; set; }

        [DataMember(Name="pricebetween")]
        public string PriceBetween { get; set; }
    }
}
