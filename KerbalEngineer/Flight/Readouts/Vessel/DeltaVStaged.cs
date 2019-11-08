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

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    using System.Linq;
    using Helpers;
    using KSP.UI.Screens;
    using Sections;
    using VesselSimulator;
    using KSP.Localization;

    public class DeltaVStaged : ReadoutModule
    {
        public DeltaVStaged()
        {
            Name = Localizer.Format("#KE_DeltaVStaged");//"DeltaV Staged"
            Category = ReadoutCategory.GetCategory("Vessel");
            HelpString = Localizer.Format("#KE_DeltaVStaged_desc");//"Shows the vessel's delta velocity for each stage."
            IsDefault = true;
        }

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (SimulationProcessor.ShowDetails == false || StageManager.Instance == null)
            {
                return;
            }

            foreach (Stage stage in SimulationProcessor.Stages.Where(stage => stage.deltaV > 0 || stage.number == StageManager.CurrentStage))
            {
                DrawLine("DeltaV (S" + stage.number + ")", stage.deltaV.ToString("N0") + "m/s (" + TimeFormatter.ConvertToString(stage.time) + ")", section.IsHud);
            }
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(SimulationProcessor.Instance);
        }

        public override void Update()
        {
            SimulationProcessor.RequestUpdate();
        }
    }
}