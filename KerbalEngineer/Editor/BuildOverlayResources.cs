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
    using System;
    using System.Collections.Generic;
    using Extensions;
    using UnityEngine;
    using KSP.Localization;

    public class BuildOverlayResources : MonoBehaviour
    {
        #region Fields
        private static bool visible = true;

        private readonly Dictionary<int, ResourceInfoItem> resources = new Dictionary<int, ResourceInfoItem>();

        private bool open = true;
        private float openPercent;
        private GUIContent tabContent;
        private Rect tabPosition;
        private Vector2 tabSize;
        private Rect windowPosition = new Rect(0.0f, 0.0f, BuildOverlay.MinimumWidth, 0.0f);
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
        #endregion

        #region Methods: protected
        protected void OnGUI()
        {
            try
            {
                if (!Visible || resources.Count == 0)
                {
                    return;
                }

                open = GUI.Toggle(tabPosition, open, tabContent, BuildOverlay.TabStyle);
                if (openPercent > 0.0)
                {
                    windowPosition = GUILayout.Window(GetInstanceID(), windowPosition, Window, String.Empty, BuildOverlay.WindowStyle);
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
                tabContent = new GUIContent(Localizer.Format("#KE_RESOURCESLabel"));//"RESOURCES"
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
                if (!Visible)
                {
                    return;
                }

                SetResources();
                SetSlidePosition();
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }
        #endregion

        #region Methods: private
        private static Part part;
        private static PartResource partResource;

        private void SetResources()
        {
            int previousCount = resources.Count;
            resources.Clear();

            for (int i = 0; i < EditorLogic.fetch.ship.parts.Count; ++i)
            {
                part = EditorLogic.fetch.ship.parts[i];
                for (int j = 0; j < part.Resources.dict.Count; ++j)
                {
                    partResource = part.Resources.dict.At(j);

                    if (resources.ContainsKey(partResource.info.id))
                    {
                        resources[partResource.info.id].Amount += partResource.amount;
                    }
                    else
                    {
                        resources.Add(partResource.info.id, new ResourceInfoItem(partResource));
                    }
                }
            }

            if (resources.Count < previousCount)
            {
                windowPosition.height = 0;
            }
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

            windowPosition.x = BuildOverlay.BuildOverlayVessel.WindowPosition.xMax + 5.0f;
            windowPosition.y = Mathf.Lerp(Screen.height, Screen.height - windowPosition.height, openPercent);
            tabPosition.width = tabSize.x;
            tabPosition.height = tabSize.y;
            tabPosition.x = windowPosition.x;
            tabPosition.y = windowPosition.y - tabPosition.height;
        }

        private void Window(int windowId)
        {
            try
            {
                bool firstItem = true;
                foreach (KeyValuePair<int, ResourceInfoItem> resource in resources)
                {
                    if (!firstItem)
                    {
                        GUILayout.Space(2.0f);
                    }
                    firstItem = false;

                    GUILayout.BeginHorizontal();

                    GUILayout.Label(resource.Value.Name + ":", BuildOverlay.NameStyle);
                    GUILayout.Space(50.0f);
                    if (resource.Value.Mass > 0.0)
                    {
                        GUILayout.Label("(" + resource.Value.Mass.ToMass() + ") " + resource.Value.Amount.ToString("N1"), BuildOverlay.ValueStyle);
                    }
                    else
                    {
                        GUILayout.Label(resource.Value.Amount.ToString("N1"), BuildOverlay.ValueStyle);
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