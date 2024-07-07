using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KerbalEngineer.UIControls {
    public static class UIElements {
        private static readonly GUIStyle checkboxStyle = new GUIStyle(HighLogic.Skin.button) {
            normal = { textColor = Color.white },
            //margin = new RectOffset(0, 0, 5, 5),
            margin = new RectOffset(0, 0, 0, 0),
            padding = new RectOffset(),
            alignment = TextAnchor.MiddleCenter,
            fontSize = 14,
            fontStyle = FontStyle.Normal,
            stretchHeight = true
        };

        public static bool Checkbox(bool currentValue, float size = 20.0f) {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            bool ret = GUILayout.Toggle(currentValue, currentValue ? "✓" : ""/*"✕"*/, checkboxStyle, GUILayout.Height(size), GUILayout.Width(size)); //🗸(<- doesn't work)✓✔ ×✕✖
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            return ret;
        }

        public static void IntTextBox(string currentValue, ref int target, int digits = 3, float width = 30.0f) {
            string returnedString = GUILayout.TextField(currentValue, digits, GUILayout.Width(width));
            if (returnedString != currentValue) {
                if (int.TryParse(returnedString, out int parsedInt)) target = parsedInt;
            }
        }

        public static int IntTextBox(int currentValue, int digits = 3, float width = 30.0f) {
            string currentString = currentValue.ToString();
            string returnedString = GUILayout.TextField(currentString, digits, GUILayout.Width(width));
            if (returnedString != currentString) {
                if (int.TryParse(returnedString, out int parsedInt)) return parsedInt;
            }
            return currentValue;
        }

        public static string DPTextBox(string currentValue, int digits = 3, float width = 30.0f) {
            return GUILayout.TextField(currentValue, digits, GUILayout.Width(width));
        }
    }
}