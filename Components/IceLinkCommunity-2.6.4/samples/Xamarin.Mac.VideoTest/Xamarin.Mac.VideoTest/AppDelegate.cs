using System;
using System.Drawing;
using Foundation;
using AppKit;
using ObjCRuntime;

namespace Xamarin.Mac.VideoTest
{
	public partial class AppDelegate : NSApplicationDelegate
	{
		MainWindowController mainWindowController;

		public AppDelegate()
		{ }

        public override void DidFinishLaunching(NSNotification notification)
        {
            mainWindowController = new MainWindowController();
            mainWindowController.Window.MakeKeyAndOrderFront(this);
        }
	}
}

