using System;
using System.Threading.Tasks;
using Foundation;
using AppKit;
using AVFoundation;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using CoreFoundation;
using System.Threading;
using System.Linq;
using CoreGraphics;
using static ImageMusic.SynthHelper;

namespace ImageMusic
{
    public partial class MainWindowController : NSWindowController
    {
        NSImage ChosenImage;
        Scale? ChosenScale;

        const int InFlightAudioBuffers = 2;
        const int SamplesPerBuffer = 1000000000;

        AVAudioEngine AudioEngine = new AVAudioEngine();
        AVAudioPlayerNode PlayerNode = new AVAudioPlayerNode();
        AVAudioFormat AudioFormat = new AVAudioFormat(44100, 2);
        List<AVAudioPcmBuffer> AudioBuffers = new List<AVAudioPcmBuffer>();
        int BufferIndex;
        DispatchQueue AudioQueue = new DispatchQueue("AudioQueue", false);
        SemaphoreSlim Semaphore = new SemaphoreSlim(InFlightAudioBuffers);

        FrequencyDictionary FrequenciesByOctave;
        List<int> AllFrequencies;

        NodeEditorWindowController NodeEditorWindowController;

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
            if (ChosenImage.Size.Width > 150 || ChosenImage.Size.Height > 150)
            {
                ChosenImage = ImageHelpers.ResizeImage(ChosenImage, new CGSize(150, 150));
            }

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

                        var carrierFrequency = GetCarrierFrequencyForColor(color);
                        var modulatorFrequency = GetModulatorFrequencyForColor(color);
                        var sampleLength = GetNoteLengthForColor(color);
                        var octave = GetOctaveForColor(color);
                        var pan = GetPanForColor(color);

                        var closestOctave = FrequenciesByOctave.Keys.Aggregate((a, b) => Math.Abs(a - octave) < Math.Abs(b - octave) ? a : b);

                        var closestCarrier = ScaleHelper.GetNotesForScale(ChosenScale.Value, closestOctave).Aggregate(
                            (a, b) => Math.Abs(a - carrierFrequency) < Math.Abs(b - carrierFrequency) ? a : b);

                        var closestModulator = ScaleHelper.GetNotesForScale(ChosenScale.Value, closestOctave).Aggregate(
                            (a, b) => Math.Abs(a - modulatorFrequency) < Math.Abs(b - modulatorFrequency) ? a : b);

                        PlayTone(closestCarrier, closestModulator, sampleLength, pan, color);
                    }
                }
            }
        }

        unsafe void PlayTone(float carrierFrequency, float modulatorFrequency, uint sampleLength, float pan, NSColor color)
        {
            const float modulatorAmplitude = .8f;

            var unitVelocity = 2 * Math.PI / AudioFormat.SampleRate;
            var carrierVelocity = carrierFrequency * unitVelocity;
            var modulatorVelocity = modulatorFrequency * unitVelocity;

            AudioQueue.DispatchAsync(() =>
            {
                var sampleTime = 0f;

                Semaphore.Wait();

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

                Console.WriteLine("PAN: " + pan);
                PlayerNode.Pan = pan;

                PlayerNode.Play();

                PlayerNode.ScheduleBuffer(buffer, () =>
                {
                    Semaphore.Release();
                });

                BufferIndex = (BufferIndex + 1) % AudioBuffers.Count;
            });


        }

        #region Lifecycle

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            NodeEditorWindowController = new NodeEditorWindowController();

            InitFrequencies();
            InitScaleChooser();

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

        partial void RandomImageClicked(NSButton sender)
        {
            var service = new RandomImageService();

            Task.Run(async () =>
            {
                var image = await service.GetRandomImage();

                if (image != null)
                {
                    BeginInvokeOnMainThread(() => ImageCell.Image = image);
                    ChosenImage = image;
                }
            }); 
        }

        partial void ImagePicked(NSImageView sender)
        {
            ChosenImage = sender.Image;
        }

        partial void ScaleChosen(NSPopUpButtonCell sender)
        {
            //use the selected index from the actual object
            //instead of the sender, there's something wrong
            //where the sender is providing indexes in the thousands.
            var selectedIndex = ScaleChooser.IndexOfSelectedItem - 1;

            ChosenScale = Enum.GetValues(typeof(Scale))
                              .Cast<Scale>()
                              .ToArray()
                              [selectedIndex];
        }

        partial void EditNodesClicked(NSButton sender)
        {
            if (!NodeEditorWindowController.Window?.IsVisible ?? false)
            {
                NodeEditorWindowController.ShowWindow(this);
            }
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

        bool InputIsValid => (ChosenImage?.IsValid ?? false) && (ChosenScale.HasValue);

        void ShowValidationError()
        {
            ErrorLabel.StringValue = "Please provide and image and choose a scale";
        }

        void ShowAPIError()
        {
            ErrorLabel.StringValue = "Error getting a random image";
        }

        void ClearErrorMessage()
        {
            ErrorLabel.StringValue = string.Empty;
        }

        #endregion

        void InitScaleChooser()
        {
            ScaleChooser.RemoveAllItems();

            ScaleChooser.AddItem("Scale"); 

            ScaleChooser.AddItems(
                Enum.GetValues(typeof(Scale))
                .Cast<Scale>()
                .Select(s => s.GetFriendlyName())
                .ToArray()
            );
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
    }
}