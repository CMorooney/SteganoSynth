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
        const int SamplesPerBuffer = 1024;

        AVAudioEngine AudioEngine = new AVAudioEngine();
        AVAudioPlayerNode PlayerNode = new AVAudioPlayerNode();
        AVAudioFormat AudioFormat = new AVAudioFormat(44100, 2);
        List<AVAudioPcmBuffer> AudioBuffers = new List<AVAudioPcmBuffer>();
        int BufferIndex;
        DispatchQueue AudioQueue = new DispatchQueue("AudioQueue", false);
        SemaphoreSlim Semaphore = new SemaphoreSlim(InFlightAudioBuffers);

        // key: octave // value: frequences
        // 8 octaves, 12 frequencies per octave
        Dictionary<int, int[]> FrequenciesByOctave;
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
                        var modulatorFrequency = (float)color.BlueComponent * 1000;
                        var octave = (float)color.GreenComponent * 10;

                        var closestOctave = FrequenciesByOctave.Keys.Aggregate((a, b) => Math.Abs(a - octave) < Math.Abs(b - octave) ? a : b);
                        var closestCarrier = FrequenciesByOctave[closestOctave].Aggregate((a, b) => Math.Abs(a - carrierFrequency) < Math.Abs(b - carrierFrequency) ? a : b);

                        PlayTone(closestCarrier, color, modulatorFrequency);
                    }
                }
            }
        }

        unsafe void PlayTone(float carrierFrequency, NSColor color, float modulatorFrequency = 679f, float modulatorAmplitude = .8f)
        {
            var unitVelocity = 2 * Math.PI / AudioFormat.SampleRate;
            var carrierVelocity = carrierFrequency * unitVelocity;
            var modulatorVelocity = modulatorFrequency * unitVelocity;

            AudioQueue.DispatchAsync(() =>
            {
                var sampleTime = 0f;

                Semaphore.Wait();

                Console.WriteLine($"PLAYING: {carrierFrequency}");

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

                for (int sampleIndex = 0; sampleIndex < SamplesPerBuffer; sampleIndex++)
                {
                    var sample = (float)Math.Sin(carrierVelocity * sampleTime + modulatorAmplitude *
                                          Math.Sin(modulatorVelocity * sampleTime));

                    leftChannel[sampleIndex] = sample;
                    rightChannel[sampleIndex] = sample;

                    sampleTime++;
                }

                buffer.FrameLength = SamplesPerBuffer;

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
            FrequenciesByOctave = new Dictionary<int, int[]>
            {
                { 0, new [] { 16, 17, 18, 20, 21, 22, 23, 25, 26, 28, 29, 31 } },
                { 1, new [] { 33, 35, 37, 39, 41, 44, 46, 49, 52, 55, 58, 62 } },
                { 2, new [] { 65, 69, 73, 78, 82, 87, 93, 98, 104, 110, 117, 124 } },
                { 3, new [] { 131, 139, 147, 156, 165, 175, 185, 196, 208, 220, 233, 247 } },
                { 4, new [] { 262, 278, 294, 311, 330, 349, 370, 392, 415, 440, 466, 494 } },
                { 5, new [] { 523, 554, 587, 622, 659, 699, 740, 784, 831, 880, 932, 988 } },
                { 6, new [] { 1047, 1109, 1175, 1245, 1319, 1397, 1475, 1568, 1661, 1760, 1865, 1976 } },
                { 7, new [] { 2093, 2218, 2349, 2489, 2637, 2794, 2960, 3136, 3322, 3520, 3729, 3951 } },
                { 8, new [] { 4186, 4435, 4699, 4978, 5274, 5588, 5920, 6272, 6645, 7040, 7459, 7902 } }
            };

            AllFrequencies = new List<int>();
            foreach (var fq in FrequenciesByOctave.Values)
            {
                AllFrequencies.AddRange(fq);
            }
        }
    }
}