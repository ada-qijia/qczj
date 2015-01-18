using Model;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
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

        //Pass data from page to page using PhoneApplicationService state.
        public const string MyInfoStateKey = "MyInfo";

        public static void InitalizeMePersistence()
        {
            ViewModels.Me.FavoriteViewModel.FilePath = MyFavoriteFileName;
            ViewModels.Me.DraftViewModel.FilePath = DraftBoxFileName;
            ViewModels.Me.ViewHistoryViewModel.FilePath = RecentFileName;
        }

        #region url

        /// <param name="userID">if null, means me</param>
        public static string GetUserInfoUrl(string userID = null, int pageIndex = 1)
        {
            var userInfoModel = IsolatedStorageSettings.ApplicationSettings["userInfo"] as MyForumModel;
            string auth = string.IsNullOrEmpty(userID) ? userInfoModel.Authorization : string.Empty;
            string id = string.IsNullOrEmpty(userID) ? userInfoModel.Id.ToString() : userID;
            string url = string.Format("http://221.192.136.99:804/wpv1.7/User/GetUserInfo.ashx?a=2&pm=3&v=1.7.0&au={0}&u={1}&p={2}&s=20", auth, id, pageIndex);
            return url;
        }

        public static string GetThirdPartyBindingUrl()
        {
            var userInfoModel = IsolatedStorageSettings.ApplicationSettings["userInfo"] as MyForumModel;
            string url = string.Format("http://221.192.136.99:804/wpv1.7/user/GetUserOpenPlats.ashx?a=2&pm=3&v=1.7.0&au={0}&pfids={1}", userInfoModel.Authorization, "15,16");
            return url;
        }

        public static string GetFavoriteUrl(int pageIndex = 1)
        {
            var userInfoModel = IsolatedStorageSettings.ApplicationSettings["userInfo"] as MyForumModel;
            string url = string.Format("http://221.192.136.99:804/wpv1.7/User/GetCollectList.ashx?a=2&pm=3&v=1.7.0&p={0}&s=50&type=0&au={1}", pageIndex, userInfoModel.Authorization);
            return url;
        }

        public static string GetMyTritanUrl(int pageIndex = 1)
        {
            var userInfoModel = IsolatedStorageSettings.ApplicationSettings["userInfo"] as MyForumModel;
            string url = string.Format("http://221.192.136.99:804/wpv1.7/User//usermainpost-a2-pm3-v1.7.0-u{0}-p{1}-s20.html", userInfoModel.Id, pageIndex);
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

        #endregion
    }
}
