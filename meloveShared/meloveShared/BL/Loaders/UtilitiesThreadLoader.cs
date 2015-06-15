using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace meloveShared.BL
{
	public class UtilitiesThreadLoader
	{
		//public static UtilitiesThreadTaskScheduler mWebThreadTaskScheduler;
		//public static UtilitiesThreadTaskScheduler mFileThreadTaskScheduler;
	}

	/*
	public sealed class UtilitiesThreadTaskScheduler : TaskScheduler, IDisposable
	{
		private BlockingCollection<Task> mTasks = new BlockingCollection<Task>();

		private Thread mThread;

		public UtilitiesThreadTaskScheduler()
		{
			mThread = new Thread 
			(
				() =>
				{
						foreach (var pTask in mTasks.GetConsumingEnumerable())
						{
							TryExecuteTask(pTask);
						}
				}
			);
			mThread.IsBackground = true;
			mThread.Start ();
		}

		protected override void QueueTask(Task pTask)
		{
			
		}
	}
	*/
}

