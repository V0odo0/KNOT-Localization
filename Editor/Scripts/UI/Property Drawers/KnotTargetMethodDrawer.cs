using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Knot.Localization.Editor
{
    public abstract class KnotTargetMethodDrawer<TValueType> : PropertyDrawer
    {
        protected GUIContent DropdownButtonContent => _dropdownButtonContent ?? (_dropdownButtonContent = new GUIContent());
        private GUIContent _dropdownButtonContent;


        private static Type[] _excludedTargetTypes = 
        {
            typeof(Transform), typeof(RectTransform), typeof(GameObject)
        };

        private static string[] excludeMethodNames = 
        {
            "GetComponent", "set_tag", "CompareTag", "SendMessageUpwards", "SendMessage", "BroadcastMessage", "set_name",
            "StartCoroutine", "StopCoroutine", "CancelInvoke", "IsInvoking"
        };

        protected string GetMethodDisplayName(string methodName)
        {
            return methodName.StartsWith("set_") ? methodName.Remove(0, 4) : methodName;
        }

        protected bool IsValidTarget(Object obj, string methodName)
        {
            if (obj == null)
                return false;

            var method = obj.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (method == null)
                return false;

            if (method.GetParameters().Length != 1 && method.GetParameters()[0].ParameterType != typeof(TValueType))
                return false;

            return true;
        }

        protected virtual Dictionary<Object, string[]> GetMethodNamesFromObjects(params Object[] objects)
        {
            Dictionary<Object, string[]> targets = new Dictionary<Object, string[]>();
            foreach (var obj in objects)
            {
                var type = obj.GetType();
                if (_excludedTargetTypes.Contains(type))
                    continue;
                
                var methodNames = type.GetMethods(BindingFlags.Public | BindingFlags.Instance).
                    Where(m => m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(TValueType)).
                    Select(m => m.Name).Where(m => !excludeMethodNames.Contains(m)).ToArray();

                if (methodNames.Length > 0)
                    targets.Add(obj, methodNames);
            }

            return targets;
        }

        protected virtual MonoBehaviour GetSource(SerializedProperty property)
            => property?.serializedObject.targetObject as MonoBehaviour;

        protected virtual void ShowTargetMethodMenu(MonoBehaviour source, Action<Object, string> onTargetSelected)
        {
            var components = source.GetComponents<Component>().Cast<Object>().ToArray();
            var behaviours = source.GetComponents<MonoBehaviour>().Where(b => b != source).Cast<Object>().ToArray();
            
            var targets = GetMethodNamesFromObjects(components.Union(behaviours).ToArray());
            
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("None"), false, () => onTargetSelected?.Invoke(null, string.Empty));
            menu.AddSeparator("");

            foreach (var obj in targets)
            {
                foreach (var methodName in obj.Value)
                {
                    string name = $"{obj.Key.GetType().Name}/{GetMethodDisplayName(methodName)}";
                    GUIContent methodContent = new GUIContent(name);
                    menu.AddItem(methodContent, false, () =>
                    {
                        onTargetSelected?.Invoke(obj.Key, methodName);
                    });
                }
            }

            menu.ShowAsContext();
        }


        public override VisualElement CreatePropertyGUI(SerializedProperty property)
            => property.GetFallbackPropertyGUI();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MonoBehaviour source = GetSource(property);
            if (source == null)
            {
                base.OnGUI(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty objectProp = property.FindPropertyRelative("_object");
            SerializedProperty methodNameProp = property.FindPropertyRelative("_methodName");

            bool isValidTarget = IsValidTarget(objectProp.objectReferenceValue, methodNameProp.stringValue);
            bool isPropertiesDifferent =
                objectProp.hasMultipleDifferentValues || methodNameProp.hasMultipleDifferentValues;

            Rect pos = position;
            pos.height = EditorGUIUtility.singleLineHeight;
            pos = EditorGUI.PrefixLabel(pos, label);

            var lastIconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(16, 16));

            string dropdownText = isValidTarget ?
                $"{objectProp.objectReferenceValue.GetType().Name}.{GetMethodDisplayName(methodNameProp.stringValue)}" :
                "None";
            if (isPropertiesDifferent)
                dropdownText = "-";

            DropdownButtonContent.text = dropdownText;
            DropdownButtonContent.image = isValidTarget && !isPropertiesDifferent ? AssetPreview.GetMiniThumbnail(objectProp.objectReferenceValue) : null;

            if (EditorGUI.DropdownButton(pos, DropdownButtonContent, FocusType.Keyboard))
            {
                ShowTargetMethodMenu(source, (o, s) =>
                {
                    objectProp.objectReferenceValue = o;
                    methodNameProp.stringValue = s;
                    property.serializedObject.ApplyModifiedProperties();

                    if (Application.IsPlaying(property.serializedObject.targetObject))
                        if (property.serializedObject.targetObject is MonoBehaviour go)
                            go.SendMessage("ForceUpdateValue", SendMessageOptions.DontRequireReceiver);
                });
            }

            EditorGUIUtility.SetIconSize(lastIconSize);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (GetSource(property) == null)
                return base.GetPropertyHeight(property, label);

            return 15;
        }
    }
}