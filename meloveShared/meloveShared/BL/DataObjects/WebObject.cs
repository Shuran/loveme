using System;
using Newtonsoft.Json;

namespace meloveShared.BL
{
	public class WebObject
	{
		private string mAuthToken;

		[JsonProperty(PropertyName = "AuthenticationToken")]
		public string mAuthenticationToken { set { mAuthToken = value;}}
	}
}

