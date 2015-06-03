using System;
using meloveShared.DAL;

namespace meloveShared.DL
{
	//Load all the managers and services in this class
	public static class ServiceAndManagerLoader
	{
		//Managers
		public static IGlobalInfoManager mGlobalInfoManager { get; private set;}

		//Remote Services
		public static IUserServiceRemote mUserServiceRemote { get; private set;}

		//Local Services
		public static IDatabaseServiceLocal mDatabaseServiceLocal { get; private set; }
		public static IUserServiceLocal mUserServiceLocal { get; private set; }

		public static void Init()
		{
			//Managers
			mGlobalInfoManager = GlobalInfoManager.mInstance;

			//Remote Services
			mUserServiceRemote = UserServiceRemote.mInstance;

			//Local Services
			mDatabaseServiceLocal = DatabaseServiceLocal.mInstance;
			mUserServiceLocal = UserServiceLocal.mInstance;
		}
	}
}

