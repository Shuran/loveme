using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using meloveShared.DL;
using meloveShared.GCL;
using System.Threading;

namespace meloveShared.VL
{
	public partial class LoginPage_1 : ContentPage
	{
		public static LoginPageController loginPageController;

		public LoginPage_1()
		{
			//To use the initializecomponent, the xaml has to set the x:class parameter to this class
			InitializeComponent();
			loginPageController = new LoginPageController ();
		}

		protected override void OnAppearing()
		{
			VLGlobalInfoManager.mInstance.mCurrentPage = PageNameEnum.LoginPage_1;
		}

		// Completed: Implement the login event handler
		// TODO-working-on: Put the LogUserInNormal task in a queue
		void onLoginButtonClicked(object sender, EventArgs args)
		{
			if(loginPageController.loginFlag)
			{
				//Use of Wait: https://msdn.microsoft.com/zh-cn/library/system.threading.tasks.task.wait(v=vs.110).aspx
				//Callback on original thread: http://developer.xamarin.com/guides/cross-platform/application_fundamentals/building_cross_platform_applications/part_5_-_practical_code_sharing_strategies/
				loginPageController.lockLogInTask();
				Console.WriteLine("UI Thread: "+Thread.CurrentThread.ManagedThreadId);

				Task.Factory.StartNew (delegate {
					Console.WriteLine("Current Thread: "+Thread.CurrentThread.ManagedThreadId);
					//This comment is in a new thread, and notice there shouldn't be any await here, otherwise the continuewith function 
					//will be executed immediately (the task returns immediately)
					loginPageController.SetLogInUserNormalCallback(new AsyncVoidCallback(loginCallBack));
					loginPageController.logUserInNormal(xNameEntry.Text,xPasswordEntry.Text);
				}, CancellationToken.None, TaskCreationOptions.None, LogicThreadLoader.mTaskScheduler);
			}
		}

		// TODO: Implement the register event handler
		void onRegisterButtonClicked(object sender, EventArgs args)
		{
			
		}

		//When will the callback be executed: http://stackoverflow.com/questions/11397163/continuewith-a-task-on-the-main-thread
		async Task loginCallBack()
		{
			//Completed: Open the new page upon logged in
			//Navigation must be accessed in UI thread: http://forums.xamarin.com/discussion/19109/navigation-pushasync-not-working-with-task-run

			//Judge the current page to determine whether the action shall be taken
			if (VLGlobalInfoManager.mInstance.mCurrentPage == PageNameEnum.LoginPage_1) 
			{
				await Navigation.PushAsync (new HomePage_1 ());
				Console.WriteLine("Login Page is Pushed");
			}
			loginPageController.releaseLogInTask ();
		}
	}
}
