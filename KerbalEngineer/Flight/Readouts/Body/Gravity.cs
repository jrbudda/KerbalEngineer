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
using System;

#endregion

namespace KerbalEngineer.Flight.Readouts.Body {
    public class BodyGravity : ReadoutModule {
        #region Constructors

        public BodyGravity() {
            this.Name = "Surface Gravity";
            this.Category = ReadoutCategory.GetCategory("Body");
            this.HelpString = "The surface gravity of the body.";
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section) {
            this.DrawLine(Helpers.Units.ToAcceleration(FlightGlobals.ActiveVessel.mainBody.gravParameter / Math.Pow(FlightGlobals.currentMainBody.Radius , 2), section.IsHud ? HudDecimalPlaces : DecimalPlaces), section);
        }

        #endregion
    }
}