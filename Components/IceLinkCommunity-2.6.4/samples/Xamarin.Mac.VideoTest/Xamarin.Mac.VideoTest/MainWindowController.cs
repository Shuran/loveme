
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreGraphics;
using ObjCRuntime;
using FM;
using FM.IceLink;
using FM.IceLink.WebRTC;
using CoreFoundation;
using System.Drawing;
using Xamarin.Mac.VideoTest.Opus;
using Xamarin.Mac.VideoTest.VP8;

[assembly: RequiredFramework("lib/FMAudio.dylib")]
[assembly: RequiredFramework("lib/FMVideo.dylib")]

namespace Xamarin.Mac.VideoTest
{
	public delegate void WindowWillCloseDelegate(NSNotification notification);

	public class MainWindowDelegate : NSWindowDelegate
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

	public partial class MainWindowController : NSWindowController
	{
		private LocalMediaStream LocalMediaReceiver;
		private LocalMediaStream LocalMediaSender;

		private Conference Sender = null;
		private Conference Receiver = null;

		private MainWindowDelegate WindowDelegate = null;

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
			WindowDelegate = new MainWindowDelegate();
			WindowDelegate.WindowWillClose += WindowWillClose;
			Window.Delegate = WindowDelegate;

			// Log to the console.
			Log.Provider = new NSLogProvider(LogLevel.Info);

			// WebRTC has chosen VP8 as its mandatory video codec.
			// Since video encoding is best done using native code,
			// reference the video codec at the application-level.
			// This is required when using a WebRTC video stream.
			VideoStream.RegisterCodec("VP8", () =>
			{
				return new Vp8Codec();
			}, true);

			// For improved audio quality, we can use Opus. By
			// setting it as the preferred audio codec, it will
			// override the default PCMU/PCMA codecs.
			AudioStream.RegisterCodec("opus", 48000, 2, () =>
			{
				return new OpusCodec();
			}, true);

			StartReceiverMedia();
		}

		private void StartReceiverMedia()
		{
			// Get a blank local media stream. This first
			// conference will be receive-only.
			UserMedia.GetMedia(new GetMediaArgs(false, false)
			{
				OnFailure = (e) =>
				{
					Alert("Could not get media. {0}", e.Exception.Message);
				},
				OnSuccess = (e) =>
				{
					LocalMediaReceiver = e.LocalStream;

					StartReceiverConference();
				}
			});
		}

		private void StartReceiverConference()
		{
			// Create our video stream template for peer connections
			// and attach events for when we're connected to the peer.
			VideoStream videoStream = new VideoStream(LocalMediaReceiver);
			videoStream.OnLinkInit += (e) =>
			{
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					var remoteVideoControl = (NSView)e.Link.GetRemoteVideoControl();
					if (remoteVideoControl != null)
					{
						var frame = remoteVideoControl.Frame;
						var frameSize = frame.Size;
						frameSize.Width = containerRemote.Frame.Size.Width;
						frameSize.Height = containerRemote.Frame.Size.Height;
						remoteVideoControl.Frame = new CGRect(frame.Location, frameSize);
						containerRemote.AutoresizesSubviews = true;
						containerRemote.AddSubview(remoteVideoControl);
					}
				});
			};
			videoStream.OnLinkDown += (e) =>
			{
				DispatchQueue.MainQueue.DispatchAsync(() =>
				{
					var remoteVideoControl = (NSView)e.Link.GetRemoteVideoControl();
					remoteVideoControl.RemoveFromSuperview();
				});
			};

			// Create a new IceLink conference.
			Receiver = new Conference(new Stream[]
            {
                new AudioStream(LocalMediaReceiver),
                videoStream
            });
            
            Receiver.OnLinkInit += (e) =>
            {
                Log.Info("Link to sender is initializing...");
            };
            Receiver.OnLinkUp += (e) =>
            {
                Log.Info("Link to sender is UP.");
            };
            Receiver.OnLinkDown += (e) =>
            {
                Log.Info("Link to sender is DOWN.", e.Exception);
            };

			// In-memory signalling.
			Receiver.OnLinkOfferAnswer += (e) =>
			{
				Sender.ReceiveOfferAnswer(e.OfferAnswer, e.PeerId);
			};
			Receiver.OnLinkCandidate += (e) =>
			{
				Sender.ReceiveCandidate(e.Candidate, e.PeerId);
			};

			StartSenderMedia();
		}

		private void StartSenderMedia()
		{
			// Get a video-based local media stream. This
			// conference will be send-only.
			UserMedia.GetMedia(new GetMediaArgs(true, true)
			{
				VideoWidth = 640,    // optional
				VideoHeight = 480,   // optional
				VideoFrameRate = 30, // optional
				OnFailure = (e) =>
				{
					Alert("Could not get media. {0}", e.Exception.Message);
				},
				OnSuccess = (e) =>
				{
					LocalMediaSender = e.LocalStream;

					StartSenderConference((NSView)e.LocalVideoControl);
				}
			});
		}

		private void StartSenderConference(NSView localVideoControl)
		{
			// Add the local preview to the left container.
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				if (localVideoControl != null)
				{
					var frame = localVideoControl.Frame;
					var frameSize = frame.Size;
					frameSize.Width = containerLocal.Frame.Size.Width;
					frameSize.Height = containerLocal.Frame.Size.Height;
					localVideoControl.Frame = new CGRect(frame.Location, frameSize);
					containerLocal.AutoresizesSubviews = true;
					containerLocal.AddSubview(localVideoControl);
				}
			});

			// Create a new IceLink conference.
			Sender = new Conference(new Stream[]
			{
				new AudioStream(LocalMediaSender),
				new VideoStream(LocalMediaSender)
			});

            Sender.OnLinkInit += (e) =>
            {
                Log.Info("Link to receiver is initializing...");
            };
            Sender.OnLinkUp += (e) =>
            {
                Log.Info("Link to receiver is UP.");
            };
            Sender.OnLinkDown += (e) =>
            {
                Log.Info("Link to receiver is DOWN.", e.Exception);
            };

			// In-memory signalling.
			Sender.OnLinkOfferAnswer += (e) =>
			{
				Receiver.ReceiveOfferAnswer(e.OfferAnswer, e.PeerId);
			};
			Sender.OnLinkCandidate += (e) =>
			{
				Receiver.ReceiveCandidate(e.Candidate, e.PeerId);
			};

			// Kick things off.
			Sender.Link("remote");
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
			
		public void WindowWillClose(NSNotification notification)
		{
			// Stop automatically when window closes.
			LocalMediaReceiver.Stop();
			LocalMediaSender.Stop();
			Receiver.UnlinkAll();
			Sender.UnlinkAll();

			// Terminate the application when the window closes.
			NSApplication.SharedApplication.Terminate(this);
		}

		//strongly typed window accessor
		public new NSWindow Window
		{
			get
			{
				return (NSWindow)base.Window;
			}
		}
	}
}

