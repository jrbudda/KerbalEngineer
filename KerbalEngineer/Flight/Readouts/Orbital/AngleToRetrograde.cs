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

using KSP.Localization;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class AngleToRetrograde : ReadoutModule
    {
        #region Constructors

        public AngleToRetrograde()
        {
            this.Name = Localizer.Format("#KE_AngleToRetrograde");//"Angle to Retrograde"
            this.Category = ReadoutCategory.GetCategory("Orbital");
            this.HelpString =   Localizer.Format("#KE_AngleToRetrograde_desc");//"Angular Distance from the vessel to crossing the Orbit of the central body on it's retrograde side."
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            this.DrawLine(FlightGlobals.ship_orbit.GetAngleToRetrograde().ToAngle(), section.IsHud);
        }

        #endregion
    }
}