using AppKit;
using Foundation;

namespace SteganoSynth
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        MainWindowController MainWindowController;

        public override void DidFinishLaunching(NSNotification notification)
        {
            MainWindowController = new MainWindowController();
            MainWindowController.Window.MakeKeyAndOrderFront(this);
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender) => true;
    }
}
