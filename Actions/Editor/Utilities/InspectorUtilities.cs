using UnityEngine;
using UnityEditor;

namespace Ru1t3rl.Editors.Utilities
{
    public static class InspectorUtilities
    {
        public static readonly float Spacing = 15f;

        public const int PREVIEW_SIZE = 50;
        public const int MARGIN = 10;
        public const int PADDING = 25;

        #region Colors
        public static readonly Color SelectedButtonColor = new Color(.4f, .8f, .4f);
        public static Color textColor => EditorGUIUtility.isProSkin ?
            new Color(175f / 255f, 175f / 255f, 175f / 255f) : // DarkTheme
            new Color(30f / 255f, 30f / 255f, 30f / 255f); // Default theme

        public static Color LightTint => EditorGUIUtility.isProSkin ? new Color(71f / 255f, 71f / 255f, 71f / 255f) : new Color(190f / 255f, 190f / 255f, 190f / 255f);
        public static Color DarkTint => EditorGUIUtility.isProSkin ? new Color(59f / 255f, 59f / 255f, 59f / 255f) : new Color(182f / 255f, 182f / 255f, 182f / 255f);
        #endregion

        #region Text Styles
        private static GUIStyle _titleStyle;
        public static GUIStyle TitleStyle
        {
            get
            {
                if (_titleStyle == null)
                {
                    _titleStyle = new GUIStyle();
                    _titleStyle.fontSize = 50;
                    _titleStyle.fontStyle = FontStyle.Bold;
                    _titleStyle.normal.textColor = textColor;
                    _titleStyle.alignment = TextAnchor.UpperCenter;
                }

                return _titleStyle;
            }
        }

        private static GUIStyle _labelStyle;
        public static GUIStyle LabelStyle
        {
            get
            {
                if (_labelStyle == null)
                {
                    _labelStyle = new GUIStyle();
                    _labelStyle.fontStyle = FontStyle.Bold;
                    _labelStyle.normal.textColor = textColor;
                }

                return _labelStyle;
            }
        }
        #endregion
    }
}