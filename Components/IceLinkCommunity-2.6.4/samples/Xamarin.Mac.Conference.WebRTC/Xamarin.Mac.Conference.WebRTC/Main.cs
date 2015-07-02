using System;
using System.Drawing;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace Xamarin.Mac.Conference.WebRTC
{
	class MainClass
	{
		static void Main(string[] args)
		{
			NSApplication.Init();
			NSApplication.Main(args);
		}
	}
}

