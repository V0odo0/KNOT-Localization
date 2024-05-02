#if KNOT_LOCALIZATION_EXPERIMENTAL
using Knot.Localization.Data;
using UnityEditor;
using UnityEngine;

namespace Knot.Localization.Editor
{
    [CustomPropertyDrawer(typeof(KnotPluralFormParserMetadata))]
    public class KnotPluralFormatMetadataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            position.height += EditorGUIUtility.standardVerticalSpacing * 2;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3;
        }
    }
}
#endif