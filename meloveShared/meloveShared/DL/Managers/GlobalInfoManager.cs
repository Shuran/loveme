using System;
using meloveShared.DAL;
using meloveShared.BL;

namespace meloveShared.DL
{
	public class GlobalInfoManager : IGlobalInfoManager
	{
		//Singleton Set-up
		private static GlobalInfoManager mGlobalInfoManager;
		public static GlobalInfoManager mInstance
		{
			get
			{
				if (mGlobalInfoManager==null) 
				{
					mGlobalInfoManager = new GlobalInfoManager ();
				}

				return mGlobalInfoManager;
			}
		}
		private GlobalInfoManager ()
		{
		}

		//Interface Implementation
		//User Data
		public User mCurrentUser { get; set; }
	}
}

