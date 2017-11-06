// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ImageMusic
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		ImageMusic.ViewWithABackgroundColorBecauseApparentlyThatsALotToAskFor ColorIndicator { get; set; }

		[Outlet]
		AppKit.NSTextField ErrorLabel { get; set; }

		[Outlet]
		AppKit.NSImageCell ImageCell { get; set; }

		[Outlet]
		AppKit.NSButtonCell NodeEditorButton { get; set; }

		[Outlet]
		AppKit.NSButton PausePlayButton { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator ProgressIndicator { get; set; }

		[Outlet]
		AppKit.NSButton RandomImageButton { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator RandomImageSpinner { get; set; }

		[Outlet]
		AppKit.NSPopUpButton ScaleChooser { get; set; }

		[Outlet]
		AppKit.NSButton StopButton { get; set; }

		[Action ("EditNodesClicked:")]
		partial void EditNodesClicked (AppKit.NSButton sender);

		[Action ("ImagePicked:")]
		partial void ImagePicked (AppKit.NSImageView sender);

		[Action ("RandomImageClicked:")]
		partial void RandomImageClicked (AppKit.NSButton sender);

		[Action ("ScaleChosen:")]
		partial void ScaleChosen (AppKit.NSPopUpButtonCell sender);

		[Action ("StartClicked:")]
		partial void StartClicked (AppKit.NSButton sender);

		[Action ("StopClicked:")]
		partial void StopClicked (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ColorIndicator != null) {
				ColorIndicator.Dispose ();
				ColorIndicator = null;
			}

			if (RandomImageSpinner != null) {
				RandomImageSpinner.Dispose ();
				RandomImageSpinner = null;
			}

			if (ErrorLabel != null) {
				ErrorLabel.Dispose ();
				ErrorLabel = null;
			}

			if (ImageCell != null) {
				ImageCell.Dispose ();
				ImageCell = null;
			}

			if (NodeEditorButton != null) {
				NodeEditorButton.Dispose ();
				NodeEditorButton = null;
			}

			if (PausePlayButton != null) {
				PausePlayButton.Dispose ();
				PausePlayButton = null;
			}

			if (ProgressIndicator != null) {
				ProgressIndicator.Dispose ();
				ProgressIndicator = null;
			}

			if (RandomImageButton != null) {
				RandomImageButton.Dispose ();
				RandomImageButton = null;
			}

			if (ScaleChooser != null) {
				ScaleChooser.Dispose ();
				ScaleChooser = null;
			}

			if (StopButton != null) {
				StopButton.Dispose ();
				StopButton = null;
			}
		}
	}
}
