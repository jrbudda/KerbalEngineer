// Draw(Unity.Flight.ISectionModule section)
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

using KerbalEngineer.Flight.Sections;
using KerbalEngineer.Helpers;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class Acceleration : ReadoutModule
    {
        #region Constructors

        public Acceleration()
        {
            this.Name = "Acceleration";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = "Shows the current and maximum acceleration of the craft.";
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (SimulationProcessor.ShowDetails)
            {
                double a_Acceleration = Vessel.SimulationProcessor.LastStage.totalMass > 0 ? Vessel.SimulationProcessor.LastStage.actualThrust / Vessel.SimulationProcessor.LastStage.totalMass : 0;
                double m_Acceleration = Vessel.SimulationProcessor.LastStage.totalMass > 0 ? Vessel.SimulationProcessor.LastStage.thrust / Vessel.SimulationProcessor.LastStage.totalMass : 0;
                this.DrawLine(Units.ToAcceleration(a_Acceleration, m_Acceleration, section.IsHud ? HudDecimalPlaces : DecimalPlaces), section);
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

        #endregion
    }
}