using System;
using Newtonsoft.Json.Linq;
using meloveShared.DL;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using meloveShared.DAL;

namespace meloveShared.GCL
{
	public class LoginPageController
	{
		public bool loginFlag { get; private set; }
		public IUserServiceRemote mUserServiceRemote = ServiceAndManagerLoader.mUserServiceRemote;
		public VoidCallback logUserInCallback;

		public LoginPageController ()
		{
			this.loginFlag = true;
		}

		public void logUserInNormal(string pUserName, string pPassword)
		{
			//TODO-suspend: Obtain the user information from remote server
			//TODO-suspend: Construct the mGlobalInfoManager Object
			//TODO-suspend: Construct the mUserServiceRemote Object
			//Completed: Bind the text fields
			//TODO: Extract the info from the returned http message and convert it to LoggedInUser
			Console.WriteLine("Current Thread: "+Thread.CurrentThread.ManagedThreadId);

			//Set-up callback first
			mUserServiceRemote.SetUserRemoteCallback(new WebCallback(logInUserNormalCallback));

			//How to deal with Json: https://components.xamarin.com/view/json.net
			mUserServiceRemote.GetUserRemote(pUserName,pPassword);
		}

		void logInUserNormalCallback(JObject loginResult)
		{
			Console.WriteLine("Current Thread: (logInUserNormalCallback)"+Thread.CurrentThread.ManagedThreadId);
			Console.WriteLine ("logInUserNormalCallback:"+loginResult.ToString());

			Task.Factory.StartNew (delegate {
				Console.WriteLine("Current Thread: (Should be on UI Thread)"+Thread.CurrentThread.ManagedThreadId);
				logUserInCallback ();
			}, CancellationToken.None, TaskCreationOptions.None, meloveShared.VL.UIThreadLoader.mUIThreadTaskScheduler);

			/*
			if (!isLoginJsonValid(loggedInUserJson)) 
			{
				await DisplayAlert ("X", "X", "X");
			} 
			else if (!isLoginValid(loggedInUserJson))
			{
				
			}
			else
			{
				ServiceAndManagerLoader.mGlobalInfoManager.mCurrentUser = loginJsonInterpret (loggedInUserJson);
				//Completed: Construct the mUserServiceLocal Object
				//Completed: Store user info for restoration of data after RESTART
				ServiceAndManagerLoader.mUserServiceLocal.SaveUserLocal (ServiceAndManagerLoader.mGlobalInfoManager.mCurrentUser,xPasswordEntry.Text);
			}*/
		}

		public void SetLogInUserNormalCallback(VoidCallback pLogUserInCallback)
		{
			logUserInCallback = pLogUserInCallback;
		}

		public void lockLogInTask()
		{
			this.loginFlag = false;
		}

		public void releaseLogInTask()
		{
			this.loginFlag = true;
		}
	}
}

