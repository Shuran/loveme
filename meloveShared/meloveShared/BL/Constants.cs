using System;

namespace meloveShared.BL
{
	public static class Constants
	{
		public static string mServerUrl = "https://melove.azure-mobile.net/";
		public static string mApplicationKey = "CoyDdlBgmTSVTAGeUyXvuLHXLqYwLk11";

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

