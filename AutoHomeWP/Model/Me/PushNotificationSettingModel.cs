using System;
using System.Runtime.Serialization;

namespace Model.Me
{
    [DataContract]
    public class PushNotificationSettingModel
    {
        [DataMember(Name="UserId")]
        public string UserId { get; set; }

        [DataMember]
        public string DeviceName { get; set; }

        [DataMember]
        public string DeviceToken { get; set; }

        [DataMember(Name="AllowPerson")]
        public bool NotAllowPerson { get; set; }

        [DataMember(Name = "AllowSystem")]
        public bool NotAllowSystem { get; set; }

        [DataMember]
        public int StartTime { get; set; }

        [DataMember]
        public int EndTime { get; set; }

        [DataMember]
        public DateTime CreateTime { get; set; }

        [DataMember]
        public DateTime UpdateTime { get; set; }
    }
}
