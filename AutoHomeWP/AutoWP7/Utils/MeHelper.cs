using Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ViewModels.Handler;

namespace AutoWP7.Utils
{
    public class MeHelper
    {
        public const string appID = "app";
        //发帖回帖，注册和第三方注册
        public const string appIDWp = "app.wp";
        public const string MyFavoriteFileName = "MyFavorite.json";
        public const string DraftBoxFileName = "DraftBox.json";
        public const string RecentFileName = "Recent.json";
        public const string PrivateMessageFileName = "PrivateMessage.json";

        //小图模式，默认为大图模式
        public const string SmallImageModeKey = "SmallImageMode";

        //微博Token,保存到IsolatedStorageSettings
        public const string weiboAccountKey = "WeiboAccount";
        public const string QQAuthResultKey = "QQAuthResultKey";

        //Pass data from page to page using PhoneApplicationService state.
        public const string MyInfoStateKey = "MyInfo";
        //用来在页面间共享要收藏的信息
        public const string FavoriteStateKey = "Favorite";

        public const int QQPlatformID = 15;
        public const int WeiboPlatformID = 16;
        public const string QQAppKey = "100372335";
        public const string QQAppSecret = "767c3b0b69635ea72353a0019fbb6e2e";
        public const string QQRedirectUri = "http://account.autohome.com.cn";
        public const string WeiboAppKey = "2351935287";
        public const string WeiboAppSecret = "120cb25307676b0273a0dc433ab45a6f";
        public const string WeiboRedirectUri = "http://account.autohome.com.cn/oauth/SinaoauthResult";

        public const string FavoriteTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public static void InitalizeMePersistence()
        {
            ViewModels.Me.FavoriteViewModel.FilePath = MyFavoriteFileName;
            ViewModels.Me.DraftViewModel.FilePath = DraftBoxFileName;
            ViewModels.Me.ViewHistoryViewModel.FilePath = RecentFileName;
            ViewModels.Me.PrivateMessageCacheViewModel.FilePath = PrivateMessageFileName;
        }

        public static string UTF8ToGB2312(string content)
        {
            var gb2312Encoding = DBCSEncoding.GetDBCSEncoding("gb2312");

            byte[] fromBytes = Encoding.UTF8.GetBytes(content);
            byte[] toBytes = Encoding.Convert(Encoding.UTF8, gb2312Encoding, fromBytes);

            string toString = gb2312Encoding.GetString(toBytes, 0, toBytes.Length);
            return toString;
        }

        public static bool GetIsSmallImageMode()
        {
            bool isSmallImageMode = false;
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains(SmallImageModeKey))
            {
                bool.TryParse(settings[SmallImageModeKey].ToString(), out isSmallImageMode);
            }

            return isSmallImageMode;
        }

        public static MyForumModel GetMyInfoModel()
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("userInfo"))
            {
                return settings["userInfo"] as MyForumModel;
            }
            else
            {
                return null;
            }
        }

        public static void SaveMyInfo(MyForumModel model)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings["userInfo"] = model;
            settings.Save();
        }

        #region weibo

        public static void GetWeiboUserNickname(string accessToken, string userId, EventHandler<string> completed = null)
        {
            string url = string.Format("https://api.weibo.com/2/users/show.json?access_token={0}&uid={1}", accessToken, userId);

            WebClient client = new WebClient();
            client.DownloadStringCompleted += (s, e) =>
            {
                string nickname = null;
                try
                {
                    if (e.Error == null && e.Cancelled == false)
                    {
                        JObject json = JObject.Parse(e.Result);
                        nickname = json.SelectToken("screen_name").ToString();
                    }
                }
                catch
                { }

                if (completed != null)
                {
                    completed(s, nickname);
                }
            };
            client.DownloadStringAsync(new Uri(url, UriKind.Absolute));
        }

        #endregion

        #region url

        public const string UserBaseUrl = "http://app.api.autohome.com.cn/wpv1.7/User"; //"http://221.192.136.99:804/wpv1.6/user"; 

        public const string ThirdPartyLoginUrl = "http://i.api.autohome.com.cn/api/Login/OAuthLogin";
        public const string ThirdPartyRegisterUrl = "http://i.api.autohome.com.cn/api/Register/OAuthRegister";
        public const string ThirdPartyUpdateTokenUrl = "http://i.api.autohome.com.cn/api/openplatform/updatetoken";
        public const string ServiceProtocolUrl = "http://app.api.autohome.com.cn/wpv1.7/Html/Register_protocol.html";
        public const string ConnectAccountUrl = "http://i.api.autohome.com.cn/api/OpenPlatform/BindOpenPlantRelation";
        public const string UserRegisterUrl = "http://i.api.autohome.com.cn/api/Register/index";
        public const string SendPrivateMessageUrl = "http://i.api.autohome.com.cn/api/privateletter/sendprivateletter";
        public const string SyncFavoriteCarUrl = "http://club.api.autohome.com.cn/api/user/AppSyncCar";
        public const string SyncFavoriteCollectionUrl = "http://i.api.autohome.com.cn/api/collection/AppSyncCollection";
        public const string SyncPrivateMessageFriendsUrl = "http://i.api.autohome.com.cn/api/privateletter/privateletterdelete";
        public const string ServerTimestampUrl = "http://club.api.autohome.com.cn/api/system/timestamp";
        public const string ThirdPartyBindingStateUrl = UserBaseUrl + "/GetUserOpenPlats.ashx";

        /// <param name="userID">if null, means me</param>
        public static string GetUserInfoUrl(string userID = null, int pageIndex = 1)
        {
            string url = null;
            if (string.IsNullOrEmpty(userID))
            {
                var userInfoModel = GetMyInfoModel();
                if (userInfoModel != null)
                {
                    url = string.Format("{3}/GetUserInfo.ashx?a=2&pm=3&v={4}&au={0}&u={1}&p={2}&s=20", userInfoModel.Authorization, userInfoModel.UserID.ToString(), pageIndex, UserBaseUrl, App.version);
                }
            }
            else
            {
                url = string.Format("{2}/GetUserInfo.ashx?a=2&pm=3&v={3}&au=&u={0}&p={1}&s=20", userID, pageIndex, UserBaseUrl, App.version);
            }

            return url;
        }

        public static string GetFavoriteUrl(int type = 0, int pageIndex = 1)
        {
            var userInfoModel = GetMyInfoModel();
            string url = userInfoModel == null ? null : string.Format("{3}/GetCollectList.ashx?a=2&pm=3&v={4}&p={0}&s=20&type={1}&au={2}", pageIndex, type, userInfoModel.Authorization, UserBaseUrl, App.version);
            return url;
        }

        public static string GetMyTritanUrl(int pageIndex = 1)
        {
            var userInfoModel = GetMyInfoModel();
            string url = userInfoModel == null ? null : string.Format("{2}/usermainpost-a2-pm3-v{3}-u{0}-p{1}-s20.html", userInfoModel.UserID, pageIndex, UserBaseUrl, App.version);
            return url;
        }

        /// <returns>null if user not login.</returns>
        public static string GetMyCommentReplyUrl(int pageIndex = 1)
        {
            var userInfoModel = GetMyInfoModel();
            string url = userInfoModel == null ? null : string.Format("{2}/ReCommentReply.ashx?a=2&pm=3&v={3}&au={0}&p={1}&s=20", userInfoModel.Authorization, pageIndex, UserBaseUrl, App.version);
            return url;
        }

        /// <returns>null if user not login.</returns>
        public static string GetMyForumReplyUrl(int pageIndex = 1)
        {
            var userInfoModel = GetMyInfoModel();
            string url = userInfoModel == null ? null : string.Format("{2}/ReForumReply.ashx?a=2&pm=3&v={3}&au={0}&p={1}&s=20", userInfoModel.Authorization, pageIndex, UserBaseUrl, App.version);
            return url;
        }

        /// <returns>null if user not login.</returns>
        public static string GetPrivateMessageFriendsUrl(int pageIndex = 1)
        {
            var userInfoModel = GetMyInfoModel();
            string url = userInfoModel == null ? null : string.Format("{2}/GetPrivateLetterUserList.ashx?a=2&pm=3&v={3}&au={0}&p={1}&s=20", userInfoModel.Authorization, pageIndex, UserBaseUrl, App.version);
            return url;
        }

        /// <param name="friendID">the userID</param>
        /// <param name="baseMessageID">the newest messageID</param>
        /// <param name="range">0, whose ID larger than baseMessageID, otherwise, smaller</param>
        /// <param name="sort">0,desc, 1, asc</param>
        public static string GetPrivateMessageUrl(int friendID, int baseMessageID, int range = 1, int sort = 1)
        {
            var userInfoModel = GetMyInfoModel();
            string url = userInfoModel == null ? null : string.Format("{5}/GetPrivateLetterListContainSelf.ashx?a=2&pm=3&v={6}&au={0}&tid={1}&mid={2}&t={3}&o={4}&s=50", userInfoModel.Authorization, friendID, baseMessageID, range, sort, UserBaseUrl, App.version);
            return url;
        }

        public static string GetSendCheckCodeUrl()
        {
            return string.Format("{0}/SentCheckCode.ashx", UserBaseUrl);
        }

        public static string GetCountryListUrl()
        {
            return string.Format("{0}/GetCountryList.ashx?a=2&pm=3&v={1}", UserBaseUrl, App.version);
        }

        #endregion
    }
}
