using System.Runtime.Serialization;

namespace Model.Me
{
    [DataContract]
    public class MeModel
    {
        [IgnoreDataMember]
        public bool LoggedIn { get; set; }

        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string PortraitImg { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public int Score { get; set; }

        [DataMember]
        public int JiaYouQuan { get; set; }
    }
}
