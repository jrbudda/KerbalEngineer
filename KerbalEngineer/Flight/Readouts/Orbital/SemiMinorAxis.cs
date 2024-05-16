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

using KerbalEngineer.Flight.Sections;
using KerbalEngineer.Helpers;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class SemiMinorAxis : ReadoutModule
    {
        #region Constructors

        public SemiMinorAxis()
        {
            this.Name = "Semi-minor Axis";
            this.Category = ReadoutCategory.GetCategory("Orbital");
            this.HelpString = "Shows the distance from the centre of an orbit to the nearest edge.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            int dec = section.IsHud ? HudDecimalPlaces : DecimalPlaces;
            this.DrawLine(Units.ToDistance(FlightGlobals.ship_orbit.semiMinorAxis, dec < 0 ? 3 : dec), section);
        }

        #endregion
    }
}