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

using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Sections;

#endregion

namespace KerbalEngineer.Flight.Readouts.Orbital
{
    public class AngleToEquatorialDescendingNode : ReadoutModule
    {
        #region Constructors

        public AngleToEquatorialDescendingNode()
        {
            this.Name = "Angle to Equ. DN";
            this.Category = ReadoutCategory.GetCategory("Orbital");
            this.HelpString = "Angular Distance from the vessel to crossing the Equator of the central body, going south of it.";
            this.IsDefault = false;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            this.DrawLine(OrbitExtensions.GetAngleToDescendingNode(FlightGlobals.ActiveVessel.orbit).ToAngle(), section.Width, section.IsHud);
        }

        #endregion
    }
}