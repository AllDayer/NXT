using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NXT.Services
{
	public interface IGraphRequest
	{
		// Properties
		string Path { get; set; }
		string HttpMethod { get; set; }
		string Version { get; set; }

		// Methods
//		IGraphRequest NewRequest (IAccessToken token, string path, string parameters, string httpMethod = default(string), string version = default(string));
		IGraphRequest NewRequest (FbAccessToken token, string path, Dictionary<string,string> parameters, string httpMethod = default(string), string version = default(string));
		Task<IGraphResponse> ExecuteAsync();
	}
}

