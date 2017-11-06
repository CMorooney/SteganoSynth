using System;
using Foundation;
using AppKit;
using CoreGraphics;

namespace ImageMusic
{
    public partial class ViewWithABackgroundColorBecauseApparentlyThatsALotToAskFor : NSView
    {
        NSColor BackgroundColor = NSColor.Clear;

        #region Constructors

        // Called when created from unmanaged code
        public ViewWithABackgroundColorBecauseApparentlyThatsALotToAskFor(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        // Called when created directly from a XIB file
        [Export("initWithCoder:")]
        public ViewWithABackgroundColorBecauseApparentlyThatsALotToAskFor(NSCoder coder) : base(coder)
        {
            Initialize();
        }

        // Shared initialization code
        void Initialize()
        {
            WantsLayer = true;
            Layer.CornerRadius = 5;
        }

        #endregion

        public void SetBackgroundColor(NSColor color)
        {
            BackgroundColor = color;
            SetNeedsDisplayInRect(Bounds);
        }

        public override void DrawRect(CGRect dirtyRect)
        {
            var gContext = NSGraphicsContext.CurrentContext.CGContext;
            gContext.SetFillColor(BackgroundColor.CGColor);
            gContext.FillRect(dirtyRect);

            base.DrawRect(dirtyRect);
        }
    }
}
