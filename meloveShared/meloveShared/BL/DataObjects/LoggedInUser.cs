using System;

namespace meloveShared.BL
{
	public class LoggedInUser
	{
		public string mName { get; set; }
		public string mUserId { get; set; }
		public string mAuthToken { get; set; }

		public LoggedInUser (string pName, string pUserId, string pAuthToken)
		{
			this.mName = pName;
			this.mUserId = pUserId;
			this.mAuthToken = pAuthToken;
		}

		public void LoggingOut ()
		{
			this.mUserId = null;
			this.mAuthToken = null;
		}
	}
}

