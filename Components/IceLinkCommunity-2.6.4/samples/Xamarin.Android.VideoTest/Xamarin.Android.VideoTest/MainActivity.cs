using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;

using FM;
using FM.IceLink;
using FM.IceLink.WebRTC;

using Xamarin.Android.VideoTest.Opus;
using Xamarin.Android.VideoTest.VP8;

namespace Xamarin.Android.VideoTest
{
    [Activity(Label = "IceLink Video Test", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private LinearLayout Layout;
        private static RelativeLayout ContainerLocal;
        private static RelativeLayout ContainerRemote;

        private LocalMediaStream LocalMediaReceiver;
        private LocalMediaStream LocalMediaSender;

        private Conference Sender = null;
		private Conference Receiver = null;

        private static bool EnableEchoCancellation = true; // experimental
		private static OpusEchoCanceller OpusEchoCanceller = null;
		private static int OpusClockRate = 48000;
		private static int OpusChannels = 2;

        static MainActivity()
        {
            // Log to the console.
            Log.Provider = new AndroidLogProvider(LogLevel.Info);

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
			AudioStream.RegisterCodec("opus", OpusClockRate, OpusChannels, () =>
            {
                if (EnableEchoCancellation)
                {
                    return new OpusCodec(OpusEchoCanceller);
                }
                else
                {
                    return new OpusCodec();
                }
            }, true);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

			Java.Lang.JavaSystem.LoadLibrary("audioprocessing");
			Java.Lang.JavaSystem.LoadLibrary("audioprocessingJNI");
            Java.Lang.JavaSystem.LoadLibrary("opus");
            Java.Lang.JavaSystem.LoadLibrary("opusJNI");
            Java.Lang.JavaSystem.LoadLibrary("vpx");
            Java.Lang.JavaSystem.LoadLibrary("vpxJNI");

            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.AddFlags(WindowManagerFlags.KeepScreenOn);
            //Window.AddFlags(WindowManagerFlags.Fullscreen);

            SetContentView(Resource.Layout.Main);

            // Get the activity's layout and give it a black background.
            Layout = (LinearLayout)FindViewById(Resource.Id.Layout);
            Layout.RootView.SetBackgroundColor(Color.Black);

            // Create a static container that will be preserved across
            // activity destruction/recreation.
            if (ContainerLocal == null)
            {
                ContainerLocal = new RelativeLayout(ApplicationContext);
                ContainerLocal.LayoutParameters = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.MatchParent, 1);

                ContainerRemote = new RelativeLayout(ApplicationContext);
                ContainerRemote.LayoutParameters = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.MatchParent, 1);
            }

            // Android's video providers need a context
            // in order to create surface views for video
            // rendering, so we need to supply one before
            // we start up the local media.
            DefaultProviders.AndroidContext = this;

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
                RunOnUiThread(new Action(() =>
                {
                    var remoteVideoControl = (View)e.Link.GetRemoteVideoControl();
                    ContainerRemote.AddView(remoteVideoControl, new RelativeLayout.LayoutParams(
                            RelativeLayout.LayoutParams.MatchParent,
                            RelativeLayout.LayoutParams.MatchParent));
                }));
            };
            videoStream.OnLinkDown += (e) =>
            {
                RunOnUiThread(new Action(() =>
                {
                    var remoteVideoControl = (View)e.Link.GetRemoteVideoControl();
                    ContainerRemote.RemoveView(remoteVideoControl);
                }));
            };

            // Create a new IceLink conference.
			Receiver = new Conference(new FM.IceLink.Stream[]
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

                    StartSenderConference((View)e.LocalVideoControl);
                }
            });
        }

        private void StartSenderConference(View localVideoControl)
        {
            // Add the local preview to the left container.
            RunOnUiThread(new Action(() =>
            {
                ContainerLocal.AddView(localVideoControl, new RelativeLayout.LayoutParams(
                        RelativeLayout.LayoutParams.MatchParent,
                        RelativeLayout.LayoutParams.MatchParent));
            }));

            // Create a new IceLink conference.
			Sender = new Conference(new FM.IceLink.Stream[]
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

			// Start echo canceller.
            if (EnableEchoCancellation)
            {
                OpusEchoCanceller = new OpusEchoCanceller(OpusClockRate, OpusChannels);
                OpusEchoCanceller.Start();
            }

            // Kick things off.
            Sender.Link("remote");
        }

        protected override void OnPause()
        {
            if (LocalMediaSender != null)
            {
                // Pause the local video feed.
                LocalMediaSender.PauseVideo();
            }

            // Remove the static container from the current layout.
            Layout.RemoveView(ContainerRemote);
            Layout.RemoveView(ContainerLocal);

            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();

            // Add the static container to the current layout.
            Layout.AddView(ContainerLocal);
            Layout.AddView(ContainerRemote);
            
            if (LocalMediaSender != null)
            {
                // Resume the local video feed.
                LocalMediaSender.ResumeVideo();
            }
        }

        private void Alert(string format, params object[] args)
        {
            RunOnUiThread(() =>
            {
                if (!IsFinishing)
                {
                    var alert = new AlertDialog.Builder(this);
                    alert.SetMessage(string.Format(format, args));
                    alert.SetPositiveButton("OK", (sender, e) => { });
                    alert.Show();
                }
            });
        }
    }
}

