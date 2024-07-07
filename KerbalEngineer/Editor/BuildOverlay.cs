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
    using Helpers;
    using Settings;
    using UnityEngine;

    #endregion

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class BuildOverlay : MonoBehaviour
    {
        #region Fields

        private static BuildOverlayPartInfo buildOverlayPartInfo;
        private static BuildOverlayResources buildOverlayResources;
        private static BuildOverlayVessel buildOverlayVessel;
        private static BuildOverlay instance;

        private static float minimumWidth = 200.0f;
        private static GUIStyle nameStyle;
        private static float tabSpeed = 5.0f;
        private static GUIStyle tabStyle;
        private static GUIStyle titleStyle;
        private static GUIStyle valueStyle;
        private static GUIStyle windowStyle;

        #endregion

        #region Properties

        public static BuildOverlayPartInfo BuildOverlayPartInfo
        {
            get { return buildOverlayPartInfo; }
        }

        public static BuildOverlayResources BuildOverlayResources
        {
            get { return buildOverlayResources; }
        }

        public static BuildOverlayVessel BuildOverlayVessel
        {
            get { return buildOverlayVessel; }
        }

        public static float MinimumWidth
        {
            get { return minimumWidth; }
            set { minimumWidth = value; }
        }

        public static GUIStyle NameStyle
        {
            get
            {
                return (nameStyle != null && !hasChanged) ? nameStyle : (nameStyle = new GUIStyle
                {
                    normal =
                    {
                        textColor = Color.white
                    },
                    fontSize = (int)(11 * GuiDisplaySize.Offset),
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.UpperLeft,
                    stretchWidth = true
                });
            }
        }

        public static float TabSpeed
        {
            get { return tabSpeed; }
            set { tabSpeed = value; }
        }

        public static GUIStyle TabStyle
        {
            get
            {
                return (tabStyle != null && !hasChanged) ? tabStyle : (tabStyle = new GUIStyle
                {
                    normal =
                    {
                        background = TextureHelper.CreateTextureFromColour(new Color(0.0f, 0.0f, 0.0f, 0.5f)),
                        textColor = Color.yellow
                    },
                    hover =
                    {
                        background = TextureHelper.CreateTextureFromColour(new Color(0.0f, 0.0f, 0.0f, 0.75f)),
                        textColor = Color.yellow
                    },
                    onNormal =
                    {
                        background = TextureHelper.CreateTextureFromColour(new Color(0.0f, 0.0f, 0.0f, 0.5f)),
                        textColor = Color.yellow
                    },
                    onHover =
                    {
                        background = TextureHelper.CreateTextureFromColour(new Color(0.0f, 0.0f, 0.0f, 0.75f)),
                        textColor = Color.yellow
                    },
                    padding = new RectOffset(20, 20, 0, 0),
                    fontSize = (int)(11 * GuiDisplaySize.Offset),
                    fontStyle = FontStyle.Bold,
                    alignment = TextAnchor.MiddleCenter,
                    fixedHeight = 15.0f,
                    stretchWidth = true
                });
            }
        }

        public static GUIStyle TitleStyle
        {
            get
            {
                return (titleStyle != null && !hasChanged ) ? titleStyle : (titleStyle = new GUIStyle
                {
                    normal =
                    {
                        textColor = Color.yellow
                    },
                    fontSize = (int)(11 * GuiDisplaySize.Offset),
                    fontStyle = FontStyle.Bold,
                    stretchWidth = true
                });
            }
        }

        private static bool hasChanged = false, resetHasChanged = false;

        protected void OnGUI() {
            if (hasChanged && Event.current.type == EventType.Repaint) {
                if (resetHasChanged) hasChanged = resetHasChanged = false;
                else resetHasChanged = true;
            }
        }

        public static GUIStyle ValueStyle
        {
            get
            {
                return (valueStyle != null && !hasChanged) ? valueStyle : (valueStyle = new GUIStyle
                {
                    normal =
                    {
                        textColor = Color.white
                    },
                    fontSize = (int)(11 * GuiDisplaySize.Offset),
                    fontStyle = FontStyle.Normal,
                    alignment = TextAnchor.UpperRight,
                    stretchWidth = true
                });
            }
        }

        public static bool Visible
        {
            get { return BuildOverlayPartInfo.Visible && BuildOverlayVessel.Visible && BuildOverlayResources.Visible; }
            set { BuildOverlayPartInfo.Visible = BuildOverlayVessel.Visible = BuildOverlayResources.Visible = value; }
        }

        public static GUIStyle WindowStyle
        {
            get
            {
                return (windowStyle != null && !hasChanged) ? windowStyle : (windowStyle = new GUIStyle
                {
                    normal =
                    {
                        background = TextureHelper.CreateTextureFromColour(new Color(0.0f, 0.0f, 0.0f, 0.5f))
                    },
                    padding = new RectOffset(5, 5, 3, 3),
                });
            }
        }

        #endregion

        #region Methods

        public static void Load()
        {
            var handler = SettingHandler.Load("BuildOverlay.xml");
            Visible = handler.GetSet("visible", Visible);
            BuildOverlayPartInfo.NamesOnly = handler.GetSet("namesOnly", BuildOverlayPartInfo.NamesOnly);
            BuildOverlayPartInfo.ClickToOpen = handler.GetSet("clickToOpen", BuildOverlayPartInfo.ClickToOpen);
            buildOverlayVessel.Open = handler.GetSet("vesselOpen", buildOverlayVessel.Open);
            buildOverlayResources.Open = handler.GetSet("resourcesOpen", buildOverlayResources.Open);
            buildOverlayVessel.WindowX = handler.GetSet("vesselWindowX", buildOverlayVessel.WindowX);
            handler.Save("BuildOverlay.xml");
        }

        public static void Save()
        {
            var handler = SettingHandler.Load("BuildOverlay.xml");
            handler.Set("visible", Visible);
            handler.Set("namesOnly", BuildOverlayPartInfo.NamesOnly);
            handler.Set("clickToOpen", BuildOverlayPartInfo.ClickToOpen);
            handler.Set("vesselOpen", buildOverlayVessel.Open);
            handler.Set("resourcesOpen", buildOverlayResources.Open);
            handler.Set("vesselWindowX", buildOverlayVessel.WindowX);
            handler.Save("BuildOverlay.xml");
        }

        protected void Awake()
        {
            try
            {
                if (instance != null)
                {
                    Destroy(this);
                    return;
                }
                instance = this;
                tabStyle = windowStyle = null; //The background textures need to be reinitialized each scene change
                buildOverlayPartInfo = this.gameObject.AddComponent<BuildOverlayPartInfo>();
                buildOverlayVessel = this.gameObject.AddComponent<BuildOverlayVessel>();
                buildOverlayResources = this.gameObject.AddComponent<BuildOverlayResources>();
                Load();
                GuiDisplaySize.OnSizeChanged += OnSizeChanged;
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        private void OnSizeChanged() {
            hasChanged = true;
        }

        protected void OnDestroy()
        {
            try
            {
                Save();
                if (buildOverlayPartInfo != null)
                {
                    Destroy(buildOverlayPartInfo);
                }
                if (buildOverlayVessel != null)
                {
                    Destroy(buildOverlayVessel);
                }
                if (buildOverlayResources != null)
                {
                    Destroy(buildOverlayResources);
                }
                GuiDisplaySize.OnSizeChanged -= OnSizeChanged;
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        #endregion
    }
}