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
using KSP.Localization;

#endregion

namespace KerbalEngineer.TrackingStation {
    public class SectionEditorTS : MonoBehaviour {
        #region Constants

        public const float Height = 500.0f;
        public const float Width = 500.0f;

        #endregion

        #region Fields

        private GUIStyle categoryButtonActiveStyle;
        private GUIStyle categoryButtonStyle;
        private PopOutElement categoryList;
        private PopOutColorPicker colorPicker;
        private GUIStyle categoryTitleButtonStyle;
        private GUIStyle helpBoxStyle;
        private GUIStyle helpTextStyle;
        private GUIStyle panelTitleStyle;
        private Rect position;
        private PopOutElement presetList;
        private GUIStyle readoutButtonStyle;
        private GUIStyle readoutNameStyle;
        private Vector2 scrollPositionAvailable;
        private Vector2 scrollPositionInstalled;
        private GUIStyle textStyle;
        private GUIStyle windowStyle;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets and sets the parent section for the section editor.
        /// </summary>
        public SectionModuleTS ParentSection { get; set; }

        /// <summary>
        ///     Gets and sets the window position.
        /// </summary>
        public Rect Position {
            get { return this.position; }
            set { this.position = value; }
        }

        #endregion

        #region Methods: protected

        protected void Awake() {
            try {
                this.categoryList = this.gameObject.AddComponent<PopOutElement>();
                this.categoryList.DrawCallback = this.DrawCategories;
                this.presetList = this.gameObject.AddComponent<PopOutElement>();
                this.presetList.DrawCallback = this.DrawPresets;
                this.colorPicker = this.gameObject.AddComponent<PopOutColorPicker>();
                this.colorPicker.DrawCallback = this.DrawColorPicker;
                this.colorPicker.ClosedCallback = this.saveColor;
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        /// <summary>
        ///     Runs when the object is destroyed.
        /// </summary>
        protected void OnDestroy() {
            try {
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        /// <summary>
        ///     Initialises the object's state on creation.
        /// </summary>
        protected void Start() {
            try {
                this.InitialiseStyles();
                //ReadoutCategory.Selected = ReadoutCategory.GetCategory("Orbital");
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        #endregion

        #region Methods: private

        private void saveColor() {
            if (editingReadout == null) return;
            if (editingReadout.ValueStyle.normal.textColor == HighLogic.Skin.label.normal.textColor)
                ReadoutLibrary.RemoveReadoutConfig(editingReadout);
            else
                ReadoutLibrary.SaveReadoutConfig(editingReadout);
        }

        /// <summary>
        ///     Called to draw the editor when the UI is enabled.
        /// </summary>
        private void OnGUI() {
            this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, Localizer.Format("#KE_SECTIONEDIT_title", this.ParentSection.Name.ToUpper()), this.windowStyle).ClampToScreen();//"EDIT SECTION - " + 
            this.ParentSection.EditorPositionX = this.position.x;
            this.ParentSection.EditorPositionY = this.position.y;
        }

        /// <summary>
        ///     Draws the available readouts panel.
        /// </summary>
        private void DrawAvailableReadouts() {
            GUI.skin = HighLogic.Skin;
            this.scrollPositionAvailable = GUILayout.BeginScrollView(this.scrollPositionAvailable, false, true, GUILayout.Height(200.0f));
            GUI.skin = null;

            GUILayout.Label(Localizer.Format("#KE_ReadoutsAvailable"), this.panelTitleStyle);//"AVAILABLE"

            foreach (var readout in ReadoutLibrary.GetCategory(ReadoutCategory.Selected)) {
                if (!this.ParentSection.ReadoutModules.Contains(readout) || readout.Cloneable) {
                    GUILayout.BeginHorizontal(GUILayout.Height(30.0f));
                    GUILayout.Label(readout.Name, this.readoutNameStyle);
                    readout.ShowHelp = GUILayout.Toggle(readout.ShowHelp, "?", this.readoutButtonStyle, GUILayout.Width(30.0f));
                    if (GUILayout.Button(Localizer.Format("#KE_ReadoutsINSTALL"), this.readoutButtonStyle, GUILayout.Width(75.0f))) {//"INSTALL"
                        this.ParentSection.ReadoutModules.Add(readout);
                    }
                    GUILayout.EndHorizontal();

                    this.ShowHelpMessage(readout);
                }
            }

            GUILayout.EndScrollView();
        }

        /// <summary>
        ///     Draws the categories list drop down UI.
        /// </summary>
        private void DrawCategories() {
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
        ///     Draws the readoutCategories selection list.
        /// </summary>
        private void DrawCategorySelector() {
            this.categoryList.enabled = GUILayout.Toggle(this.categoryList.enabled, Localizer.Format("#KE_CategorySelector", ReadoutCategory.Selected.ToString().ToUpper()), this.categoryTitleButtonStyle);//"▼ SELECTED CATEGORY: " +  + " ▼"
            if (Event.current.type == EventType.Repaint) {
                this.categoryList.SetPosition(GUILayoutUtility.GetLastRect().Translate(this.position), GUILayoutUtility.GetLastRect());
            }
        }

        /// <summary>
        ///     Draws the options for editing custom sections.
        /// </summary>
        private void DrawCustomOptions() {
            //GUILayout.BeginHorizontal(GUILayout.Height(25.0f));
            //this.ParentSection.Name = GUILayout.TextField(this.ParentSection.Name, this.textStyle);
            //var isShowingInControlBar = !string.IsNullOrEmpty(this.ParentSection.Abbreviation);
            //this.ParentSection.Abbreviation = GUILayout.TextField(this.ParentSection.Abbreviation, this.textStyle, GUILayout.Width(75.0f));

            //ParentSection.IsHud = GUILayout.Toggle(this.ParentSection.IsHud, "HUD", this.readoutButtonStyle, GUILayout.Width(50.0f));
            //if (ParentSection.IsHud) {
            //    this.ParentSection.IsHudBackground = GUILayout.Toggle(this.ParentSection.IsHudBackground, "BG", this.readoutButtonStyle, GUILayout.Width(50.0f));
            //}

            //if (this.ParentSection.IsCustom) {
            //    if (isShowingInControlBar && string.IsNullOrEmpty(this.ParentSection.Abbreviation)) {
            //        DisplayStackTS.Instance.RequestResize();
            //    }

            //    if (GUILayout.Button("DELETE SECTION", this.readoutButtonStyle, GUILayout.Width(150.0f))) {
            //        this.ParentSection.IsFloating = false;
            //        this.ParentSection.IsEditorVisible = false;
            //        this.ParentSection.IsDeleted = true;
            //        SectionLibraryTS.CustomSections.Remove(this.ParentSection);
            //        DisplayStack.Instance.RequestResize();
            //    }
            //}
            //GUILayout.EndHorizontal();
        }

        Texture2D swatch = new Texture2D(16, 20);
        private Rect scrollRectInstalled = new Rect();

        /// <summary>
        ///     Draws the installed readouts panel.
        /// </summary>
        private void DrawInstalledReadouts() {
            GUI.skin = HighLogic.Skin;
            this.scrollPositionInstalled = GUILayout.BeginScrollView(this.scrollPositionInstalled, false, true);
            GUI.skin = null;

            GUILayout.Label(Localizer.Format("#KE_ReadoutsINSTALLED"), this.panelTitleStyle);//"INSTALLED"
            var removeReadout = false;
            var removeReadoutIndex = 0;

            for (var i = 0; i < this.ParentSection.ReadoutModules.Count; i++) {
                var readout = this.ParentSection.ReadoutModules[i];

                GUILayout.BeginHorizontal(GUILayout.Height(30.0f));
                GUILayout.Label(readout.Name, this.readoutNameStyle);
                if (GUILayout.Button("▲", this.readoutButtonStyle, GUILayout.Width(30.0f))) {
                    if (i > 0) {
                        this.ParentSection.ReadoutModules[i] = this.ParentSection.ReadoutModules[i - 1];
                        this.ParentSection.ReadoutModules[i - 1] = readout;
                    }
                }
                if (GUILayout.Button("▼", this.readoutButtonStyle, GUILayout.Width(30.0f))) {
                    if (i < this.ParentSection.ReadoutModules.Count - 1) {
                        this.ParentSection.ReadoutModules[i] = this.ParentSection.ReadoutModules[i + 1];
                        this.ParentSection.ReadoutModules[i + 1] = readout;
                    }
                }

                Color temp = GUI.color;

                GUI.color = readout.ValueStyle.normal.textColor;

                if (readout.Cloneable == false) {
                    if (GUILayout.Button(swatch, this.readoutButtonStyle, GUILayout.Width(30.0f))) {
                        editingReadout = readout;
                        colorPicker.enabled = true;
                    }

                    if (Event.current.type == EventType.Repaint && editingReadout == readout) {
                        colorPicker.SetPosition(GUILayoutUtility.GetLastRect().Translate(this.position).Translate(new Rect(8, scrollRectInstalled.y - scrollPositionInstalled.y, 8, 8)), new Rect(0, 0, 180, 20));
                    }

                } else { //dont show for separators.
                    GUILayout.Label("", GUILayout.Width(26.0f));
                }


                GUI.color = temp;

                readout.ShowHelp = GUILayout.Toggle(readout.ShowHelp, "?", this.readoutButtonStyle, GUILayout.Width(30.0f));
                if (GUILayout.Button(Localizer.Format("#KE_ReadoutsREMOVE"), this.readoutButtonStyle, GUILayout.Width(75.0f))) {//"REMOVE"
                    removeReadout = true;
                    removeReadoutIndex = i;
                }
                GUILayout.EndHorizontal();

                this.ShowHelpMessage(readout);
            }

            GUILayout.EndScrollView();

            if (Event.current.type == EventType.Repaint) {
                scrollRectInstalled = GUILayoutUtility.GetLastRect();
            }

            if (removeReadout) {
                this.ParentSection.ReadoutModules.RemoveAt(removeReadoutIndex);
            }
        }

        private void DrawPresetButton(Preset preset) {
            if (!GUILayout.Button("<b>" + preset.Name.ToUpper() + "</b>", this.categoryButtonStyle)) {
                return;
            }

            this.ParentSection.ApplyPreset(preset);

            this.presetList.enabled = false;
        }

        private void DrawPresetSaveButton() {
            if (!GUILayout.Button(Localizer.Format("#KE_PresetSaveButton"), this.categoryButtonStyle)) {//"<b>SAVE PRESET</b>"
                return;
            }

            this.SavePreset(PresetLibrary.Presets.Find(p => String.Equals(p.Name, this.ParentSection.Name, StringComparison.CurrentCultureIgnoreCase)));
        }

        /// <summary>
        ///     Draws the presetsList selection list.
        /// </summary>
        private void DrawPresetSelector() {
            //this.presetList.enabled = GUILayout.Toggle(this.presetList.enabled, "▼ PRESETS ▼", this.categoryTitleButtonStyle, GUILayout.Width(150.0f));
            //if (Event.current.type == EventType.repaint) {
            //    this.presetList.SetPosition(GUILayoutUtility.GetLastRect().Translate(this.position));
            //}
        }

        /// <summary>
        ///     Draws the preset list drop down UI.
        /// </summary>
        private void DrawPresets() {
            Preset removePreset = null;
            foreach (var preset in PresetLibrary.Presets) {
                GUILayout.BeginHorizontal();
                this.DrawPresetButton(preset);
                if (GUILayout.Button("<b>X</b>", this.categoryButtonStyle, GUILayout.Width(30.0f))) {
                    removePreset = preset;
                }
                GUILayout.EndHorizontal();
            }
            if (removePreset != null && PresetLibrary.Remove(removePreset)) {
                this.presetList.Resize = true;
            }

            this.DrawPresetSaveButton();
        }

        private ReadoutModule editingReadout = null;

        /// <summary>
        ///     Initialises all the styles required for this object.
        /// </summary>
        private void InitialiseStyles() {
            this.windowStyle = new GUIStyle(HighLogic.Skin.window);

            this.categoryButtonStyle = new GUIStyle(HighLogic.Skin.button) {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(0, 0, 2, 0),
                padding = new RectOffset(5, 5, 5, 5),
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Normal,
                richText = true
            };

            this.categoryButtonActiveStyle = new GUIStyle(this.categoryButtonStyle) {
                normal = this.categoryButtonStyle.onNormal,
                hover = this.categoryButtonStyle.onHover
            };

            this.panelTitleStyle = new GUIStyle(HighLogic.Skin.label) {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                fixedHeight = 30.0f,
                stretchWidth = true
            };

            this.textStyle = new GUIStyle(HighLogic.Skin.textField) {
                margin = new RectOffset(3, 3, 3, 3),
                alignment = TextAnchor.MiddleLeft,
                stretchWidth = true,
                stretchHeight = true
            };

            this.readoutNameStyle = new GUIStyle(HighLogic.Skin.label) {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(10, 0, 0, 0),
                alignment = TextAnchor.MiddleLeft,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                stretchWidth = true,
                stretchHeight = true
            };

            this.readoutButtonStyle = new GUIStyle(HighLogic.Skin.button) {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(2, 2, 2, 2),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                stretchHeight = true
            };

            this.helpBoxStyle = new GUIStyle(HighLogic.Skin.box) {
                margin = new RectOffset(2, 2, 2, 10),
                padding = new RectOffset(10, 10, 10, 10)
            };

            this.helpTextStyle = new GUIStyle(HighLogic.Skin.label) {
                normal =
                {
                    textColor = Color.yellow
                },
                margin = new RectOffset(),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleLeft,
                fontSize = 13,
                fontStyle = FontStyle.Normal,
                stretchWidth = true,
                richText = true
            };

            this.categoryTitleButtonStyle = new GUIStyle(this.readoutButtonStyle) {
                fixedHeight = 30.0f,
                stretchHeight = false
            };
        }

        private void SavePreset(Preset preset) {
            if (preset == null) {
                preset = new Preset();
            }

            preset.Name = this.ParentSection.Name;
            preset.Abbreviation = this.ParentSection.Abbreviation;
            preset.ReadoutNames = this.ParentSection.ReadoutModuleNames;
            preset.IsHud = this.ParentSection.IsHud;
            preset.IsHudBackground = this.ParentSection.IsHudBackground;

            PresetLibrary.Save(preset);
        }

        private void ShowHelpMessage(ReadoutModule readout) {
            if (!readout.ShowHelp) {
                return;
            }

            GUILayout.BeginVertical(this.helpBoxStyle);
            GUILayout.Label(!String.IsNullOrEmpty(readout.HelpString) ? readout.HelpString : Localizer.Format("#KE_noHelpMessage"), this.helpTextStyle);//"Sorry, no help information has been provided for this readout module."
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the editor window.
        /// </summary>
        private void Window(int windowId) {
            try {
                this.DrawCustomOptions();
                GUILayout.BeginHorizontal();
                this.DrawCategorySelector();
                this.DrawPresetSelector();
                GUILayout.EndHorizontal();
                this.DrawAvailableReadouts();
                GUILayout.Space(5.0f);
                this.DrawInstalledReadouts();

                if (GUILayout.Button(Localizer.Format("#KE_CloseEditorButton"), this.categoryTitleButtonStyle)) {//"CLOSE EDITOR"
                    this.ParentSection.IsEditorVisible = false;
                }

                GUI.DragWindow();
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        private void DrawColorPicker() {
            if (editingReadout == null) return;
            editingReadout.ValueStyle.normal.textColor = this.colorPicker.DrawColorPicker(editingReadout.ValueStyle.normal.textColor);
        }

        #endregion
    }
}