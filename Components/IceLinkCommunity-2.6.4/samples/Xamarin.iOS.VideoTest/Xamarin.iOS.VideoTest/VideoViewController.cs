using System;
using System.Drawing;

using CoreFoundation;
using CoreGraphics;
using UIKit;
using Foundation;

using FM;
using FM.IceLink;
using FM.IceLink.WebRTC;

using AVFoundation;

using Xamarin.iOS.VideoTest.VP8;
using Xamarin.iOS.VideoTest.Opus;

namespace Xamarin.iOS.VideoTest
{
    public partial class VideoViewController : UIViewController
    {
        private LocalMediaStream LocalMediaReceiver;
        private LocalMediaStream LocalMediaSender;

        private Conference Sender = null;
        private Conference Receiver = null;

        public VideoViewController()
            : base("VideoViewController", null)
        { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            InitializeController();

            AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.PlayAndRecord,
                AVAudioSessionCategoryOptions.MixWithOthers |
                AVAudioSessionCategoryOptions.AllowBluetooth |
                AVAudioSessionCategoryOptions.DefaultToSpeaker);

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
                    var remoteVideoControl = (UIView)e.Link.GetRemoteVideoControl();
					if (remoteVideoControl != null)
					{
	                    var frame = remoteVideoControl.Frame;
	                    var frameSize = frame.Size;
	                    frameSize.Width = ContainerRemote.Frame.Size.Width;
	                    frameSize.Height = ContainerRemote.Frame.Size.Height;
	                    remoteVideoControl.Frame = new CGRect(frame.Location, frameSize);
	                    ContainerRemote.AutosizesSubviews = true;
	                    ContainerRemote.AddSubview(remoteVideoControl);
					}
                });
            };
            videoStream.OnLinkDown += (e) =>
            {
                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    var remoteVideoControl = (UIView)e.Link.GetRemoteVideoControl();
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
                VideoFrameRate = 15, // optional
                OnFailure = (e) =>
                {
                    Alert("Could not get media. {0}", e.Exception.Message);
                },
                OnSuccess = (e) =>
                {
                    LocalMediaSender = e.LocalStream;

                    StartSenderConference((UIView)e.LocalVideoControl);
                }
            });
        }

        private void StartSenderConference(UIView localVideoControl)
        {
            // Add the local preview to the left container.
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
				if (localVideoControl != null)
				{
	                var frame = localVideoControl.Frame;
	                var frameSize = frame.Size;
	                frameSize.Width = ContainerLocal.Frame.Size.Width;
	                frameSize.Height = ContainerLocal.Frame.Size.Height;
	                localVideoControl.Frame = new CGRect(frame.Location, frameSize);
	                ContainerLocal.AutosizesSubviews = true;
	                ContainerLocal.AddSubview(localVideoControl);
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
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                new UIAlertView("Alert", string.Format(format, args), null, "OK").Show();
            });
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            // Disconnect automatically when view disappears.
            LocalMediaReceiver.Stop();
            LocalMediaSender.Stop();
            Receiver.UnlinkAll();
            Sender.UnlinkAll();
        }

        private void InitializeController()
        {
            // Hide the status bar to give us more screen real estate.
            // Also disable the idle timer since there isn't much touch
            // screen interaction during a video chat.
            UIApplication.SharedApplication.StatusBarHidden = true;
            UIApplication.SharedApplication.IdleTimerDisabled = true;
        }
    }
}