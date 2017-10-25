using CoreGraphics;
using AppKit;

namespace ImageMusic
{
    public class NodePort : NSView
    {
        public bool IsHovering;
        public bool HasConnection;

        public NodePort(CGRect frame) : base(frame)
        {
            WantsLayer = true;

            Layer.CornerRadius = frame.Height / 2;
            Layer.BorderColor = NSColor.Black.CGColor;
            Layer.BorderWidth = 2;
        }

        public override bool IsFlipped => true;

        public override void DrawRect(CGRect dirtyRect)
        {
            if (IsHovering || HasConnection)
            {
                var gContext = NSGraphicsContext.CurrentContext.CGContext;
                gContext.SetFillColor(NSColor.Black.CGColor);
                gContext.FillRect(dirtyRect);
            }

            base.DrawRect(dirtyRect);
        }

        public override void UpdateTrackingAreas()
        {
            base.UpdateTrackingAreas();

            var options = NSTrackingAreaOptions.MouseEnteredAndExited |
                                               NSTrackingAreaOptions.ActiveInActiveApp |
                                               NSTrackingAreaOptions.InVisibleRect |
                                               NSTrackingAreaOptions.EnabledDuringMouseDrag;

            var hoverArea = new NSTrackingArea(Frame, options, this, null);

            AddTrackingArea(hoverArea);
        }

        public override void MouseEntered(NSEvent theEvent)
        {
            base.MouseEntered(theEvent);

            IsHovering = true;
            SetNeedsDisplayInRect(Frame);
        }

        public override void MouseExited(NSEvent theEvent)
        {
            base.MouseExited(theEvent);

            IsHovering = false;
            SetNeedsDisplayInRect(Frame);
        }
    }
}
