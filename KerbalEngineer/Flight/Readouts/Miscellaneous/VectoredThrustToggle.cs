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

namespace KerbalEngineer.Flight.Readouts.Miscellaneous
{
    #region Using Directives

    using Sections;
    using UnityEngine;
    using VesselSimulator;
    using KSP.Localization;

    #endregion

    public class VectoredThrustToggle : ReadoutModule
    {
        #region Constructors

        public VectoredThrustToggle()
        {
            this.Name = Localizer.Format("#KE_VectoredThrustToggle");//"Vectored Thrust"
            this.Category = ReadoutCategory.GetCategory("Miscellaneous");
            this.HelpString = Localizer.Format("#KE_VectoredThrustToggle_desc");//"Shows a control that will allow you to adjust whether the vessel simulation should account for vectored thrust."
            this.IsDefault = false;
        }

        #endregion

        #region Methods

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Vectored Thrust: ", this.NameStyle);
            SimManager.vectoredThrust = GUILayout.Toggle(SimManager.vectoredThrust, "ENABLED", this.ButtonStyle);
            GUILayout.EndHorizontal();
        }

        #endregion
    }
}