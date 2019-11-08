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

using System;

using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Sections;

using KSP.Localization;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode {
    public class PostBurnRealtiveInclination : ReadoutModule {
        #region Constructors

        public PostBurnRealtiveInclination() {
            this.Name = Localizer.Format("#KE_PostBurnRealtiveInclination");//"Post-burn Rel. Inclination"
            this.Category = ReadoutCategory.GetCategory("Orbital");
            this.HelpString = Localizer.Format("#KE_PostBurnRealtiveInclination_desc") ;//"The inclination of the vessel's orbit relative to the target after the burn."
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section) {
            if (!ManoeuvreProcessor.ShowDetails) {
                return;
            }
            if (FlightGlobals.ActiveVessel.targetObject == null || FlightGlobals.ActiveVessel.targetObject.GetOrbit() == null)
                this.DrawLine("N/A", section.IsHud);
            else
                this.DrawLine(ManoeuvreProcessor.PostBurnRelativeInclination.ToAngle(), section.IsHud);
        }

        public override void Reset() {
            ManoeuvreProcessor.Reset();
        }

        public override void Update() {
            Rendezvous.RendezvousProcessor.RequestUpdate();
        }

        #endregion
    }
}