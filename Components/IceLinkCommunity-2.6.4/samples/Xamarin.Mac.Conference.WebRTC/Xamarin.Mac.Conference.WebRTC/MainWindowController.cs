
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using ObjCRuntime;
using FM;
using FM.IceLink;
using FM.IceLink.WebRTC;
using CoreFoundation;
using CoreGraphics;
using System.Drawing;
using Xamarin.Mac.Conference.WebRTC.Opus;
using Xamarin.Mac.Conference.WebRTC.VP8;

[assembly: RequiredFramework("lib/FMAudio.dylib")]
[assembly: RequiredFramework("lib/FMVideo.dylib")]

namespace Xamarin.Mac.Conference.WebRTC
{
	public partial class MainWindowController : NSWindowController
	{
		private App App;

		private bool _StopLocalMedia;
		private bool _StopConference;

		private WindowDelegate WindowDelegate = null;

		public Action OnClose;

		public NSView        View         { get { return _view;         } }
		public NSPopUpButton AudioDevices { get { return _audioDevices; } }
		public NSPopUpButton VideoDevices { get { return _videoDevices; } }

		#region Constructors

		// Called when created from unmanaged code
		public MainWindowController(IntPtr handle)
			: base(handle)
		{
			Initialize();
		}
		
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public MainWindowController(NSCoder coder)
			: base(coder)
		{
			Initialize();
		}
		
		// Call to load from the XIB/NIB file
		public MainWindowController()
			: base("MainWindow")
		{
			Initialize();
		}
		
		// Shared initialization code
		void Initialize()
		{ }

		#endregion

		public override void WindowDidLoad()
		{
			base.WindowDidLoad();
			WindowDelegate = new WindowDelegate();
			WindowDelegate.WindowWillClose += WindowWillClose;
			Window.Delegate = WindowDelegate;
			Window.Restorable = false;

			App = App.Instance;

			_sessionID.StringValue = App.SessionId;

			// Start local media when the view loads.
			StartLocalMedia();
		}

		public void WindowWillClose(NSNotification notification)
		{
			if (_StopConference)
			{
				StopConference();
			}

			if (_StopLocalMedia)
			{
				StopLocalMedia();
			}

			if (OnClose != null)
			{
				OnClose();
			}
		}

		private void StartLocalMedia()
		{
			App.StartLocalMedia(this, (error) =>
			{
				if (error != null)
				{
					Alert(error);
				}
				else
				{
					// Stop local media when the view unloads.
					_StopLocalMedia = true;

					// Start conference now that the local media is available.
					StartConference();
				}
			});
		}

		private void StopLocalMedia()
		{
			App.StopLocalMedia((error) =>
			{
				if (error != null)
				{
					Alert(error);
				}
			});
		}

		private void StartConference()
		{
			App.StartConference((error) =>
			{
				if (error != null)
				{
					Alert(error);
				}
				else
				{
					// Stop conference when the view unloads.
					_StopConference = true;
				}
			});
		}

		private void StopConference()
		{
			App.StopConference((error) =>
			{
				if (error != null)
				{
					Alert(error);
				}
			});
		}

		private void Alert(string format, params object[] args)
		{
			var alert = new NSAlert();
			alert.AddButton("OK");
			alert.MessageText = string.Format(format, args);
			alert.AlertStyle = NSAlertStyle.Warning;

			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				alert.BeginSheet(Window);
			});
		}

		//strongly typed window accessor
		public new AppKit.NSWindow Window
		{
			get
			{
				return (AppKit.NSWindow)base.Window;
			}
		}
	}

	[Register("ColorView")]
	public class ColorView : NSView
	{
		private NSColor Background;

		public ColorView(IntPtr p)
			: base(p)
		{ }

		public void SetBackground(NSColor color)
		{
			if (Background != color)
			{
				Background = color;
				SetNeedsDisplayInRect(Bounds);
			}
		}

		public override void DrawRect(CGRect dirtyRect)
		{
			if (Background != null)
			{
				Background.Set();
				NSRectFill(Bounds);
			}
		}

		[System.Runtime.InteropServices.DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
		extern static IntPtr NSRectFill(CGRect rect);
	}
}

