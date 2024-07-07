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

using UnityEngine;

#endregion

namespace KerbalEngineer.UIControls
{
    public class PopOutElement : MonoBehaviour
    {
        #region Fields

        private Rect button;
        private Rect position = new Rect(-9000.0f, -9000.0f, 0, 0);
        private int windowId;
        private bool hasWindowId = false;

        #endregion

        #region Properties

        public int ResizeCounter { get; set; }
        public int Depth { get; set; } = -10000;

        public Callback DrawCallback { get; set; }
        public Callback ClosedCallback { get; set; }


        public Rect Position
        {
            get { return this.position; }
        }

        #endregion

        public void Open() {
            if (this.enabled && hasWindowId) GUI.BringWindowToFront(windowId);
            this.enabled = true;
        }

        public void Close() {
            OnClose();
            this.enabled = false;
            this.ClosedCallback?.Invoke();
            position = new Rect(-9000.0f, -9000.0f, 0, 0); //There's a 1-frame delay before we're repositioned the next time we're opened, this keeps it off-screen for that frame
        }

        protected virtual void OnClose() { }

        protected virtual bool AllowClose() { return true; }

        #region Initialisation

        private void Awake()
        {
            try
            {
                this.enabled = false;
                this.ResizeCounter = 1;
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        private void Start()
        {
            try
            {
                this.InitialiseStyles();
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        #endregion

        #region Styles

        private GUIStyle windowStyle;

        private void InitialiseStyles()
        {
            try
            {
                this.windowStyle = new GUIStyle
                {
                    normal =
                    {
                        background = GameDatabase.Instance.GetTexture("KerbalEngineer/Textures/DropDownBackground", false)
                    },
                    border = new RectOffset(8, 8, 1, 8),
                    margin = new RectOffset(),
                    padding = new RectOffset(5, 5, 5, 5)
                };
            }
            catch (Exception ex)
            {
                MyLogger.Exception(ex);
            }
        }

        #endregion

        #region Updating

        private void Update()
        {
            try {
                if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) &&
                    !this.position.MouseIsOver() && !this.button.MouseIsOver() && this.enabled && this.AllowClose())
                {
                    Close();
                }
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        #endregion

        #region Drawing

        private void OnGUI()
        {
            try {
                if (this.ResizeCounter > 0) {
                    this.position.height = 0;
                    this.ResizeCounter--;
                }

                GUI.skin = null;
                this.position = GUILayout.Window(this.GetInstanceID(), this.position, this.Window, string.Empty, this.windowStyle);
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        private void Window(int _windowId)
        {
            try {
                windowId = _windowId;
                hasWindowId = true;
                GUI.depth = Depth;
                if (this.DrawCallback != null) this.DrawCallback.Invoke();
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        #endregion

        #region Public Methods

        public void SetPosition(Rect button, Rect size)
        {
            try {
                this.position.x = button.x;
                this.position.y = button.y + button.height;
                this.position.width = size.width;
                this.button = button;
            } catch (Exception ex) {
                MyLogger.Exception(ex);
            }
        }

        #endregion
    }
}