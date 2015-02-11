using System.Runtime.Serialization;

namespace Model.Me
{
    [DataContract]
    public class CountryModel
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "countrycode")]
        public int CountryCode { get; set; }

        [DataMember(Name = "phonelength")]
        public int PhoneLength { get; set; }
    }
}
