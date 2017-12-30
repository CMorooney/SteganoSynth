using System;

namespace SteganoSynth.Core
{
    public class FriendlyNameAttribute : Attribute
    {
        public readonly string FriendlyName;

        public FriendlyNameAttribute(string friendlyName)
        {
            FriendlyName = friendlyName;
        }
    }
}
