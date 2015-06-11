using System;
using meloveShared.DAL;
using meloveShared.BL;

namespace meloveShared.DL
{
	public class LogicGlobalInfoManager : ILogicGlobalInfoManager
	{
		//Singleton Set-up
		private static LogicGlobalInfoManager mGlobalInfoManager;
		public static LogicGlobalInfoManager mInstance
		{
			get
			{
				if (mGlobalInfoManager==null) 
				{
					mGlobalInfoManager = new LogicGlobalInfoManager ();
				}

				return mGlobalInfoManager;
			}
		}
		private LogicGlobalInfoManager ()
		{
		}

		//Interface Implementation
		//User Data
		public LoggedInUser mCurrentUser { get; set; }
	}
}

