using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CoreFoundation;
using Foundation;
using AppKit;

using FM;
using FM.IceLink.WebRTC;
using AVFoundation;

namespace Xamarin.Mac.Conference.WebRTC
{
    class LocalMedia
    {
        // We're going to need both audio and video
        // for this example. We can constrain the
        // video slightly for performance benefits.
        private bool Audio = true;
        private bool Video = true;
        private int  VideoWidth = 640;
        private int  VideoHeight = 480;
        private int  VideoFrameRate = 30;

        public LocalMediaStream LocalMediaStream { get; private set; }
        public LayoutManager LayoutManager { get; private set; }

		private MainWindowController VideoChat;

		public void Start(MainWindowController videoChat, Action<string> callback)
        {
            UserMedia.GetMedia(new GetMediaArgs(Audio, Video)
            {
                VideoWidth = VideoWidth,           // optional
                VideoHeight = VideoHeight,         // optional
                VideoFrameRate = VideoFrameRate,   // optional
                DefaultVideoPreviewScale = LayoutScale.Contain, // optional
                DefaultVideoScale = LayoutScale.Contain,        // optional
                OnFailure = (e) =>
                {
                    callback(string.Format("Could not get media. {0}", e.Exception.Message));
                },
                OnSuccess = (e) =>
                {
                    // We have successfully acquired access to the local
                    // audio/video device! Grab a reference to the media.
                    // Internally, it maintains access to the local audio
                    // and video feeds coming from the device hardware.
					LocalMediaStream = e.LocalStream;

					// Wire up the UI.
					VideoChat = videoChat;
					DispatchQueue.MainQueue.DispatchAsync(() =>
					{
						VideoChat.AudioDevices.AddItems(LocalMediaStream.GetAudioDeviceNames());
						VideoChat.VideoDevices.AddItems(LocalMediaStream.GetVideoDeviceNames());
						VideoChat.AudioDevices.Activated += SwitchAudioDevice;
						VideoChat.VideoDevices.Activated += SwitchVideoDevice;
						VideoChat.AudioDevices.SelectItem(VideoChat.AudioDevices.ItemAtIndex(LocalMediaStream.GetAudioDeviceNumber()));
						videoChat.VideoDevices.SelectItem(videoChat.VideoDevices.ItemAtIndex(LocalMediaStream.GetVideoDeviceNumber()));
					});

					// Keep the UI updated if devices are switched.
					LocalMediaStream.OnAudioDeviceNumberChanged += UpdateSelectedAudioDevice;
					LocalMediaStream.OnVideoDeviceNumberChanged += UpdateSelectedVideoDevice;

                    // This is our local video control, a UIView (iOS) or
                    // and NSView (Mac). It is constantly updated with our
                    // live video feed since we requested video above. Add
                    // it directly to the UI or use the IceLink layout manager,
                    // which we do below.
                    var localVideoControl = (NSView)e.LocalVideoControl;

                    // Create an IceLink layout manager, which makes the task
                    // of arranging video controls easy. Give it a reference
                    // to a UIView that can be filled with video feeds.
					LayoutManager = new LayoutManager(videoChat.View);

                    // Position and display the local video control on-screen
                    // by passing it to the layout manager created above.
                    LayoutManager.SetLocalVideoControl(localVideoControl);

                    callback(null);
                }
            });
		}

		private void SwitchAudioDevice(object sender, EventArgs e)
		{
			if (VideoChat.AudioDevices.IndexOfSelectedItem >= 0)
			{
				LocalMediaStream.SetAudioDeviceNumber((int)VideoChat.AudioDevices.IndexOfSelectedItem);
			}
		}

		private void SwitchVideoDevice(object sender, EventArgs e)
		{
			if (VideoChat.VideoDevices.IndexOfSelectedItem >= 0)
			{
				LocalMediaStream.SetVideoDeviceNumber((int)VideoChat.VideoDevices.IndexOfSelectedItem);
			}
		}

		private void UpdateSelectedAudioDevice(AudioDeviceNumberChangedArgs e)
		{
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				VideoChat.AudioDevices.SelectItem(VideoChat.AudioDevices.ItemAtIndex(e.DeviceNumber));
			});
		}

		private void UpdateSelectedVideoDevice(VideoDeviceNumberChangedArgs e)
		{
			DispatchQueue.MainQueue.DispatchAsync(() =>
			{
				VideoChat.VideoDevices.SelectItem(VideoChat.VideoDevices.ItemAtIndex(e.DeviceNumber));
			});
		}

        public void Stop(Action<string> callback)
        {
            // Clear out the layout manager.
            if (LayoutManager != null)
            {
                LayoutManager.UnsetLocalVideoControl();
                LayoutManager.RemoveRemoteVideoControls();
                LayoutManager = null;
			}

			if (LocalMediaStream != null)
			{
				LocalMediaStream.OnAudioDeviceNumberChanged -= UpdateSelectedAudioDevice;
				LocalMediaStream.OnVideoDeviceNumberChanged -= UpdateSelectedVideoDevice;
			}

			if (VideoChat != null)
			{
				VideoChat.AudioDevices.Activated -= SwitchAudioDevice;
				VideoChat.VideoDevices.Activated -= SwitchVideoDevice;
				VideoChat = null;
			}

            // Stop the local media stream.
            if (LocalMediaStream != null)
            {
                LocalMediaStream.Stop();
                LocalMediaStream = null;
            }

            callback(null);
        }
    }
}