using System;
using System.Threading.Tasks;

namespace NXT.Services
{
	public interface IFacebookLogin
	{

		Task<FbAccessToken> LogIn(string[] permissions);
		bool IsLoggedIn ();
		FbAccessToken GetAccessToken();
		void Logout();
	}
}

