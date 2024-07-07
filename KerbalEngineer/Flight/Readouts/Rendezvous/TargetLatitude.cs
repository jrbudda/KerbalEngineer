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

using KerbalEngineer.Flight.Readouts.Rendezvous;
using KerbalEngineer.Flight.Sections;
using KerbalEngineer.Helpers;

#endregion

namespace KerbalEngineer.Flight.Readouts.Surface {
    public class TargetLatitude : ReadoutModule {
        #region Constructors

        public TargetLatitude() {
            Name = "Target Latitude";
            Category = ReadoutCategory.GetCategory("Rendezvous");
            HelpString = "Shows the target vessel's latitude position around the celestial body. Latitude is the angle from the equator to poles.";
            IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section) {
            if (!RendezvousProcessor.ShowDetails) return;

            var target = Rendezvous.RendezvousProcessor.targetVessel;

            if (target != null) {
                var vessel = target.GetVessel();
                if (vessel == null) {
                    DrawLine("N/A", section);
                } else {
                    double latitude = AngleHelper.Clamp180(vessel.latitude);
                    DrawLine(Units.ToAngleDMS(latitude) + (latitude < 0 ? " S" : " N"), section);
                }
            }
        }

        public override void Update() {
            RendezvousProcessor.RequestUpdate();
        }

        #endregion
    }
}