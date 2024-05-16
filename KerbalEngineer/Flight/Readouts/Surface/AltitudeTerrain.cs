// 
//     Kerbal Engineer Redux
// 
//     Copyright (C) 2016 CYBUTEK
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
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  

namespace KerbalEngineer.Flight.Readouts.Surface
{
    using Extensions;
    using Sections;
    using Vessel = global::Vessel;

    public class AltitudeTerrain : ReadoutModule
    {
        public AltitudeTerrain()
        {
            Name = "Altitude (Terrain)";
            Category = ReadoutCategory.GetCategory("Surface");
            HelpString = "Shows the vessel's altitude above the terrain and water's surface, or altitude above underwater terrain whilst splashed down.";
            IsDefault = true;
        }

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (FlightGlobals.ActiveVessel.terrainAltitude > 0.0 
            || FlightGlobals.ActiveVessel.situation == Vessel.Situations.SPLASHED
            || FlightGlobals.ActiveVessel.situation == Vessel.Situations.LANDED)
            {
                DrawLine((FlightGlobals.ship_altitude - FlightGlobals.ActiveVessel.terrainAltitude).ToDistance(), section.Width, section.IsHud);
            }
            else
            {
                DrawLine((FlightGlobals.ship_altitude).ToDistance(), section.Width, section.IsHud);
            }
        }
    }
}
