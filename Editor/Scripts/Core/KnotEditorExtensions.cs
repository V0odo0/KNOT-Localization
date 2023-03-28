using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Knot.Localization.Attributes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public static class KnotEditorExtensions
    {
        private static readonly Dictionary<Type, string> _managedReferenceTypeNamesCache = new Dictionary<Type, string>();
        private static readonly Dictionary<Type, TypeInfo[]> _derivedTypesInfoCache = new Dictionary<Type, TypeInfo[]>();


        internal static TypeInfo[] GetDerivedTypesInfo(this Type baseType)
        {
            if (_derivedTypesInfoCache.ContainsKey(baseType) && _derivedTypesInfoCache[baseType] != null)
                return _derivedTypesInfoCache[baseType];

            bool IsValidType(Type t)
            {
                //Non abstract
                return !t.IsAbstract &&
                       //Non generic 
                       !t.IsGenericType &&
                       //Not interface
                       !t.IsInterface &&
                       //Not derived from Unity Object
                       !t.IsSubclassOf(typeof(UnityEngine.Object)) &&
                       //Has default constructor
                       t.GetConstructors().Any(c => c.GetParameters().Length == 0);
            }

            List<TypeInfo> derivedTypes = new List<TypeInfo>();

            foreach (var type in TypeCache.GetTypesDerivedFrom(baseType).Where(IsValidType))
                derivedTypes.Add(new TypeInfo(type));
            
            _derivedTypesInfoCache.Add(baseType, derivedTypes.OrderBy(t => t.Info.Order).ToArray());

            return _derivedTypesInfoCache[baseType];
        }

        internal static SerializedProperty FindParentProperty(this SerializedProperty serializedProperty)
        {
            var propertyPaths = serializedProperty.propertyPath.Split('.');
            if (propertyPaths.Length <= 1)
                return default;

            var parentSerializedProperty = serializedProperty.serializedObject.FindProperty(propertyPaths.First());
            for (var index = 1; index < propertyPaths.Length - 1; index++)
            {
                if (propertyPaths[index] == "Array")
                {
                    if (index + 1 == propertyPaths.Length - 1)
                        break;

                    if (propertyPaths.Length > index + 1 && Regex.IsMatch(propertyPaths[index + 1], "^data\\[\\d+\\]$"))
                    {
                        var match = Regex.Match(propertyPaths[index + 1], "^data\\[(\\d+)\\]$");
                        var arrayIndex = int.Parse(match.Groups[1].Value);
                        parentSerializedProperty = parentSerializedProperty.GetArrayElementAtIndex(arrayIndex);
                        index++;
                    }
                }
                else parentSerializedProperty = parentSerializedProperty.FindPropertyRelative(propertyPaths[index]);
            }

            return parentSerializedProperty;
        }

        internal static VisualElement GetVisualInput(this VisualElement e)
        {
            return e?.GetType().GetProperty("visualInput", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(e) as VisualElement;
        }

        internal static void SetMargins(this IStyle style, StyleLength m)
        {
            if (style == null)
                return;

            style.marginTop = m;
            style.marginBottom = m;
            style.marginLeft = m;
            style.marginRight = m;
        }

        internal static void SetIcon(this ToolbarToggle t, Texture icon)
        {
            VisualElement checkmark;
            if ((checkmark = t?.Q<VisualElement>("unity-checkmark")) == null)
                return;

            checkmark.style.backgroundImage = icon as Texture2D;
            checkmark.pickingMode = PickingMode.Ignore;
            checkmark.style.minWidth = checkmark.style.minHeight = 16;
            checkmark.SendToBack();
        }

        internal static bool ContainsCaseInsensitive(this string source, string other)
        {
            return source?.IndexOf(other, StringComparison.OrdinalIgnoreCase) >= 0;
        }
        
        internal static bool HasInstanceOfTypeInArray(this SerializedProperty property, Type type)
        {
            if (property == null || type == null || !property.isArray || !property.arrayElementType.StartsWith("managed"))
                return false;

            for (int i = 0; i < property.arraySize; i++)
            {
                if (property.GetArrayElementAtIndex(i).managedReferenceFullTypename.Split(' ').LastOrDefault() ==
                    type.FullName)
                    return true;
            }

            return false;
        }
        
        internal static Type GetManagedReferenceType(this SerializedProperty property)
        {
            var parts = property.managedReferenceFullTypename.Split(' ');
            if (parts.Length == 2)
            {
                var assemblyPart = parts[0];
                var nsClassnamePart = parts[1];
                return Type.GetType($"{nsClassnamePart}, {assemblyPart}");
            }

            return null;
        }
        
        internal static string GetManagedReferenceTypeName(this SerializedProperty property)
        {
            var type = property.GetManagedReferenceType();
            if (type == null)
                return property.displayName;

            if (_managedReferenceTypeNamesCache.ContainsKey(type))
                return _managedReferenceTypeNamesCache[type];

            var name = type.GetCustomAttribute<KnotTypeInfoAttribute>()?.DisplayName
                       ?? ObjectNames.NicifyVariableName(type.Name);
            _managedReferenceTypeNamesCache.Add(type, name);

            return name;
        }

        internal static float GetChildPropertiesHeight(this SerializedProperty property,
            params string[] exceptPropertyPaths)
        {
            float h = 0;
            foreach (SerializedProperty childProperty in property)
            {
                if (exceptPropertyPaths.Contains(childProperty.propertyPath))
                    continue;

                h += EditorGUI.GetPropertyHeight(childProperty, childProperty.isExpanded) + EditorGUIUtility.standardVerticalSpacing;
            }

            return h;
        }

        internal static void DrawChildProperties(this SerializedProperty property, Rect position, 
            params string[] exceptPropertyPaths)
        {
            if (!property.hasChildren)
                return;

            EditorGUI.BeginChangeCheck();
            foreach (SerializedProperty childProperty in property)
            {
                if (exceptPropertyPaths.Contains(childProperty.propertyPath))
                    continue;

                EditorGUI.PropertyField(position, childProperty, true);
                position.y += EditorGUI.GetPropertyHeight(childProperty, childProperty.isExpanded) + EditorGUIUtility.standardVerticalSpacing;
            }

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
        }

        internal static VisualElement GetFallbackPropertyGUI(this SerializedProperty property)
        {
            IMGUIContainer container = new IMGUIContainer(() => { EditorGUILayout.PropertyField(property); });
            container.style.marginLeft = container.style.marginRight = 3;
            container.style.marginTop = container.style.marginBottom = 1;

            return container;
        }

        internal static string GetDisplayName(this CultureInfo cultureInfo) => $"{cultureInfo.DisplayName} [{cultureInfo.Name}]";

        internal static bool IsPersistent(this UnityEngine.Object obj) => EditorUtility.IsPersistent(obj);


        public class TypeInfo
        {
            public readonly Type Type;
            public readonly KnotTypeInfoAttribute Info;
            public readonly GUIContent Content;


            public TypeInfo(Type type)
            {
                Type = type;
                Info = type.GetCustomAttribute<KnotTypeInfoAttribute>() ??
                       new KnotTypeInfoAttribute(ObjectNames.NicifyVariableName(type.Name));

                Content = new GUIContent(Info.DisplayName, KnotEditorUtils.GetIcon(Info.IconName));
            }

            public object GetInstance() => Activator.CreateInstance(Type);
        }
    }
}