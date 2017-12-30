namespace SteganoSynth.Core
{
    public enum Scale
    {
        #region Major

        [FriendlyName ("A Major")]
        [MiddleFrequencies(220, 247, 278, 294, 330, 370, 415)]
        AMajor,//A, B, C#, D, E, F#, G#

        [FriendlyName ("A# Major")]
        [MiddleFrequencies(233, 262, 294, 311, 349, 392, 440)]
        ASharpMajor,//A#, C, D, D#, F, G, A

        [FriendlyName ("B Major")]
        [MiddleFrequencies(247, 278, 311, 330, 370, 415, 466)]
        BMajor,//B, C#, D#, E, F#, G#, A#

        [FriendlyName ("C Major")]
        [MiddleFrequencies(262, 294, 330, 349, 392, 440, 494)]
        CMajor,//C, D, E, F, G, A, B

        [FriendlyName("C# Major")]
        [MiddleFrequencies(278, 311, 349, 370, 415, 466, 523)]
        CSharpMajor,//C#, D#, F, F#, G#, A#, C

        [FriendlyName("D Major")]
        [MiddleFrequencies(294, 330, 370, 392, 440, 494, 554)]
        DMajor,//D, E, F♯, G, A, B, C♯

        [FriendlyName("D# Major")]
        [MiddleFrequencies(311, 349, 392, 415, 466, 523, 587)]
        DSharpMajor,//D#, F, G, G#, A#, C, D

        [FriendlyName("E Major")]
        [MiddleFrequencies(330, 370, 415, 440, 494, 554, 622)]
        EMajor,//E, F#, G#, A, B, C#, D#

        [FriendlyName ("F Major")]
        [MiddleFrequencies(349, 392, 440, 466, 523, 587, 659)]
        FMajor,//F, G, A, A#, C, D, E

        [FriendlyName ("F# Major")]
        [MiddleFrequencies(185, 208, 233, 247, 278, 311, 349)]
        FSharpMajor,//F#, G#, A#, B, C#, D#, F

        [FriendlyName ("G Major")]
        [MiddleFrequencies(196, 220, 247, 262, 294, 330, 370)]
        GMajor,//G, A, B, C, D, E, F#

        [FriendlyName ("G# Major")]
        [MiddleFrequencies(208, 233, 262, 278, 311, 349, 392)]
        GSharpMajor,//G#, A#, C, C#, D#, F, G

        #endregion

        #region Minor

        [FriendlyName ("A Minor")]
        [MiddleFrequencies(220, 247, 262, 294, 330, 349, 392)]
        AMinor,//A, B, C, D, E, F, G

        [FriendlyName ("A# Minor")]
        [MiddleFrequencies(233, 262, 278, 311, 349, 370, 415)]
        ASharpMinor,//A#, C, C#, D#, F, F#, G#

        [FriendlyName ("B Minor")]
        [MiddleFrequencies(247, 278, 294, 330, 370, 392, 440)]
        BMinor,//B, C#, D, E, F#, G, A

        [FriendlyName ("C Minor")]
        [MiddleFrequencies(262, 294, 311, 349, 392, 415, 466)]
        CMinor,//C, D, D#, F, G, G#, A#

        [FriendlyName ("C# Minor")]
        [MiddleFrequencies(278, 311, 330, 370, 415, 440, 494)]
        CSharpMinor,//C#, D#, E, F#, G#, A, B

        [FriendlyName ("D Minor")]
        [MiddleFrequencies(294, 330, 349, 392, 440, 466, 523)]
        DMinor,//D, E, F, G, A, A#, C

        [FriendlyName ("D# Minor")]
        [MiddleFrequencies(311, 349, 370, 415, 466, 494, 554)]
        DSharpMinor,//D#, F F#, G#, A#, B, C#

        [FriendlyName ("E Minor")]
        [MiddleFrequencies(330, 370, 392, 440, 494, 523, 587)]
        EMinor,//E, F#, G, A, B, C, D

        [FriendlyName ("F Minor")]
        [MiddleFrequencies(349, 392, 415466, 523, 554, 622)]
        FMinor,//F, G, G#, A#, C, C#, D#

        [FriendlyName ("F# Minor")]
        [MiddleFrequencies(185, 208, 220, 247, 278, 294, 330)]
        FSharpMinor,//F#, G#, A, B, C#, D, E

        [FriendlyName ("G Minor")]
        [MiddleFrequencies(196, 220, 233, 262, 294, 311, 349)]
        GMinor,//G, A, A#, C, D, D#, F

        [FriendlyName ("G# Minor")]
        [MiddleFrequencies(208, 233, 247, 278, 311, 330, 370)]
        GSharpMinor,//G#, A#, B, C#, D#, E, F#

        #endregion
    }
}