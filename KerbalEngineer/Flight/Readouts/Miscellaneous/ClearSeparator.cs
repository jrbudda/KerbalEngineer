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
using KSP.Localization;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Readouts.Miscellaneous
{
    public class ClearSeparator : ReadoutModule
    {
        #region Fields

        private GUIStyle boxStyle;
        private GUIStyle boxStyleHud;

        #endregion

        #region Constructors

        public ClearSeparator()
        {
            this.Name = Localizer.Format("#KE_ClearSeparator");//"Clear Separator"
            this.Category = ReadoutCategory.GetCategory("Miscellaneous");
            this.HelpString = Localizer.Format("#KE_ClearSeparator_desc");//"Creats a space to help seperate subsections in a module."
            this.IsDefault = false;
            this.Cloneable = true;

            this.InitialiseStyles();

            GuiDisplaySize.OnSizeChanged += this.InitialiseStyles;
        }

        #endregion

        #region Methods: public

        public override void Draw(Unity.Flight.ISectionModule section)
        {
            GUILayout.Box(String.Empty, section.IsHud ? this.boxStyleHud : this.boxStyle);
        }

        #endregion

        #region Methods: private

        private void InitialiseStyles()
        {
            this.boxStyle = new GUIStyle
            {
                fixedHeight = 1.0f,
                stretchWidth = true
            };

            this.boxStyleHud = new GUIStyle(this.boxStyle)
            {
                margin = new RectOffset(0, 0, (int)(8 * GuiDisplaySize.Offset), 0)
            };
        }

        #endregion
    }
}