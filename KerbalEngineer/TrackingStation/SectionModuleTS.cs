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

using UnityEngine;

#endregion

namespace KerbalEngineer.TrackingStation {
    using Flight.Sections;

    /// <summary>
    ///     Object for management and display of readout modules.
    /// </summary>
    public class SectionModuleTS : SectionModule {
        #region Properties

        /// <summary>
        ///     Gets and sets whether the section is custom.
        /// </summary>
        public bool IsCustom { get; set; }

        /// <summary>
        ///     Gets and sets whether the section editor is visible.
        /// </summary>
        public override bool IsEditorVisible {
            get { return this.editor != null; }
            set {
                if (value && this.editor == null) {
                    Debug.Log("[FlightEngineer]: Creating editor");
                    this.editor = DisplayStackTS.Instance.MakeEditor();
                    Debug.Log("[FlightEngineer]: Created editor");
                } else if (!value && this.editor != null) {
                    Debug.Log("[FlightEngineer]: Destroying editor");
                    Object.Destroy(this.editor);
                    DisplayStackTS.editor = null;
                    Debug.Log("[FlightEngineer]: Created editor");
                }
            }
        }

        /// <summary>
        ///     Gets and sets whether the section is in a floating state.
        /// </summary>
        public override bool IsFloating {
            get { return this.Window != null; }
            set {
                if (value && this.Window == null) {
                   // this.Window = FlightEngineerCore.Instance.AddSectionWindow(this);
                } else if (!value && this.Window != null) {
                    Object.Destroy(this.Window);
                }
            }
        }

        #endregion
    }
}