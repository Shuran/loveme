
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace Xamarin.Mac.Conference.WebRTC
{
	public partial class SessionWindow : AppKit.NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public SessionWindow(IntPtr handle)
			: base(handle)
		{
			Initialize();
		}
		
		// Called when created directly from a XIB file
		[Export("initWithCoder:")]
		public SessionWindow(NSCoder coder)
			: base(coder)
		{
			Initialize();
		}
		
		// Shared initialization code
		void Initialize()
		{
		}

		#endregion
	}
}

