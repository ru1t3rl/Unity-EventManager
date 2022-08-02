using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Ru1t3rl.Extensions.Events;
using Ru1t3rl.Events;


namespace Ru1t3rl.Editors.Events
{
    // [CustomPropertyDrawer(typeof(ActionField<System.EventArgs>))]
    public class ActionProperty<EventArgs> : PropertyDrawer
    {
        private const float UI_DIVIDER = 2f;
        private const float MARGIN = 2f;
        private float height = EditorGUIUtility.singleLineHeight * 2;

        private Component[] components;
        private MethodInfo[] methodNames;
        private ParameterInfo[] parameters;

        private bool showParamters = false;
        private bool hasObject = false;

        private BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            #region Get Properties
            SerializedProperty objectProperty = property.FindPropertyRelative("obj");
            SerializedProperty classProperty = property.FindPropertyRelative("component");
            SerializedProperty methodNameProperty = property.FindPropertyRelative("methodName");
            SerializedProperty gameObjectProperty = property.FindPropertyRelative("gameObject");
            SerializedProperty selectedMethodIndex = property.FindPropertyRelative("selectedMethodIndex");
            SerializedProperty selectedComponentIndex = property.FindPropertyRelative("selectedComponentIndex");

            SerializedProperty parametersProperty = property.FindPropertyRelative("parameters");
            #endregion

            // Set the rect base rect width and height for the objectfield
            Rect objectFieldRect = Rect.zero;
            try
            {
                if (objectProperty.objectReferenceValue != null &&
                    objectProperty.objectReferenceValue.GetType() == typeof(GameObject) ||
                    objectProperty.objectReferenceValue.GetType().IsSubclassOf(typeof(MonoBehaviour)))
                {
                    objectFieldRect = new Rect(position.x, position.y, position.width / UI_DIVIDER - MARGIN, EditorGUIUtility.singleLineHeight);
                    hasObject = true;
                }
                else
                {
                    objectFieldRect = new Rect(position.x, position.y, position.width / 1f - MARGIN, EditorGUIUtility.singleLineHeight);
                    hasObject = false;
                }
            }
            catch (System.NullReferenceException)
            {
                objectFieldRect = new Rect(position.x, position.y, position.width / 1f - MARGIN, EditorGUIUtility.singleLineHeight);
                hasObject = false;
            }

            DrawObjectField(objectProperty, gameObjectProperty, classProperty, selectedComponentIndex, objectFieldRect);

            height = EditorGUIUtility.singleLineHeight;

            DrawDropDowns(objectProperty, gameObjectProperty, classProperty, methodNameProperty, selectedComponentIndex, selectedMethodIndex, position);

            property.serializedObject.ApplyModifiedProperties();
        }

        // Draws the object field for selecting a component or gameobject
        public void DrawObjectField(SerializedProperty objectProperty, SerializedProperty gameObjectProperty,
                                   SerializedProperty classProperty, SerializedProperty selectedComponentIndex,
                                   Rect objectFieldRect)
        {
            // Object field to select a script of GameObject
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(objectFieldRect, objectProperty, GUIContent.none);
            if (EditorGUI.EndChangeCheck() && objectProperty.objectReferenceValue != null)
            {
                if (objectProperty.objectReferenceValue.GetType() == typeof(GameObject))
                {
                    gameObjectProperty.objectReferenceValue = objectProperty.objectReferenceValue;
                    UpdateComponents(gameObjectProperty.objectReferenceValue);

                    // Assign the class field
                    classProperty.objectReferenceValue = components[selectedComponentIndex.intValue];
                }
                else if (objectProperty.objectReferenceValue.GetType().IsSubclassOf(typeof(MonoBehaviour)))
                {
                    classProperty.objectReferenceValue = objectProperty.objectReferenceValue;
                    UpdateMethodNames(classProperty.objectReferenceValue);

                    // Assign the gameObject field
                    gameObjectProperty.objectReferenceValue = (classProperty.objectReferenceValue as MonoBehaviour).gameObject;
                    objectProperty.objectReferenceValue = gameObjectProperty.objectReferenceValue;

                    UpdateComponents(gameObjectProperty.objectReferenceValue);
                    selectedComponentIndex.intValue = System.Array.IndexOf(components, classProperty.objectReferenceValue as Component);
                }
            }
        }

        // Draws the drop doowns for Components & Methods
        public void DrawDropDowns(SerializedProperty objectProperty, SerializedProperty gameObjectProperty,
                                  SerializedProperty classProperty, SerializedProperty methodNameProperty,
                                  SerializedProperty selectedComponentIndex, SerializedProperty selectedMethodIndex,
                                  Rect position)
        {
            try
            {
                if (objectProperty.objectReferenceValue != null &&
                    objectProperty.objectReferenceValue.GetType() == typeof(GameObject) || objectProperty.objectReferenceValue.GetType().IsSubclassOf(typeof(MonoBehaviour)))
                {
                    if (gameObjectProperty.objectReferenceValue != null)
                    {
                        // Update the list of possible components/class to select
                        UpdateComponents(gameObjectProperty.objectReferenceValue);

                        selectedComponentIndex.intValue = EditorGUI.Popup(
                            new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + MARGIN, position.width / UI_DIVIDER - MARGIN, EditorGUIUtility.singleLineHeight),
                            selectedComponentIndex.intValue,
                            components.ToNames()
                        );

                        // Update the selected class property based on the selected component
                        classProperty.objectReferenceValue = components[selectedComponentIndex.intValue];

                        //  Update the possible methods to invoke
                        UpdateMethodNames(classProperty.objectReferenceValue);
                        selectedMethodIndex.intValue = EditorGUI.Popup(
                            new Rect(position.x + position.width / UI_DIVIDER + MARGIN, position.y + EditorGUIUtility.singleLineHeight + MARGIN, position.width / UI_DIVIDER - MARGIN, EditorGUIUtility.singleLineHeight),
                            selectedMethodIndex.intValue,
                            methodNames.ToNames()
                        );

                        if (methodNames.Length > 0)
                        {
                            selectedMethodIndex.intValue = selectedMethodIndex.intValue < methodNames.Length ? selectedMethodIndex.intValue : methodNames.Length - 1;
                            methodNameProperty.stringValue = new Regex(@"\(([A-Za-z]([ .,])*)*\)").Replace(methodNames.ToNames()[selectedMethodIndex.intValue], "");
                        }
                    }

                    height += EditorGUIUtility.singleLineHeight + MARGIN;
                }

            }
            catch (System.NullReferenceException) { }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return height;
        }

        private void UpdateComponents(UnityEngine.Object obj, bool excludeTransform = true)
        {
            GameObject gobj = obj as GameObject;
            components = gobj.GetComponents<Component>().
                                Where(component => (excludeTransform && component.GetType() != typeof(Transform)) || !excludeTransform).ToArray();
        }

        private void UpdateMethodNames(UnityEngine.Object obj)
        {
            System.Type type = obj.GetType();
            methodNames = type.GetMethods(bindingFlags);
            string[] monoBehaviourMethodNames = typeof(MonoBehaviour).GetMethods(bindingFlags).Select(method => method.Name).ToArray();
            methodNames = methodNames.Where(method => !Contains(monoBehaviourMethodNames, method.Name)).ToArray();
            methodNames = methodNames.Where(method => method.GetParameters().Where(parameter => parameter.ParameterType == typeof(EventArgs)).ToArray().Length == 0).ToArray();
        }

        private bool Contains(string[] namesList, string name)
        {
            for (int iName = 0; iName < namesList.Length; iName++)
            {
                if (namesList[iName] == name)
                {
                    return true;
                }
            }
            return false;
        }
    }
}