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

    #endregion

    public class AngleOfSideslip : ReadoutModule
    {
        #region Constructors

        public AngleOfSideslip() {
            this.Name = "AoS";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = "Angle-of-sideslip: the angle between velocity and the vessel's fuselage, along the vessel's horizontal axis. Aircraft should keep this at 0, rolling to turn instead.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods

        public override void Draw(Unity.Flight.ISectionModule section) {
            this.DrawLine(AttitudeProcessor.SideslipAngle == AttitudeProcessor.INVALID_ANGLE ? "--" : Units.ToAngle(AttitudeProcessor.SideslipAngle, section.IsHud ? HudDecimalPlaces : DecimalPlaces), section);
        }

        public override void Reset() {
            FlightEngineerCore.Instance.AddUpdatable(AttitudeProcessor.Instance);
        }

        public override void Update() {
            AttitudeProcessor.RequestUpdate();
        }

        #endregion
    }
}