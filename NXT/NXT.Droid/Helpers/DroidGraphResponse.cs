using System;
using NXT.Droid;
using Xamarin.Facebook;
using NXT.Services;

[assembly:Xamarin.Forms.Dependency(typeof(DroidGraphResponse))]
namespace NXT.Droid
{
	public class DroidGraphResponse: IGraphResponse
	{
		public string RawResponse { get ; set; }

		public DroidGraphResponse (GraphResponse graphResponse)
		{
			RawResponse = graphResponse.RawResponse;
		}
	}
}

