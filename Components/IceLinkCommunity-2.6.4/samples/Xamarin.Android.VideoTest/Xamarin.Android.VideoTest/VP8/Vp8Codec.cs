﻿using System;
using System.Collections.Generic;
using System.Text;
using FM.IceLink.WebRTC;
using FM.IceLink;
using FM;

namespace Xamarin.Android.VideoTest.VP8
{
    public class Vp8Codec : VideoCodec
    {
        private Vp8Padep Padep;
        private Encoder Encoder;
        private Decoder Decoder;

        public Vp8Codec()
        {
            Padep = new Vp8Padep();
        }

        /// <summary>
        /// Encodes a frame.
        /// </summary>
        /// <param name="videoBuffer">The video buffer.</param>
        /// <returns></returns>
        public override byte[] Encode(VideoBuffer videoBuffer)
        {
            if (Encoder == null)
            {
                Encoder = new Encoder();
            }

            // frame -> vp8
            int width, height;
            var rotate = videoBuffer.Rotate;
            if (rotate % 180 == 0)
            {
                width = videoBuffer.Width;
                height = videoBuffer.Height;
            }
            else
            {
                height = videoBuffer.Width;
                width = videoBuffer.Height;
            }
            return Encoder.Encode(width, height, videoBuffer.Plane.Data, LibVpxEnc.FOURCC_NV21, rotate);
        }

        /// <summary>
        /// Decodes an encoded frame.
        /// </summary>
        /// <param name="encodedFrame">The encoded frame.</param>
        /// <returns></returns>
        public override VideoBuffer Decode(byte[] encodedFrame)
        {
            if (Decoder == null)
            {
                Decoder = new Decoder();
            }

            if (Padep.SequenceNumberingViolated)
            {
                Decoder.NeedsKeyFrame = true;
                return null;
            }

            // vp8 -> frame
            var width = 0;
            var height = 0;
            var frame = Decoder.Decode(encodedFrame, out width, out height);
            if (frame == null)
            {
                return null;
            }
            try
            {
                return new VideoBuffer(width, height, new VideoPlane(frame, width));
            }
            catch (Exception ex)
            {
                Log.Error("Could not convert decoded image to video buffer.", ex);
                return null;
            }
        }

        /// <summary>
        /// Gets whether the decoder needs a keyframe. This
        /// is checked after every failed Decode operation.
        /// </summary>
        /// <returns></returns>
        public override bool DecoderNeedsKeyFrame()
        {
            if (Decoder == null)
            {
                return false;
            }
            return Decoder.NeedsKeyFrame;
        }

        /// <summary>
        /// Packetizes an encoded frame.
        /// </summary>
        /// <param name="encodedFrame">The encoded frame.</param>
        /// <returns></returns>
        public override RTPPacket[] Packetize(byte[] encodedFrame)
        {
            return Padep.Packetize(encodedFrame, ClockRate);
        }

        /// <summary>
        /// Depacketizes a packet.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <returns></returns>
        public override byte[] Depacketize(RTPPacket packet)
        {
            return Padep.Depacketize(packet);
        }

        private int LossyCount;
        private int LosslessCount;

        /// <summary>
        /// Processes RTCP packets.
        /// </summary>
        /// <param name="packets">The packets to process.</param>
        public override void ProcessRTCP(RTCPPacket[] packets)
        {
            if (Encoder != null)
            {
                foreach (var packet in packets)
                {
                    if (packet is RTCPPliPacket)
                    {
                        Encoder.ForceKeyframe();
                    }
                    else if (packet is RTCPReportPacket)
                    {
                        var report = (RTCPReportPacket)packet;
                        foreach (var block in report.ReportBlocks)
                        {
                            Log.InfoFormat("VP8 report: {0}% packet loss ({1} cumulative packets lost)", ((int)(block.PercentLost * 100)).ToString(), block.CumulativeNumberOfPacketsLost.ToString());
                            if (block.PercentLost > 0)
                            {
                                LosslessCount = 0;
                                LossyCount++;
                                if (LossyCount > 5 && Encoder.Quality > 0.0)
                                {
                                    LossyCount = 0;
                                    Encoder.Quality = MathAssistant.Max(0.0, Encoder.Quality - 0.1);
                                    Log.InfoFormat("Decreasing VP8 encoder quality to {0}%.", ((int)(Encoder.Quality * 100)).ToString());
                                }
                            }
                            else
                            {
                                LossyCount = 0;
                                LosslessCount++;
                                if (LosslessCount > 5 && Encoder.Quality < 1.0)
                                {
                                    LosslessCount = 0;
                                    Encoder.Quality = MathAssistant.Min(1.0, Encoder.Quality + 0.1);
                                    Log.InfoFormat("Increasing VP8 encoder quality to {0}%.", ((int)(Encoder.Quality * 100)).ToString());
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Destroys the codec.
        /// </summary>
        public override void Destroy()
        {
            if (Encoder != null)
            {
                Encoder.Destroy();
                Encoder = null;
            }

            if (Decoder != null)
            {
                Decoder.Destroy();
                Decoder = null;
            }
        }
    }
}