using System;

namespace SteganoSynth.Core
{
    public class MiddleFrequenciesAttribute : Attribute
    {
        public readonly int[] Frequencies;

        public MiddleFrequenciesAttribute(params int[] frequencies)
        {
            Frequencies = frequencies;
        }
    }
}