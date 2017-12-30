using SteganoSynth.Core;
using AppKit;

namespace SteganoSynth
{
    public static class PlatformColorExtensions
    {
        public static NSColor ToNSColor(this Color color) => NSColor.FromRgb(color.Red, color.Green, color.Blue);
    }
}
