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

namespace KerbalEngineer.Flight.Readouts.Vessel
{
    #region Using Directives

    using Helpers;
    using Sections;

    #endregion

    public class Roll : ReadoutModule
    {
        #region Constructors

        public Roll()
        {
            this.Name = "Roll";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = "Shows the current Roll angle.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            this.DrawLine(Units.ToAngle(AttitudeProcessor.Roll, section.IsHud ? HudDecimalPlaces : DecimalPlaces), section);
        }

        public override void Reset()
        {
            FlightEngineerCore.Instance.AddUpdatable(AttitudeProcessor.Instance);
        }

        public override void Update()
        {
            AttitudeProcessor.RequestUpdate();
        }

        #endregion
    }
}