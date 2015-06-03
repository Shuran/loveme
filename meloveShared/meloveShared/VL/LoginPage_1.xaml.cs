using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using meloveShared.DAL;
using meloveShared.BL;
using meloveShared.DL;

namespace meloveShared.VL
{
	public partial class LoginPage_1 : ContentPage
	{
		public LoginPage_1()
		{
			//To use the initializecomponent, the xaml has to set the x:class parameter to this class
			InitializeComponent();
		}

		// TODO-working-on: Implement the login event handler
		async void onLoginButtonClicked(object sender, EventArgs args)
		{
			//TODO-suspend: Obtain the user information from remote server
			//TODO-suspend: Construct the mGlobalInfoManager Object
			//TODO-suspend: Construct the mUserServiceRemote Object
			//Completed: Bind the text fields
			ServiceAndManagerLoader.mGlobalInfoManager.mCurrentUser = await ServiceAndManagerLoader.mUserServiceRemote.GetUserRemote(xNameEntry.Text,xPasswordEntry.Text);

			//TODO-suspend: NOINT
			if (ServiceAndManagerLoader.mGlobalInfoManager.mCurrentUser == null) {
				await DisplayAlert ("X", "X", "X");
			} else {
				//Completed: Construct the mUserServiceLocal Object
				//Completed: Store user info for restoration of data after RESTART
				ServiceAndManagerLoader.mUserServiceLocal.SaveUserLocal (ServiceAndManagerLoader.mGlobalInfoManager.mCurrentUser);
				//Completed: Open the new page upon logged in
				await Navigation.PushModalAsync(new HomePage_1());
			}
		}

		// TODO: Implement the register event handler
		void onRegisterButtonClicked(object sender, EventArgs args)
		{
			
		}
	}
}
