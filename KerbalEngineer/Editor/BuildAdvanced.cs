// 
//     Copyright (C) 2015 CYBUTEK
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

namespace KerbalEngineer.Editor
{
    using System;
    using Extensions;
    using Flight;
    using Helpers;
    using KeyBinding;
    using Settings;
    using UIControls;
    using Unity;
    using UnityEngine;
    using VesselSimulator;

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BuildAdvanced : MonoBehaviour
    {
        public static float Altitude;

        private static Rect compactModeRect = new Rect(0.0f, 5.0f, 0.0f, 20.0f);
        private static Stage stage;
        private static int stagesCount;
        private static int stagesLength;
        private static string title;

        private GUIStyle areaSettingStyle;
        private GUIStyle areaStyle;
        private float atmosphericMach;
        private GUIStyle bodiesButtonActiveStyle;
        private GUIStyle bodiesButtonStyle;
        private PopOutElement bodiesList;
        private Rect bodiesListPosition;
        private GUIStyle buttonStyle;
        private int compactCheck;
        private bool compactCollapseRight;
        private bool compactMode;
        private float compactRight;
        private bool hasChanged;
        private GUIStyle infoStyle;
        private bool isEditorLocked;
        private float maxMach;
        private int numberOfStages;
        private Rect position = new Rect(265.0f, 45.0f, 0, 0);
        private GUIStyle settingAtmoStyle;
        private GUIStyle settingStyle;
        private bool showAllStages;
        private bool showAtmosphericDetails;
        private bool showRCS;
        private bool showSettings;
        private Stage[] stages;
        private GUIStyle titleStyle;
        private bool visible = true;
        private GUIStyle windowStyle;

        /// <summary>
        ///     Gets the current instance if started or returns null.
        /// </summary>
        public static BuildAdvanced Instance { get; private set; }

        /// <summary>
        ///     Gets and sets whether to show in compact mode.
        /// </summary>
        public bool CompactMode
        {
            get
            {
                return compactMode;
            }
            set
            {
                compactMode = value;
            }
        }

        /// <summary>
        ///     Gets and sets whether to show all stages.
        /// </summary>
        public bool ShowAllStages
        {
            get
            {
                return showAllStages;
            }
            set
            {
                showAllStages = value;
            }
        }

        /// <summary>
        ///     Gets and sets whether to use atmospheric details.
        /// </summary>
        public bool ShowAtmosphericDetails
        {
            get
            {
                return showAtmosphericDetails;
            }
            set
            {
                showAtmosphericDetails = value;
            }
        }

        /// <summary>
        ///     Gets and sets whether to show the settings display.
        /// </summary>
        public bool ShowSettings
        {
            get
            {
                return showSettings;
            }
            set
            {
                showSettings = value;
            }
        }

        /// <summary>
        ///     Gets and sets whether the display is enabled.
        /// </summary>
        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
            }
        }

        protected void Awake()
        {
            try
            {
                Instance = this;
                bodiesList = gameObject.AddComponent<PopOutElement>();
                bodiesList.DrawCallback = DrawBodiesList;
                Load();

                SimManager.UpdateModSettings();
                SimManager.OnReady -= GetStageInfo;
                SimManager.OnReady += GetStageInfo;
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex, "BuildAdvanced.Awake()");
            }
        }

        /// <summary>
        ///     Saves the settings when this object is destroyed.
        /// </summary>
        protected void OnDestroy()
        {
            MyLogger.Log("BuildAdvanced->OnDestroy");

            try
            {
                SettingHandler handler = new SettingHandler();
                handler.Set("visible", visible);
                handler.Set("windowPositionX", position.x);
                handler.Set("windowPositionY", position.y);
                handler.Set("compactMode", compactMode);
                handler.Set("compactCollapseRight", compactCollapseRight);
                handler.Set("showAllStages", showAllStages);
                handler.Set("showAtmosphericDetails", showAtmosphericDetails);
                handler.Set("showSettings", showSettings);
                handler.Set("selectedBodyName", CelestialBodies.SelectedBody.Name);
                handler.Save("BuildAdvanced.xml");
                GuiDisplaySize.OnSizeChanged -= OnSizeChanged;
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex, "BuildAdvanced.OnDestroy()");
            }

            EditorLock(false);
        }

        protected void OnGUI()
        {
            try
            {
                if (!visible || EditorLogic.fetch == null || EditorLogic.fetch.ship.parts.Count == 0 )
                {
                    return;
                }

                if (stages == null)
                {
                    return;
                }

                // Change the window title based on whether in compact mode or not.
                title = !compactMode ? "KERBAL ENGINEER REDUX " + EngineerGlobals.ASSEMBLY_VERSION : "K.E.R. " + EngineerGlobals.ASSEMBLY_VERSION;

                // Reset the window size when the staging or something else has changed.
                stagesLength = stages.Length;
                if (showAllStages)
                {
                    stagesCount = stagesLength;
                }
                if (showAllStages == false)
                {
                    stagesCount = 0;
                    for (int i = 0; i < stagesLength; ++i)
                    {
                        if (stages[i].deltaV > 0.0f)
                        {
                            stagesCount = stagesCount + 1;
                        }
                    }
                }

                if (hasChanged || stagesCount != numberOfStages)
                {
                    hasChanged = false;
                    numberOfStages = stagesCount;

                    position.width = 0;
                    position.height = 0;
                }

                GUI.skin = null;
                position = GUILayout.Window(GetInstanceID(), position, Window, title, windowStyle).ClampToScreen();

                if (compactCheck > 0 && compactCollapseRight)
                {
                    position.x = compactRight - position.width;
                    compactCheck--;
                }
                else if (compactCheck > 0)
                {
                    compactCheck = 0;
                }

                // Check editor lock to manage click-through.
                CheckEditorLock();
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex, "BuildAdvanced.OnGUI()");
            }
        }

        protected void Start()
        {
            try
            {
                InitialiseStyles();
                GuiDisplaySize.OnSizeChanged += OnSizeChanged;
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex, "BuildAdvanced.Start()");
            }
        }

        protected void Update()
        {
            try
            {
                if (Input.GetKeyDown(KeyBinder.EditorShowHide))
                {
                    visible = !visible;
                    if (!visible)
                    {
                        EditorLock(false);
                    }
                }

                if (!visible || EditorLogic.fetch == null || EditorLogic.fetch.ship.parts.Count == 0)
                {
                    bodiesList.enabled = false;
                    return;
                }

                // Configure the simulation parameters based on the selected reference body.
                SimManager.Gravity = CelestialBodies.SelectedBody.Gravity;

                if (showAtmosphericDetails)
                {
                    SimManager.Atmosphere = CelestialBodies.SelectedBody.GetAtmospheres(Altitude);
                }
                else
                {
                    SimManager.Atmosphere = 0;
                }

                SimManager.Mach = atmosphericMach;

                SimManager.RequestSimulation();
                SimManager.TryStartSimulation();
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex, "BuildAdvanced.Update()");
            }
        }

        /// <summary>
        ///     Checks whether the editor should be locked to stop click-through.
        /// </summary>
        private void CheckEditorLock()
        {
            if ((position.MouseIsOver() || bodiesList.Position.MouseIsOver()) && !isEditorLocked)
            {
                EditorLock(true);
            }
            else if (!position.MouseIsOver() && !bodiesList.Position.MouseIsOver() && isEditorLocked)
            {
                EditorLock(false);
            }
        }

        /// <summary>
        ///     Draws the atmospheric settings.
        /// </summary>
        private void DrawAtmosphericDetails()
        {
            try
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                GUILayout.Label("Altitude: " + (Altitude * 0.001f).ToString("F1") + "km", settingAtmoStyle, GUILayout.Width(125.0f * GuiDisplaySize.Offset));
                GUI.skin = HighLogic.Skin;
                Altitude = GUILayout.HorizontalSlider(Altitude, 0.0f, (float)(CelestialBodies.SelectedBody.CelestialBody.atmosphereDepth));
                GUI.skin = null;
                GUILayout.EndVertical();

                GUILayout.Space(5.0f);

                GUILayout.BeginVertical();
                GUILayout.Label("Mach: " + atmosphericMach.ToString("F2"), settingAtmoStyle, GUILayout.Width(125.0f * GuiDisplaySize.Offset));
                GUI.skin = HighLogic.Skin;
                atmosphericMach = GUILayout.HorizontalSlider(Mathf.Clamp(atmosphericMach, 0.0f, maxMach), 0.0f, maxMach);
                GUI.skin = null;
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex, "BuildAdvanced.DrawAtmosphericDetails()");
            }
        }

        private void DrawBodiesList()
        {
            if (CelestialBodies.SystemBody == CelestialBodies.SelectedBody)
            {
                DrawBody(CelestialBodies.SystemBody);
            }
            else
            {
                foreach (CelestialBodies.BodyInfo body in CelestialBodies.SystemBody.Children)
                {
                    DrawBody(body);
                }
            }
        }

        private void DrawBody(CelestialBodies.BodyInfo bodyInfo, int depth = 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(20.0f * depth);
            if (GUILayout.Button(bodyInfo.Children.Count > 0 ? bodyInfo.Name + " [" + bodyInfo.Children.Count + "]" : bodyInfo.Name, bodyInfo.Selected && bodyInfo.SelectedDepth == 0 ? bodiesButtonActiveStyle : bodiesButtonStyle))
            {
                CelestialBodies.SetSelectedBody(bodyInfo.Name);
                Altitude = 0.0f;
                bodiesList.ResizeCounter = 1;
            }
            GUILayout.EndHorizontal();

            if (bodyInfo.Selected)
            {
                for (int i = 0; i < bodyInfo.Children.Count; ++i)
                {
                    DrawBody(bodyInfo.Children[i], depth + 1);
                }
            }
        }

        /// <summary>
        ///     Draws the burn time column.
        /// </summary>
        private void DrawBurnTime()
        {
            GUILayout.BeginVertical(GUILayout.Width(75.0f * GuiDisplaySize.Offset));
            GUILayout.Label("BURN", titleStyle);
            for (int i = 0; i < stagesLength; ++i)
            {
                stage = stages[i];
                if (showAllStages || stage.deltaV > 0.0)
                {
                    GUILayout.Label(TimeFormatter.ConvertToString(stage.time), infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the cost column.
        /// </summary>
        private void DrawCost()
        {
            GUILayout.BeginVertical(GUILayout.Width(110.0f * GuiDisplaySize.Offset));
            GUILayout.Label("COST", titleStyle);
            for (int i = 0; i < stagesLength; ++i)
            {
                stage = stages[i];
                if (showAllStages || stage.deltaV > 0.0)
                {
                    GUILayout.Label(Units.Cost(stage.cost, stage.totalCost), infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the deltaV column.
        /// </summary>
        private void DrawDeltaV()
        {
            GUILayout.BeginVertical(GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            GUILayout.Label("DELTA-V", titleStyle);
            for (int i = 0; i < stagesLength; ++i)
            {
                stage = stages[i];
                if (showAllStages || stage.deltaV > 0.0)
                {
                    GUILayout.Label(stage.deltaV.ToString("N0") + " / " + stage.inverseTotalDeltaV.ToString("N0") + "m/s", infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the specific impluse column.
        /// </summary>
        private void DrawIsp()
        {
            GUILayout.BeginVertical(GUILayout.Width(75.0f * GuiDisplaySize.Offset));
            GUILayout.Label("ISP", titleStyle);
            for (int i = 0; i < stagesLength; ++i)
            {
                stage = stages[i];
                if (showAllStages || stage.deltaV > 0.0)
                {
                    GUILayout.Label(stage.isp.ToString("F1") + "s", infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the mass column.
        /// </summary>
        private void DrawMass()
        {
            GUILayout.BeginVertical(GUILayout.Width(110.0f * GuiDisplaySize.Offset));
            GUILayout.Label("MASS", titleStyle);
            for (int i = 0; i < stagesLength; ++i)
            {
                stage = stages[i];
                if (showAllStages || stage.deltaV > 0.0)
                {
                    GUILayout.Label(Units.ToMass(stage.mass, stage.totalMass), infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the part count column.
        /// </summary>
        private void DrawPartCount()
        {
            GUILayout.BeginVertical(GUILayout.Width(50.0f * GuiDisplaySize.Offset));
            GUILayout.Label("PARTS", titleStyle);
            for (int i = 0; i < stagesLength; ++i)
            {
                stage = stages[i];
                if (showAllStages || stage.deltaV > 0.0)
                {
                    GUILayout.Label(stage.partCount + " / " + stage.totalPartCount, infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the settings panel.
        /// </summary>
        private void DrawSettings()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Compact mode collapses to the:", settingStyle);
            compactCollapseRight = !GUILayout.Toggle(!compactCollapseRight, "LEFT", buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            compactCollapseRight = GUILayout.Toggle(compactCollapseRight, "RIGHT", buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Simulate using vectored thrust values:", settingStyle);
            SimManager.vectoredThrust = GUILayout.Toggle(SimManager.vectoredThrust, "ENABLED", buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            GUILayout.EndHorizontal();

            if (SimManager.hasInstalledRealFuels)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Simulate using residual predictions:", settingStyle);
                SimManager.RFResiduals = GUILayout.Toggle(SimManager.RFResiduals, "ENABLED", buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Verbose Simulation Log:", settingStyle);
            SimManager.logOutput = GUILayout.Toggle(SimManager.logOutput, "ENABLED", buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Build Engineer Overlay:", settingStyle);
            BuildOverlay.Visible = GUILayout.Toggle(BuildOverlay.Visible, "VISIBLE", buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            BuildOverlayPartInfo.NamesOnly = GUILayout.Toggle(BuildOverlayPartInfo.NamesOnly, "NAMES ONLY", buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            BuildOverlayPartInfo.ClickToOpen = GUILayout.Toggle(BuildOverlayPartInfo.ClickToOpen, "CLICK TO OPEN", buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Flight Engineer activation mode:", settingStyle);
            FlightEngineerCore.IsCareerMode = GUILayout.Toggle(FlightEngineerCore.IsCareerMode, "CAREER", buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            FlightEngineerCore.IsCareerMode = !GUILayout.Toggle(!FlightEngineerCore.IsCareerMode, "PARTLESS", buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Flight Engineer Career Limitations:", settingStyle);
            FlightEngineerCore.IsKerbalLimited = GUILayout.Toggle(FlightEngineerCore.IsKerbalLimited, "KERBAL", buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            FlightEngineerCore.IsTrackingStationLimited = GUILayout.Toggle(FlightEngineerCore.IsTrackingStationLimited, "TRACKING", buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Flight Engineer Toolbar Icon:", settingStyle);
            FlightAppLauncher.IsHoverActivated = GUILayout.Toggle(FlightAppLauncher.IsHoverActivated, "MOUSE HOVER", buttonStyle, GUILayout.Width(125.0f * GuiDisplaySize.Offset));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Key Bindings:", settingStyle);
            if (GUILayout.Button("EDIT KEY BINDINGS", buttonStyle, GUILayout.Width(200.0f * GuiDisplaySize.Offset)))
            {
                KeyBinder.Show();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("GUI Size: " + GuiDisplaySize.Increment, settingStyle);
            if (GUILayout.Button("<", buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset)))
            {
                GuiDisplaySize.Increment--;
            }
            if (GUILayout.Button(">", buttonStyle, GUILayout.Width(100.0f * GuiDisplaySize.Offset)))
            {
                GuiDisplaySize.Increment++;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("VAB Overlay X Pos.: " + BuildOverlay.BuildOverlayVessel.WindowX, settingStyle);
            if (GUILayout.Button("<<", buttonStyle, GUILayout.Width(50 * GuiDisplaySize.Offset))) {
                BuildOverlay.BuildOverlayVessel.WindowX-=10;
            }
            if (GUILayout.Button("<", buttonStyle, GUILayout.Width(50 * GuiDisplaySize.Offset))) {
                BuildOverlay.BuildOverlayVessel.WindowX--;
            }
            if (GUILayout.Button(">", buttonStyle, GUILayout.Width(50 * GuiDisplaySize.Offset))) {
                BuildOverlay.BuildOverlayVessel.WindowX++;
            }
            if (GUILayout.Button(">>", buttonStyle, GUILayout.Width(50 * GuiDisplaySize.Offset))) {
                BuildOverlay.BuildOverlayVessel.WindowX+=10;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("Minimum delay between simulations: " + SimManager.minSimTime.TotalMilliseconds + "ms", settingStyle);
            GUI.skin = HighLogic.Skin;
            SimManager.minSimTime = TimeSpan.FromMilliseconds(GUILayout.HorizontalSlider((float)SimManager.minSimTime.TotalMilliseconds, 0, 2000.0f));

            GUI.skin = null;
        }

        /// <summary>
        ///     Draws the stage number column.
        /// </summary>
        private void DrawStageNumbers()
        {
            GUILayout.BeginVertical(GUILayout.Width(30.0f * GuiDisplaySize.Offset));
            GUILayout.Label(string.Empty, titleStyle);
            for (int i = 0; i < stagesLength; ++i)
            {
                stage = stages[i];
                if (showAllStages || stage.deltaV > 0.0)
                {
                    GUILayout.Label("S" + stage.number, titleStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the thrust column.
        /// </summary>
        private void DrawThrust()
        {
            GUILayout.BeginVertical(GUILayout.Width(75.0f * GuiDisplaySize.Offset));
            GUILayout.Label("THRUST", titleStyle);
            for (int i = 0; i < stagesLength; ++i)
            {
                stage = stages[i];
                if (showAllStages || stage.deltaV > 0.0)
                {
                    GUILayout.Label(stage.thrust.ToForce(), infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Draws the torque column.
        /// </summary>
        private void DrawTorque()
        {
            GUILayout.BeginVertical(GUILayout.Width(75.0f * GuiDisplaySize.Offset));
            GUILayout.Label("TORQUE", titleStyle);
            for (int i = 0; i < stagesLength; ++i)
            {
                stage = stages[i];
                if (showAllStages || stage.deltaV > 0.0)
                {
                    GUILayout.Label(stage.maxThrustTorque.ToTorque(), infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Drwas the thrust to weight ratio column.
        /// </summary>
        private void DrawTwr()
        {
            GUILayout.BeginVertical(GUILayout.Width(100.0f * GuiDisplaySize.Offset));
            GUILayout.Label("TWR (MAX)", titleStyle);
            for (int i = 0; i < stagesLength; ++i)
            {
                stage = stages[i];
                if (showAllStages || stage.deltaV > 0.0)
                {
                    GUILayout.Label(stage.thrustToWeight.ToString("F2") + " (" + stage.maxThrustToWeight.ToString("F2") + ")", infoStyle);
                }
            }
            GUILayout.EndVertical();
        }

        /// <summary>
        ///     Drwas the RCS crap ratio column.
        /// </summary>
        private void DrawRCS() {

            GUILayout.BeginVertical(GUILayout.Width(85.0f * GuiDisplaySize.Offset));
            GUILayout.Label("RCS ISP", titleStyle);
            for (int i = 0; i < stagesLength; ++i) {
                stage = stages[i];
                if (showAllStages || stage.deltaV > 0.0) {
                    GUILayout.Label(stage.RCSIsp.ToString("F2") +"s", infoStyle);
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(85.0f * GuiDisplaySize.Offset));
            GUILayout.Label("RCS THRUST", titleStyle);
            for (int i = 0; i < stagesLength; ++i) {
                stage = stages[i];
                if (showAllStages || stage.deltaV > 0.0) {
                    GUILayout.Label(Units.ToForce(stage.RCSThrust), infoStyle);
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(105.0f * GuiDisplaySize.Offset));
            GUILayout.Label("RCS TWR (MAX)", titleStyle);
            for (int i = 0; i < stagesLength; ++i) {
                stage = stages[i];
                if (showAllStages || stage.deltaV > 0.0) {
                    GUILayout.Label(stage.RCSTWRStart.ToString("F2") + " (" + stage.RCSTWREnd.ToString("F2") + ")", infoStyle);
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(120.0f * GuiDisplaySize.Offset));
            GUILayout.Label("RCS DELTA-V (MAX)", titleStyle);
            for (int i = 0; i < stagesLength; ++i) {
                stage = stages[i];
                if (showAllStages || stage.deltaV > 0.0) {
                    GUILayout.Label(Units.ToSpeed(stage.RCSdeltaVStart,0) + " (" + Units.ToSpeed(stage.RCSdeltaVEnd,0) + ")", infoStyle);
                }
            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(105.0f * GuiDisplaySize.Offset));
            GUILayout.Label("RCS BURN TIME", titleStyle);
            for (int i = 0; i < stagesLength; ++i) {
                stage = stages[i];
                if (showAllStages || stage.deltaV > 0.0) {
                    GUILayout.Label(Units.ToTime(stage.RCSBurnTime), infoStyle);
                }
            }
            GUILayout.EndVertical();

        }

        private void EditorLock(bool state)
        {
            if (state)
            {
                InputLockManager.SetControlLock(ControlTypes.All, "KER_BuildAdvanced");
                BuildOverlayPartInfo.Hidden = true;
                isEditorLocked = true;
            }
            else
            {
                InputLockManager.SetControlLock(ControlTypes.None, "KER_BuildAdvanced");
                BuildOverlayPartInfo.Hidden = false;
                isEditorLocked = false;
            }
        }

        private void GetStageInfo()
        {
            stages = SimManager.Stages;
            if (stages != null && stages.Length > 0)
            {
                maxMach = stages[stages.Length - 1].maxMach;
            }
        }

        /// <summary>
        ///     Initialises all the styles that are required.
        /// </summary>
        private void InitialiseStyles()
        {
            windowStyle = new GUIStyle(HighLogic.Skin.window)
            {
                alignment = TextAnchor.UpperLeft
            };

            areaStyle = new GUIStyle(HighLogic.Skin.box)
            {
                padding = new RectOffset(0, 0, 9, 0)
            };

            areaSettingStyle = new GUIStyle(HighLogic.Skin.box)
            {
                padding = new RectOffset(10, 10, 10, 10)
            };

            buttonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                normal =
                {
                    textColor = Color.white
                },
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            titleStyle = new GUIStyle(HighLogic.Skin.label)
            {
                normal =
                {
                    textColor = Color.white
                },
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true
            };

            infoStyle = new GUIStyle(HighLogic.Skin.label)
            {
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                stretchWidth = true
            };

            settingStyle = new GUIStyle(titleStyle)
            {
                alignment = TextAnchor.MiddleLeft,
                stretchWidth = true,
                stretchHeight = true
            };

            settingAtmoStyle = new GUIStyle(titleStyle)
            {
                margin = new RectOffset(),
                padding = new RectOffset(),
                alignment = TextAnchor.UpperLeft
            };

            bodiesButtonStyle = new GUIStyle(HighLogic.Skin.button)
            {
                margin = new RectOffset(0, 0, 2, 0),
                padding = new RectOffset(5, 5, 5, 5),
                normal =
                {
                    textColor = Color.white
                },
                active =
                {
                    textColor = Color.white
                },
                fontSize = (int)(11 * GuiDisplaySize.Offset),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                fixedHeight = 20.0f
            };

            bodiesButtonActiveStyle = new GUIStyle(bodiesButtonStyle)
            {
                normal = bodiesButtonStyle.onNormal,
                hover = bodiesButtonStyle.onHover
            };
        }

        /// <summary>
        ///     Loads the settings when this object is created.
        /// </summary>
        private void Load()
        {
            try
            {
                SettingHandler handler = SettingHandler.Load("BuildAdvanced.xml");
                handler.Get("visible", ref visible);
                position.x = handler.Get("windowPositionX", position.x);
                position.y = handler.Get("windowPositionY", position.y);
                handler.Get("compactMode", ref compactMode);
                handler.Get("compactCollapseRight", ref compactCollapseRight);
                handler.Get("showAllStages", ref showAllStages);
                handler.Get("showAtmosphericDetails", ref showAtmosphericDetails);
                handler.Get("showSettings", ref showSettings);
                CelestialBodies.SetSelectedBody(handler.Get("selectedBodyName", CelestialBodies.SelectedBody.Name));
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex, "BuildAdvanced.Load()");
            }
        }

        private void OnSizeChanged()
        {
            InitialiseStyles();
            hasChanged = true;
        }

        /// <summary>
        ///     Draws the OnGUI window.
        /// </summary>
        private void Window(int windowId)
        {
            try
            {
                compactModeRect = new Rect(position.width - 70.0f * GuiDisplaySize.Offset, 5.0f, 65.0f * GuiDisplaySize.Offset, 20.0f);

                // Draw the compact mode toggle.
                if (GUI.Toggle(compactModeRect, compactMode, "COMPACT", buttonStyle) != compactMode)
                {
                    hasChanged = true;
                    compactCheck = 2;
                    compactRight = position.xMax;
                    compactMode = !compactMode;
                }

                // When not in compact mode draw the 'All Stages' and 'Atmospheric' toggles.
                if (!compactMode)
                {
                    //if (GUI.Button(new Rect(position.width - 143.0f * GuiDisplaySize.Offset, 5.0f, 70.0f * GuiDisplaySize.Offset, 20.0f), "SETTINGS", buttonStyle))
                    //{
                    //    SettingsWindow.Open();
                    //}
                    if (GUI.Toggle(new Rect(position.width - 143.0f * GuiDisplaySize.Offset, 5.0f, 70.0f * GuiDisplaySize.Offset, 20.0f), showSettings, "SETTINGS", buttonStyle) != showSettings)
                    {
                        hasChanged = true;
                        showSettings = !showSettings;
                    }

                    if (GUI.Toggle(new Rect(position.width - 226.0f * GuiDisplaySize.Offset, 5.0f, 80.0f * GuiDisplaySize.Offset, 20.0f), showAllStages, "ALL STAGES", buttonStyle) != showAllStages)
                    {
                        hasChanged = true;
                        showAllStages = !showAllStages;
                    }

                    if (GUI.Toggle(new Rect(position.width - 324.0f * GuiDisplaySize.Offset, 5.0f, 95.0f * GuiDisplaySize.Offset, 20.0f), showAtmosphericDetails, "ATMOSPHERIC", buttonStyle) != showAtmosphericDetails)
                    {
                        hasChanged = true;
                        showAtmosphericDetails = !showAtmosphericDetails;
                    }

                    bodiesListPosition = new Rect(position.width - 452.0f * GuiDisplaySize.Offset, 5.0f, 125.0f * GuiDisplaySize.Offset, 20.0f);
                    bodiesList.enabled = GUI.Toggle(bodiesListPosition, bodiesList.enabled, "BODY: " + CelestialBodies.SelectedBody.Name.ToUpper(), buttonStyle);
                    bodiesList.SetPosition(bodiesListPosition.Translate(position), bodiesListPosition);

                    if (GUI.Toggle(new Rect(position.width - 485.0f * GuiDisplaySize.Offset, 5.0f, 30.0f * GuiDisplaySize.Offset, 20.0f), showRCS, "RCS", buttonStyle) != showRCS) {
                        hasChanged = true;
                        showRCS = !showRCS;
                    }
                }
                else
                {
                    if (GUI.Toggle(new Rect(position.width - 133.0f * GuiDisplaySize.Offset, 5.0f, 60.0f * GuiDisplaySize.Offset, 20.0f), showAtmosphericDetails, "ATMO", buttonStyle) != showAtmosphericDetails)
                    {
                        hasChanged = true;
                        showAtmosphericDetails = !showAtmosphericDetails;
                    }
                }

                // Draw the main informational display box.
                if (!compactMode)
                {
                    GUILayout.BeginHorizontal(areaStyle);
                    DrawStageNumbers();
                    DrawPartCount();
                    DrawCost();
                    DrawMass();

                    if (showRCS)
                        DrawRCS();
                    else {
                        DrawIsp();
                        DrawThrust();
                        DrawTorque();
                        DrawTwr();
                        DrawDeltaV();
                        DrawBurnTime();
                    }

                    GUILayout.EndHorizontal();

                    if (showAtmosphericDetails && !compactMode)
                    {
                        GUILayout.BeginVertical(areaSettingStyle);
                        DrawAtmosphericDetails();
                        GUILayout.EndVertical();
                    }

                    if (showSettings)
                    {
                        GUILayout.BeginVertical(areaSettingStyle);
                        DrawSettings();
                        GUILayout.EndVertical();
                    }
                }
                else // Draw only a few details when in compact mode.
                {
                    GUILayout.BeginHorizontal(areaStyle);
                    DrawStageNumbers();
                    DrawTwr();
                    DrawDeltaV();
                    GUILayout.EndHorizontal();
                }

                GUI.DragWindow();
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex, "BuildAdvanced.Window()");
            }
        }
    }
}