using System;

namespace meloveShared
{
	public class VLGlobalInfoManager
	{
		private static VLGlobalInfoManager mVLGlobalInfoManager;
		private VLGlobalInfoManager()
		{
			
		}
		public static VLGlobalInfoManager mInstance
		{
			get
			{
				if (mVLGlobalInfoManager == null) 
				{
					mVLGlobalInfoManager = new VLGlobalInfoManager ();
				}
				return mVLGlobalInfoManager;
			}
		}

		public CurrentPageEnum mCurrentPage;
	}

	public enum CurrentPageEnum
	{
		LoginPage_1,
		HomePage_1
	}
}

