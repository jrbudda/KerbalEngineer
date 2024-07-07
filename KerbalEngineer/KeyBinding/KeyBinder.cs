// 
//     Kerbal Engineer Redux
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

namespace KerbalEngineer.KeyBinding
{
    using System;
    using System.IO;
    using Extensions;
    using Helpers;
    using UnityEngine;

    public class KeyBinder : MonoBehaviour
    {
        private const string LOCK_ID = "KER_KeyBinder";
        private static readonly string filePath = Path.Combine(EngineerGlobals.SettingsPath, "KeyBinds.xml");
        private static KeyBindingsObject bindings;
        private static Rect position = new Rect(Screen.width, Screen.height, 500.0f, 0.0f);
        private static bool hasCentred;

        static KeyBinder()
        {
            Load();
        }

        /// <summary>
        ///     Gets whether the key binder window is open.
        /// </summary>
        public static bool IsOpen { get; private set; }

        /// <summary>
        ///     Gets and sets the key bindings object.
        /// </summary>
        public static KeyBindingsObject Bindings
        {
            get
            {
                if (bindings == null)
                {
                    bindings = new KeyBindingsObject();
                }
                return bindings;
            }
            private set
            {
                if (value != null)
                {
                    bindings = value;
                }
            }
        }

        /// <summary>
        ///     Gets and sets the editor show/hide binding.
        /// </summary>
        public static KeyCode EditorShowHide
        {
            get
            {
                return Bindings.EditorShowHide;
            }
            set
            {
                Bindings.EditorShowHide = value;
                Save();
            }
        }

        /// <summary>
        ///     Gets and sets the flight show/hide binding.
        /// </summary>
        public static KeyCode FlightShowHide
        {
            get
            {
                return Bindings.FlightShowHide;
            }
            set
            {
                Bindings.FlightShowHide = value;
                Save();
            }
        }

        /// <summary>
        ///     Gets and sets the part info show/hide binding.
        /// </summary>
        public static KeyCode PartInfoShowHide
        {
            get
            {
                return Bindings.PartInfoShowHide;
            }
            set
            {
                Bindings.PartInfoShowHide = value;
                Save();
            }
        }
        
        public static KeyCode HudGroup1ShowHide {
            get { return Bindings.HudGroup1ShowHide; }
            set {
                Bindings.HudGroup1ShowHide = value;
                Save();
            }
        }
        public static KeyCode HudGroup2ShowHide {
            get { return Bindings.HudGroup2ShowHide; }
            set
            {
                Bindings.HudGroup2ShowHide = value;
                Save();
            }
        }
        public static KeyCode HudGroup3ShowHide {
            get { return Bindings.HudGroup3ShowHide; }
            set {
                Bindings.HudGroup3ShowHide = value;
                Save();
            }
        }
        public static KeyCode HudGroup4ShowHide {
            get { return Bindings.HudGroup4ShowHide; }
            set {
                Bindings.HudGroup4ShowHide = value;
                Save();
            }
        }

        /// <summary>
        ///     Gets and sets the input lock state.
        /// </summary>
        public bool InputLock
        {
            get
            {
                return InputLockManager.GetControlLock(LOCK_ID) != ControlTypes.None;
            }
            set
            {
                if (value)
                {
                    InputLockManager.SetControlLock(ControlTypes.All, LOCK_ID);
                }
                else
                {
                    InputLockManager.SetControlLock(ControlTypes.None, LOCK_ID);
                }
            }
        }

        /// <summary>
        ///     Loads the key bindings from disk.
        /// </summary>
        public static void Load()
        {
            Bindings = XmlHelper.LoadObject<KeyBindingsObject>(filePath);
        }

        /// <summary>
        ///     Saves the key bindings to disk.
        /// </summary>
        public static void Save()
        {
            XmlHelper.SaveObject(filePath, Bindings);
        }

        /// <summary>
        ///     Shows the key binding window.
        /// </summary>
        public static void Show()
        {
            if (IsOpen)
            {
                return;
            }

            new GameObject("KeyBinder").AddComponent<KeyBinder>();
        }

        /// <summary>
        ///     Called by unity when component is created.
        /// </summary>
        protected virtual void Awake()
        {
            if (IsOpen)
            {
                Destroy(gameObject);
            }
            else
            {
                IsOpen = true;
                position.height = 0.0f;
            }
        }

        /// <summary>
        ///     Called by unity when component is destroyed.
        /// </summary>
        protected virtual void OnDestroy()
        {
            IsOpen = false;
            InputLock = false;
        }

        /// <summary>
        ///     Called by unity to draw the GUI.
        /// </summary>
        protected virtual void OnGUI()
        {
            position = GUILayout.Window(GetInstanceID(), position, RenderWindow, "Kerbal Engineer Redux - Key Bindings", HighLogic.Skin.window).ClampToScreen();
            CentreWindow();
        }

        /// <summary>
        ///     Called by unity every frame.
        /// </summary>
        protected virtual void Update()
        {
            UpdateInputLock();
        }

        /// <summary>
        ///     Renders a key bind option.
        /// </summary>
        private static void RenderKeyBind(string name, KeyCode currentBinding, Action<KeyCode> acceptClicked)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(name);
            if (GUILayout.Button(currentBinding.ToString(), HighLogic.Skin.button, GUILayout.Width(100.0f)))
            {
                KeyBindPopup.Show(name, currentBinding, acceptClicked);
            }
            GUILayout.EndHorizontal();
        }

        /// <summary>
        ///     Centres the window on screen.
        /// </summary>
        private void CentreWindow()
        {
            if (hasCentred == false && position.width > 0.0f && position.height > 0.0f)
            {
                hasCentred = true;
                position.center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            }
        }

        /// <summary>
        ///     Renders the GUI window contents.
        /// </summary>
        private void RenderWindow(int id)
        {
            GUILayout.BeginVertical(HighLogic.Skin.textArea);
            RenderKeyBind("Editor Show/Hide", EditorShowHide, binding => EditorShowHide = binding);
            RenderKeyBind("Flight Show/Hide", FlightShowHide, binding => FlightShowHide = binding);
            RenderKeyBind("Part Info Show/Hide", PartInfoShowHide, binding => PartInfoShowHide = binding);
            RenderKeyBind("HUD Group 1 Show/Hide", HudGroup1ShowHide, binding => HudGroup1ShowHide = binding);
            RenderKeyBind("HUD Group 2 Show/Hide", HudGroup2ShowHide, binding => HudGroup2ShowHide = binding);
            RenderKeyBind("HUD Group 3 Show/Hide", HudGroup3ShowHide, binding => HudGroup3ShowHide = binding);
            RenderKeyBind("HUD Group 4 Show/Hide", HudGroup4ShowHide, binding => HudGroup4ShowHide = binding);
            GUILayout.EndVertical();

            if (GUILayout.Button("Close", HighLogic.Skin.button))
            {
                Destroy(gameObject);
            }

            GUI.DragWindow();
        }

        /// <summary>
        ///     Updates the input lock.
        /// </summary>
        private void UpdateInputLock()
        {
            bool mouseOver = position.MouseIsOver();
            bool inputLock = InputLock;

            if (mouseOver && inputLock == false)
            {
                InputLock = true;
            }
            else if (mouseOver == false && inputLock)
            {
                InputLock = false;
            }
        }
    }
}