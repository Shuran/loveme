using System;
using System.Threading.Tasks;
using meloveShared.BL;
using meloveShared.DAL;
using Xamarin.Forms;
using meloveShared.BL;

namespace meloveShared.DL
{
	public class UserServiceLocal : UserService, IUserServiceLocal
	{
		//Singleton Set-up
		private static UserServiceLocal mUserServiceLocal;
		public static UserServiceLocal mInstance
		{
			get
			{
				if (mUserServiceLocal == null) 
				{
					mUserServiceLocal = new UserServiceLocal ();
				}
				return mUserServiceLocal;
			}
		}
		private UserServiceLocal()
		{

		}

		//Implementation of Interface
		public void SaveUserLocal(LoggedInUser pLoggedInUser, string pPassword)
		{
			PasswordUtility pdUtil = new PasswordUtility();

			//Completed: Functionalize SaveUser
			//How to save user settings: http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/app-lifecycle/#Properties_Dictionary
			Application.Current.Properties["user_name"] = pLoggedInUser.mName;
			//Completed: Encrypt password
			Application.Current.Properties ["user_pd_enc"] = pdUtil.EncryptToString(pPassword); 

			Console.WriteLine ((String)Application.Current.Properties ["user_name"] + (String)Application.Current.Properties ["user_pd_enc"]);
		}
	}
}


