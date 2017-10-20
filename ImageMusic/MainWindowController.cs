using System;
using Foundation;
using AppKit;
using AVFoundation;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using CoreFoundation;
using System.Threading;
using System.Linq;

namespace ImageMusic
{
    public partial class MainWindowController : NSWindowController
    {
        NSImage ChosenImage;

        const int InFlightAudioBuffers = 2;
        const int SamplesPerBuffer = 1000000000;

        AVAudioEngine AudioEngine = new AVAudioEngine();
        AVAudioPlayerNode PlayerNode = new AVAudioPlayerNode();
        AVAudioFormat AudioFormat = new AVAudioFormat(44100, 2);
        List<AVAudioPcmBuffer> AudioBuffers = new List<AVAudioPcmBuffer>();
        int BufferIndex;
        DispatchQueue AudioQueue = new DispatchQueue("AudioQueue", false);
        SemaphoreSlim Semaphore = new SemaphoreSlim(InFlightAudioBuffers);

        // key: octave // value: frequences
        // 8 octaves, 12 frequencies per octave
        FrequencyDictionary FrequenciesByOctave;
        List<int> AllFrequencies;

        #region Constructors

        public MainWindowController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public MainWindowController(NSCoder coder) : base(coder)
        {
        }

        public MainWindowController() : base("MainWindow")
        {
        }

        #endregion

        void GenerateMusic()
        {
            using (var imageRepresentation = new NSBitmapImageRep(ChosenImage.AsTiff()))
            {
                var imageSize = ChosenImage.Size;

                var totalPixels = imageSize.Width * imageSize.Height;

                ProgressIndicator.MinValue = 0;
                ProgressIndicator.MaxValue = totalPixels;

                for (var x = 0; x < imageSize.Width; x++)
                {
                    for (var y = 0; y < imageSize.Height; y++)
                    {
                        var color = imageRepresentation.ColorAt(x, y);

                        var carrierFrequency = (float)color.RedComponent * 1000;
                        var sampleLength = (uint)(color.BrightnessComponent * 10000)/2;
                        var octave = (float)color.GreenComponent * 10000;

                        var closestOctave = FrequenciesByOctave.Keys.Aggregate((a, b) => Math.Abs(a - octave) < Math.Abs(b - octave) ? a : b);
                        var closestCarrier = AllFrequencies.Aggregate((a, b) => Math.Abs(a - carrierFrequency) < Math.Abs(b - carrierFrequency) ? a : b);

                        PlayTone(closestCarrier, color, sampleLength);
                    }
                }
            }
        }

        unsafe void PlayTone(float carrierFrequency, NSColor color, uint sampleLength)
        {
            const int modulatorFrequency = 697;
            const float modulatorAmplitude = .8f;

            var unitVelocity = 2 * Math.PI / AudioFormat.SampleRate;
            var carrierVelocity = carrierFrequency * unitVelocity;
            var modulatorVelocity = modulatorFrequency * unitVelocity;

            AudioQueue.DispatchAsync(() =>
            {
                var sampleTime = 0f;

                Semaphore.Wait();

                Console.WriteLine($"PLAYING: {carrierFrequency} FOR: {sampleLength}");

                DispatchQueue.MainQueue.DispatchAsync (() =>
                {
                    ColorIndicator.FillColor = color;
                    ProgressIndicator.IncrementBy(1);
                });

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

                PlayerNode.ScheduleBuffer(buffer, () =>
                {
                    Semaphore.Release();
                });

                BufferIndex = (BufferIndex + 1) % AudioBuffers.Count;
            });

            PlayerNode.Play();
        }

        #region Lifecycle

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

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

        #endregion

        #region Event handlers

        partial void ImagePicked(NSImageView sender)
        {
            ChosenImage = sender.Image;
        }

        partial void StartClicked(NSButton sender)
        {
            if (!InputIsValid)
            {
                ShowValidationError();
            }
            else
            {
                ClearErrorMessage();
                GenerateMusic();
            }

            ProgressIndicator.DoubleValue = 0;
        }

        #endregion

        #region Validation and errors

        bool InputIsValid => ChosenImage?.IsValid ?? false;

        void ShowValidationError()
        {
            ErrorLabel.StringValue = "No image chosen";
        }

        void ClearErrorMessage()
        {
            ErrorLabel.StringValue = string.Empty;
        }

        #endregion

        void InitFrequencies()
        {
            FrequenciesByOctave = new FrequencyDictionary();

            AllFrequencies = new List<int>();
            foreach (var fq in FrequenciesByOctave.Values)
            {
                AllFrequencies.AddRange(fq);
            }
        }
    }
}