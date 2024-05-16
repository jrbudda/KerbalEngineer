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
using System;

#endregion

namespace KerbalEngineer.Flight.Readouts.Body {
    public class GeostationaryHeight : ReadoutModule {
        #region Constructors

        public GeostationaryHeight() {
            this.Name = "Synchronous Alt.";
            this.Category = ReadoutCategory.GetCategory("Body");
            this.HelpString = "The altitude where the orbital period equals the body's rotation period.";
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section) {
            var T = FlightGlobals.currentMainBody.rotationPeriod;
            var geo = System.Math.Pow(T * T * FlightGlobals.currentMainBody.gravParameter / (4 * Math.PI * Math.PI), 1.0 / 3.0);
            this.DrawLine((geo - FlightGlobals.currentMainBody.Radius).ToDistance(), section);
        }

        #endregion
    }
}