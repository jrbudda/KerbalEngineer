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

using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Presets;
using KerbalEngineer.Flight.Readouts;
using KerbalEngineer.UIControls;

using UnityEngine;

#endregion

namespace KerbalEngineer.TrackingStation {
    public class SectionEditorTS : Flight.Sections.SectionEditor {
        /// <summary>
        ///     Called to draw the editor when the UI is enabled.
        /// </summary>
        protected override void OnGUI() {
            if (HighLogic.LoadedScene != GameScenes.TRACKSTATION) {
                return;
            }

            this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, "EDIT SECTION – " + this.ParentSection.Name.ToUpper(), this.windowStyle).ClampToScreen();
            this.ParentSection.EditorPositionX = this.position.x;
            this.ParentSection.EditorPositionY = this.position.y;
        }

        /// <summary>
        ///     Draws the categories list drop down UI.
        /// </summary>
        protected override void DrawCategories() {
            foreach (var category in ReadoutCategory.Categories) {
                if (category.Name != "Rendezvous" && category.Name != "Miscellaneous") continue;

                var description = category.Description;
                if (description.Length > 50) {
                    description = description.Substring(0, 50 - 1) + "...";
                }

                if (GUILayout.Button("<b>" + category.Name.ToUpper() + "</b>" + (string.IsNullOrEmpty(category.Description) ? string.Empty : "\n<i>" + description + "</i>"), category == ReadoutCategory.Selected ? this.categoryButtonActiveStyle : this.categoryButtonStyle)) {
                    ReadoutCategory.Selected = category;
                    this.categoryList.enabled = false;
                }
            }
        }

        /// <summary>
        ///     Draws the options for editing custom sections.
        /// </summary>
        protected override void DrawCustomOptions() {}

        /// <summary>
        ///     Draws the presetsList selection list.
        /// </summary>
        protected override void DrawPresetSelector() {}
    }
}