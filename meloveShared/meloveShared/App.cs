using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using meloveShared.DL;
using meloveShared.VL;
using meloveShared.BL;

namespace meloveShared.VL
{
	public class App : Application
	{
		public App ()
		{
			//Completed: Initialize the environment
			ServiceAndManagerLoader.Init();
			LogicThreadLoader.Init();
			UtilitiesThreadLoader.Init();

			//Completed: Create the instance of LoginPage_1
			//Completed: Create the loginPage UI Interface
			var loginPage = new LoginPage_1 ();

			var navPage = new NavigationPage(loginPage);
			// The root page of your application
			MainPage = navPage;
		}

		protected override void OnStart ()
		{
			// Handle when your app starts

			// if RESTART

			// if INSTALL
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
