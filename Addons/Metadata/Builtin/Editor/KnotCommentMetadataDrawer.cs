using Knot.Localization.Data;
using UnityEditor;
using UnityEngine;

namespace Knot.Localization.Editor
{
    [CustomPropertyDrawer(typeof(KnotCommentMetadata))]
    public class KnotCommentMetadataDrawer : PropertyDrawer
    {
        private static GUIContent _label { get; } = new GUIContent("Comment");


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height -= EditorGUIUtility.standardVerticalSpacing * 2;

            EditorGUI.PropertyField(position, property.FindPropertyRelative("_comment"), _label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("_comment"));
        }
    }
}
