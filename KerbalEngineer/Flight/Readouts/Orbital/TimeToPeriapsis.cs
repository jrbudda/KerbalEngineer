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
    public class TimeToPeriapsis : ReadoutModule
    {
        #region Constructors

        public TimeToPeriapsis()
        {
            this.Name = "Time to Periapsis";
            this.Category = ReadoutCategory.GetCategory("Orbital");
            this.HelpString = "Shows the time until the vessel reaches periapsis, the lowest point of the orbit.";
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            this.DrawLine(TimeFormatter.ConvertToString(FlightGlobals.ship_orbit.timeToPe), section);
        }

        #endregion
    }
}