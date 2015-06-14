using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace meloveShared.BL
{
	/*
	public static class UtilitiesThreadLoader
	{
		public static UtilitiesThreadTaskScheduler mInstance { get; private set;}

		public static void Init()
		{
			mInstance = new UtilitiesThreadTaskScheduler ();
		}
	}
		
	public sealed class UtilitiesThreadTaskScheduler : TaskScheduler, IDisposable
	{
		private BlockingCollection<Task> mWebTasks = new BlockingCollection<Task>();
		private BlockingCollection<Task> mFileTasks = new BlockingCollection<Task>();

		private Thread mWebThread;
		private Thread mFileThread;

		public UtilitiesThreadTaskScheduler()
		{
			//Web Thread
			mWebThread = new Thread 
			(
				() =>
				{
						foreach (var pTask in mWebTasks.GetConsumingEnumerable())
						{
							TryExecuteTask(pTask);
						}
				}
			);
			mWebThread.IsBackground = true;
			mWebThread.Start ();

			//File Thread
			mFileThread = new Thread 
			(
				() =>
				{
						foreach (var pTask in mFileTasks.GetConsumingEnumerable())
						{
							TryExecuteTask(pTask);
						}
				}
			);
			mFileThread.IsBackground = true;
			mFileThread.Start ();
		}

		protected override void QueueTask(Task pTask)
		{
			
		}
	}
	*/
}

