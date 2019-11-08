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

namespace KerbalEngineer.Flight.Readouts.Vessel {
    using System;
    using Sections;
    using KSP.Localization;

    public class SurfaceThrustToWeight : ReadoutModule {
        private string m_Actual = string.Empty;
        private double m_Gravity;
        private string m_Total = string.Empty;

        public SurfaceThrustToWeight() {
            Name = Localizer.Format("#KE_SurfaceThrustToWeight");//"Surface Thrust to Weight Ratio"
            Category = ReadoutCategory.GetCategory("Vessel");
            HelpString = Localizer.Format("#KE_SurfaceThrustToWeight_desc");//"Shows the vessel's surface thrust to weight ratio."
            IsDefault = true;
        }

        public override void Draw(Unity.Flight.ISectionModule section) {
            if (FlightGlobals.currentMainBody == null || SimulationProcessor.LastStage == null ||
                !SimulationProcessor.ShowDetails) {
                return;
            }
            m_Actual = "N/A";
            m_Total = "N/A";
            if (SimulationProcessor.LastStage.totalMass > 0) {
                m_Gravity = FlightGlobals.currentMainBody.gravParameter / Math.Pow(FlightGlobals.currentMainBody.Radius, 2);
                m_Actual = (SimulationProcessor.LastStage.actualThrust / (SimulationProcessor.LastStage.totalMass * m_Gravity)).ToString("F2");
                m_Total = (SimulationProcessor.LastStage.thrust / (SimulationProcessor.LastStage.totalMass * m_Gravity)).ToString("F2");
            }
            DrawLine("TWR (Surface)", m_Actual + " / " + m_Total, section.IsHud);
        }

        public override void Reset() {
            FlightEngineerCore.Instance.AddUpdatable(SimulationProcessor.Instance);
        }

        public override void Update() {
            SimulationProcessor.RequestUpdate();
        }
    }
}