
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using CoreFoundation;
using Foundation;
using AppKit;

using FM;

namespace Xamarin.Mac.Conference.WebRTC
{
	public delegate void WindowWillCloseDelegate(NSNotification notification);

	public class WindowDelegate : NSWindowDelegate
	{
		public event WindowWillCloseDelegate WindowWillClose;

		public override void WillClose(NSNotification notification)
		{
			var handler = WindowWillClose;
			if (handler != null)
			{
				handler(notification);
			}
		}
	}

	public partial class SessionWindowController : AppKit.NSWindowController
	{
		private App App;
		private MainWindowController VideoChat;

		private bool _StopSignalling;

		private WindowDelegate WindowDelegate = null;

		#region Constructors

		// Called when created from unmanaged code
		public SessionWindowController(IntPtr handle)
			: base(handle)
		{
			Initialize();
		}
		
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public SessionWindowController(NSCoder coder)
			: base(coder)
		{
			Initialize();
		}
		
		// Call to load from the XIB/NIB file
		public SessionWindowController()
			: base("SessionWindow")
		{
			Initialize();
		}
		
		// Shared initialization code
		void Initialize()
		{
		}

		#endregion

		public override void WindowDidLoad()
		{
			base.WindowDidLoad();
			WindowDelegate = new WindowDelegate();
			WindowDelegate.WindowWillClose += WindowWillClose;
			Window.Delegate = WindowDelegate;

			_createButton.Activated += OnCreateButtonClick;
			_joinButton.Activated += OnJoinButtonClick;
			_joinSession.Changed += OnJoinSessionChanged;

			App = App.Instance;

			// Create a random 6 digit number for the new session ID.
			_createSession.StringValue = new Randomizer().Next(100000, 999999).ToString();

			var numberFormatter = new NSNumberFormatter();
			numberFormatter.NumberStyle = NSNumberFormatterStyle.None;
			numberFormatter.MaximumFractionDigits = 0;
			_joinSession.Formatter = numberFormatter;

			// Start signalling when the window loads.
			StartSignalling();
		}

		public void WindowWillClose(NSNotification notification)
		{
			if (_StopSignalling)
			{
				StopSignalling();
			}

			// Terminate the application when the window closes.
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				NSApplication.SharedApplication.Terminate(this);
			});
		}

		private void StartSignalling()
		{
			App.StartSignalling((error) =>
			{
				if (error != null)
				{
					Alert(error);
				}
				else
				{
					// Stop signalling when the view unloads.
					_StopSignalling = true;
				}
			});
		}

		private void StopSignalling()
		{
			App.StopSignalling((error) =>
			{
				if (error != null)
				{
					Alert(error);
				}
			});
		}

		private void SwitchToVideoChat(string sessionId)
		{
			if (sessionId.Length == 6)
			{
				App.SessionId = sessionId;

				// Show the video chat.
				VideoChat = new MainWindowController();
				VideoChat.OnClose = () =>
				{
					// Show the session selector.
					ShowWindow(this);
					Window.MakeKeyAndOrderFront(this);
				};
				VideoChat.ShowWindow(this);
				VideoChat.Window.MakeKeyAndOrderFront(this);

				// Hide the session selector.
				Window.OrderOut(null);
			}
			else
			{
				Alert("Session ID must be 6 digits long.");
			}
		}

		private Regex NumericRegex = new Regex(@"[^0-9]+", RegexOptions.IgnoreCase);
		public void OnJoinSessionChanged(object sender, EventArgs e)
		{
			var currentString = _joinSession.StringValue;
			if (currentString.Length >= 6)
			{
				currentString = currentString.Substring(0, 6);
			}

			_joinSession.StringValue = NumericRegex.Replace(currentString, "");
		}

		private void OnCreateButtonClick(object sender, EventArgs e)
		{
			SwitchToVideoChat(_createSession.StringValue);
		}

		private void OnJoinButtonClick(object sender, EventArgs e)
		{
			SwitchToVideoChat(_joinSession.StringValue);
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
}

