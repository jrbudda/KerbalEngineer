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

namespace KerbalEngineer.Flight.Readouts.Surface
{
    public class HorizontalSpeed : ReadoutModule
    {
        #region Constructors

        public HorizontalSpeed()
        {
            this.Name = Localizer.Format("#KE_HorizontalSpeed");//"Horizontal Speed"
            this.Category = ReadoutCategory.GetCategory("Surface");
            this.HelpString = Localizer.Format("#KE_HorizontalSpeed_desc");//"Shows the vessel's horizontal speed across a celestial body's surface."
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            this.DrawLine(FlightGlobals.ActiveVessel.horizontalSrfSpeed.ToSpeed(), section.IsHud);

            // This workaround was used for KSP 1.0.3 and 1.0.4 where horizontalSrfSpeed was really badly broken
            //var ves = FlightGlobals.ActiveVessel;
            //double horizSpeed = Math.Sqrt(ves.srfSpeed * ves.srfSpeed - ves.verticalSpeed * ves.verticalSpeed);
            //this.DrawLine(horizSpeed.ToSpeed(), section.IsHud);
        }

        #endregion
    }
}