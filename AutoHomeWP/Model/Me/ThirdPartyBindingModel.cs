using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Model.Me
{
    [DataContract]
    public class ThirdPartyBindingModel
    {
        [DataMember(Name = "id")]
        public int ThirdPartyID { get; set; }

        [DataMember(Name = "userid")]
        public int UserID { get; set; }

        [DataMember(Name = "relationtype")]
        public int RelationType { get; set; }

        [DataMember(Name = "username")]
        public string UserName { get; set; }

        [DataMember(Name = "orginalid")]
        public string OriginalID { get; set; }

        [DataMember(Name = "token")]
        public string Token { get; set; }
    }
}
