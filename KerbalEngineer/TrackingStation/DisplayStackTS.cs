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

    /// <summary>
    ///     Graphical controller for displaying stacked sections.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.TrackingStation, false)]
    public class DisplayStackTS : Flight.DisplayStack {
        public static SectionEditorTS editor;

        /// <summary>
        ///     Gets the current instance of the DisplayStack.
        /// </summary>
        public static new DisplayStackTS Instance { get; private set; }

        internal SectionEditorTS MakeEditor() {
            editor = this.gameObject.AddComponent<SectionEditorTS>();
            editor.ParentSection = SectionLibrary.TrackingStationSection;
            editor.Position = new Rect(SectionLibrary.TrackingStationSection.EditorPositionX, SectionLibrary.TrackingStationSection.EditorPositionY, SectionEditorTS.Width, SectionEditorTS.Height);
            ReadoutCategory.Selected = ReadoutCategory.GetCategory("Rendezvous");
            return editor;
        }
        
        /// <summary>
        ///     Sets the instance to this object.
        /// </summary>
        protected override void Awake()
        {
            try
            {
                if (Instance == null)
                {
                    Instance = this;
                    GuiDisplaySize.OnSizeChanged += this.OnSizeChanged;
                    //MyLogger.Log("[KerbalEngineer]: DisplayStack->Awake");
                }
                else
                {
                    Destroy(this);
                }
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        /// <summary>
        ///     Initialises the object's state on creation.
        /// </summary>
        protected override void Start() {
            try {
                SectionLibrary.LoadTS();
                this.windowId = this.GetHashCode();
                this.InitialiseStyles();
                this.Load();
                //Debug.Log("[KerbalEngineer]: DisplayStackTS->Start");
            } catch (Exception ex) {
                Debug.Log(ex.ToString() + ex.InnerException == null ? "" : ex.InnerException.ToString());
            }
        }


        private ITargetable lastSource;
        private ITargetable lastTarget;

        protected override void Update() {
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

        /// <summary>
        ///     Called to draw the display stack when the UI is enabled.
        /// </summary>
        protected override void OnGUI() {
            if (HighLogic.LoadedScene != GameScenes.TRACKSTATION) return;

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
        protected override void DrawControlBar() {
            GUILayout.Label("FLIGHT ENGINEER " + EngineerGlobals.ASSEMBLY_VERSION, this.titleStyle);
        }

        /// <summary>
        ///     Load the stack's state.
        /// </summary>
        protected override void Load() {
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

        /// <summary>
        ///     Saves the stack's state.
        /// </summary>
        protected override void Save() {
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
        protected override void Window(int windowId) {
            try {
                if (this.ShowControlBar) {
                    this.DrawControlBar();
                }

                SectionLibrary.TrackingStationSection.Name = "TRACKING";

                ITargetable src = Flight.Readouts.Rendezvous.RendezvousProcessor.TrackingStationSource;

                if (src != null) {
                    SectionLibrary.TrackingStationSection.Name = "TRACKING (REF: " + RendezvousProcessor.nameForTargetable(src) + ")";
                }

                SectionLibrary.TrackingStationSection.Draw();

                GUI.DragWindow();
            } catch (Exception ex) {
                MyLogger.Exception(ex, "DisplayStackTS->Window");
            }
        }
    }
}