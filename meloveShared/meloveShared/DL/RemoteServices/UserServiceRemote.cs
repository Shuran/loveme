using System;
using System.Threading.Tasks;
using meloveShared.BL;
using meloveShared.DAL;
using System.Net;
using System.IO;
using System.Json;

namespace meloveShared.DL
{
	public class UserServiceRemote : UserService, IUserServiceRemote
	{
		//Singleton Set-up
		private static UserServiceRemote mUserServiceRemote;
		public static UserServiceRemote mInstance
		{
			get
			{
				if (mUserServiceRemote == null) 
				{
					mUserServiceRemote = new UserServiceRemote ();
				}
				return mUserServiceRemote;
			}
		}
		private UserServiceRemote()
		{
			
		}

		//Implementation of interface
		public async Task<User> GetUserRemote(string pName, string pPassword)
		{
			WebConnectUtility webUtil = new WebConnectUtility ();

			//TODO-working-on: Functionalize GetUser
			string url = "";
			JsonValue jsonValue = await webUtil.WebRestGet (url);

			Console.WriteLine (jsonValue.ToString());
			return new User ("name", "pd");
		}
	}
}

