using UnityEngine;
namespace KerbalEngineer.Flight.Readouts {
    public class ReadoutModuleConfigNode {
        public const int DEFAULT_CHARACTER_LIMIT = 20;

        public string Name { get; set; }
        public Color TextColor { get; set; } = HighLogic.Skin.label.normal.textColor;
        public int CharacterLimit { get; set; } = DEFAULT_CHARACTER_LIMIT;
        public int HudCharacterLimit { get; set; } = DEFAULT_CHARACTER_LIMIT;
        public int DecimalPlaces { get; set; } = -9000;
        public int HudDecimalPlaces { get; set; } = -9000;
    }
}