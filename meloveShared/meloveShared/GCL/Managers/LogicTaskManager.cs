using System.Threading;
using System.Threading.Tasks;

namespace meloveShared
{
	public class LogicTaskManager
	{
		//Singleton Set-up
		private static LogicTaskManager mLogicTaskManager;
		//public static Dispatcher mDispatcher;

		private LogicTaskManager()
		{
			
		}

		public static LogicTaskManager mInstance
		{
			get
			{
				if (mLogicTaskManager == null) {
					mLogicTaskManager = new LogicTaskManager ();
				}
				return mLogicTaskManager;
			}
		}


	}
}

