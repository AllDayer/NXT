using System;
using Xamarin.Facebook;
using NXT.Droid;
using System.Threading.Tasks;
using Android.OS;
using System.Collections.Generic;
using Xamarin.Facebook.Share.Model;
using NXT.Services;

[assembly:Xamarin.Forms.Dependency(typeof(GraphRequestService))]
namespace NXT.Droid
{
	public class GraphRequestService: IGraphRequest
	{
		public string Path { get; set; }
		public string HttpMethod { get; set; }
		public string Version { get; set; }

		private AccessToken _token;
		private GraphRequest _request;


		public IGraphRequest NewRequest(FbAccessToken token, string path, Dictionary<string,string> parameters, string httpMethod = default(string), string version = default(string)) {

			Initialize (token, path, httpMethod, version);

			if (parameters != null) {
				var bundle = new Bundle ();

				foreach (var key in parameters.Keys) {
					System.Diagnostics.Debug.WriteLine (key + " : " + parameters [key]);
					bundle.PutString (key, parameters [key]);
				}

				_request.Parameters = bundle;
			}

			return this;
		}

//		public IGraphRequest NewRequest(FbAccessToken token, string path, string parameters, string httpMethod = default(string), string version = default(string)) {
//
//			Initialize (token, path, httpMethod, version);
//
//			var bundle = new Bundle();
//			bundle.PutString("fields", parameters);
//			_request.Parameters = bundle;
//
//			return this;
//		}

		public Task<IGraphResponse> ExecuteAsync() {
			TaskCompletionSource<IGraphResponse> tcs = new TaskCompletionSource<IGraphResponse> ();

			((GraphCallback)_request.Callback).RequestCompleted += (object sender, GraphResponseEventArgs e) => {
				DroidGraphResponse resp = new DroidGraphResponse(e.Response);
				tcs.SetResult(resp);
			};

			_request.ExecuteAsync ();
		
			return tcs.Task;
		}

		private void Initialize(FbAccessToken token, string path, string httpMethod = default(string), string version = default(string)) {
			_token = token.ToNative ();
			Path = path;
			HttpMethod = httpMethod;
			Version = version;

			GraphCallback callback = new GraphCallback ();
			Xamarin.Facebook.HttpMethod http = Xamarin.Facebook.HttpMethod.Get;

			if (httpMethod == "POST") {
				http = Xamarin.Facebook.HttpMethod.Post;
			}

			_request = new GraphRequest (_token, Path, null, http, callback);
		}

	}
}

