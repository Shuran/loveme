using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

namespace meloveShared.BL
{
	public class UtilitiesThreadLoader
	{
		public static UtilityThreadTaskScheduler mWebThreadTaskScheduler { get; private set; }
		public static UtilityThreadTaskScheduler mFileThreadTaskScheduler { get; private set; }

		public static void Init()
		{
			mWebThreadTaskScheduler = new UtilityThreadTaskScheduler ();
			mFileThreadTaskScheduler = new UtilityThreadTaskScheduler ();
		}
	}

	public sealed class UtilityThreadTaskScheduler : TaskScheduler, IDisposable
	{
		private BlockingCollection<Task> mTasks = new BlockingCollection<Task>();
		private Thread mThread;

		public override int MaximumConcurrencyLevel { get { return 1; } }

		public UtilityThreadTaskScheduler()
		{
			mThread = new Thread 
				(
					() =>
					{
						Console.WriteLine("Utility Layer Thread Started: " + Thread.CurrentThread.ManagedThreadId);
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

