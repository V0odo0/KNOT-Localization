using System;
using Knot.Localization.Data;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Knot.Localization.Editor
{
    [CustomPropertyDrawer(typeof(KnotResourceCollectionProvider))]
    public class KnotResourceCollectionProviderDrawer : PropertyDrawer
    {
        const string RESOURCES_SUB_PATH = "/Resources/";

        protected GUIContent Label => _label ?? (_label = new GUIContent("Resource"));
        private GUIContent _label;


        protected virtual void SetResourcePath(Object obj, SerializedProperty pathProp)
        {
            if (obj == null)
                pathProp.stringValue = "";
            else
            {
                string path = AssetDatabase.GetAssetPath(obj);
                string resourcePath = "";

                if (path.Contains(RESOURCES_SUB_PATH))
                {
                    resourcePath = path.Substring(path.LastIndexOf(RESOURCES_SUB_PATH, StringComparison.Ordinal) + RESOURCES_SUB_PATH.Length);
                    resourcePath = resourcePath.Remove(resourcePath.IndexOf('.'));
                }

                pathProp.stringValue = resourcePath;
            }

            pathProp.serializedObject.ApplyModifiedProperties();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty pathProp = property.FindPropertyRelative("Path");

            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginChangeCheck();
            var asset = Resources.Load<KnotItemCollection>(pathProp.stringValue);
            asset = (KnotItemCollection) EditorGUI.ObjectField(position, Label, asset, typeof(KnotItemCollection), false);
            if (EditorGUI.EndChangeCheck())
                SetResourcePath(asset, pathProp);
            
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

            if (asset == null)
            {
                EditorGUI.HelpBox(position, $"Asset should be placed under Resources folder", MessageType.None);
                position.y += EditorGUIUtility.singleLineHeight;
            }

            int lastIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel += 1;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("AsyncLoad"));
            
            EditorGUI.indentLevel = lastIndent;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool isResourceExist = Resources.Load<KnotItemCollection>(property.FindPropertyRelative("Path").stringValue) != null;

            return base.GetPropertyHeight(property, label)
                   + (EditorGUIUtility.singleLineHeight)
                   + (EditorGUIUtility.standardVerticalSpacing * 2) + (isResourceExist ? 0 : EditorGUIUtility.singleLineHeight);
        }
    }
}