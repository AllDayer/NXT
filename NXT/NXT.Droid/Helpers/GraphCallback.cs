using System;
using Xamarin.Facebook;

namespace NXT.Droid
{
	public class GraphCallback : Java.Lang.Object, GraphRequest.ICallback
	{
		public event EventHandler<GraphResponseEventArgs> RequestCompleted = delegate {};

		public void OnCompleted(GraphResponse response) {
			this.RequestCompleted (this, new GraphResponseEventArgs (response));
		}
	}

	public class GraphResponseEventArgs : EventArgs
	{
		GraphResponse _response;
		public GraphResponseEventArgs (GraphResponse response) {
			_response = response;
		}

		public GraphResponse Response { get { return _response; } }
	}

}

