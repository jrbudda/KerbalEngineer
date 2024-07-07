using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Readouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalEngineer.UIControls {
    public class PopOutReadoutSettings : PopOutElement {
        private const float ROW_HEIGHT = 30.0f, WINDOW_PADDING = 10.0f;

        private static readonly GUIStyle textStyle = new GUIStyle(HighLogic.Skin.label) {
            normal = { textColor = Color.white },
            margin = new RectOffset(7, 7, 0, 0),
            padding = new RectOffset(0, 0, 0, 0),
            alignment = TextAnchor.MiddleLeft,
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            stretchWidth = true,
            stretchHeight = true
        }, columnTextStyle = new GUIStyle(textStyle) {
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(0, 0, 0, 0),
            alignment = TextAnchor.MiddleCenter,
        }, buttonStyle = new GUIStyle(HighLogic.Skin.button) {
            normal = { textColor = Color.white },
            margin = new RectOffset(2, 2, 2, 2),
            padding = new RectOffset(0, 0, 0, 0),
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            stretchHeight = true
        };

        public PopOutColorPicker colorPicker;

        private bool colorPickerHudTarget = false;
        private ReadoutModule editingReadout = null;
        
        private Texture2D swatch = new Texture2D(16, 20);

        
        public PopOutReadoutSettings() {
            this.colorPicker = this.gameObject.AddComponent<PopOutColorPicker>();
            this.colorPicker.Depth = this.Depth - 1;
            this.colorPicker.DrawCallback = () => {
                if (editingReadout != null) {
                    if (colorPickerHudTarget) editingReadout.HudTextColor = editingReadout.HudValueStyle.normal.textColor = this.colorPicker.DrawColorPicker(editingReadout.HudValueStyle.normal.textColor, HighLogic.Skin.label.normal.textColor);
                    else editingReadout.TextColor = editingReadout.ValueStyle.normal.textColor = this.colorPicker.DrawColorPicker(editingReadout.ValueStyle.normal.textColor, HighLogic.Skin.label.normal.textColor);
                }
            };
        }


        public void Draw(ReadoutModule _editingReadout) {
            const float columns2and3Width = ROW_HEIGHT * 1.5f;
            float column1Width = Position.width - WINDOW_PADDING - columns2and3Width * 2.0f;

            editingReadout = _editingReadout;

            //No idea why stuff doesn't quite line up in columns...
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT));

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(column1Width));
            GUILayout.Label("", columnTextStyle);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(columns2and3Width));
            GUILayout.Label("Stack", columnTextStyle);
            GUILayout.EndHorizontal();
            
            
            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(columns2and3Width));
            GUILayout.Label("HUD", columnTextStyle);
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();
            

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT));

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(column1Width));
            GUILayout.Label("Text color", textStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(columns2and3Width));
            GUILayout.FlexibleSpace();
            ColorPickerButton(false);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(columns2and3Width));
            GUILayout.FlexibleSpace();
            ColorPickerButton(true);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();
            
            
            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT));

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(column1Width));
            GUILayout.Label("Hide name", textStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(columns2and3Width));
            GUILayout.FlexibleSpace();
            editingReadout.HideName = UIElements.Checkbox(editingReadout.HideName);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(columns2and3Width));
            GUILayout.FlexibleSpace();
            editingReadout.HudHideName = UIElements.Checkbox(editingReadout.HudHideName);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT));

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(column1Width));
            GUILayout.Label("Use short name", textStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(columns2and3Width));
            GUILayout.FlexibleSpace();
            editingReadout.UseShortName = UIElements.Checkbox(editingReadout.UseShortName);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(columns2and3Width));
            GUILayout.FlexibleSpace();
            editingReadout.HudUseShortName = UIElements.Checkbox(editingReadout.HudUseShortName);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT));

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(column1Width));
            GUILayout.Label("Character limit", textStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(columns2and3Width));
            GUILayout.FlexibleSpace();
            editingReadout.CharacterLimit = UIElements.IntTextBox(editingReadout.CharacterLimit);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(columns2and3Width));
            GUILayout.FlexibleSpace();
            editingReadout.HudCharacterLimit = UIElements.IntTextBox(editingReadout.HudCharacterLimit);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT));

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(column1Width));
            GUILayout.Label("Decimal places", textStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(columns2and3Width));
            GUILayout.FlexibleSpace();
            editingReadout.DecimalPlaces = UIElements.IntTextBox(editingReadout.DecimalPlaces);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal(GUILayout.Height(ROW_HEIGHT), GUILayout.Width(columns2and3Width));
            GUILayout.FlexibleSpace();
            editingReadout.HudDecimalPlaces = UIElements.IntTextBox(editingReadout.HudDecimalPlaces);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndHorizontal();

            if (GUILayout.Button("DONE", buttonStyle, GUILayout.Height(ROW_HEIGHT))) {
                Close();
            }
            
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();


            GUILayout.EndVertical();
            
            GUILayout.EndHorizontal();
        }

        protected override void OnClose() {
            colorPicker.Close();
        }

        protected override bool AllowClose() { return !colorPicker.enabled; }

        private void ColorPickerButton(bool targetHud) {
            Color normalGuiColor = GUI.color;
            GUI.color = targetHud ? editingReadout.HudValueStyle.normal.textColor : editingReadout.ValueStyle.normal.textColor;

            if (GUILayout.Button(swatch, buttonStyle, GUILayout.Width(30.0f))) {
                colorPickerHudTarget = targetHud;
                colorPicker.Open();
            }

            if (colorPicker.enabled && Event.current.type == EventType.Repaint && colorPickerHudTarget == targetHud) {
                colorPicker.SetPosition(GUILayoutUtility.GetLastRect().Translate(Position).Translate(new Rect(8, 0, 8, 8)), new Rect(0, 0, 180, 20));
            }
            
            GUI.color = normalGuiColor;
        }
    }
}