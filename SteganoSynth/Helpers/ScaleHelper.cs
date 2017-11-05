using System;
using System.Collections.Generic;

namespace ImageMusic
{
    public static class ScaleHelper
    {
        public static List<int> GetNotesForScale(Scale scale, int octave)
        {
            if (octave > 8)
            {
                octave = 8;
            }
            else if (octave < 0)
            {
                octave = 0;
            }

            var notes = new List<int>();

            var middleFrequencies = scale.GetMiddleFrequencies();

            var multiplier = GetMultiplierForOctave(octave);

            foreach (var frequency in middleFrequencies)
            {
                notes.Add((int)(frequency * multiplier));
            }

            return notes;
        }

        static double GetMultiplierForOctave(int octave)
        {
            switch (octave)
            {
                case 0:
                    return (1 / 16);
                case 1:
                    return (1 / 8);
                case 2:
                    return (1 / 4);
                case 3:
                    return (1 / 2);
                case 4:
                    return 1d;
                case 5:
                    return 2d;
                case 6:
                    return 4d;
                case 7:
                    return 8d;
                case 8:
                    return 16d;

            }

            throw new Exception("Invalid octave used when converting to frequency multiplier");
        }
    }
}
