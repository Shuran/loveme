using System;
using Newtonsoft.Json;

namespace meloveShared.BL
{
	public class LoginRequest : WebObject
	{
		[JsonProperty(PropertyName = "mUserName")]
		public string mUserName { get; set; }

		[JsonProperty(PropertyName = "mPassword")]
		public string mPassword { get; set; }

		public LoginRequest (string pUserName, string pPassword)
		{
			this.mUserName = pUserName;
			this.mPassword = pPassword;
		}
	}
}

