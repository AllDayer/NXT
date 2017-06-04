using System;
using NXT.Droid;
using System.Threading.Tasks;
using Android.App;
using Xamarin.Forms;
using Android.Content;
using Xamarin.Facebook;
using System.Collections.Generic;
using Android.OS;
using System.Threading;
using Xamarin.Facebook.Login;
using NXT.Services;

[assembly:Xamarin.Forms.Dependency(typeof(FacebookLoginService))]
namespace NXT.Droid
{
	public class FacebookLoginService: global::Xamarin.Forms.Platform.Android.FormsApplicationActivity, IFacebookLogin
	{
		private FbAccessToken _accessToken = null;
		private TaskCompletionSource<FbAccessToken> tcs;

		public Task<FbAccessToken> LogIn(string[] permissions) {

			// clear previous token for science
			if (AccessToken.CurrentAccessToken != null) {
				LoginManager.Instance.LogOut ();
			}

			tcs = new TaskCompletionSource<FbAccessToken> ();

			FacebookLoginActivity.OnFacebookLoginCancel += FacebookLoginActivity_OnFacebookLoginCancel;
			FacebookLoginActivity.OnFacebookLoginSuccess += FacebookLoginActivity_OnFacebookLoginSuccess;
			FacebookLoginActivity.OnFacebookLoginError += FacebookLoginActivity_OnFacebookLoginError;

			var activity = Forms.Context as Activity;
			var myIntent = new Intent (activity, typeof(FacebookLoginActivity));
			myIntent.PutExtra("permissions", permissions);
	
			activity.StartActivityForResult (myIntent, 0);

			return tcs.Task;
		}
			
		public bool IsLoggedIn() {
			return AccessToken.CurrentAccessToken != null;
		}

		public FbAccessToken GetAccessToken() {
			if (!IsLoggedIn ())
				return null;
			return _accessToken;
		}

		public void Logout() {
			LoginManager.Instance.LogOut ();
			_accessToken = null;
		}

		// Event handlers
		void FacebookLoginActivity_OnFacebookLoginError (AccessToken token)
		{
			_accessToken = AccessToken.CurrentAccessToken.ToForms();
			tcs.SetResult(_accessToken);
			UnsubscribeFromEvents ();
		}

		void FacebookLoginActivity_OnFacebookLoginSuccess (AccessToken token)
		{ 
			tcs.SetResult(token.ToForms());
			UnsubscribeFromEvents ();
		}

		void FacebookLoginActivity_OnFacebookLoginCancel (AccessToken token)
		{
			tcs.SetCanceled();
			UnsubscribeFromEvents ();
		}

		void UnsubscribeFromEvents() {
			FacebookLoginActivity.OnFacebookLoginCancel -= FacebookLoginActivity_OnFacebookLoginCancel;
			FacebookLoginActivity.OnFacebookLoginSuccess -= FacebookLoginActivity_OnFacebookLoginSuccess;
			FacebookLoginActivity.OnFacebookLoginError -= FacebookLoginActivity_OnFacebookLoginError;
		}

	}
}

