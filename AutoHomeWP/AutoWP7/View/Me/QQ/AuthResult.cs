using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoWP7.View.Me.QQ
{
    public class AuthResult
    {
        public AuthResult()
        {
            AquiredAt = DateTime.UtcNow;
        }

        /// <summary>
        /// access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string OpenId { get; set; }

        public int Expires { get; set; }

        public DateTime AquiredAt { get; set; }

        public DateTime ExpiresAt
        {
            get
            {
                return AquiredAt + TimeSpan.FromSeconds(Expires - 60);
            }
        }

        public bool IsExpired
        {
            get
            {
                return DateTime.UtcNow > ExpiresAt;
            }
        }

    }
}
