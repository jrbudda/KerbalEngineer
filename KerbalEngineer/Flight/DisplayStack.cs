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

namespace KerbalEngineer.Flight
{
    using KeyBinding;
    using Upgradeables;

    /// <summary>
    ///     Graphical controller for displaying stacked sections.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class DisplayStack : MonoBehaviour
    {
        #region Fields

        protected GUIStyle buttonStyle;
        protected int numberOfStackSections;
        protected bool resizeRequested;
        protected bool showControlBar = true;
        protected GUIStyle titleStyle;
        protected int windowId;
        protected Rect windowPosition;
        protected GUIStyle windowStyle;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the current instance of the DisplayStack.
        /// </summary>
        public static DisplayStack Instance { get; private set; }

        public bool Hidden { get; set; }

        /// <summary>
        ///     Gets and sets the visibility of the control bar.
        /// </summary>
        public bool ShowControlBar
        {
            get { return this.showControlBar; }
            set
            {
                if (showControlBar != value)
                {
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
        public void RequestResize()
        {
            this.resizeRequested = true;
        }

        #endregion

        #region Methods: protected

        /// <summary>
        ///     Sets the instance to this object.
        /// </summary>
        protected virtual void Awake()
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
        ///     Runs when the object is destroyed.
        /// </summary>
        protected void OnDestroy()
        {
            try
            {
                this.Save();
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
            //MyLogger.Log("[KerbalEngineer]: DisplayStack->OnDestroy");
        }

        /// <summary>
        ///     Initialises the object's state on creation.
        /// </summary>
        protected virtual void Start()
        {
            try
            {
                this.windowId = this.GetHashCode();
                this.InitialiseStyles();
                this.Load();
                //MyLogger.Log("[KerbalEngineer]: DisplayStack->Start");
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        protected virtual void Update()
        {
            try
            {
                if (!FlightEngineerCore.IsDisplayable)
                {
                    return;
                }

                if (Input.GetKeyDown(KeyBinder.FlightShowHide))
                {
                    this.Hidden = !this.Hidden;
                }
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }
        
        /// <summary>
        ///     Load the stack's state.
        /// </summary>
        protected virtual void Load()
        {
            try
            {
                var handler = SettingHandler.Load("DisplayStack.xml");
                this.Hidden = handler.Get("hidden", this.Hidden);
                this.ShowControlBar = handler.Get("showControlBar", this.ShowControlBar);
                this.windowPosition.x = handler.Get("windowPositionX", this.windowPosition.x);
                this.windowPosition.y = handler.Get("windowPositionY", this.windowPosition.y);
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex, "DisplayStack->Load");
            }
        }

        /// <summary>
        ///     Saves the stack's state.
        /// </summary>
        protected virtual void Save()
        {
            try
            {
                var handler = new SettingHandler();
                handler.Set("hidden", this.Hidden);
                handler.Set("showControlBar", this.ShowControlBar);
                handler.Set("windowPositionX", this.windowPosition.x);
                handler.Set("windowPositionY", this.windowPosition.y);
                handler.Save("DisplayStack.xml");
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex, "DisplayStack->Save");
            }
        }

        /// <summary>
        ///     Draws the display stack window.
        /// </summary>
        protected virtual void Window(int windowId)
        {
            try
            {
                if (this.ShowControlBar)
                {
                    this.DrawControlBar();
                }

                if (SectionLibrary.NumberOfStackSections > 0)
                {
                    this.DrawSections(SectionLibrary.StockSections);
                    this.DrawSections(SectionLibrary.CustomSections);
                }

                GUI.DragWindow();
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex, "DisplayStack->Window");
            }
        }
        
        /// <summary>
        ///     Draws the control bar.
        /// </summary>
        protected virtual void DrawControlBar()
        {
            GUILayout.Label("FLIGHT ENGINEER " + EngineerGlobals.ASSEMBLY_VERSION, this.titleStyle);
            var list = new List<SectionModule>();
            list.AddRange(SectionLibrary.StockSections);
            list.AddRange(SectionLibrary.CustomSections);
            this.DrawControlBarButtons(list);
        }
        
        /// <summary>
        ///     Initialises all the styles required for this object.
        /// </summary>
        protected void InitialiseStyles()
        {
            this.windowStyle = new GUIStyle(HighLogic.Skin.window)
            {
                margin = new RectOffset(),
                padding = new RectOffset(5, 5, 0, 5)
            };

            this.titleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                margin = new RectOffset(0, 0, 5, 3),
                padding = new RectOffset(),
                alignment = TextAnchor.MiddleCenter,
                fontSize = (int)(13 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                stretchWidth = true
            };

            this.buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
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
        
        protected void OnSizeChanged()
        {
            this.InitialiseStyles();
            this.RequestResize();
        }

        #endregion

        #region Methods: private

        /// <summary>
        ///     Called to draw the display stack when the UI is enabled.
        /// </summary>
        protected virtual void OnGUI()
        {
            if (!HighLogic.LoadedSceneIsFlight) return;

            try
            {
                if (!FlightEngineerCore.IsDisplayable)
                {
                    return;
                }

                if (this.resizeRequested || this.numberOfStackSections != SectionLibrary.NumberOfStackSections)
                {
                    this.numberOfStackSections = SectionLibrary.NumberOfStackSections;
                    this.windowPosition.width = 0;
                    this.windowPosition.height = 0;
                    this.resizeRequested = false;
                }

                if (!this.Hidden && (SectionLibrary.NumberOfStackSections > 0 || this.ShowControlBar))
                {
                    var shouldCentre = this.windowPosition.min == Vector2.zero;
                    GUI.skin = null;
                    this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, string.Empty, this.windowStyle).ClampToScreen();
                    if (shouldCentre)
                    {
                        this.windowPosition.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        /// <summary>
        ///     Draws a button list for a set of sections.
        /// </summary>
        private void DrawControlBarButtons(IEnumerable<SectionModule> sections)
        {
            var index = 0;
            foreach (var section in sections.Where(s => s.showButton))
            {
                if (index % 4 == 0)
                {
                    if (index > 0)
                    {
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.BeginHorizontal();
                }
                section.IsVisible = GUILayout.Toggle(section.IsVisible, section.Abbreviation.ToUpper(), this.buttonStyle);
                index++;
            }
            if (index > 0)
            {
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        ///     Draws a list of sections.
        /// </summary>
        private void DrawSections(IEnumerable<SectionModule> sections)
        {
            foreach (var section in sections)
            {
                if (!section.IsFloating)
                {
                    section.Draw();
                }
            }
        }

        #endregion
    }
}