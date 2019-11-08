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

    public class MeanAnomalyAtEpoc : ReadoutModule
    {
        public MeanAnomalyAtEpoc()
        {
            Name = Localizer.Format("#KE_MeanAnomalyAtEpoc");//"Mean Anomaly at Epoc"
            Category = ReadoutCategory.GetCategory("Orbital");
            HelpString = Localizer.Format("#KE_MeanAnomalyAtEpoc_desc");//string.Empty
            IsDefault = false;
        }

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            DrawLine((FlightGlobals.ship_orbit.meanAnomalyAtEpoch * Units.RAD_TO_DEG).ToAngle(), section.IsHud);
        }
    }
}