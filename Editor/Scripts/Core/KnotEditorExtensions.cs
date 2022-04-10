using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Knot.Localization.Attributes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public static class KnotEditorExtensions
    {
        private static Dictionary<Type, TypeInfo[]> _cachedDerivedTypesInfo = new Dictionary<Type, TypeInfo[]>();


        internal static TypeInfo[] GetDerivedTypesInfo(this Type baseType)
        {
            if (_cachedDerivedTypesInfo.ContainsKey(baseType) && _cachedDerivedTypesInfo[baseType] != null)
                return _cachedDerivedTypesInfo[baseType];

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
            
            _cachedDerivedTypesInfo.Add(baseType, derivedTypes.OrderBy(t => t.Info.Order).ToArray());

            return _cachedDerivedTypesInfo[baseType];
        }

        internal static VisualElement GetVisualInput(this VisualElement e)
        {
            return e?.GetType().GetProperty("visualInput", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(e) as VisualElement;
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
            return type == null
                ? property.displayName
                : (type.GetCustomAttribute<KnotTypeInfoAttribute>()?.DisplayName ??
                   ObjectNames.NicifyVariableName(type.Name));
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