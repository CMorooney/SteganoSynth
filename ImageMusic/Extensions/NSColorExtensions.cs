using System;
using AppKit;

namespace ImageMusic
{
    public static class NSColorExtensions
    {
        public static nfloat GetCarrierFrequency (this NSColor color)
        {
            var source = SynthSettings.Instance.GetSourceForTarget(TargetModifier.CarrierFrequency);

            const int multiplier = 1000;

            nfloat val = ValueForComponent(source, color);

            return val * multiplier;
        }

        public static nfloat GetModulatorFrequency(this NSColor color)
        {
            var source = SynthSettings.Instance.GetSourceForTarget(TargetModifier.ModifierFrequency);

            const int multiplier = 1000;

            nfloat val = ValueForComponent(source, color);

            return val * multiplier;
        }

        public static nfloat GetNoteLength (this NSColor color)
        {
            var source = SynthSettings.Instance.GetSourceForTarget(TargetModifier.NoteLength);

            const int multiplier = 8000;

            nfloat val = ValueForComponent(source, color);

            return val * multiplier;
        }

        public static nfloat GetOctave (this NSColor color)
        {
            var source = SynthSettings.Instance.GetSourceForTarget(TargetModifier.Octave);

            const int multiplier = 10000;

            nfloat val = ValueForComponent(source, color);

            return val * multiplier;
        }

        static nfloat ValueForComponent (ColorComponent component, NSColor color)
        {
            nfloat val = 0;

            switch (component)
            {
                case ColorComponent.Red:
                    val = color.RedComponent;
                    break;
                case ColorComponent.Green:
                    val = color.GreenComponent;
                    break;
                case ColorComponent.Blue:
                    val = color.BlueComponent;
                    break;
                case ColorComponent.Brightness:
                    val = color.BrightnessComponent;
                    break;
            }

            return val;
        }
    }
}
