using System.Reflection;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Ru1t3rl.Extensions.Events;
using Ru1t3rl.Events;

namespace Ru1t3rl.Editors.Events
{
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ActionField))]
    public class ActionProperty : PropertyDrawer
    {
        private float uiDivider = 2f;
        private float margin = 2f;
        private float height = EditorGUIUtility.singleLineHeight * 2;


        private Component[] components;
        private string[] componentNames;
        Dictionary<string, int> componentNameIndexes = new Dictionary<string, int>();

        private MethodInfo[] methodNames;
        private ParameterInfo[] parameters;

        private bool showParamters = false;
        private bool hasObject = false;

        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

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
                    objectFieldRect = new Rect(position.x, position.y, position.width / uiDivider - margin, EditorGUIUtility.singleLineHeight);
                    hasObject = true;
                }
                else
                {
                    objectFieldRect = new Rect(position.x, position.y, position.width / 1f - margin, EditorGUIUtility.singleLineHeight);
                    hasObject = false;
                }
            }
            catch (System.NullReferenceException)
            {
                objectFieldRect = new Rect(position.x, position.y, position.width / 1f - margin, EditorGUIUtility.singleLineHeight);
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

                        // Draw the component dropdown
                        selectedComponentIndex.intValue = EditorGUI.Popup(
                            new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + margin, position.width / uiDivider - margin, EditorGUIUtility.singleLineHeight),
                            selectedComponentIndex.intValue,
                            componentNames
                        );

                        // Update the selected class property based on the selected component
                        classProperty.objectReferenceValue = components[selectedComponentIndex.intValue];

                        //  Update the possible methods to invoke
                        UpdateMethodNames(classProperty.objectReferenceValue);
                        selectedMethodIndex.intValue = EditorGUI.Popup(
                            new Rect(position.x + position.width / uiDivider + margin, position.y + EditorGUIUtility.singleLineHeight + margin, position.width / uiDivider - margin, EditorGUIUtility.singleLineHeight),
                            selectedMethodIndex.intValue,
                            methodNames.ToNames()
                        );

                        if (methodNames.Length > 0)
                        {
                            selectedMethodIndex.intValue = selectedMethodIndex.intValue < methodNames.Length ? selectedMethodIndex.intValue : methodNames.Length - 1;
                            methodNameProperty.stringValue = new Regex(@"\(([A-Za-z]([ .,])*)*\)").Replace(methodNames.ToNames()[selectedMethodIndex.intValue], "");
                        }
                    }

                    height += EditorGUIUtility.singleLineHeight + margin;
                }

            }
            catch (System.NullReferenceException) { }
            catch (System.IndexOutOfRangeException)
            {
                UpdateComponents(gameObjectProperty.objectReferenceValue);
                selectedComponentIndex.intValue = componentNames.Length - 1;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return height;
        }

        // Updates the list of components on obj
        private void UpdateComponents(Object @object, bool excludeTransform = false)
        {
            GameObject gobj = @object as GameObject;
            components = gobj.GetComponents<Component>().
                                Where(component => (excludeTransform && component.GetType() != typeof(Transform)) || !excludeTransform).ToArray();

            componentNames = components.ToNames();
            componentNameIndexes.Clear();

            for (int i = 0; i < componentNames.Length; i++)
            {
                if (componentNameIndexes.ContainsKey(componentNames[i]))
                {
                    ++componentNameIndexes[componentNames[i]];
                    componentNames[i] += $"({componentNameIndexes[componentNames[i]]})";
                }
                else
                {
                    componentNameIndexes.Add(componentNames[i], 0);
                }
            }
        }

        // Updates the list of method names based on Object
        private void UpdateMethodNames(Object @object)
        {
            System.Type type = @object.GetType();
            methodNames = type.GetMethods(bindingFlags);
            string[] monoBehaviourMethodNames = typeof(MonoBehaviour).GetMethods(bindingFlags).Select(method => method.Name).ToArray();
            methodNames = methodNames.Where(method => !Contains(monoBehaviourMethodNames, method.Name)).ToArray();
            methodNames = methodNames.Where(method => method.GetParameters().Length == 0).ToArray();
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
#endif
}