using System;
using System.Drawing;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace Xamarin.Mac.Conference.WebRTC
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		SessionWindowController windowController;

		public AppDelegate()
		{ }

        public override void DidFinishLaunching(NSNotification notification)
        {
			windowController = new SessionWindowController();
			windowController.Window.MakeKeyAndOrderFront(this);
		}
	}
}

