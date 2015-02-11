using QConnectSDK.Api;
using QConnectSDK.Context;
using QConnectSDK.Exceptions;
using QConnectSDK.Models;
using System;
using System.Diagnostics;

namespace AutoWP7.View.Me.QQ
{
    public class AuthenticationProcess
    {
        RestApi restApi;

        public AuthenticationProcess()
        {
            var context = new QzoneContext();
            restApi = new RestApi(context);
        }

        public event EventHandler Authenticated;
        protected virtual void OnAuthenticated()
        {
            var e = Authenticated;
            if (e != null)
                e(this, EventArgs.Empty);
        }

        public event EventHandler AuthenticationFailed;

        protected virtual void OnAuthenticationFailed(EventArgs eArgs)
        {
            var e = AuthenticationFailed;
            if (e != null)
                e(this, EventArgs.Empty);
        }

        public bool HasAuthenticated
        {
            get
            {
                return AuthResult != null && !string.IsNullOrEmpty(AuthResult.AccessToken);
            }
        }

        public void Revoke()
        {
            AuthResult = null;
        }

        public AuthResult AuthResult
        {
            get;
            set;
        }

        public Uri AuthUri
        {
            get
            {
                var context = new QzoneContext();
                var authenticationUrl = context.GetAuthorizationUrl(Scope);
                return new Uri(authenticationUrl);
            }
        }

        /// <summary>
        /// Gets or sets the value of the <see cref="Scope" />
        /// property. This is a dependency property.
        /// </summary>
        public string Scope { get; set; }

        public void RefreshAccessToken()
        {

        }

        public void ExchangeCodeForToken(string code)
        {

            if (string.IsNullOrEmpty(code))
            {
                OnAuthenticationFailed(EventArgs.Empty);
            }
            else
            {
                OAuthToken response = this.restApi.GetUserAccessToken(code);
                GetAccessToken(response);
            }
        }

        void GetAccessToken(OAuthToken response)
        {
            Debug.Assert(response != null);
            AuthResult = new AutoWP7.View.Me.QQ.AuthResult()
            {
                AccessToken = response.AccessToken,
                Expires = response.ExpiresAt
            };
            restApi.GetUserOpenIdAsync(AuthResult.AccessToken, GetUserOpenId, GetUserOpenIdFailure);
        }

        void GetUserOpenId(string response)
        {
            if (string.IsNullOrEmpty(response))
            {
                OnAuthenticationFailed(EventArgs.Empty);
            }
            AuthResult.OpenId = response;
            OnAuthenticated();
        }

        void GetUserOpenIdFailure(QzoneException error)
        {
            OnAuthenticationFailed(EventArgs.Empty);
        }

        void GetAccessTokenFailure(QzoneException error)
        {
            OnAuthenticationFailed(EventArgs.Empty);
        }
    }
}
