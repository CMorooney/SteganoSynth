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
		AppKit.NSBox ColorIndicator { get; set; }

		[Outlet]
		AppKit.NSTextField ErrorLabel { get; set; }

		[Outlet]
		AppKit.NSButton PausePlayButton { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator ProgressIndicator { get; set; }

		[Action ("ImagePicked:")]
		partial void ImagePicked (AppKit.NSImageView sender);

		[Action ("StartClicked:")]
		partial void StartClicked (AppKit.NSButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ColorIndicator != null) {
				ColorIndicator.Dispose ();
				ColorIndicator = null;
			}

			if (ProgressIndicator != null) {
				ProgressIndicator.Dispose ();
				ProgressIndicator = null;
			}

			if (PausePlayButton != null) {
				PausePlayButton.Dispose ();
				PausePlayButton = null;
			}

			if (ErrorLabel != null) {
				ErrorLabel.Dispose ();
				ErrorLabel = null;
			}
		}
	}
}
