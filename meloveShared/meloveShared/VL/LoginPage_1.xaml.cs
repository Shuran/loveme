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

		// TODO-working-on: Implement the login event handler
		void onLoginButtonClicked(object sender, EventArgs args)
		{
			if(loginPageController.loginFlag)
			{
				//Use of Wait: https://msdn.microsoft.com/zh-cn/library/system.threading.tasks.task.wait(v=vs.110).aspx
				//If we use "async" in front of delegate, the new task is constructed to be running in the same thread, instead of a new one
				//Callback on original thread: http://developer.xamarin.com/guides/cross-platform/application_fundamentals/building_cross_platform_applications/part_5_-_practical_code_sharing_strategies/

				Task.Factory.StartNew (delegate {
					loginPageController.logUserInNormal(xNameEntry.Text,xPasswordEntry.Text).Start();
					loginPageController.logUserInNormal(xNameEntry.Text,xPasswordEntry.Text).Wait();
				}).ContinueWith (new Action<Task> (delegate {
					loginCallBack ();
				}),TaskScheduler.FromCurrentSynchronizationContext());


				/*
				loginPageController.logUserInNormal (xNameEntry.Text, xPasswordEntry.Text).ContinueWith (
					new Action<Task> (delegate {
						loginCallBack ();
					})
				);
				loginPageController.logUserInNormal(xNameEntry.Text,xPasswordEntry.Text).Start();
				loginPageController.logUserInNormal(xNameEntry.Text,xPasswordEntry.Text).Wait();
				*/

			}
		}

		// TODO: Implement the register event handler
		void onRegisterButtonClicked(object sender, EventArgs args)
		{
			
		}

		//When will the callback be executed: http://stackoverflow.com/questions/11397163/continuewith-a-task-on-the-main-thread
		void loginCallBack()
		{
			//Completed: Open the new page upon logged in
			Navigation.PushAsync(new HomePage_1());
		}
	}
}
