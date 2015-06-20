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
	public delegate void GetUserRemoteCallBack(JObject pLoginResult);

	public class UserServiceRemote : UserService, IUserServiceRemote
	{
		//Singleton Set-up
		private static UserServiceRemote mUserServiceRemote;
		public GetUserRemoteCallBack getUserRemoteCallBack;

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
		public void GetUserRemote(string pName, string pPassword)
		{
			WebConnectUtility webUtil = new WebConnectUtility ();
			JObject loginResult = null;

			//Completed: Functionalize GetUser
			LoginRequest loginRequest = new LoginRequest (pName, pPassword);
			Console.WriteLine("Current Thread (GetUserRemoteFirst): "+Thread.CurrentThread.ManagedThreadId);
			//For the delegate below, if there is an await inside, the continuation function will execute immediately, and hence not suggested
			Task.Factory.StartNew (delegate {
				Console.WriteLine("Current Thread (GetUserRemote): "+Thread.CurrentThread.ManagedThreadId);
				webUtil.WebAzurePost ("LoginRequest", loginRequest).Start();
			}, CancellationToken.None, TaskCreationOptions.None, UtilitiesThreadLoader.mWebThreadTaskScheduler)
			.ContinueWith(new Action<Task> ( delegate {
				/*
				Console.WriteLine("Current Thread (GetUserRemote Callback): "+Thread.CurrentThread.ManagedThreadId);
				Console.WriteLine (loginResult.ToString ());
				getUserRemoteCallBack(loginResult);
				*/
			}),LogicThreadLoader.mTaskScheduler);
		}

		public void SetUserRemoteCallback(GetUserRemoteCallBack pCallbackDel)
		{
			getUserRemoteCallBack = pCallbackDel;
		}
	}
}

