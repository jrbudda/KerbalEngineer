using UnityEngine;
namespace KerbalEngineer.Flight.Readouts {
    public class ReadoutModuleConfigNode {
        public string Name { get; set; }
        public Color TextColor { get; set; } = HighLogic.Skin.label.normal.textColor;
        public int DecimalPlaces { get; set; } = -9000;
        public int HudDecimalPlaces { get; set; } = -9000;
    }
}