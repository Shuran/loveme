using System;

namespace meloveShared.BL
{
	public static class Constants
	{
		public static string mServerUrl = "";
		public static string mApplicationKey = "";

		public static PlatformType mPlatformType
		{
			get
			{
				PlatformType pType;

				#if __IOS__
				pType = PlatformType.iOS;
				#else
				#if __ANDROID
				pType = PlatformType.Android;
				#else
				pType = PlatformType.WinPhone;
				#endif
				#endif

				return pType;
			}
		}
	}

	public enum PlatformType
	{
		iOS,
		Android,
		WinPhone
	}
}

