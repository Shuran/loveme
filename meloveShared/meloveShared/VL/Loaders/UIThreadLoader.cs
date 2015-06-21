using System;
using System.Threading.Tasks;

namespace meloveShared.VL
{
	public class UIThreadLoader
	{
		public static TaskScheduler mUIThreadTaskScheduler { get; private set;}

		public static void Init()
		{
			mUIThreadTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext ();
		}
	}
}

