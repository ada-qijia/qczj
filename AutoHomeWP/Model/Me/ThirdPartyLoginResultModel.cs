using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Me
{
    [DataContract]
    public class ThirdPartyLoginResultModel
    {
        [DataMember(Name = "UserId")]
        public string ID { get; set; }

        [DataMember(Name = "UserName")]
        public string UserName { get; set; }

        [DataMember(Name = "HeadImage")]
        public string Img { get; set; }

        [DataMember(Name = "IsMobilePhone")]
        public bool IsPhoneAuth { get; set; }

        [DataMember(Name="PcpopClub")]
        public string Auth { get; set; }

        /// <summary>
        /// 0保密，1男，2女
        /// </summary>
        [DataMember(Name = "Sex")]
        public int Gender { get; set; }

        [DataMember(Name = "Money")]
        public int Score { get; set; }

        [DataMember(Name = "RecommendValue")]
        public int Prestige { get; set; }

        [DataMember(Name = "iscarowner")]
        public bool IsCarOwner { get; set; }

        [DataMember(Name = "ProvinceId")]
        public int ProvinceID { get; set; }

        [DataMember(Name = "CityId")]
        public int CityID { get; set; }

        [DataMember(Name = "CountyId")]
        public int CountyID { get; set; }
    }
}
