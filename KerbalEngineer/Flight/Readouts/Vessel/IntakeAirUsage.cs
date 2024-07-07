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

using System;

using KerbalEngineer.Flight.Sections;
using KerbalEngineer.Helpers;

#endregion

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    public class IntakeAirUsage : ReadoutModule
    {
        #region Fields

        private double percentage;

        #endregion

        #region Constructors

        public IntakeAirUsage()
        {
            this.Name = "Intake Air (Usage)";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = "Displays the consumption of Intake Air.";
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            this.DrawLine(Units.ToPercent(this.percentage, section.IsHud ? HudDecimalPlaces : DecimalPlaces), section);
        }

        public override void Update()
        {
            this.percentage = IntakeAirDemandSupply.GetDemand() / IntakeAirDemandSupply.GetSupply();
            if (Double.IsNaN(this.percentage) || Double.IsInfinity(this.percentage))
            {
                this.percentage = 0.0;
            }
        }

        #endregion
    }
}