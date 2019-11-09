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
using System.Linq;

using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Sections;
using KerbalEngineer.Settings;

using UnityEngine;

#endregion

namespace KerbalEngineer.TrackingStation {
    using Flight.Readouts;
    using Flight.Readouts.Rendezvous;
    using KeyBinding;
    using KSP.Localization;

    /// <summary>
    ///     Graphical controller for displaying stacked sections.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class DisplayStackTS : MonoBehaviour {
        #region Fields

        private GUIStyle buttonStyle;
        private int numberOfStackSections;
        private bool resizeRequested;
        private bool showControlBar = true;
        private GUIStyle titleStyle;
        private int windowId;
        private Rect windowPosition;
        private GUIStyle windowStyle;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the current instance of the DisplayStack.
        /// </summary>
        public static DisplayStackTS Instance { get; private set; }

        public bool Hidden { get; set; }

        /// <summary>
        ///     Gets and sets the visibility of the control bar.
        /// </summary>
        public bool ShowControlBar {
            get { return this.showControlBar; }
            set {
                if (showControlBar != value) {
                    this.showControlBar = value;
                    RequestResize();
                }
            }
        }

        #endregion

        #region Methods: public

        /// <summary>
        ///     Request that the display stack's size is reset in the next draw call.
        /// </summary>
        public void RequestResize() {
            this.resizeRequested = true;
        }

        #endregion

        #region Methods: protected

        /// <summary>
        ///     Sets the instance to this object.
        /// </summary>
        protected void Awake() {
            try {
                if (Instance == null) {
                    Instance = this;
                    GuiDisplaySize.OnSizeChanged += this.OnSizeChanged;
                    Debug.Log("DisplayStackTS->Awake");
                } else {
                    Destroy(this);
                }
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        /// <summary>
        ///     Runs when the object is destroyed.
        /// </summary>
        protected void OnDestroy() {
            try {
                this.Save();
                SectionLibrary.SaveTS();
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
            MyLogger.Log("DisplayStackTS->OnDestroy");
        }

        internal SectionEditorTS MakeEditor() {
            editor = this.gameObject.AddComponent<SectionEditorTS>();
            editor.ParentSection = SectionLibrary.TrackingStationSection;
            editor.Position = new Rect(SectionLibrary.TrackingStationSection.EditorPositionX, SectionLibrary.TrackingStationSection.EditorPositionY, SectionEditorTS.Width, SectionEditorTS.Height);
            ReadoutCategory.Selected = ReadoutCategory.GetCategory("Rendezvous");
            return editor;
        }

        /// <summary>
        ///     Initialises the object's state on creation.
        /// </summary>
        protected void Start() {
            try {
                SectionLibrary.LoadTS();
                this.windowId = this.GetHashCode();
                this.InitialiseStyles();
                this.Load();
                Debug.Log("DisplayStackTS->Start");
            } catch (Exception ex) {
                Debug.Log(ex.ToString() + ex.InnerException == null ? "" : ex.InnerException.ToString());
            }
        }

        public static SectionEditorTS editor;


        private ITargetable lastSource;
        private ITargetable lastTarget;

        protected void Update() {
            try {

                SectionLibrary.TrackingStationSection.Update();

                Flight.Readouts.Rendezvous.RendezvousProcessor.Instance.Update();

                if (Flight.Readouts.Rendezvous.RendezvousProcessor.TrackingStationSource != lastSource)
                    this.RequestResize();

                if (Flight.Readouts.Rendezvous.RendezvousProcessor.activeTarget != lastTarget)
                    this.RequestResize();

                lastSource = Flight.Readouts.Rendezvous.RendezvousProcessor.TrackingStationSource;
                lastTarget = Flight.Readouts.Rendezvous.RendezvousProcessor.activeTarget;

                //if (!FlightEngineerCore.IsDisplayable)
                //{
                //    return;
                //}

                //if (Input.GetKeyDown(KeyBinder.FlightShowHide)) {
                //    this.Hidden = !this.Hidden;
                //}
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        #endregion

        #region Methods: private

        /// <summary>
        ///     Called to draw the display stack when the UI is enabled.
        /// </summary>
        private void OnGUI() {
            try {
                //if (!FlightEngineerCore.IsDisplayable)
                //{
                //    return;
                //}

                if (resizeRequested) {
                    this.numberOfStackSections = 1;
                    this.windowPosition.width = 0;
                    this.windowPosition.height = 0;
                    this.resizeRequested = false;
                }

                if (!this.Hidden && this.ShowControlBar) {
                    var shouldCentre = this.windowPosition.min == Vector2.zero;
                    GUI.skin = null;
                    this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, string.Empty, this.windowStyle).ClampToScreen();
                    if (shouldCentre) {
                        this.windowPosition.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                    }
                }
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        /// <summary>
        ///     Draws the control bar.
        /// </summary>
        private void DrawControlBar() {
            GUILayout.Label(Localizer.Format("#KE_VERSION_title", EngineerGlobals.ASSEMBLY_VERSION), this.titleStyle);//"FLIGHT ENGINEER " + 
        }


        /// <summary>
        ///     Initialises all the styles required for this object.
        /// </summary>
        private void InitialiseStyles() {
            this.windowStyle = new GUIStyle(HighLogic.Skin.window) {
                margin = new RectOffset(),
                padding = new RectOffset(5, 5, 0, 5)
            };

            this.titleStyle = new GUIStyle(HighLogic.Skin.label) {
                margin = new RectOffset(0, 0, 5, 3),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleCenter,
                fontSize = (int)(13 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button) {
                normal =
                {
                    textColor = Color.white
                },
                margin = new RectOffset(),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleCenter,
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                fixedWidth = 60.0f * GuiDisplaySize.Offset,
                fixedHeight = 25.0f * GuiDisplaySize.Offset,
            };
        }

        /// <summary>
        ///     Load the stack's state.
        /// </summary>
        private void Load() {
            try {
                var handler = SettingHandler.Load("DisplayStackTS.xml");
                this.Hidden = handler.Get("hidden", this.Hidden);
                this.ShowControlBar = handler.Get("showControlBar", this.ShowControlBar);
                this.windowPosition.x = handler.Get("windowPositionX", this.windowPosition.x);
                this.windowPosition.y = handler.Get("windowPositionY", this.windowPosition.y);
            } catch (Exception ex) {
                MyLogger.Exception(ex, "DisplayStackTS->Load");
            }
        }

        private void OnSizeChanged() {
            this.InitialiseStyles();
            this.RequestResize();
        }

        /// <summary>
        ///     Saves the stack's state.
        /// </summary>
        private void Save() {
            try {
                var handler = new SettingHandler();
                handler.Set("hidden", this.Hidden);
                handler.Set("showControlBar", this.ShowControlBar);
                handler.Set("windowPositionX", this.windowPosition.x);
                handler.Set("windowPositionY", this.windowPosition.y);
                handler.Save("DisplayStackTS.xml");
            } catch (Exception ex) {
                MyLogger.Exception(ex, "DisplayStackTS->Save");
            }
        }

        /// <summary>
        ///     Draws the display stack window.
        /// </summary>
        private void Window(int windowId) {
            try {

                if (this.ShowControlBar) {
                    this.DrawControlBar();
                }

                SectionLibrary.TrackingStationSection.Name = Localizer.Format("#KE_TRACKING_title");//"TRACKING"

                ITargetable src = Flight.Readouts.Rendezvous.RendezvousProcessor.TrackingStationSource;

                if (src != null) {
                    SectionLibrary.TrackingStationSection.Name = Localizer.Format("#KE_TRACKING_TR", RendezvousProcessor.nameForTargetable(src));//"TRACKING (REF: <<1>> )"
                }

                SectionLibrary.TrackingStationSection.Draw();

                GUI.DragWindow();
            } catch (Exception ex) {
                MyLogger.Exception(ex, "DisplayStackTS->Window");
            }
        }

        #endregion
    }
}