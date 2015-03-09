using System;
using System.Runtime.Serialization;

namespace Model.Me
{
    [DataContract]
    public class ThirdPartyAccountModel
    {
        /// <summary>
        /// weibo 16
        /// </summary>
        [DataMember]
        public int PlatformId { get; set; }

        /// <summary>
        /// sina weibo originalId, qq OpenId
        /// </summary>
        [DataMember]
        public string OpenId { get; set; }

        [DataMember]
        public string AccessToken { get; set; }

        [DataMember]
        public int ExpiresIn { get; set; }

        [DataMember]
        public string RefreshToken { get; set; }
    }
}
