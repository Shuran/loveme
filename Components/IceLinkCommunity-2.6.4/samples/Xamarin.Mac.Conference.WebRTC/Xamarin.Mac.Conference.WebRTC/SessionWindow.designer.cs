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
	[Register ("SessionWindowController")]
	partial class SessionWindowController
	{
		[Outlet]
		AppKit.NSButton _createButton { get; set; }

		[Outlet]
		AppKit.NSTextField _createSession { get; set; }

		[Outlet]
		AppKit.NSButton _joinButton { get; set; }

		[Outlet]
		AppKit.NSTextField _joinSession { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (_createSession != null) {
				_createSession.Dispose ();
				_createSession = null;
			}

			if (_joinSession != null) {
				_joinSession.Dispose ();
				_joinSession = null;
			}

			if (_createButton != null) {
				_createButton.Dispose ();
				_createButton = null;
			}

			if (_joinButton != null) {
				_joinButton.Dispose ();
				_joinButton = null;
			}
		}
	}

	[Register ("SessionWindow")]
	partial class SessionWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
