using System;

namespace meloveShared.BL
{
	public class User
	{
		public string mName { get; set; }
		public string mPassword { get; set; }

		public User (string pName, string pPassword)
		{
			this.mName = pName;
			this.mPassword = pPassword;
		}
	}
}

