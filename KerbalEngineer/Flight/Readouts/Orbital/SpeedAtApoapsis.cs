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
using KerbalEngineer.Flight.Sections;
using KerbalEngineer.Helpers;
using KerbalEngineer.Extensions;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class SpeedAtApoapsis : ReadoutModule
    {
        #region Constructors

        public SpeedAtApoapsis()
        {
            this.Name = "Speed at Apoapsis";
            this.Category = ReadoutCategory.GetCategory("Orbital");
            this.HelpString = "Shows the orbital speed of the vessel when at apoapsis, the highest point of the orbit.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            // Vis-viva: v^2 = GM(2/r - 1/a)
            // All this is easily got from the ships orbit (and reference body)
            String str;
            Orbit orbit = FlightGlobals.ship_orbit;
            if (orbit.eccentricity > 1.0)
                str = "---m/s";
            else
            {
                double speedsqr = orbit.referenceBody.gravParameter * ((2 / orbit.ApR) - (1 / orbit.semiMajorAxis));
                if (Double.IsNaN(speedsqr) || speedsqr < 0)
                    str = "---m/s";     // Don't think this is possible barring bugs in the Orbit class
                else
                    str = Math.Sqrt(speedsqr).ToSpeed();
            }
            this.DrawLine(str, section.Width, section.IsHud);
        }

        #endregion
    }
}