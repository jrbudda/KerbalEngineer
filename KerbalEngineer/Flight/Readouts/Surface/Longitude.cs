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

namespace KerbalEngineer.Flight.Readouts.Surface
{
    using Helpers;
    using Sections;

    public class Longitude : ReadoutModule
    {
        public Longitude()
        {
            Name = "Longitude";
            Category = ReadoutCategory.GetCategory("Surface");
            HelpString = "Shows the vessel's longitude around a celestial body. Longitude is the angle from the bodies prime meridian.";
            IsDefault = true;
        }

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            double angle = AngleHelper.Clamp180(FlightGlobals.ship_longitude);
            DrawLine(Units.ToAngleDMS(angle) + (angle < 0.0 ? " W" : " E"), section);
        }
    }
}