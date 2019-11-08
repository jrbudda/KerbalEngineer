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
using System.Collections.Generic;

using KerbalEngineer.Flight.Sections;

using UnityEngine;
using KSP.Localization;

#endregion

namespace KerbalEngineer.Flight {
    public class ActionMenuGui : MonoBehaviour {
        #region Fields

        private int numberOfSections;
        private Rect position = new Rect(Screen.width, 38.0f, 300.0f, 0);

        #endregion

        #region Properties

        public bool StayOpen { get; set; }

        public bool Hovering { get; set; }

        public bool Hidden { get; set; }

        #endregion

        #region Initialisation

        private void Awake() {
            try {
                this.enabled = false;
                MyLogger.Log("ActionMenuGui was created.");
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        private void Start() {
            try {
                this.InitialiseStyles();
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        #endregion

        #region GUIStyles

        private GUIStyle buttonStyle;
        private GUIStyle windowStyle;

        /// <summary>
        ///     Initialises all the styles required for this object.
        /// </summary>
        private void InitialiseStyles() {
            try {
                this.windowStyle = new GUIStyle {
                    border = new RectOffset(10, 0, 20, 10),
                    margin = new RectOffset(0, 0, 3, 0),
                    padding = new RectOffset(5, 5, 26, 5),
                    normal =
                    {
                        background = GameDatabase.Instance.GetTexture("KerbalEngineer/Textures/ToolbarBackground", false)
                    }
                };

                this.buttonStyle = new GUIStyle(HighLogic.Skin.button) {
                    normal =
                    {
                        textColor = Color.white,
                    },
                    margin = new RectOffset(),
                    padding = new RectOffset(),
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    fixedHeight = 20.0f,
                };
            } catch (Exception ex) {
                MyLogger.Exception(ex, "ActionMenu->InitialiseStyles");
            }
        }

        #endregion

        #region Drawing

        /// <summary>
        ///     Called to draw the menu when the UI is enabled.
        /// </summary>
        private void OnGUI() {
            try {
                if (this.Hidden || !FlightEngineerCore.IsDisplayable) {
                    return;
                }

                if (!this.position.Contains(Event.current.mousePosition) && !this.StayOpen && !this.Hovering) {
                    this.enabled = false;
                    return;
                }

                if (this.numberOfSections < SectionLibrary.NumberOfSections) {
                    this.numberOfSections = SectionLibrary.NumberOfSections;
                } else if (this.numberOfSections > SectionLibrary.NumberOfSections) {
                    this.numberOfSections = SectionLibrary.NumberOfSections;
                    this.position.height = 0;
                }

                GUI.skin = null;
                this.position.x = Mathf.Clamp(Screen.width * 0.5f + this.transform.parent.position.x - 19.0f, Screen.width * 0.5f, Screen.width - this.position.width);
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, string.Empty, this.windowStyle);
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        /// <summary>
        ///     Draws the menu window.
        /// </summary>
        private void Window(int windowId) {
            try {
                GUILayout.BeginVertical();

                this.DrawControlBarButton();
                GUILayout.Space(5.0f);
                this.DrawSections(SectionLibrary.StockSections);
                this.DrawSections(SectionLibrary.CustomSections);
                GUILayout.Space(5.0f);
                if (HighLogic.LoadedSceneIsFlight) this.DrawNewButton();

                GUILayout.EndVertical();
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        /// <summary>
        ///     Draws and performs the control bar button action.
        /// </summary>
        private void DrawControlBarButton() {
            try {
                GUILayout.BeginHorizontal();
                DisplayStack.Instance.Hidden = !GUILayout.Toggle(!DisplayStack.Instance.Hidden, Localizer.Format("#KE_ShowButton"), this.buttonStyle);//"SHOW ENGINEER"
                if (GUILayout.Toggle(DisplayStack.Instance.ShowControlBar, Localizer.Format("#KE_ShowControlBar"), this.buttonStyle) != DisplayStack.Instance.ShowControlBar) {//"CONTROL BAR"
                    DisplayStack.Instance.ShowControlBar = !DisplayStack.Instance.ShowControlBar;
                    DisplayStack.Instance.RequestResize();
                }
                GUILayout.EndHorizontal();
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        /// <summary>
        ///     Draws an action list for the supplied sections.
        /// </summary>
        private void DrawSections(IEnumerable<SectionModule> sections) {
            try {
                foreach (var section in sections) {
                    GUILayout.BeginHorizontal();
                    section.IsVisible = GUILayout.Toggle(section.IsVisible, section.Name.ToUpper(), this.buttonStyle);
                    section.IsEditorVisible = GUILayout.Toggle(section.IsEditorVisible, Localizer.Format("#KE_EditButton"), this.buttonStyle, GUILayout.Width(50.0f));//"EDIT"
                    GUILayout.EndHorizontal();
                }
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        /// <summary>
        ///     Draws and performs the new section button action.
        /// </summary>
        private void DrawNewButton() {
            try {
                GUILayout.BeginHorizontal();

                if (GUILayout.Button(Localizer.Format(""), this.buttonStyle)) {//"NEW CUSTOM SECTION"
                    SectionLibrary.CustomSections.Add(new SectionModule {
                        Name = "Custom " + (SectionLibrary.CustomSections.Count + 1),
                        Abbreviation = "CUST " + (SectionLibrary.CustomSections.Count + 1),
                        IsVisible = true,
                        IsEditorVisible = true
                    });
                }
                GUILayout.EndHorizontal();
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        #endregion

        #region Destruction

        private void OnDestroy() {
            try {
                MyLogger.Log("ActionMenuGui was destroyed.");
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        #endregion
    }
}