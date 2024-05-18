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

using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using KerbalEngineer.Flight.Readouts;

using UnityEngine;

#endregion

namespace KerbalEngineer.TrackingStation {
    using Flight.Presets;
    using Flight.Sections;
    using Unity.Flight;

    /// <summary>
    ///     Object for management and display of readout modules.
    /// </summary>
    public class SectionModuleTS : ISectionModule {
        #region Fields

        private SectionEditorTS editor;
        private bool isHud;
        private int numberOfReadouts;

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates a new section module.
        /// </summary>
        public SectionModuleTS() {
            this.FloatingPositionX = Screen.width * 0.5f - 125.0f;
            this.FloatingPositionY = 100.0f;
            this.EditorPositionX = Screen.width * 0.5f - SectionEditorTS.Width * 0.5f;
            this.EditorPositionY = Screen.height * 0.5f - SectionEditorTS.Height * 0.5f;
            this.ReadoutModules = new List<ReadoutModule>();
            this.InitialiseStyles();
            GuiDisplaySize.OnSizeChanged += this.OnSizeChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets and sets the abbreviation of the section.
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        ///     Gets and sets the X position of the editor window. (Only used for serialisation.)
        /// </summary>
        public float EditorPositionX { get; set; }

        /// <summary>
        ///     Gets and sets the Y position of the editor window. (Only used for serialisation.)
        /// </summary>
        public float EditorPositionY { get; set; }

        /// <summary>
        ///     Gets and sets the X position of the floating window. (Only used for serialisation.)
        /// </summary>
        public float FloatingPositionX { get; set; }

        /// <summary>
        ///     Gets and sets the Y position of the floating window. (Only used for serialisation.)
        /// </summary>
        public float FloatingPositionY { get; set; }

        /// <summary>
        ///     Gets and sets whether the section is custom.
        /// </summary>
        public bool IsCustom { get; set; }

        /// <summary>
        ///     Gets and sets whether to show the edit button
        /// </summary>
        public bool showEditButton { get; set; } = true;

        /// <summary>
        ///     Gets and sets whether to show the float button
        /// </summary>
        public bool showFloatButton { get; set; } = true;


        /// <summary>
        ///     Gets and sets whether the section editor is visible.
        /// </summary>
        public bool IsEditorVisible {
            get { return this.editor != null; }
            set {
                if (value && this.editor == null) {
                    this.editor = DisplayStackTS.Instance.MakeEditor();
                } else if (!value && this.editor != null) {
                    Object.Destroy(this.editor);
                    DisplayStackTS.editor = null;
                }
            }
        }

        /// <summary>
        ///     Gets and sets whether the section is in a floating state.
        /// </summary>
        public bool IsFloating {
            get { return this.Window != null; }
            set {
                if (value && this.Window == null) {
                   // this.Window = FlightEngineerCore.Instance.AddSectionWindow(this);
                } else if (!value && this.Window != null) {
                    Object.Destroy(this.Window);
                }
            }
        }

        /// <summary>
        ///     Gets and sets whether the section module is a HUD.
        /// </summary>
        public bool IsHud {
            get { return this.isHud; }
            set {
                if (this.isHud == value) {
                    return;
                }

                this.isHud = value;
                if (this.isHud) {
                    this.IsFloating = true;
                }
                if (this.Window != null) {
                    this.Window.RequestResize();
                }
            }
        }

        /// <summary>
        ///     Gets and sets whether the section module has been deleted.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        ///     Gets and sets whether the section module has a background as a HUD.
        /// </summary>
        public bool IsHudBackground { get; set; }

        /// <summary>
        ///     Gets and sets the visibility of the section.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        ///     Gets the number of drawn readout lines.
        /// </summary>
        public int LineCount { get; set; }

        /// <summary>
        ///     Gets and sets the width of the section. (Only used with serialisation.)
        /// </summary>
        public float Width { get; set; } = OOPSux.DEFAULT_SECTION_WIDTH;
        public float HudWidth { get; set; } = OOPSux.DEFAULT_SECTION_WIDTH;
        
        public Color HudBackgroundColor { get; set; } = OOPSux.DEFAULT_HUD_BACKGROUND_COLOR;

        /// <summary>
        ///     Gets and sets the name of the section.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets and sets the names of the installed readout modules. (Only used with serialisation.)
        /// </summary>
        public string[] ReadoutModuleNames {
            get { return this.ReadoutModules.Select(r => r.Category + "." + r.GetType().Name).ToArray(); }
            set { this.ReadoutModules = value.Select(ReadoutLibrary.GetReadout).ToList(); }
        }

        /// <summary>
        ///     Gets and sets the list of readout modules.
        /// </summary>
        [XmlIgnore]
        public List<ReadoutModule> ReadoutModules { get; set; }

        /// <summary>
        ///     Gets and sets the floating window.
        /// </summary>
        [XmlIgnore]
        public SectionWindow Window { get; set; }

        #endregion

        #region GUIStyles

        #region Fields

        private GUIStyle boxStyle;
        private GUIStyle buttonStyle;
        private GUIStyle messageStyle;
        private GUIStyle titleStyle;

        #endregion

        /// <summary>
        ///     Initialises all the styles required for this object.
        /// </summary>
        private void InitialiseStyles() {
            this.boxStyle = new GUIStyle(HighLogic.Skin.box) {
                margin = new RectOffset(),
                padding = new RectOffset(5, 5, 5, 5)
            };

            this.titleStyle = new GUIStyle(HighLogic.Skin.label) {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(2, 0, 5, 2),
                fontSize = (int)(13 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button) {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(0, 0, 5, 3),
                padding = new RectOffset(),
                fontSize = (int)(10 * GuiDisplaySize.Offset),
                stretchHeight = true,
                fixedWidth = 60.0f * GuiDisplaySize.Offset
            };

            this.messageStyle = new GUIStyle(HighLogic.Skin.label) {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleCenter,
                fontSize = (int)(12 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                fixedWidth = 220.0f * GuiDisplaySize.Offset,
                fixedHeight = 20.0f * GuiDisplaySize.Offset
            };
        }

        private void OnSizeChanged() {
            this.InitialiseStyles();
        }

        #endregion

        #region Updating

        /// <summary>
        ///     Updates all of the internal readout modules at fixed time intervals.
        /// </summary>
        public void FixedUpdate() {
            if (!this.IsVisible) {
                return;
            }

            foreach (var readout in this.ReadoutModules) {
                readout.FixedUpdate();
            }
        }

        /// <summary>
        ///     Updates all of the internal readout modules.
        /// </summary>
        public void Update() {
            if (!this.IsVisible) {
                return;
            }

            foreach (var readout in this.ReadoutModules) {
                readout.Update();
            }

            if (this.numberOfReadouts != this.ReadoutModules.Count) {
                this.numberOfReadouts = this.ReadoutModules.Count;
                if (!this.IsFloating) {
                    DisplayStackTS.Instance.RequestResize();
                } else {
                    this.Window.RequestResize();
                }
            }
        }

        #endregion

        #region Drawing

        #region Methods: public

        /// <summary>
        ///     Draws the section and all of the internal readout modules.
        /// </summary>
        public void Draw() {
            if (!this.IsVisible) {
                return;
            }

            if (!this.IsHud) {
                this.DrawSectionTitleBar();
            }

            this.DrawReadoutModules();
        }

        #endregion

        #region Methods: private

        /// <summary>
        ///     Draws all the readout modules.
        /// </summary>
        private void DrawReadoutModules() {
            if (!this.IsHud) {
                GUILayout.BeginVertical(this.boxStyle);
            }

            this.LineCount = 0;
            if (this.ReadoutModules.Count > 0) {
                foreach (var readout in this.ReadoutModules) {
                    readout.LineCountStart();
                    readout.Draw(this);
                    readout.LineCountEnd();
                    this.LineCount += readout.LineCount;
                }
            } else {
                GUILayout.Label("No readouts are installed.", this.messageStyle);
                this.LineCount = 1;
            }

            if (!this.IsHud) {
                GUILayout.EndVertical();
            }
        }

        /// <summary>
        ///     Draws the section title and action buttons.
        /// </summary>
        private void DrawSectionTitleBar() {
            GUILayout.BeginHorizontal();
            GUILayout.Label(this.Name.ToUpper(), this.titleStyle);
            if (showEditButton) this.IsEditorVisible = GUILayout.Toggle(this.IsEditorVisible, "EDIT", this.buttonStyle);
            if (showFloatButton) this.IsFloating = GUILayout.Toggle(this.IsFloating, "FLOAT", this.buttonStyle);
            GUILayout.EndHorizontal();
        }

        #endregion

        #endregion

        #region Public Methods

        public void ClearNullReadouts() {
            this.ReadoutModules.RemoveAll(r => r == null);
        }

        public void ApplyPreset(Preset preset) {
            if (preset == null) return;
            this.Name = preset.Name;
            this.Abbreviation = preset.Abbreviation;
            this.ReadoutModuleNames = preset.ReadoutNames;
            this.IsHud = preset.IsHud;
            this.IsHudBackground = preset.IsHudBackground;
        }

        #endregion
    }
}