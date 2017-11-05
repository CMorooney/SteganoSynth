using System;

namespace ImageMusic
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
