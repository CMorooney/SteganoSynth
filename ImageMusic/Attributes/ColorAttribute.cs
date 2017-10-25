using System;
using AppKit;

namespace ImageMusic
{
    public class ColorAttribute : Attribute
    {
        public readonly NSColor Color;

        public ColorAttribute(int r, int g, int b)
        {
            Color = NSColor.FromRgb(r, g, b);
        }
    }
}
