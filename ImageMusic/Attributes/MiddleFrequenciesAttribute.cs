using System;

namespace ImageMusic
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