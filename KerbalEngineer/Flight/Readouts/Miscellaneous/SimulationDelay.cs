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
using UnityEngine;

using KerbalEngineer.Flight.Sections;
using KerbalEngineer.VesselSimulator;

#endregion

namespace KerbalEngineer.Flight.Readouts.Miscellaneous
{
    public class SimulationDelay : ReadoutModule
    {
        private static readonly GUIStyle sliderStyle = new GUIStyle(HighLogic.Skin.horizontalSlider) {
            margin = new RectOffset(5, 0, 6, 6),
            stretchHeight = false
        };
        private static readonly GUIStyle sliderThumbStyle = new GUIStyle(HighLogic.Skin.horizontalSliderThumb);

        #region Constructors

        public SimulationDelay()
        {
            this.Name = "Minimum Simulation Delay";
            this.ShortName = "Sim Delay";
            this.UseShortName = this.HudUseShortName = true;
            this.Category = ReadoutCategory.GetCategory("Miscellaneous");
            this.HelpString = "Controls the minimum delay between processing vessel simulations required for certain readouts. Does not affect the game's \"Max Physics Delta-Time per Frame\" option.";
            this.IsDefault = true;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            if (!section.IsHud) {
                GUILayout.BeginHorizontal(GUILayout.Width(section.Width * GuiDisplaySize.Offset));
                if (!this.HideName) GUILayout.Label((this.UseShortName && !string.IsNullOrEmpty(this.ShortName)) ? this.ShortName : this.Name, NameStyle);
            } else {
                GUILayout.BeginHorizontal(GUILayout.Width(section.HudWidth * GuiDisplaySize.Offset));
                if (!this.HudHideName && !section.HideHudReadoutNames) GUILayout.Label((this.HudUseShortName && !string.IsNullOrEmpty(this.ShortName)) ? this.ShortName : this.Name, NameStyle, GUILayout.Height(NameStyle.fontSize * 1.2f));
            }
            //GUILayout.BeginVertical();
            //GUILayout.FlexibleSpace(); //Doesn't work :(
            SimManager.minSimTime = TimeSpan.FromMilliseconds(GUILayout.HorizontalSlider((float)SimManager.minSimTime.TotalMilliseconds, 0.0f, 2000.0f, sliderStyle, sliderThumbStyle));
            //GUILayout.FlexibleSpace();
            //GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        #endregion
    }
}