using System;
using System.Threading.Tasks;
using meloveShared.BL;
using meloveShared.DAL;
using System.Net;
using System.IO;
using System.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

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
		//TODO: Make the returned data more of the specific type
		public async Task<JObject> GetUserRemote(string pName, string pPassword)
		{
			WebConnectUtility webUtil = new WebConnectUtility ();
			JObject loginResult = null;

			//Completed: Functionalize GetUser
			LoginRequest loginRequest = new LoginRequest (pName, pPassword);

			Task.Factory.StartNew (async delegate {
				Console.WriteLine("Current Thread (GetUserRemote): "+Thread.CurrentThread.ManagedThreadId);
				loginResult = await webUtil.WebAzurePost ("LoginRequest", loginRequest);
			}, CancellationToken.None, TaskCreationOptions.None, UtilitiesThreadLoader.mWebThreadTaskScheduler).Wait();

			Console.WriteLine (loginResult.ToString ());
			return loginResult;
		}
	}
}

