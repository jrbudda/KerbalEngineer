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
using KerbalEngineer.Helpers;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class ThrustOffsetAngle : ReadoutModule
    {
        #region Constructors

        public ThrustOffsetAngle()
        {
            this.Name = "Thrust offset angle";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = "Thrust angle offset due to vessel asymmetries and gimballing";
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (SimulationProcessor.ShowDetails)
            {
                this.DrawLine(Units.ToAngle(SimulationProcessor.LastStage.thrustOffsetAngle, 1), section.Width, section.IsHud);
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
