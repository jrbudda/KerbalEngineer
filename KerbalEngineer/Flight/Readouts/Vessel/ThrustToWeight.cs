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

using KerbalEngineer.Flight.Sections;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class ThrustToWeight : ReadoutModule
    {
        #region Fields

        private string actual = string.Empty;
        private double gravity;
        private string total = string.Empty;

        #endregion

        #region Constructors

        public ThrustToWeight()
        {
            this.Name = "Thrust to Weight Ratio";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = "Shows the vessel's actual and total thrust to weight ratio.";
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (!SimulationProcessor.ShowDetails)
            {
                return;
            }

            actual = "N/A";
            total = "N/A";

            if (SimulationProcessor.LastStage.totalMass > 0) {
                this.gravity = FlightGlobals.getGeeForceAtPosition(FlightGlobals.ship_position).magnitude;
                this.actual = (SimulationProcessor.LastStage.actualThrust / (SimulationProcessor.LastStage.totalMass * this.gravity)).ToString("F2");
                this.total = (SimulationProcessor.LastStage.thrust / (SimulationProcessor.LastStage.totalMass * this.gravity)).ToString("F2");
            }
            this.DrawLine("TWR", this.actual + " / " + this.total, section.Width, section.IsHud);
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