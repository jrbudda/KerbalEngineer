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

using KerbalEngineer.Extensions;
using KerbalEngineer.Helpers;

using UnityEngine;

#endregion

namespace KerbalEngineer.Flight.Sections {
    public class SectionWindow : MonoBehaviour {
        #region Fields

        private bool resizeRequested;
        private int windowId;
        private Rect windowPosition;
        private bool dragStartedOnUs = false;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets and sets the parent section for the floating section window.
        /// </summary>
        public SectionModule ParentSection { get; set; }

        /// <summary>
        ///     Gets and sets the window position.
        /// </summary>
        public Rect WindowPosition {
            get { return this.windowPosition; }
            set { this.windowPosition = value; }
        }

        #endregion

        #region GUIStyles

        #region Fields

        private GUIStyle hudWindowBgStyle;
        private GUIStyle hudWindowStyle;
        private GUIStyle windowStyle;

        private const int BORDER_WIDTH = 1, BORDER_TEXTURE_DIMENSIONS = BORDER_WIDTH * 2 + 1;
        private static Texture2D borderTexture;
        private static GUIStyle borderStyle;

        #endregion

        /// <summary>
        ///     Initialises all the styles required for this object.
        /// </summary>
        private void InitialiseStyles() {
            this.windowStyle = new GUIStyle(HighLogic.Skin.window) {
                margin = new RectOffset(),
                padding = new RectOffset(3, 3, 0, 5),
            };

            this.hudWindowStyle = new GUIStyle(this.windowStyle) {
                normal = { background = null },
                onNormal = { background = null },
                padding = new RectOffset(3, 3, 0, 10),
            };

            var hudBackgroundColorTexture = TextureHelper.CreateTextureFromColour(this.ParentSection == null ? new Color(0.0f, 0.0f, 0.0f, 0.5f) : this.ParentSection.HudBackgroundColor);
            this.hudWindowBgStyle = new GUIStyle(this.hudWindowStyle) {
                normal = { background = hudBackgroundColorTexture },
                onNormal = { background = hudBackgroundColorTexture }
            };

            //Hardest stroked DrawRect in the universe
            if (borderTexture == null) {
                borderTexture = new Texture2D(BORDER_TEXTURE_DIMENSIONS, BORDER_TEXTURE_DIMENSIONS);
                Color[] textureArray = new Color[BORDER_TEXTURE_DIMENSIONS * BORDER_TEXTURE_DIMENSIONS];
                for (int i = 0; i < textureArray.Length; i++) textureArray[i] = new Color(0.0f, 1.0f, 0.0f, 1.0f);
                borderTexture.filterMode = FilterMode.Point;
                //borderTexture.wrapMode = TextureWrapMode.Clamp;
                borderTexture.SetPixels(textureArray);
                borderTexture.SetPixel(BORDER_WIDTH, BORDER_WIDTH, new Color(0.0f, 0.0f, 0.0f, 0.0f));
                borderTexture.Apply();
            }
            if (borderStyle == null) {
                borderStyle = new GUIStyle() {
                    normal = { background = borderTexture },
                    border = new RectOffset(BORDER_WIDTH, BORDER_WIDTH, BORDER_WIDTH, BORDER_WIDTH)
                };
            }
        }

        private void OnSizeChanged() {
            this.InitialiseStyles();
            this.RequestResize();
        }

        #endregion

        #region Drawing

        /// <summary>
        ///     Called to draw the floating section window when the UI is enabled.
        /// </summary>
        private void OnGUI() {
            if (!HighLogic.LoadedSceneIsFlight || this.ParentSection == null || !this.ParentSection.IsVisible || DisplayStack.Instance == null ||
                (DisplayStack.Instance.Hidden && !this.ParentSection.IsHud) || !FlightEngineerCore.IsDisplayable)
            { return; }

            if (this.resizeRequested) {
                this.windowPosition.width = 0;
                this.windowPosition.height = 0;
                this.resizeRequested = false;
            }
            GUI.skin = null;
            this.windowPosition = GUILayout.Window(this.windowId, this.windowPosition, this.Window, string.Empty,
                                                   !this.ParentSection.IsHud ? this.windowStyle
                                                       : this.ParentSection.IsHudBackground && this.ParentSection.LineCount > 0
                                                           ? this.hudWindowBgStyle
                                                           : this.hudWindowStyle);

            windowPosition = (ParentSection.IsHud) ? windowPosition.ClampInsideScreen() : windowPosition.ClampToScreen();
            
            if (this.ParentSection.IsHud && this.ParentSection.IsEditorVisible) {
                GUI.depth = -1000;
                GUI.Box(this.windowPosition, GUIContent.none, borderStyle);
            }


            this.ParentSection.FloatingPositionX = this.windowPosition.x;
            this.ParentSection.FloatingPositionY = this.windowPosition.y;


            switch (Event.current.type) {
                case EventType.MouseDown:
                    dragStartedOnUs = this.windowPosition.Contains(Event.current.mousePosition);

                    if (Event.current.button == 2 /* MMB */ && this.ParentSection.IsHud && dragStartedOnUs) {
                        this.ParentSection.IsEditorVisible = !this.ParentSection.IsEditorVisible;
                    }
                    break;

                case EventType.MouseDrag:
                    if ((!this.ParentSection.IsHud || this.ParentSection.IsEditorVisible) &&
                        dragStartedOnUs && ResizingWidth())
                    {
                        if (ParentSection.IsHud) ParentSection.HudWidth += Event.current.delta.x;
                        else ParentSection.Width += Event.current.delta.x;
                        this.resizeRequested = true;
                    }
                    break;

                case EventType.MouseUp:
                    dragStartedOnUs = false;
                    break;
            }
        }

        /// <summary>
        ///     Draws the floating section window.
        /// </summary>
        private void Window(int windowId) {
            this.ParentSection.Draw();

            if ((!this.ParentSection.IsHud || this.ParentSection.IsEditorVisible) && !ResizingWidth()) {
                GUI.DragWindow();
            }
        }

        #endregion

        #region Destruction

        /// <summary>
        ///     Runs when the object is destroyed.
        /// </summary>
        private void OnDestroy() {
            GuiDisplaySize.OnSizeChanged -= this.OnSizeChanged;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Request that the floating section window's size is reset in the next draw call.
        /// </summary>
        public void RequestResize() {
            this.resizeRequested = true;
        }

        #endregion

        #region Methods: private

        /// <summary>
        ///     Initialises the object's state on creation.
        /// </summary>
        private void Start() {
            this.windowId = this.GetHashCode();
            this.InitialiseStyles();

            GuiDisplaySize.OnSizeChanged += this.OnSizeChanged;
        }

        private bool ResizingWidth() { return Event.current.button == 1 /* RMB */ || Event.current.alt; }

        #endregion
    }
}