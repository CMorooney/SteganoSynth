using System;
using Foundation;
using AppKit;

namespace ImageMusic
{
    public partial class InfoWindowController : NSWindowController
    {
        const string SourceUrl = "https://github.com/CMorooney/SteganoSynth";
        const string BlogUrl = "http://www.calvins.pizza/";

        #region Constructors

        public InfoWindowController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public InfoWindowController(NSCoder coder) : base(coder)
        {
        }

        public InfoWindowController() : base("InfoWindow")
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
        }

        #endregion

        #region Event handlers

        partial void ViewSourceClicked(NSObject sender)
        {
            NSWorkspace.SharedWorkspace.OpenUrl(NSUrl.FromString(SourceUrl));
        }

        partial void ViewBlogClicked(NSObject sender)
        {
            NSWorkspace.SharedWorkspace.OpenUrl(NSUrl.FromString(BlogUrl));
        }

        #endregion
    }
}
