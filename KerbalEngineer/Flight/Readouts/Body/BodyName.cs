// Kerbal Engineer Redux
//
// Copyright (C) 2014 CYBUTEK
//
// This program is free software: you can redistribute it and/or modify it under the terms of the GNU
// General Public License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program. If not,
// see <http://www.gnu.org/licenses/>.

using KerbalEngineer.Flight.Sections;
using KSP.Localization;

namespace KerbalEngineer.Flight.Readouts.Body {
    public class BodyName : ReadoutModule {
        public BodyName() {
            Name = Localizer.Format("#KE_BodyName");//"Current Body Name"
            Category = ReadoutCategory.GetCategory("Body");
            HelpString = Localizer.Format("#KE_BodyName_desc");//"Shows the name of the current body."
            IsDefault = false;
        }

        public override void Draw(Unity.Flight.ISectionModule section) {
            if (FlightGlobals.ActiveVessel.mainBody == null)
                DrawLine("N/A", section.IsHud);
            else
                DrawLine(FlightGlobals.ActiveVessel.mainBody.bodyName, section.IsHud);
        }
    }
}