using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalEngineer.UIControls {
    public class PopOutColorPicker : PopOutElement {

        float colorPickerSliderValueR = -1;
        string colorPickerStringValueR = "";
        float colorPickerSliderValueG = -1;
        string colorPickerStringValueG = "";
        float colorPickerSliderValueB = -1;
        string colorPickerStringValueB = "";

        /// <summary>
        ///     Draws the color picker
        /// </summary>
        public Color DrawColorPicker(Color initial) {

            Color color = initial;

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("Box");

            {//RED
                GUILayout.BeginHorizontal();
                GUILayout.Label("R", GUILayout.Width(10));
                float r = GUILayout.HorizontalSlider(color.r, 0f, 1f);
                if (r != colorPickerSliderValueR) {
                    colorPickerSliderValueR = r;
                    color.r = r;
                    colorPickerStringValueR = ((int)(r * 255)).ToString();
                }
                int rint = (int)(color.r * 255);
                var rstring = GUILayout.TextField(rint.ToString(), 3, GUILayout.Width(30));
                if (rstring != colorPickerStringValueR) {
                    if (int.TryParse(rstring, out rint)) {
                        color.r = (float)rint / 255;
                        colorPickerSliderValueR = color.r;
                        colorPickerStringValueR = rstring;
                    }
                }
                GUILayout.EndHorizontal();
            }

            { //Green
                GUILayout.BeginHorizontal();
                GUILayout.Label("G", GUILayout.Width(10));
                float g = GUILayout.HorizontalSlider(color.g, 0f, 1f);
                if (g != colorPickerSliderValueG) {
                    colorPickerSliderValueG = g;
                    color.g = g;
                    colorPickerStringValueG = ((int)(g * 255)).ToString();
                }
                int gint = (int)(color.g * 255);
                var gstring = GUILayout.TextField(gint.ToString(), 3, GUILayout.Width(30));
                if (gstring != colorPickerStringValueG) {
                    if (int.TryParse(gstring, out gint)) {
                        color.g = (float)gint / 255;
                        colorPickerSliderValueG = color.g;
                        colorPickerStringValueG = gstring;
                    }
                }
                GUILayout.EndHorizontal();
            }

            { //Blue
                GUILayout.BeginHorizontal();
                GUILayout.Label("B", GUILayout.Width(10));
                float b = GUILayout.HorizontalSlider(color.b, 0f, 1f);
                if (b != colorPickerSliderValueB) {
                    colorPickerSliderValueB = b;
                    color.b = b;
                    colorPickerStringValueB = ((int)(b * 255)).ToString();
                }
                int bint = (int)(color.b * 255);
                var bstring = GUILayout.TextField(bint.ToString(), 3, GUILayout.Width(30));
                if (bstring != colorPickerStringValueB) {
                    if (int.TryParse(bstring, out bint)) {
                        color.b = (float)bint / 255;
                        colorPickerSliderValueB = color.b;
                        colorPickerStringValueB = bstring;
                    }
                }
                GUILayout.EndHorizontal();
            }

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

            //   GUILayout.EndArea();
            //Finally return the modified value.

            return color;

        }

    }
}
