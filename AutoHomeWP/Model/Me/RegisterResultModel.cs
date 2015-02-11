using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model.Me
{
    [DataContract]
    public class RegisterResultModel
    {
        [DataMember(Name = "MemberId")]
        public int ID { get; set; }

        [DataMember(Name = "NickName")]
        public string UserName { get; set; }

        [DataMember(Name = "PcpopClub")]
        public string Auth { get; set; }

        [DataMember]
        public int UserState { get; set; }

        /// <summary>
        /// 0保密，1男，2女
        /// </summary>
        [DataMember(Name = "Sex")]
        public int Gender { get; set; }

        [DataMember(Name="UserPwd")]
        public string Password { get; set; }

        [DataMember(Name="UserTypeId")]
        public int UserType { get; set; }

        [DataMember(Name = "MobilePhone")]
        public string Mobile { get; set; }
    }
}
