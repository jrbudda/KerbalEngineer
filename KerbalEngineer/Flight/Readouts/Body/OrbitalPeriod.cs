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

#endregion

namespace KerbalEngineer.Flight.Readouts.Body {
    public class BodyOrbitalPeriod : ReadoutModule {
        #region Constructors

        public BodyOrbitalPeriod() {
            this.Name = "Body Orbital Period";
            this.Category = ReadoutCategory.GetCategory("Body");
            this.HelpString = "The time to complete one orbit about the body's parent.";
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section) {
            if (FlightGlobals.ActiveVessel.mainBody.orbit == null)
                this.DrawLine("N/A", section.Width, section.IsHud);
            else
                this.DrawLine(Helpers.Units.ToTime(FlightGlobals.ActiveVessel.mainBody.orbit.period), section.Width, section.IsHud);
        }

        #endregion
    }
}