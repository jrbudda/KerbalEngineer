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
using KSP.Localization;

#endregion

namespace KerbalEngineer.Flight.Readouts.Body {
    public class EscapeVelocity : ReadoutModule {
        #region Constructors

        public EscapeVelocity() {
            this.Name = Localizer.Format("#KE_EscapeVelocity");//"Surface Escape Velocity"
            this.Category = ReadoutCategory.GetCategory("Body");
            this.HelpString = Localizer.Format("#KE_EscapeVelocity_desc");//"The velocity needed to escape the SOI, starting from sea level."
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section) {
            var ev = Math.Sqrt(2 * FlightGlobals.currentMainBody.gravParameter / FlightGlobals.currentMainBody.Radius);
            this.DrawLine(Helpers.Units.ToSpeed(ev), section.IsHud);
        }

        #endregion
    }
}