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
using KerbalEngineer.VesselSimulator;

using UnityEngine;
using KSP.Localization;

#endregion

namespace KerbalEngineer.Flight.Readouts.Miscellaneous
{
    using System;

    public class SimulationDelay : ReadoutModule
    {
        #region Constructors

        public SimulationDelay()
        {
            this.Name = Localizer.Format("#KE_SimulationDelay");//"Minimum Simulation Delay"
            this.Category = ReadoutCategory.GetCategory("Miscellaneous");
            this.HelpString = Localizer.Format("#KE_SimulationDelay_desc");//"Controls the minimum delay between processing vessel simulations."
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Sim Delay", this.NameStyle);
            GUI.skin = HighLogic.Skin;
            SimManager.minSimTime = TimeSpan.FromMilliseconds(GUILayout.HorizontalSlider((float)SimManager.minSimTime.TotalMilliseconds, 0, 2000.0f));
            GUI.skin = null;
            GUILayout.EndHorizontal();
        }

        #endregion
    }
}