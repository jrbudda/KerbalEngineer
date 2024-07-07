using UnityEngine;
namespace KerbalEngineer.Flight.Readouts {
    public class ReadoutModuleConfigNode {
        public const int DEFAULT_CHARACTER_LIMIT = 20;

        
        public Color TextColor { get; set; } = HighLogic.Skin.label.normal.textColor;
        public Color HudTextColor { get; set; } = HighLogic.Skin.label.normal.textColor;

        //Displayed value strings will be truncated if they're longer than this.
        public int CharacterLimit { get; set; } = DEFAULT_CHARACTER_LIMIT;
        public int HudCharacterLimit { get; set; } = DEFAULT_CHARACTER_LIMIT;

        //Decimal-place override for floating-point value readouts. Negative values will use the default number of decimal places for that type of unit.
        public int DecimalPlaces { get; set; } = -9000;
        public int HudDecimalPlaces { get; set; } = -9000;
        
        //Whether to display the name/label in front of the readout value.
        public bool HideName { get; set; } = false;
        public bool HudHideName { get; set; } = false;

        //Whether to display the short name in readouts rather than the full one.
        public bool UseShortName { get; set; } = false;
        public bool HudUseShortName { get; set; } = false;


        public void CopyFrom(ReadoutModuleConfigNode source) {
            TextColor = source.TextColor;
            HudTextColor = source.HudTextColor;
            DecimalPlaces = source.DecimalPlaces;
            HudDecimalPlaces = source.HudDecimalPlaces;
            CharacterLimit = source.CharacterLimit;
            HudCharacterLimit = source.HudCharacterLimit;
            HideName = source.HideName;
            HudHideName = source.HudHideName;
            UseShortName = source.UseShortName;
            HudUseShortName = source.HudUseShortName;
        }
    }
}