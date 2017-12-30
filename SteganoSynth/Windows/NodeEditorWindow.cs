using System;
using Foundation;
using AppKit;

namespace SteganoSynth
{
    public partial class NodeEditorWindow : NSWindow
    {
        public NodeEditorWindow(IntPtr handle) : base(handle)
        {
            Init();
        }

        [Export("initWithCoder:")]
        public NodeEditorWindow(NSCoder coder) : base(coder)
        {
            Init();
        }

        void Init ()
        {
            Console.WriteLine("INIT");
            Delegate = new c();
        }
    }

    public class c : NSWindowDelegate
    {
        public override void DidBecomeKey(NSNotification notification)
        {
            base.DidBecomeKey(notification);
        }

        public override void DidBecomeMain(NSNotification notification)
        {
            base.DidBecomeMain(notification);
        }

        public override void DidChangeScreen(NSNotification notification)
        {
            base.DidChangeScreen(notification);
        }
    }
}
