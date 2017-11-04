using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using AppKit;
using AVFoundation;
using CoreFoundation;
using CoreGraphics;
using static ImageMusic.SynthHelper;
using System.Runtime.InteropServices;

namespace ImageMusic
{
    public delegate void NotePlayedForColor(NSColor color);

    public class Synth
    {
        public event NotePlayedForColor NotePlayedForColor;

        const int InFlightAudioBuffers = 2;
        const int SamplesPerBuffer = 1000000000;

        FrequencyDictionary FrequenciesByOctave;
        List<int> AllFrequencies;

        AVAudioEngine AudioEngine = new AVAudioEngine();
        AVAudioPlayerNode PlayerNode = new AVAudioPlayerNode();
        AVAudioFormat AudioFormat = new AVAudioFormat(44100, 2);
        List<AVAudioPcmBuffer> AudioBuffers = new List<AVAudioPcmBuffer>();
        int BufferIndex;
        DispatchQueue AudioQueue = new DispatchQueue("AudioQueue", false);
        SemaphoreSlim Semaphore = new SemaphoreSlim(InFlightAudioBuffers);

        #region Initialization

        public void Init()
        {
            InitFrequencies();

            for (int i = 0; i < InFlightAudioBuffers; i++)
            {
                var buffer = new AVAudioPcmBuffer(AudioFormat, SamplesPerBuffer);
                AudioBuffers.Add(buffer);
            }

            AudioEngine.AttachNode(PlayerNode);
            AudioEngine.Connect(PlayerNode, AudioEngine.MainMixerNode, AudioFormat);
            AudioEngine.StartAndReturnError(out var error);
        }

        void InitFrequencies()
        {
            FrequenciesByOctave = new FrequencyDictionary();

            AllFrequencies = new List<int>();
            foreach (var fq in FrequenciesByOctave.Values)
            {
                AllFrequencies.AddRange(fq);
            }
        }

        #endregion

        public void PlayImageInScale(NSImage image, Scale scale)
        {
            if (image.Size.Width > 150 || image.Size.Height > 150)
            {
                image = ImageHelpers.ResizeImage(image, new CGSize(150, 150));
            }

            using (var imageRepresentation = new NSBitmapImageRep(image.AsTiff()))
            {
                var imageSize = image.Size;

                for (var x = 0; x < imageSize.Width; x++)
                {
                    for (var y = 0; y < imageSize.Height; y++)
                    {
                        var color = imageRepresentation.ColorAt(x, y);

                        var carrierFrequency = GetCarrierFrequencyForColor(color);
                        var modulatorFrequency = GetModulatorFrequencyForColor(color);
                        var sampleLength = GetNoteLengthForColor(color);
                        var octave = GetOctaveForColor(color);
                        var pan = GetPanForColor(color);

                        var closestOctave = FrequenciesByOctave.Keys.Aggregate((a, b) => Math.Abs(a - octave) < Math.Abs(b - octave) ? a : b);

                        var closestCarrier = ScaleHelper.GetNotesForScale(scale, closestOctave).Aggregate(
                            (a, b) => Math.Abs(a - carrierFrequency) < Math.Abs(b - carrierFrequency) ? a : b);

                        var closestModulator = ScaleHelper.GetNotesForScale(scale, closestOctave).Aggregate(
                            (a, b) => Math.Abs(a - modulatorFrequency) < Math.Abs(b - modulatorFrequency) ? a : b);

                        PlayToneForColor(closestCarrier, closestModulator, sampleLength, pan, color);
                    }
                }
            }
        }

        unsafe void PlayToneForColor(float carrierFrequency, float modulatorFrequency, uint sampleLength, float pan, NSColor color)
        {
            const float modulatorAmplitude = .8f;

            var unitVelocity = 2 * Math.PI / AudioFormat.SampleRate;
            var carrierVelocity = carrierFrequency * unitVelocity;
            var modulatorVelocity = modulatorFrequency * unitVelocity;

            AudioQueue.DispatchAsync(() =>
            {
                var sampleTime = 0f;

                Semaphore.Wait();

                var outChannels = AudioFormat.ChannelCount;
                var outDataPointers = new float*[outChannels];

                var buffer = AudioBuffers[BufferIndex];

                for (int i = 0; i < outChannels; i++)
                {
                    // buffer.FloatChannelData is a native array of pointers to audio data.
                    // convert that into a managed array of pointers to audio data.
                    outDataPointers[i] = (float*)Marshal.ReadIntPtr(buffer.FloatChannelData, i * IntPtr.Size);
                }

                var leftChannel = outDataPointers[0];
                var rightChannel = outDataPointers[1];

                for (int sampleIndex = 0; sampleIndex < sampleLength; sampleIndex++)
                {
                    var sample = (float)Math.Sin(carrierVelocity * sampleTime + modulatorAmplitude *
                                          Math.Sin(modulatorVelocity * sampleTime));

                    leftChannel[sampleIndex] = sample;
                    rightChannel[sampleIndex] = sample;

                    sampleTime++;
                }

                buffer.FrameLength = sampleLength;

                PlayerNode.Pan = pan;

                NotePlayedForColor?.Invoke(color);
                PlayerNode.Play();

                PlayerNode.ScheduleBuffer(buffer, () =>
                {
                    Semaphore.Release();
                });

                BufferIndex = (BufferIndex + 1) % AudioBuffers.Count;
            });
        }
    }
}