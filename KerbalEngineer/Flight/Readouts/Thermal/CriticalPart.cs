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

namespace KerbalEngineer.Flight.Readouts.Thermal
{
    using Sections;

    public class CriticalPart : ReadoutModule
    {
        public CriticalPart()
        {
            Name = "Critical Part";
            ShortName = "Crit Part";
            Category = ReadoutCategory.GetCategory("Thermal");
            HelpString = "This part is structually most critical. If it endures too high temperature there is a high chance for major structual failure!";
            IsDefault = true;
        }

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (ThermalProcessor.ShowDetails)
            {
                DrawLine(ThermalProcessor.CriticalPartName, section);
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(ThermalProcessor.Instance);
        }

        public override void Update()
        {
            ThermalProcessor.RequestUpdate();
        }
    }
}