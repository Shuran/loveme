using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

namespace meloveShared.DL
{
	public static class LogicThreadLoader
	{
		public static LogicThreadTaskScheduler mInstance { get; private set; }

		public static void Init()
		{
			mInstance = new LogicThreadTaskScheduler();
		}
	}

	//How to make custom taskscheduler
	//Reference: https://msdn.microsoft.com/en-us/library/dd997413(VS.100).aspx
	public class LogicThreadTaskScheduler : TaskScheduler, IDisposable
	{
		private BlockingCollection<Task> mTasks = new BlockingCollection<Task>();
		private Thread mThread;

		public override int MaximumConcurrencyLevel { get { return 1; } }

		public LogicThreadTaskScheduler()
		{
			mThread = new Thread 
			(
				() => 
				{
					foreach(var pTask in mTasks.GetConsumingEnumerable())
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
			mTasks.Add (pTask);
		}

		protected override bool TryExecuteTaskInline(Task pTask, bool pTaskWasPreviouslyQueued)
		{
			if (Thread.CurrentThread != mThread) 
			{
				return false;
			}
			return TryExecuteTask (pTask);
		}

		protected override IEnumerable<Task> GetScheduledTasks()
		{
			return mTasks.ToArray ();
		}

		public void Dispose()
		{
			if (mThread != null) 
			{
				mTasks.CompleteAdding ();
				mThread.Join ();
				mTasks.Dispose ();
				mTasks = null;
				mThread = null;
			}
		}
	}
}

