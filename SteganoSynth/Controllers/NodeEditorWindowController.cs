using System;
using Foundation;
using AppKit;

namespace SteganoSynth
{
    public partial class NodeEditorWindowController : NSWindowController
    {
        #region Constructors

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

        #endregion
    }
}
