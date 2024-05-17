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
using KerbalEngineer.Settings;
using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Readouts {
    using Extensions;
    using Unity.Flight;

    public abstract class ReadoutModule {
        #region Fields

        private int lineCountEnd;
        private int lineCountStart;

        #endregion

        #region Constructors

        protected ReadoutModule() {
            this.InitialiseStyles(false);
            GuiDisplaySize.OnSizeChanged += this.OnSizeChanged;
        }

        #endregion

        #region Properties


        /// <summary>
        ///     Gets ans sets the readout category.
        /// </summary>
        public ReadoutCategory Category { get; set; }

        /// <summary>
        ///     Gets and sets whether the readout can be added to a section multiple times.
        /// </summary>
        public bool Cloneable { get; set; }

        /// <summary>
        ///     Gets the width of the content. (Sum of NameStyle + ValueStyle widths.)
        /// </summary>
        public float ContentWidth {
            get { return OOPSux.DEFAULT_SECTION_WIDTH * GuiDisplaySize.Offset; }
        }

        /// <summary>
        ///     Gets and sets the flexible label style.
        /// </summary>
        public GUIStyle FlexiLabelStyle { get; set; }

        /// <summary>
        ///     Gets and sets the help string which is shown in the editor.
        /// </summary>
        public string HelpString { get; set; }

        /// <summary>
        ///     Gets and sets whether the readout should be shown on new installs.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        ///     Gets the number of drawn lines.
        /// </summary>
        public int LineCount { get; private set; }
        
        /// <summary>
        ///     Gets and sets the readout character limit. Displayed value strings will be truncated if they're longer than this.
        /// </summary>
        public int CharacterLimit { get; set; } = ReadoutModuleConfigNode.DEFAULT_CHARACTER_LIMIT;
        public int HudCharacterLimit { get; set; } = ReadoutModuleConfigNode.DEFAULT_CHARACTER_LIMIT;

        /// <summary>
        ///     Gets and sets the readout decimal-place override for floating-point value readouts displayed in the main window. Negative values will use the default number of decimal places for that type of unit.
        /// </summary>
        public int DecimalPlaces { get; set; } = -9000;

        /// <summary>
        ///     Gets and sets the readout decimal-place override for floating-point value readouts displayed in the HUD. Negative values will use the default number of decimal places for that type of unit.
        /// </summary>
        public int HudDecimalPlaces { get; set; } = -9000;

        /// <summary>
        ///     Gets and sets whether to display the short name in readouts rather than the full one.
        /// </summary>
        public bool UseShortName { get; set; } = false;
        public bool HudUseShortName { get; set; } = false;

        /// <summary>
        ///     Gets and sets the readout name.
        /// </summary>
        public string Name { get; set; }
        public string ShortName { get; set; }

        /// <summary>
        ///     Gets and sets whether the readout has requested a section resize.
        /// </summary>
        public bool ResizeRequested { get; set; }

        /// <summary>
        ///     Gets and sets whether the help string is being shown in the editor.
        /// </summary>
        public bool ShowHelp { get; set; }


        /// <summary>
        ///     Gets and sets the text field style.
        /// </summary>
        public GUIStyle TextFieldStyle { get; set; }

        /// <summary>
        ///     Gets and sets the value style.
        /// </summary>
        public GUIStyle ValueStyle { get; set; }

        /// <summary>
        ///     Gets and sets the name style.
        /// </summary>
        public GUIStyle NameStyle { get; set; }

        /// <summary>
        ///     Gets and sets the message style.
        /// </summary>
        public GUIStyle MessageStyle { get; set; }

        /// <summary>
        ///     Gets and sets the button style.
        /// </summary>
        public GUIStyle ButtonStyle { get; set; }

        /// <summary>
        ///     Gets and sets the button style.
        /// </summary>
        public GUIStyle CompactButtonStyle { get; set; }

        #endregion

        #region Methods: public

        /// <summary>
        ///     Called when a readout is asked to draw its self.
        /// </summary>
        public virtual void Draw(Unity.Flight.ISectionModule section) { }

        /// <summary>
        ///     Called on each fixed update frame where the readout is visible.
        /// </summary>
        public virtual void FixedUpdate() { }

        public void LineCountEnd() {
            this.LineCount = this.lineCountEnd;
            if (this.lineCountEnd.CompareTo(this.lineCountStart) < 0) {
                this.ResizeRequested = true;
            }
        }

        public void LineCountStart() {
            this.lineCountStart = this.lineCountEnd;
            this.lineCountEnd = 0;
        }

        /// <summary>
        ///     Called when FlightEngineerCore is started.
        /// </summary>
        public virtual void Reset() { }

        /// <summary>
        ///     Called on each update frame when the readout is visible.
        /// </summary>
        public virtual void Update() { }

        #endregion

        #region Methods: protected

        protected void DrawLine(string value, Unity.Flight.ISectionModule section) {
            if (!section.IsHud) {
                GUILayout.BeginHorizontal(GUILayout.Width(section.Width * GuiDisplaySize.Offset));
                GUILayout.Label((this.UseShortName && !string.IsNullOrEmpty(this.ShortName)) ? this.ShortName : this.Name, NameStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(value.ToLength(CharacterLimit), ValueStyle);
            } else {
                GUILayout.BeginHorizontal(GUILayout.Width(section.HudWidth * GuiDisplaySize.Offset));
                GUILayout.Label((this.HudUseShortName && !string.IsNullOrEmpty(this.ShortName)) ? this.ShortName : this.Name, NameStyle, GUILayout.Height(NameStyle.fontSize * 1.2f));
                GUILayout.FlexibleSpace();
                GUILayout.Label(value.ToLength(HudCharacterLimit), ValueStyle, GUILayout.Height(ValueStyle.fontSize * 1.2f));
            }
            GUILayout.EndHorizontal();

            this.lineCountEnd++;
        }

        protected void DrawLine(string name, string value, Unity.Flight.ISectionModule section) {
            if (!section.IsHud) {
                GUILayout.BeginHorizontal(GUILayout.Width(section.Width * GuiDisplaySize.Offset));
                GUILayout.Label(name, NameStyle);
                GUILayout.FlexibleSpace();
                GUILayout.Label(value.ToLength(CharacterLimit), ValueStyle);
            } else {
                GUILayout.BeginHorizontal(GUILayout.Width(section.HudWidth * GuiDisplaySize.Offset));
                GUILayout.Label(name, NameStyle, GUILayout.Height(NameStyle.fontSize * 1.2f));
                GUILayout.FlexibleSpace();
                GUILayout.Label(value.ToLength(HudCharacterLimit), ValueStyle, GUILayout.Height(ValueStyle.fontSize * 1.2f));
            }
            GUILayout.EndHorizontal();
            this.lineCountEnd++;
        }

        protected void DrawLine(Action drawAction, Unity.Flight.ISectionModule section, bool showName = true) {
            GUILayout.BeginHorizontal(GUILayout.Width((section.IsHud ? section.HudWidth : section.Width) * GuiDisplaySize.Offset));
            if (showName) {
                if (!section.IsHud) {
                    GUILayout.Label((this.UseShortName && !string.IsNullOrEmpty(this.ShortName)) ? this.ShortName : this.Name, NameStyle);
                } else {
                    GUILayout.Label((this.HudUseShortName && !string.IsNullOrEmpty(this.ShortName)) ? this.ShortName : this.Name, NameStyle, GUILayout.Height(NameStyle.fontSize * 1.2f));
                }
                GUILayout.FlexibleSpace();
            }
            drawAction();
            GUILayout.EndHorizontal();
            this.lineCountEnd++;
        }

        protected void DrawMessageLine(string value, float width, bool compact = false) {
            GUILayout.BeginHorizontal(GUILayout.Width(width * GuiDisplaySize.Offset));
            if (!compact) {
                GUILayout.Label(value, MessageStyle);
            } else {
                GUILayout.Label(value, MessageStyle, GUILayout.Height(MessageStyle.fontSize * 1.2f));
            }
            GUILayout.EndHorizontal();
            this.lineCountEnd++;
        }

        #endregion

        #region Methods: private

        /// <summary>
        ///     Initialises all the styles required for this object.
        /// </summary>
        private void InitialiseStyles(bool force) {

            if (NameStyle != null && !force) return;

            ReadoutModule existing = ReadoutLibrary.GetReadout(this.Name);
            Color c = HighLogic.Skin.label.normal.textColor;
            if (existing != null)
                c = existing.ValueStyle.normal.textColor;

            NameStyle = new GUIStyle(HighLogic.Skin.label) {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(5, 0, 0, 0),
                alignment = TextAnchor.MiddleLeft,
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                fixedHeight = 20.0f * GuiDisplaySize.Offset
            };

            ValueStyle = new GUIStyle(HighLogic.Skin.label) {
                margin = new RectOffset(),
                padding = new RectOffset(0, 5, 0, 0),
                alignment = TextAnchor.MiddleRight,
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Normal,
                fixedHeight = 20.0f * GuiDisplaySize.Offset,
            };

            MessageStyle = new GUIStyle(HighLogic.Skin.label) {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleCenter,
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Normal,
                fixedHeight = 20.0f * GuiDisplaySize.Offset,
                stretchWidth = true
            };

            FlexiLabelStyle = new GUIStyle(NameStyle) {
                fixedWidth = 0,
                stretchWidth = true
            };

            ButtonStyle = new GUIStyle(HighLogic.Skin.button) {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(0, 0, 1, 1),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleCenter,
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fixedHeight = 18.0f * GuiDisplaySize.Offset
            };

            CompactButtonStyle = new GUIStyle(ButtonStyle) {
                fontSize = (int)(10 * GuiDisplaySize.Offset),
                margin = new RectOffset(0, 0, 5, 5),
                fixedHeight = ButtonStyle.fontSize
            };

            TextFieldStyle = new GUIStyle(HighLogic.Skin.textField) {
                margin = new RectOffset(0, 0, 1, 1),
                padding = new RectOffset(5, 5, 0, 0),
                alignment = TextAnchor.MiddleLeft,
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fixedHeight = 18.0f * GuiDisplaySize.Offset
            };


           this.ValueStyle.normal.textColor = c;
  

        }

        private void OnSizeChanged() {
            this.InitialiseStyles(true);
            this.ResizeRequested = true;
        }

        #endregion
    }
}