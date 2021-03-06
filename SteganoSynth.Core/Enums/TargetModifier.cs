﻿namespace SteganoSynth.Core
{
    public enum TargetModifier
    {
        [FriendlyName("Carrier Frequency")]
        CarrierFrequency,

        [FriendlyName("Modifier Frequency")]
        ModifierFrequency,

        [FriendlyName("Octave")]
        Octave,

        [FriendlyName("Note Length")]
        NoteLength,

        [FriendlyName("Pan")]
        Pan
    }
}