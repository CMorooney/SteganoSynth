using System;

using Foundation;
using AppKit;

namespace SteganoSynth
{
    public partial class InfoWindow : NSWindow
    {
        public InfoWindow(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public InfoWindow(NSCoder coder) : base(coder)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }
    }
}
