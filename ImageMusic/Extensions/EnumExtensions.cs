using System.Linq;

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

        public static string GetFriendlyName (this Scale scale)
        {
            return (typeof(Scale)
                    .GetField(scale.ToString())
                    .GetCustomAttributes(typeof(FriendlyNameAttribute), false)
                    .First() as FriendlyNameAttribute).FriendlyName;
        }
    }
}
