using System;
using System.Threading.Tasks;
using Foundation;
using AppKit;
using System.Linq;
using CoreFoundation;

namespace ImageMusic
{
    public partial class MainWindowController : NSWindowController
    {
        NSImage ChosenImage;
        Scale? ChosenScale;
        NodeEditorWindowController NodeEditorWindowController;
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
            Synth.Init();
            Synth.NotePlayedForColor += SynthDidPlayNoteForColor;

            NodeEditorWindowController = new NodeEditorWindowController();

            InitScaleChooser();
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
                ResetProgressIndicator();
                Synth.PlayImageInScale(ChosenImage, ChosenScale.Value);
            }
        }

        void SynthDidPlayNoteForColor(NSColor color)
        {
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                ColorIndicator.FillColor = color;
                ProgressIndicator.IncrementBy(1);
            });
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