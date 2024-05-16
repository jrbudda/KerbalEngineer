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

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class AngleToPrograde : ReadoutModule
    {
        #region Constructors

        public AngleToPrograde()
        {
            this.Name = "Angle to Prograde";
            this.Category = ReadoutCategory.GetCategory("Orbital");
            this.HelpString = "Angular Distance from the vessel to crossing the Orbit of the central body on it's retrograde side.";
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            this.DrawLine(OrbitExtensions.GetAngleToPrograde(FlightGlobals.ship_orbit).ToAngle(section.IsHud ? HudDecimalPlaces : DecimalPlaces), section);
        }

        #endregion
    }
}