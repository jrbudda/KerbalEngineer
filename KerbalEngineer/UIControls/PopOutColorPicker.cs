using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalEngineer.UIControls {
    public class PopOutColorPicker : PopOutElement {
        private ColorPickerElement Red = new ColorPickerElement("R"),
                                 Green = new ColorPickerElement("G"),
                                  Blue = new ColorPickerElement("B"),
                                 Alpha = new ColorPickerElement("A");

        /// <summary>
        ///     Draws the color picker
        /// </summary>
        public Color DrawColorPicker(Color initial) {
            Color color = initial;

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical("Box");
            color.r =   Red.Draw(color.r);
            color.g = Green.Draw(color.g);
            color.b =  Blue.Draw(color.b);
            color.a = Alpha.Draw(color.a);
            GUILayout.EndVertical();

            ////Color Preview
            //GUILayout.BeginVertical("Box", new GUILayoutOption[] { GUILayout.Width(44), GUILayout.Height(44) });
            ////Apply color to following label
            //GUI.color = color;
            //GUILayout.Label(tex);

            ////Revert color to white to avoid messing up any following controls.
            //GUI.color = Color.white;

            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("RESET")) {
                color = HighLogic.Skin.label.normal.textColor;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            //Finally return the modified value.
            return color;
        }

        private class ColorPickerElement {
            public string label = "";
            public float labelWidth = 10.0f;

            private float sliderValue = -1;
            private string stringValue = "";

            public ColorPickerElement(string _label, float _labelWidth = 10.0f) {
                label = _label;
                labelWidth = _labelWidth;
            }

            public float Draw(float currentValue) {
                GUILayout.BeginHorizontal();

                GUILayout.Label(label, GUILayout.Width(labelWidth));

                float returnedFloat = GUILayout.HorizontalSlider(currentValue, 0f, 1f);
                if (returnedFloat != sliderValue) {
                    sliderValue = returnedFloat;
                    currentValue = returnedFloat;
                    stringValue = ((int)(returnedFloat * 255)).ToString();
                }

                int valueInt = (int)(currentValue * 255);
                var returnedString = GUILayout.TextField(valueInt.ToString(), 3, GUILayout.Width(30));
                if (returnedString != stringValue) {
                    if (int.TryParse(returnedString, out valueInt)) {
                        currentValue = (float)valueInt / 255;
                        sliderValue = currentValue;
                        stringValue = returnedString;
                    }
                }

                GUILayout.EndHorizontal();

                return currentValue;
            }
        }
    }
}
