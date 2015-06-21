using System;
using meloveShared.DAL;
using meloveShared.BL;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace meloveShared.DL
{
	public delegate void VoidCallback();
	public delegate Task AsyncVoidCallback();
	public delegate void WebCallback(JObject pWebResult);

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

