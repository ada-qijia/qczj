using Model;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutoWP7.Utils
{
    public class MeHelper
    {
        public const string MyFavoriteFileName = "MyFavorite.json";
        public const string DraftBoxFileName = "DraftBox.json";
        public const string RecentFileName = "Recent.json";
        public const string PrivateMessageFileName = "PrivateMessage.json";

        //小图模式，默认为大图模式
        public const string SmallImageModeKey = "SmallImageMode";

        //微博Token,保存到IsolatedStorageSettings
        public const string WeiboAccessTokenKey = "WeiboAccessToken";
        public const string WeiboRefreshTokenKey = "WeiboRefreshToken";
        public const string QQAuthResultKey = "QQAuthResultKey";

        //Pass data from page to page using PhoneApplicationService state.
        public const string MyInfoStateKey = "MyInfo";
        //用来在页面间共享要收藏的信息
        public const string FavoriteStateKey = "Favorite";

        public static void InitalizeMePersistence()
        {
            ViewModels.Me.FavoriteViewModel.FilePath = MyFavoriteFileName;
            ViewModels.Me.DraftViewModel.FilePath = DraftBoxFileName;
            ViewModels.Me.ViewHistoryViewModel.FilePath = RecentFileName;
            ViewModels.Me.PrivateMessageCacheViewModel.FilePath = PrivateMessageFileName;
        }

        public static string UTF8ToGB2312(string content)
        {
            byte[] fromBytes = Encoding.UTF8.GetBytes(content);
            byte[] toBytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("gb2312"), fromBytes);

            string toString = Encoding.GetEncoding("gb2312").GetString(toBytes, 0, toBytes.Length);
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

        public static string SortURLParamAsc(string urlParam)
        {
            throw new NotImplementedException();
        }

        public static void PrepareUploadClientHeaders(ref WebClient wc)
        {
            wc.Headers["Accept-Charset"] = "utf-8";
            wc.Headers["Referer"] = "http://www.autohome.com.cn/china";
            wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            wc.Headers["User-Agent"] = AutoWP7.Handler.Common.GetAutoHomeUA();
        }

        #region url

        public const string ThirdPartyLoginUrl = "http://i.api.autohome.com.cn/api/Login/OAuthLogin";
        public const string ServiceProtocolUrl = "http://account.m.autohome.com.cn/RegisterAgreement.html";
        public const string ConnectAccountUrl = "http://i.api.autohome.com.cn/api/OpenPlatform/BindOpenPlantRelation";
        public const string UserRegisterUrl = "http://i.api.autohome.com.cn/api/Register/index";
        public const string SendPrivateMessageUrl = "http://i.api.autohome.com.cn/api/privateletter/sendprivateletter";
        public const string SyncFavoriteCarUrl = "http://club.api.autohome.com.cn/api/user/AppSyncCar";
        public const string SyncFavoriteCollectionUrl = "http://i.api.autohome.com.cn/api/collection/AppSyncCollection";

        /// <param name="userID">if null, means me</param>
        public static string GetUserInfoUrl(string userID = null, int pageIndex = 1)
        {
            var userInfoModel = IsolatedStorageSettings.ApplicationSettings["userInfo"] as MyForumModel;
            string auth = string.IsNullOrEmpty(userID) ? userInfoModel.Authorization : string.Empty;
            string id = string.IsNullOrEmpty(userID) ? userInfoModel.UserID.ToString() : userID;
            string url = string.Format("http://221.192.136.99:804/wpv1.7/User/GetUserInfo.ashx?a=2&pm=3&v=1.7.0&au={0}&u={1}&p={2}&s=20", auth, id, pageIndex);
            return url;
        }

        public static string GetThirdPartyBindingUrl()
        {
            var userInfoModel = IsolatedStorageSettings.ApplicationSettings["userInfo"] as MyForumModel;
            string url = string.Format("http://221.192.136.99:804/wpv1.7/user/GetUserOpenPlats.ashx?a=2&pm=3&v=1.7.0&au={0}&pfids={1}", userInfoModel.Authorization, "15,16");
            return url;
        }

        public static string GetFavoriteUrl(int type=0, int pageIndex = 1)
        {
            var userInfoModel = IsolatedStorageSettings.ApplicationSettings["userInfo"] as MyForumModel;
            string url = string.Format("http://221.192.136.99:804/wpv1.7/User/GetCollectList.ashx?a=2&pm=3&v=1.7.0&p={0}&s=20&type={1}&au={2}", pageIndex, type, userInfoModel.Authorization);
            return url;
        }

        public static string GetMyTritanUrl(int pageIndex = 1)
        {
            var userInfoModel = IsolatedStorageSettings.ApplicationSettings["userInfo"] as MyForumModel;
            string url = string.Format("http://221.192.136.99:804/wpv1.7/User/usermainpost-a2-pm3-v1.7.0-u{0}-p{1}-s20.html", userInfoModel.UserID, pageIndex);
            return url;
        }

        public static string GetMyCommentReplyUrl(int pageIndex = 1)
        {
            var userInfoModel = IsolatedStorageSettings.ApplicationSettings["userInfo"] as MyForumModel;
            string url = string.Format("http://221.192.136.99:804/wpv1.7/User/ReCommentReply.ashx?a=2&pm=3&v=1.7.0&au={0}&p={1}&s=20", userInfoModel.Authorization, pageIndex);
            return url;
        }

        public static string GetMyForumReplyUrl(int pageIndex = 1)
        {
            var userInfoModel = IsolatedStorageSettings.ApplicationSettings["userInfo"] as MyForumModel;
            string url = string.Format("http://221.192.136.99:804/wpv1.7/User/ReForumReply.ashx?a=2&pm=3&v=1.7.0&au={0}&p={1}&s=20", userInfoModel.Authorization, pageIndex);
            return url;
        }

        public static string GetPrivateMessageFriendsUrl(int pageIndex = 1)
        {
            var userInfoModel = IsolatedStorageSettings.ApplicationSettings["userInfo"] as MyForumModel;
            string url = string.Format("http://221.192.136.99:804/wpv1.7/User/GetPrivateLetterUserList.ashx?a=2&pm=3&v=1.7.0&au={0}&p={1}&s=20", userInfoModel.Authorization, pageIndex);
            return url;
        }

        /// <param name="friendID">the userID</param>
        /// <param name="baseMessageID">the newest messageID</param>
        /// <param name="range">0, whose ID larger than baseMessageID, otherwise, smaller</param>
        /// <param name="sort">0,desc, 1, asc</param>
        public static string GetPrivateMessageUrl(int friendID, int baseMessageID, int range = 1, int sort = 1)
        {
            var userInfoModel = IsolatedStorageSettings.ApplicationSettings["userInfo"] as MyForumModel;
            string url = string.Format("http://221.192.136.99:804/wpv1.7/User/GetPrivateLetterListContainSelf.ashx?a=2&pm=3&v=1.7.0&au={0}&tid={1}&mid={2}&t={3}&o={4}&s=50", userInfoModel.Authorization, friendID, baseMessageID, range, sort);
            return url;
        }

        public static string GetSendCheckCodeUrl()
        {
            return "http://221.192.136.99:804/wpv1.7/user/SentCheckCode.ashx";
        }

        public static string GetCountryListUrl()
        {
            return "http://221.192.136.99:804/wpv1.7/User/GetCountryList.ashx?a=2&pm=3&v=1.7.0";
        }

        #endregion
    }
}
