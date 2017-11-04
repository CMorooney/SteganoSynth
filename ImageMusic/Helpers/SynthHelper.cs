using System;
using AppKit;

namespace ImageMusic
{
    public static class SynthHelper
    {
        public static nfloat GetCarrierFrequencyForColor (NSColor color)
        {
            var source = SynthSettings.Instance.GetSourceForTarget(TargetModifier.CarrierFrequency);

            const int multiplier = 1000;

            nfloat val = ValueForComponent(source, color);

            return val * multiplier;
        }

        public static nfloat GetModulatorFrequencyForColor (NSColor color)
        {
            var source = SynthSettings.Instance.GetSourceForTarget(TargetModifier.ModifierFrequency);

            const int multiplier = 1000;

            nfloat val = ValueForComponent(source, color);

            return val * multiplier;
        }

        public static uint GetNoteLengthForColor ( NSColor color)
        {
            var source = SynthSettings.Instance.GetSourceForTarget(TargetModifier.NoteLength);

            const int multiplier = 8000;

            nfloat val = ValueForComponent(source, color);

            return (uint)Math.Max(1024, val * multiplier);
        }

        public static nfloat GetOctaveForColor (NSColor color)
        {
            var source = SynthSettings.Instance.GetSourceForTarget(TargetModifier.Octave);

            const int multiplier = 10000;

            nfloat val = ValueForComponent(source, color);

            return val * multiplier;
        }

        public static float GetPanForColor (NSColor color)
        {
            var source = SynthSettings.Instance.GetSourceForTarget(TargetModifier.Pan);

            float val = (float)ValueForComponent(source, color);

            return val.ConvertToRange(0, 1, -.5f, .5f);
        }

        static nfloat ValueForComponent(ColorComponent component, NSColor color)
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
                case ColorComponent.Hue:
                    val = color.HueComponent;
                    break;
            }

            return val;
        }
    }
}
