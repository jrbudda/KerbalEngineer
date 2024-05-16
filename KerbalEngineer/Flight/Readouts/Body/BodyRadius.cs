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
    public class BodyRadius : ReadoutModule {
        #region Constructors

        public BodyRadius() {
            this.Name = "Body Radius";
            this.Category = ReadoutCategory.GetCategory("Body");
            this.HelpString = "The radius of the body at sea level.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section) {
            if (FlightGlobals.ActiveVessel.mainBody == null)
                DrawLine("N/A", section.Width, section.IsHud);
            else
                this.DrawLine(Helpers.Units.ToDistance(FlightGlobals.ActiveVessel.mainBody.Radius), section.Width, section.IsHud);
        }

        #endregion
    }
}