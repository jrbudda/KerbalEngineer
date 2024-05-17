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

    public class AngleOfAttack : ReadoutModule
    {
        #region Constructors

        public AngleOfAttack() {
            this.Name = "Angle-of-Attack";
            this.ShortName = "AoA";
            this.Category = ReadoutCategory.GetCategory("Vessel");
            this.HelpString = "The angle between velocity and the vessel's fuselage, along the vessel's vertical axis. Lift begins to decrease at 30 degrees, but you should generally keep it below 10, and below 5 in most situations.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods

        public override void Draw(Unity.Flight.ISectionModule section) {
            this.DrawLine(AttitudeProcessor.AttackAngle == AttitudeProcessor.INVALID_ANGLE ? "--" : Units.ToAngle(AttitudeProcessor.AttackAngle, section.IsHud ? HudDecimalPlaces : DecimalPlaces), section);
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