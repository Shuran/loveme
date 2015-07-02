Let's build a video chat that loops back on itself. We need to:

1. Capture audio and video from the camera and microphone.
2. Show a live preview of the video.
3. Create a P2P connection between two in-memory clients.
4. Play the incoming audio from the network.
5. Display the incoming video from the network.

## Codecs

IceLink lets you use any audio or video codec. Out-of-the-box support is provided for VP8 (video), Opus (audio), and G.711 (audio, also known as PCMU/PCMA). 

Let's use the VP8 video codec:

    VideoStream.RegisterCodec("VP8", () =>
    {
        return new Vp8Codec();
    }, true);

... and the Opus audio codec:

    AudioStream.RegisterCodec("opus", 48000, 2, () =>
    {
        return new OpusCodec();
    }, true);

## Local Media

We need two local media streams - one for the sender and one for the receiver.

The sender will be capturing local audio and video:

    LocalMediaStream senderLocalMedia = null;
    Object senderLocalVideoControl = null;

    UserMedia.GetMedia(new GetMediaArgs(true, true)
    {
        OnFailure = (e) =>
        {
			Log.Error("Could not get sender local media.", e.Exception);
        },
        OnSuccess = (e) =>
        {
            senderLocalMedia = e.LocalStream;
            senderLocalVideoControl = e.LocalVideoControl;
        }
    });

(Note that we are keeping a reference to something called a "local video control". We'll come back to this later.)

The receiver won't be capturing local audio or video (but we still need a local media stream instance):

    LocalMediaStream receiverLocalMedia = null;

    UserMedia.GetMedia(new GetMediaArgs(false, false)
    {
        OnFailure = (e) =>
        {
			Log.Error("Could not get receiver local media.", e.Exception);
        },
        OnSuccess = (e) =>
        {
            receiverLocalMedia = e.LocalStream;
        }
    });

## Stream Descriptions

Next, we need to describe the data we intend to send or receive over the P2P connections. The sender and receiver need to agree, so we need to describe the streams for both sides.

First, let's describe the streams we want to send:

	var senderAudioStream = new AudioStream(SenderLocalMedia);
    var senderVideoStream = new VideoStream(SenderLocalMedia);

... and then describe the streams we want to receive:

    var receiverAudioStream = new AudioStream(ReceiverLocalMedia);
    var receiverVideoStream = new VideoStream(ReceiverLocalMedia);

## Conference

The Conference is what pulls everything together. You can think of a conference as a client of sorts. The sending side needs a conference:

    var sender = new Conference(new Stream[]
    {
        senderAudioStream,
        senderVideoStream
    });

... as does the receiving side:

    var receiver = new Conference(new Stream[]
    {
        receiverAudioStream,
        receiverVideoStream
    });

## Signalling

Now comes a short lesson. In order to create a P2P connection, the two sides have to exchange some information - an "offer", an "answer", and a few "candidates". One side will generate an "offer", the other side will respond with an "answer", and both sides will generate "candidates". The process of passing these messages between the two peers is called **signalling**.

In-memory signalling is trivial. When the sender generates an offer/answer or candidate, it simply passes it to the receiver:

    sender.OnLinkOfferAnswer += (e) =>
    {
        receiver.ReceiveOfferAnswer(e.OfferAnswer, e.PeerId);
    };
    sender.OnLinkCandidate += (e) =>
    {
        receiver.ReceiveCandidate(e.Candidate, e.PeerId);
    };

... and the receiver does the same in reverse:

    receiver.OnLinkOfferAnswer += (e) =>
    {
        sender.ReceiveOfferAnswer(e.OfferAnswer, e.PeerId);
    };
    receiver.OnLinkCandidate += (e) =>
    {
        sender.ReceiveCandidate(e.Candidate, e.PeerId);
    };

In most applications, you would use a real-time messaging system (like XMPP or WebSync) to exchange this data since the clients wouldn't exist in the same process. Both the OfferAnswer and Candidate classes include ToJson/FromJson serialization helpers to make this easier.

## Link

We are ready to go! One side simply needs to call the other:

    sender.Link("receiver");

Let's run it!

## Local Video Control

But wait! We hear audio, but we don't see any video!

What gives?

It's because UI decisions are best left to you. IceLink automatically creates controls to display the live video feeds, but it's up to you to add them into your user interface.

Remember that "local video control" that we kept a reference to earlier? It's time to use it. On iOS, the local video control is actually a UIView, and on Android, the local video control is actually a View:

	// iOS
    DispatchQueue.MainQueue.DispatchAsync(() =>
    {
        var control = (UIView)localVideoControl;
        var width = SenderContainer.Frame.Width;
        var height = SenderContainer.Frame.Height;
        control.Frame = new RectangleF(0, 0, width, height);
        SenderContainer.AddSubview(control);
    });

    // Android
    RunOnUiThread(new Action(() =>
    {
        var control = (View)localVideoControl;
        SenderContainer.AddView(control,
			new RelativeLayout.LayoutParams(
                RelativeLayout.LayoutParams.MatchParent,
                RelativeLayout.LayoutParams.MatchParent));
    }));

(Note that you can add the control anywhere in the view hierachy. For simplicity, we assume that an empty "SenderContainer" exists somewhere in your interface definition.)

Let's run it again!

## Remote Video Controls

Hmmm... Now we can see the local sender preview, but what about the video that's being sent over the network to the receiver?

Again, IceLink creates a control to display the live video feed, but leaves the decision about where to put it to you.

We can get a reference to the "remote video control" as soon as a new link is initializing. Let's add an event handler to the OnLinkInit event of the "receiverVideoStream" we created earlier:

    receiverVideoStream.OnLinkInit += (e) =>
    {
        // iOS
        DispatchQueue.MainQueue.DispatchAsync(() =>
        {
            var control = (UIView)e.Link.GetRemoteVideoControl();
	        var width = ReceiverContainer.Frame.Width;
	        var height = ReceiverContainer.Frame.Height;
	        control.Frame = new RectangleF(0, 0, width, height);
	        ReceiverContainer.AddSubview(control);
        });

		// Android
        RunOnUiThread(new Action(() =>
        {
            var control = (View)e.Link.GetRemoteVideoControl();
	        ReceiverContainer.AddView(control,
				new RelativeLayout.LayoutParams(
	                RelativeLayout.LayoutParams.MatchParent,
	                RelativeLayout.LayoutParams.MatchParent));
        }));
    };

Likewise, to remove the control from the UI when the link goes down, we add an event handler to the OnLinkDown event:

    receiverVideoStream.OnLinkDown += (e) =>
    {
        DispatchQueue.MainQueue.DispatchAsync(() =>
        {
			// iOS
            var control = (UIView)e.Link.GetRemoteVideoControl();
            control.RemoveFromSuperview();

			// Android
            var control = (View)e.Link.GetRemoteVideoControl();
			((ViewGroup)control.getParent()).removeView(control);
        });
    };

(Again, note that you can add the control anywhere in the view hierachy. For simplicity, we assume that an empty "ReceiverContainer" exists somewhere in your interface definition.)

Let's run it again!

## W00t!

Now we're cooking!

If all went well, you should have an audio loop (probably with some feedback) as well as a video loop.

The local video preview should be pretty crisp and snappy, and the remote video should be slightly lower quality (it's been compressed, after all) and perhaps a few milliseconds behind.

## Resources

* [Frozen Mountain Software](http://www.frozenmountain.com/)
* [IceLink Documentation](http://docs.frozenmountain.com/icelink2/index.html)
* [IceLink Downloads](http://www.frozenmountain.com/downloads#icelink)
* [IceLink Feature Requests](http://support.frozenmountain.com/hc/communities/public/topics/200090149-IceLink-Feature-Requests)
* [IceLink Platform Support](http://www.frozenmountain.com/products/icelink/platforms)
* [IceLink Support](http://support.frozenmountain.com/hc/communities/public/topics/200090139-IceLink-Support)