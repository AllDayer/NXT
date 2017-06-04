using NXT.Services;
using System;
using System.Collections.Generic;
using Xamarin.Facebook;

namespace NXT.Droid
{
	public static class FBSdkHelper
	{
		public static AccessToken ToNative (this FbAccessToken token) {
			Xamarin.Facebook.AccessTokenSource source;
			if (token.AccessTokenSource == Services.AccessTokenSource.WEB_VIEW)
				source = Xamarin.Facebook.AccessTokenSource.WebView;
			else
				source = Xamarin.Facebook.AccessTokenSource.FacebookApplicationNative;
			
			var newToken = new AccessToken (
				token.Token,
				token.ApplicationId,
				token.UserId,
				token.Permissions,
				token.DeclinedPermissions,
				source,
				DateTimeHelper.ToUnixTime(token.ExpirationTime),
				DateTimeHelper.ToUnixTime(token.LastRefreshTime)
			);

			return newToken;
		}

		public static FbAccessToken ToForms(this AccessToken token)
		{
			if (token == null)
				return null;

			var formsToken = new FbAccessToken();

			formsToken.Token = token.Token;
			formsToken.ApplicationId = token.ApplicationId;
			formsToken.UserId = token.UserId;
			formsToken.Permissions = new List<string>();
			formsToken.DeclinedPermissions = new List<string>();
			formsToken.ExpirationTime = DateTimeHelper.FromUnixTime(token.Expires.Time);
			formsToken.LastRefreshTime = DateTimeHelper.FromUnixTime (token.LastRefresh.Time);

			if (token.Source == Xamarin.Facebook.AccessTokenSource.WebView)
				formsToken.AccessTokenSource = Services.AccessTokenSource.WEB_VIEW;
			else
				formsToken.AccessTokenSource = Services.AccessTokenSource.FACEBOOK_APPLICATION;

			foreach (var p in token.Permissions) {
				formsToken.Permissions.Add(p.ToString());
			}
			foreach (var p in token.DeclinedPermissions) {
				formsToken.DeclinedPermissions.Add(p.ToString());
			}

			return formsToken;
		}
	}
}

