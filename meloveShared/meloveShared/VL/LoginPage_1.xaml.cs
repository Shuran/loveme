using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using meloveShared.DAL;
using meloveShared.BL;
using meloveShared.DL;
using Newtonsoft.Json.Linq;
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
				//If we use "async" in front of delegate, the new task is constructed to be running in the calling thread, instead of a new one
				//Callback on original thread: http://developer.xamarin.com/guides/cross-platform/application_fundamentals/building_cross_platform_applications/part_5_-_practical_code_sharing_strategies/

				//Notice that completed task can not be restarted page-wise
				Task.Factory.StartNew (delegate {
					//This comment is in a new thread
					//Why can just use wait here: http://stackoverflow.com/questions/14230372/start-may-not-be-called-on-a-promise-style-task-exception-is-coming
					//Even notice that the Wait() here can be ommited, because by calling the constructor, it starts automatically
					loginPageController.logUserInNormal(xNameEntry.Text,xPasswordEntry.Text).Start();
					loginPageController.logUserInNormal(xNameEntry.Text,xPasswordEntry.Text).Wait();
				}).ContinueWith (new Action<Task> (async delegate {
					//This comment is in the calling (UI) thread no matter there is async or not
					await loginCallBack();
				}),TaskScheduler.FromCurrentSynchronizationContext());
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
			}
			loginPageController.releaseLogInTask ();
		}
	}
}
