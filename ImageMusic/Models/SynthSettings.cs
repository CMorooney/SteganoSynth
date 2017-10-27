using System.Collections.Generic;

namespace ImageMusic
{
    public class SynthSettings
    {
        #region Properties

        public Dictionary<ColorComponent, TargetModifier> CurrentSettings { get; set; }

        #endregion

        #region Singleton implementation

        public SynthSettings Instance
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

        SynthSettings _Instance;

        void SetupDefaults()
        {
            CurrentSettings = new Dictionary<ColorComponent, TargetModifier>
            {
                { ColorComponent.Red, TargetModifier.CarrierFrequency },
                { ColorComponent.Green, TargetModifier.Octave },
                { ColorComponent.Blue, TargetModifier.ModifierFrequency },
                { ColorComponent.Brightness, TargetModifier.NoteLength }
            };
        }

        #endregion
    }
}