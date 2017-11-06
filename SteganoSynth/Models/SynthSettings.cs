using System;
using System.Linq;
using System.Collections.Generic;

namespace ImageMusic
{
    public class SynthSettings
    {

        Dictionary<ColorComponent, TargetModifier> CurrentSettings { get; set; }

        #region Singleton implementation

        static SynthSettings _Instance;

        public static SynthSettings Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new SynthSettings();
                }

                return _Instance;
            }
        }

        #endregion

        #region Construction and defaults

        SynthSettings()
        {
            SetupDefaults();
        }

        void SetupDefaults()
        {
            CurrentSettings = new Dictionary<ColorComponent, TargetModifier>
            {
                { ColorComponent.Red, TargetModifier.CarrierFrequency },
                { ColorComponent.Green, TargetModifier.Octave },
                { ColorComponent.Blue, TargetModifier.ModifierFrequency },
                { ColorComponent.Brightness, TargetModifier.NoteLength },
                { ColorComponent.Hue, TargetModifier.Pan }
            };
        }

        #endregion

        public void SetSourceForTarget(ColorComponent source, TargetModifier target)
        {
            CurrentSettings[source] = target;
        }

        public ColorComponent GetSourceForTarget (TargetModifier target)
        {
            return CurrentSettings.First(s => s.Value == target).Key;
        }

        public bool IsComplete()
        {
            var duplicates = CurrentSettings.Select(s => s.Value).GroupBy(s => s).Where(s => s.Count () > 1);

            return duplicates.Count() == 0;
        }
    }
}