using System;

using Foundation;
using AppKit;

namespace ImageMusic
{
    public partial class NodeEditorWindow : NSWindow
    {
        public NodeEditorWindow(IntPtr handle) : base(handle)
        {
        }

        [Export("initWithCoder:")]
        public NodeEditorWindow(NSCoder coder) : base(coder)
        {
        }
    }
}
