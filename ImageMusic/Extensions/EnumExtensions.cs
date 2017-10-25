using System;
using System.Linq;
using AppKit;

namespace ImageMusic
{
    public static class EnumExtensions
    {
        public static int[] GetMiddleFrequencies(this Scale scale)
        {
            return (typeof(Scale)
                    .GetField(scale.ToString())
                    .GetCustomAttributes(typeof(MiddleFrequenciesAttribute), false)
                    .First() as MiddleFrequenciesAttribute).Frequencies;
        }

        public static NSColor GetColor (this ColorComponent colorComponent)
        {
            return (typeof(ColorComponent)
                    .GetField(colorComponent.ToString())
                    .GetCustomAttributes(typeof(ColorAttribute), false)
                    .First() as ColorAttribute).Color;
        }

        public static string GetFriendlyName (this Enum @enum)
        {
            return (@enum.GetType()
                    .GetField(@enum.ToString())
                    .GetCustomAttributes(typeof(FriendlyNameAttribute), false)
                    .First() as FriendlyNameAttribute).FriendlyName;
        }
    }
}
