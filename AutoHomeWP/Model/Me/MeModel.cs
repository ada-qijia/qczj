using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Model.Me
{
    [DataContract]
    public class MeModel
    {
        [IgnoreDataMember]
        public bool LoggedIn { get; set; }

        [DataMember(Name = "userid")]
        public string ID { get; set; }

        [DataMember(Name = "minpic")]
        public string Img { get; set; }

        [DataMember(Name = "name")]
        public string UserName { get; set; }

        [DataMember(Name = "sex")]
        public string Gender { get; set; }

        [DataMember(Name = "areaname")]
        public string City { get; set; }

        [DataMember(Name = "regtime")]
        public string RegisterTime { get; set; }

        [DataMember(Name = "isphoneauth")]
        public bool IsPhoneAuth { get; set; }

        [DataMember(Name = "mycarname")]
        public string FollowCar { get; set; }

        [DataMember(Name = "integral")]
        public int Score { get; set; }

        [DataMember(Name = "stampcount")]
        public int JiaYouQuan { get; set; }

        [DataMember(Name = "medalsnum")]
        public int MedalsNum { get; set; }
        /// <summary>
        /// 是否认证车主
        /// </summary>
        [DataMember(Name = "iscarowner")]
        public bool IsCarOwner { get; set; }

        [DataMember(Name = "unreadnum")]
        public Unreadnum Unread { get; set; }
    }

    [DataContract]
    public class Unreadnum
    {
        [DataMember(Name="list")]
        public List<UnreadItem> Items { get; set; }

        [DataMember(Name="total")]
        public int Total { get; set; }
    }

    [DataContract]
    public class UnreadItem
    {
        [DataMember(Name = "typeid")]
        public int Type { get; set; }

        [DataMember(Name = "count")]
        public int Count { get; set; }
    }
}
