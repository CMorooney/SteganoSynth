using System;
using Foundation;
using AppKit;

namespace ImageMusic
{
    public partial class NodeEditorWindowController : NSWindowController
    {
        public NodeEditorWindowController(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public NodeEditorWindowController(NSCoder coder) : base(coder)
        {
        }

        public NodeEditorWindowController() : base("NodeEditorWindow")
        {
        }
    }
}
