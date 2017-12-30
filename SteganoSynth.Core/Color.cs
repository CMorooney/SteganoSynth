using static System.Math;

namespace SteganoSynth.Core
{
    public class Color
    {
        public uint Red { get; private set; }

        public uint Green { get; private set; }

        public uint Blue { get; private set; }

        #region Constructors

        Color() { }

        Color(uint red, uint green, uint blue)
        {
            Red = NormalizeColorValue(red);
            Green = NormalizeColorValue(green);
            Blue = NormalizeColorValue(blue);
        }

        #endregion

        #region Factory methods

        public static Color FromRgb(uint red, uint green, uint blue) => new Color(red, green, blue);

        #endregion

        uint NormalizeColorValue(uint val)
        {
            const int floor = 0;
            const int ceiling = 255;

            return Min(Max(floor, val), ceiling);
        }
    }
}