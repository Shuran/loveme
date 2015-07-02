// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace Xamarin.Mac.Conference.WebRTC
{
	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}

	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		AppKit.NSPopUpButton _audioDevices { get; set; }

		[Outlet]
		AppKit.NSTextField _sessionID { get; set; }

		[Outlet]
		AppKit.NSPopUpButton _videoDevices { get; set; }

		[Outlet]
		AppKit.NSView _view { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_audioDevices != null) {
				_audioDevices.Dispose ();
				_audioDevices = null;
			}

			if (_sessionID != null) {
				_sessionID.Dispose ();
				_sessionID = null;
			}

			if (_videoDevices != null) {
				_videoDevices.Dispose ();
				_videoDevices = null;
			}

			if (_view != null) {
				_view.Dispose ();
				_view = null;
			}
		}
	}
}
