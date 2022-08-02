using UnityEngine;
using UnityEditor;
using Ru1t3rl.Editors.Utilities;

namespace Ru1t3rl.Editors
{
    public abstract class BaseCustomEditor : Editor
    {
        protected Color backupColor;

        public override void OnInspectorGUI()
        {
            DrawHeader(
                EditorGUIUtility.ObjectContent(target, target.GetType()).image,
                target.GetType().Name
            );

            using (EditorGUILayout.VerticalScope vScope = new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (EditorGUI.ChangeCheckScope checkScope = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                    DrawGeneral();
                    EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
                    DrawAdvanced();
                    EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing);

                    if (checkScope.changed)
                        serializedObject.ApplyModifiedProperties();
                }
            }
        }

        /// <summary>Draw the title and icon to the inspector</summary>
        protected virtual void DrawHeader(Texture icon, string title)
        {
            GUILayout.Space(InspectorUtilities.Spacing);
            using (GUILayout.HorizontalScope horizontalScope = new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                // Draw Icon
                backupColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(1, 1, 1, 0);
                GUILayout.Box(icon, GUILayout.Width(50), GUILayout.Height(50));
                GUI.backgroundColor = backupColor;

                // Draw Title
                GUILayout.Label(title, InspectorUtilities.TitleStyle);

                GUILayout.FlexibleSpace();
            }
            GUILayout.Space(InspectorUtilities.Spacing);
        }

        /// <summary>Get the general properties and draw them in the inspector</summary>
        protected abstract void DrawGeneral();

        /// <summary>Get the events for 'onActive' and 'onDeactivate' and draw them to the inspector</summary>
        protected abstract void DrawAdvanced();
    }
}