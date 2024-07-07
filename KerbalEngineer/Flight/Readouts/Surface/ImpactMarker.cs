// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2014 CYBUTEK
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

#region Using Directives

using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Sections;
using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface {
    public class ImpactMarker : ReadoutModule {
        #region Constructors

        public ImpactMarker() {
            this.Name = "Impact Marker";
            this.Category = ReadoutCategory.GetCategory("Surface");
            this.HelpString = "Shows your estimated impact position on the surface and the map.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section) {
            if (ImpactProcessor.ShowDetails) {
                this.DrawLine(() => {
                    GUIStyle s = section.IsHud ? this.CompactButtonStyle : this.ButtonStyle;
                    if (GUILayout.Button(ImpactProcessor.ShowMarker ? "Hide" : "Show", s,
                        GUILayout.Width(this.ContentWidth / 4), GUILayout.Height(s.fixedHeight))) {
                        show = !show;
                    }
                }, section, true);
            }
        }

        private bool show = true;

        public override void Reset() {
            FlightEngineerCore.Instance.AddUpdatable(ImpactProcessor.Instance);
        }

        public override void Update() {
            if (show)
                FlightEngineerCore.markerDeadman = 2;
            ImpactProcessor.RequestUpdate();
        }

        #endregion
    }
}