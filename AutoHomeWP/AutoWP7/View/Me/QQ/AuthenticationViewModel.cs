using Microsoft.Phone.Controls;
using QConnectSDK.Api;
using QConnectSDK.Context;
using QConnectSDK.Exceptions;
using QConnectSDK.Models;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoWP7.View.Me.QQ
{
    public class AuthenticationViewModel : ViewModels.BaseViewModel
    {
        private AuthenticationProcess _process;

        private object _sync = new object();

        /// <summary>
        /// 用已存的认证结果初始化
        /// </summary>
        private AuthenticationViewModel()
        {
            _process = new AuthenticationProcess()
            {
                Scope = "get_user_info,add_share,list_album,upload_pic,check_page_fans,add_t,add_pic_t,del_t,get_repost_list,get_info,get_other_info,get_fanslist,get_idolist,add_idol,del_idol,add_one_blog,add_topic,get_tenpay_addr"
            };

            _process.Authenticated += new EventHandler(_process_Authenticated);
            _process.AuthenticationFailed += new EventHandler(_process_AuthenticationFailed);

            AuthResult auth;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue<AuthResult>(Utils.MeHelper.QQAuthResultKey, out auth);
            _process.AuthResult = auth;
        }

        private static AuthenticationViewModel _singleInstance;
        public static AuthenticationViewModel SingleInstance
        {
            get
            {
                if (_singleInstance == null)
                {
                    _singleInstance = new AuthenticationViewModel();
                }

                return _singleInstance;
            }
        }

        public bool HasAuthenticated
        {
            get
            {
                lock (_sync)
                    return _process.HasAuthenticated;
            }
        }

        public DateTime TokenExpiresAt
        {
            get
            {
                lock (_sync)
                {
                    if (HasAuthenticated)
                        return _process.AuthResult.ExpiresAt;
                }
                return DateTime.MinValue;
            }
        }

        private User _profile;
        public User Profile
        {
            get { return _profile; }
            set
            {
                if (_profile != value)
                {
                    _profile = value;
                    OnPropertyChanged("Profile");
                }
            }
        }

        private Uri _authUri = new Uri("about:blank");
        public Uri AuthUri
        {
            get
            {
                return _authUri;
            }
            set
            {
                if (_authUri != value)
                {
                    _authUri = value;
                    OnPropertyChanged("AuthUri");
                }
            }
        }

        private string _code;
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
                //this cause the _process_Authenticated or _process_AuthenticationFailed called
                _process.ExchangeCodeForToken(Code);
            }
        }

        private bool _isAuthenticating;
        private Queue<Action<string, string>> _queuedRequests = new Queue<Action<string, string>>();

        #region 验证授权

        //验证授权
        public void GetAccessCode(Action<string, string> callback)
        {
            lock (_sync)
            {
                if (_isAuthenticating)
                {
                    _queuedRequests.Enqueue(callback);
                }
                else if (HasAuthenticated)
                {
                    if (!_process.AuthResult.IsExpired)
                    {
                        callback(_process.AuthResult.AccessToken, _process.AuthResult.OpenId);
                    }
                    else
                    {
                        InvokeCallback(callback);
                    }
                }
                else
                {
                    InvokeCallback(callback);
                }
            }
        }

        void _process_Authenticated(object sender, EventArgs e)
        {
            lock (_sync)
            {
                _isAuthenticating = false;

                while (_queuedRequests.Count > 0)
                    _queuedRequests.Dequeue()(_process.AuthResult.AccessToken, _process.AuthResult.OpenId);

                IsolatedStorageSettings.ApplicationSettings[Utils.MeHelper.QQAuthResultKey] = _process.AuthResult;
                IsolatedStorageSettings.ApplicationSettings.Save();
            }

            OnPropertyChanged("HasAuthenticated");
        }

        void _process_AuthenticationFailed(object sender, EventArgs e)
        {
            lock (_sync)
            {
                _isAuthenticating = false;
                _process.Revoke();
                OnPropertyChanged("HasAuthenticated");

                AuthUri = _process.AuthUri;
            }
        }

        //需要重新验证
        private void InvokeCallback(Action<string, string> callback)
        {
            _isAuthenticating = true;
            _queuedRequests.Enqueue(callback);

            ((PhoneApplicationFrame)App.Current.RootVisual).Navigate(new Uri("/View/Me/QQ/AuthenticationPage.xaml", UriKind.Relative));
            AuthUri = _process.AuthUri;
        }

        #endregion

        public void Logout()
        {
            lock (_sync)
            {
                _process.Revoke();
            }
            OnPropertyChanged("HasAuthenticated");
        }

        #region 登录

        private EventHandler<QzoneException> LoadProfileCompleted;

        public void Login()
        {
            this.GetAccessCode((s, t) => LoadProfile(s, t));
        }

        //验证结果返回后
        public void LoadProfile(string access_token, string openId, EventHandler<QzoneException> completed = null)
        {
            this.LoadProfileCompleted = completed;
            if (this.Profile == null)
            {
                OAuthToken token = new OAuthToken()
                {
                    AccessToken = access_token,
                    OpenId = openId
                };
                QzoneContext context = new QzoneContext(token);
                RestApi restapi = new RestApi(context);
                restapi.GetCurrentUserAsync(ProfileLoaded, ProfileLoadedFailed);
            }
        }

        private void ProfileLoaded(User user)
        {
            Profile = user;
            if (LoadProfileCompleted != null)
            {
                LoadProfileCompleted(this, null);
            }
        }

        private void ProfileLoadedFailed(QzoneException ex)
        {
            Profile = null;
            if (LoadProfileCompleted != null)
            {
                LoadProfileCompleted(this, ex);
            }
        }

        #endregion
    }
}
