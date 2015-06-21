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

		public WebCallback getUserRemoteCallBack;

		//Implementation of interface
		public void GetUserRemote(string pName, string pPassword)
		{
			WebConnectUtility webUtil = new WebConnectUtility ();

			//Completed: Functionalize GetUser
			LoginRequest loginRequest = new LoginRequest (pName, pPassword);
			Console.WriteLine("Current Thread (GetUserRemoteFirst): "+Thread.CurrentThread.ManagedThreadId);
			//For the delegate below, if there is an await inside, the continuation function will execute immediately, and hence not suggested

			Task.Factory.StartNew (delegate {
				Console.WriteLine ("Current Thread (GetUserRemote): " + Thread.CurrentThread.ManagedThreadId);
				webUtil.SetWebCallback(new BL_WebCallback((WebCallback)getUserRemoteCallBack.Clone()));
				//Can't use continuewith here because Start() is similar to "await" and returns immediately
				webUtil.WebAzurePost ("LoginRequest", loginRequest).Start ();
			}, CancellationToken.None, TaskCreationOptions.None, UtilitiesThreadLoader.mWebThreadTaskScheduler);
		}

		public void SetUserRemoteCallback(WebCallback pCallbackDel)
		{
			getUserRemoteCallBack = pCallbackDel;
		}
	}
}

