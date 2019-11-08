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
using KSP.Localization;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class SpecificImpulse : ReadoutModule
    {
        #region Constructors

        public SpecificImpulse()
        {
            this.Name = Localizer.Format("#KE_SpecificImpulse");//"Specific Impulse"
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = Localizer.Format("#KE_SpecificImpulse_desc");//"Shows the average Specific Impulse of all engines in the current stage."
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (SimulationProcessor.ShowDetails)
            {
                this.DrawLine(SimulationProcessor.LastStage.isp.ToString("F1") + "s", section.IsHud);
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