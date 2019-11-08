// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2015 CYBUTEK
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

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    using Extensions;
    using Helpers;
    using Sections;
    using KSP.Localization;

    public class MeanAnomaly : ReadoutModule
    {
        public MeanAnomaly()
        {
            Name = Localizer.Format("#KE_MeanAnomaly");//"Mean Anomaly"
            Category = ReadoutCategory.GetCategory("Orbital");
            HelpString = Localizer.Format("#KE_MeanAnomaly_desc");//string.Empty
            IsDefault = false;
        }

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            DrawLine((FlightGlobals.ship_orbit.meanAnomaly * Units.RAD_TO_DEG).ToAngle(), section.IsHud);
        }
    }
}