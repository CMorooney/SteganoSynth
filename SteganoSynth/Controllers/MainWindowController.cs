﻿using System;
using System.Threading.Tasks;
using Foundation;
using AppKit;
using System.Linq;
using SteganoSynth.Core;

namespace SteganoSynth
{
    public partial class MainWindowController : NSWindowController
    {
        NSImage ChosenImage = NSImage.ImageNamed("Gradient");
        Scale? ChosenScale;
        NodeEditorWindowController NodeEditorWindowController;
        InfoWindowController InfoWindowController;
        Synth Synth;

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

        #region Lifecycle

        public override void WindowDidLoad()
        {
            base.WindowDidLoad();

            Synth = new Synth();
            Synth.NotePlayedForColor += SynthDidPlayNoteForColor;

            NodeEditorWindowController = new NodeEditorWindowController();
            InfoWindowController = new InfoWindowController();

            InitScaleChooser();
        }

        #endregion

        #region Event handlers

        partial void InfoClicked(NSObject sender)
        {
            if (!InfoWindowController.Window?.IsVisible ?? false)
            {
                InfoWindowController.ShowWindow(this);
            }
        }

        partial void RandomImageClicked(NSButton sender)
        {
            var service = new RandomImageService();

            Task.Run(async () =>
            {
                BeginInvokeOnMainThread(() =>
                {
                    RandomImageButton.Enabled = false;
                    RandomImageSpinner.Hidden = false;
                    RandomImageSpinner.StartAnimation(this);
                });

                var imageUrl = await service.GetRandomImage();

                if (imageUrl != null)
                {
                    var image = new NSImage(NSUrl.FromString(imageUrl.ToString()));

                    BeginInvokeOnMainThread(() => ImageCell.Image = image);
                    SetImage(image);
                }
                else
                {
                    BeginInvokeOnMainThread(() => ErrorLabel.StringValue = "Error getting random image, try again");
                }

                BeginInvokeOnMainThread(() =>
                {
                    RandomImageButton.Enabled = true;
                    RandomImageSpinner.Hidden = true;
                    RandomImageSpinner.StopAnimation(this);
                });
            }); 
        }

        partial void ImagePicked(NSImageView sender)
        {
            SetImage(sender.Image);
        }

        void SetImage(NSImage image)
        {
            ChosenImage = image;
            BeginInvokeOnMainThread(() =>
            {
                PausePlayButton.Image = NSImage.ImageNamed("Play");
                ColorIndicator.SetBackgroundColor(NSColor.Clear);
            });
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
            else if (Synth.IsPlaying)
            {
                PauseSynth();
            }
            else if (Synth.IsPaused)
            {
                ContinueSynth();
            }
            else
            {
                StartNewSynth();
            }
        }

        partial void StopClicked(NSObject sender)
        {
            StopSynth();
            StopButton.Enabled = false;
        }

        void SynthDidPlayNoteForColor(NSColor color)
        {
            BeginInvokeOnMainThread(() =>
            {
                ColorIndicator.SetBackgroundColor(color);
                ProgressIndicator.IncrementBy(1);
            });
        }

        #endregion

        #region Synth handling

        void StartNewSynth()
        {
            ImageCell.Enabled = false;
            ImageCell.Editable = false;
            NodeEditorButton.Enabled = false;
            RandomImageButton.Enabled = false;

            StopButton.Enabled = true;

            BeginInvokeOnMainThread (() => PausePlayButton.Image = NSImage.ImageNamed("Pause"));
            ClearErrorMessage();
            ResetProgressIndicator();

            Synth.PlayNewImageInScale(ChosenImage, ChosenScale.Value);
        }

        void PauseSynth()
        {
            Synth.Pause();
            StopButton.Enabled = true;
            BeginInvokeOnMainThread(() => PausePlayButton.Image = NSImage.ImageNamed("Play"));
        }

        void ContinueSynth()
        {
            Synth.ContinuePlaying();
            StopButton.Enabled = true;
            BeginInvokeOnMainThread(() => PausePlayButton.Image = NSImage.ImageNamed("Pause"));
        }

        void StopSynth()
        {
            ImageCell.Enabled = true;
            ImageCell.Editable = true;
            RandomImageButton.Enabled = true;
            NodeEditorButton.Enabled = true;
            Synth.Stop();
            BeginInvokeOnMainThread(() =>
            {
                PausePlayButton.Image = NSImage.ImageNamed("Play");
                ColorIndicator.SetBackgroundColor(NSColor.Clear);
            });
            ProgressIndicator.DoubleValue = 0;
        }

        #endregion

        #region Validation and errors

        bool InputIsValid => (ChosenImage?.IsValid ?? false) && (ChosenScale.HasValue) && (SynthSettings.Instance.IsComplete());

        void ShowValidationError()
        {
            ErrorLabel.StringValue = "Please provide and image, " +
                "choose a scale,\nand ensure that all sources have targets in the node editor";
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

        void ResetProgressIndicator()
        {
            var totalPixels = ChosenImage.Size.Width * ChosenImage.Size.Height;
            ProgressIndicator.DoubleValue = 0;
            ProgressIndicator.MinValue = 0;
            ProgressIndicator.MaxValue = totalPixels;
        }

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
    }
}