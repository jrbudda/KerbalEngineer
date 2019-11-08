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

#endregion

namespace KerbalEngineer.Editor
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using Helpers;
    using UnityEngine;
    using VesselSimulator;
    using KSP.Localization;

    #endregion

    public class BuildOverlayVessel : MonoBehaviour
    {
        #region Constants

        private const float Width = 175.0f;

        #endregion

        #region Fields

        private static bool visible = true;

        private readonly List<PartInfoItem> infoItems = new List<PartInfoItem>();

        private Stage lastStage;
        private bool open = true;
        private float openPercent;
        private GUIContent tabContent;
        private Rect tabPosition;
        private Vector2 tabSize;
        private Rect windowPosition = new Rect(330.0f, 0.0f, Width, 0.0f);

        #endregion

        #region Properties

        public static bool Visible
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

        public bool Open
        {
            get
            {
                return open;
            }
            set
            {
                open = value;
            }
        }

        public Rect WindowPosition
        {
            get
            {
                return windowPosition;
            }
        }

        public float WindowX
        {
            get
            {
                return windowPosition.x;
            }
            set
            {
                windowPosition.x = value;
            }
        }

        #endregion

        #region Methods

        protected void Awake()
        {
            try
            {
                SimManager.OnReady -= GetStageInfo;
                SimManager.OnReady += GetStageInfo;
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        protected void OnGUI()
        {
            try
            {
                if (!Visible || EditorLogic.RootPart == null)
                {
                    return;
                }

                open = GUI.Toggle(tabPosition, open, tabContent, BuildOverlay.TabStyle);
                if (openPercent > 0.0)
                {
                    windowPosition = GUILayout.Window(GetInstanceID(), windowPosition, VesselWindow, string.Empty, BuildOverlay.WindowStyle);
                }
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        protected void Start()
        {
            try
            {
                tabContent = new GUIContent(Localizer.Format("#KE_VESSELLabel"));//"VESSEL"
                tabSize = BuildOverlay.TabStyle.CalcSize(tabContent);
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        protected void Update()
        {
            try
            {
                if (!Visible || EditorLogic.RootPart == null)
                {
                    return;
                }

                if (openPercent > 0.0)
                {
                    SetVesselInfo();
                }

                SetSlidePosition();
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        private void GetStageInfo()
        {
            lastStage = SimManager.LastStage;
        }

        private void SetSlidePosition()
        {
            if (open && openPercent < 1.0f)
            {
                openPercent = Mathf.Clamp(openPercent + Time.deltaTime * BuildOverlay.TabSpeed, 0.0f, 1.0f);
            }
            else if (!open && openPercent > 0.0f)
            {
                openPercent = Mathf.Clamp(openPercent - Time.deltaTime * BuildOverlay.TabSpeed, 0.0f, 1.0f);
            }

            windowPosition.y = Mathf.Lerp(Screen.height, Screen.height - windowPosition.height, openPercent);
            if (windowPosition.width < Width)
            {
                windowPosition.width = Width;
            }
            tabPosition.width = tabSize.x;
            tabPosition.height = tabSize.y;
            tabPosition.x = windowPosition.x;
            tabPosition.y = windowPosition.y - tabPosition.height;
        }

        private void SetVesselInfo()
        {
            SimManager.Gravity = CelestialBodies.SelectedBody.Gravity;

            if (BuildAdvanced.Instance.ShowAtmosphericDetails)
            {
                SimManager.Atmosphere = CelestialBodies.SelectedBody.GetAtmospheres(BuildAdvanced.Altitude);
            }
            else
            {
                SimManager.Atmosphere = 0.0;
            }

            SimManager.RequestSimulation();
            SimManager.TryStartSimulation();

            if (lastStage != null)
            {
                PartInfoItem.Release(infoItems);
                infoItems.Clear();
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_VESSEL_DeltaV"), lastStage.deltaV.ToString("N0") + " / " + lastStage.totalDeltaV.ToString("N0") + "m/s"));//"Delta-V"
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_VESSEL_Mass"), Units.ToMass(lastStage.mass, lastStage.totalMass)));//"Mass"
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_VESSEL_TWR"), lastStage.thrustToWeight.ToString("F2") + " (" + lastStage.maxThrustToWeight.ToString("F2") + ")"));//"TWR"
                infoItems.Add(PartInfoItem.Create(Localizer.Format("#KE_VESSEL_Parts"), lastStage.partCount + " / " + lastStage.totalPartCount));//"Parts"
            }
        }

        private void VesselWindow(int windowId)
        {
            try
            {
                bool firstItem = true;
                foreach (PartInfoItem item in infoItems)
                {
                    if (!firstItem)
                    {
                        GUILayout.Space(2.0f);
                    }
                    firstItem = false;

                    GUILayout.BeginHorizontal();
                    if (item.Value != null)
                    {
                        GUILayout.Label(item.Name + ":", BuildOverlay.NameStyle);
                        GUILayout.FlexibleSpace();
                        GUILayout.Label(item.Value, BuildOverlay.ValueStyle);
                    }
                    else
                    {
                        GUILayout.Label(item.Name, BuildOverlay.NameStyle);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        #endregion
    }
}