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

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class Latitude : ReadoutModule
    {
        #region Constructors

        public Latitude()
        {
            this.Name = "Latitude";
            this.Category = ReadoutCategory.GetCategory("Surface");
            this.HelpString = "Shows the vessel's latitude position around the celestial body. Latitude is the angle from the equator to poles.";
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            this.DrawLine(Units.ToAngleDMS(FlightGlobals.ship_latitude) + (FlightGlobals.ship_latitude < 0 ? " S" : " N"), section.Width, section.IsHud);
        }

        #endregion
    }
}