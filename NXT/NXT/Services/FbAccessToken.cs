using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NXT.Services
{

	public enum AccessTokenSource
	{
		FACEBOOK_APPLICATION,
		WEB_VIEW,
		NONE
	}

	public class FbAccessToken
	{
		// Properties
		public string Token { get; set; }
		public string ApplicationId { get; set; }
		public string UserId { get; set; }
		public ICollection<string> Permissions { get; set; }
		public ICollection<string> DeclinedPermissions { get; set; }
		public AccessTokenSource AccessTokenSource { get; set; }
		public DateTime ExpirationTime { get; set; }
		public DateTime LastRefreshTime { get; set; }

		private static FbAccessToken _currentAccessToken;
		public static FbAccessToken Current
        {
			get { return _currentAccessToken ?? (_currentAccessToken = GetCurrentAccessToken()); }
		}

        public static void Clear()
        {
            _currentAccessToken = null;
        }

        public static FbAccessToken GetCurrentAccessToken() {
	
			try
            {
				var token = new FbAccessToken();

				if (Application.Current.Properties.ContainsKey("fb_access_token_string"))
                {
					token.Token = (string)Application.Current.Properties["fb_access_token_string"];
					token.ApplicationId = (string)Application.Current.Properties["fb_access_token_app_id"];
					token.UserId = (string)Application.Current.Properties["fb_access_token_user_id"];
					token.ExpirationTime = (DateTime)Application.Current.Properties["fb_access_token_expiry"];
					token.LastRefreshTime = (DateTime)Application.Current.Properties["fb_access_token_refresh"];
					return token;
				}
                else
                {
					return null;
				}

			}
            catch (Exception )
            {
				System.Diagnostics.Debug.WriteLine("No access token found");
				return null;
			}

		}

		public async Task Save()
        {

			try
            {
				Application.Current.Properties["fb_access_token_string"] = Token;
				Application.Current.Properties["fb_access_token_app_id"] = ApplicationId;
				Application.Current.Properties["fb_access_token_user_id"] = UserId;
				Application.Current.Properties["fb_access_token_expiry"] = ExpirationTime;
				Application.Current.Properties["fb_access_token_refresh"] = LastRefreshTime;
				await Application.Current.SavePropertiesAsync();
			}
            catch (Exception )
            {
				System.Diagnostics.Debug.WriteLine("Error saving access token");
			}

		}

        public async Task Logout()
        {
            try
            {
                Application.Current.Properties.Clear();
                await Application.Current.SavePropertiesAsync();
            }
            catch
            {

            }
        }
	}
}
