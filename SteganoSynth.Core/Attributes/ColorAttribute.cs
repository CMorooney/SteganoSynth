using System;

namespace SteganoSynth.Core
{
    public class ColorAttribute : Attribute
    {
        public readonly Color Color;

        public ColorAttribute(uint r, uint g, uint b)
        {
            Color = Color.FromRgb(r, g, b);
        }
    }
}
