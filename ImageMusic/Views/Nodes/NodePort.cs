using System;
using CoreGraphics;
using AppKit;

namespace ImageMusic
{
    public class NodePort : NSView
    {
        public bool IsHovering;
        public bool HasConnection;
        public event EventHandler MouseDidEnter;
        public event EventHandler MouseDidExit;

        NSColor Color;

        public NodePort(CGRect frame) : base(frame)
        {
            WantsLayer = true;

            SetColor (NSColor.Black);

            Layer.CornerRadius = frame.Height / 2;
            Layer.BorderWidth = 2;
        }

        public void SetColor (NSColor color)
        {
            Color = color;
            ApplyColor();
        }

        void ApplyColor()
        {
            Layer.BorderColor = Color.CGColor;
            SetNeedsDisplayInRect(Bounds);
        }

        public override bool IsFlipped => true;

        public override void DrawRect(CGRect dirtyRect)
        {
            if (IsHovering || HasConnection)
            {
                var gContext = NSGraphicsContext.CurrentContext.CGContext;
                gContext.SetFillColor(Color.CGColor);
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

            MouseDidEnter?.Invoke(this, null);

            IsHovering = true;
            SetNeedsDisplayInRect(Frame);
        }

        public override void MouseExited(NSEvent theEvent)
        {
            base.MouseExited(theEvent);

            MouseDidExit?.Invoke(this, null);

            IsHovering = false;
            SetNeedsDisplayInRect(Frame);
        }
    }
}
