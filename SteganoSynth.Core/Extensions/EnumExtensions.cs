using System;
using System.Linq;
using System.Reflection;

namespace SteganoSynth.Core
{
    public static class EnumExtensions
    {
        public static int[] GetMiddleFrequencies(this Scale scale)
        {
            return (typeof(Scale)
                    .GetRuntimeField(scale.ToString())
                    .GetCustomAttributes(typeof(MiddleFrequenciesAttribute), false)
                    .First() as MiddleFrequenciesAttribute).Frequencies;
        }

        public static Color GetColor(this ColorComponent colorComponent)
        {
            return (typeof(ColorComponent)
                    .GetRuntimeField(colorComponent.ToString())
                    .GetCustomAttributes(typeof(ColorAttribute), false)
                    .First() as ColorAttribute).Color;
        }

        public static string GetFriendlyName (this Enum @enum)
        {
            return (@enum.GetType()
                    .GetRuntimeField(@enum.ToString())
                    .GetCustomAttributes(typeof(FriendlyNameAttribute), false)
                    .First() as FriendlyNameAttribute).FriendlyName;
        }
    }
}
