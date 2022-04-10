using Knot.Localization.Data;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Knot.Localization.Editor
{
    public class KnotCollectionProviderReorderableList : ReorderableList
    {
        public KnotCollectionProviderReorderableList(SerializedProperty collectionProperty = null) : base(collectionProperty?.serializedObject, collectionProperty)
        {
            drawHeaderCallback = DrawHeader;
            drawElementCallback = DrawElement;
            elementHeightCallback = GetElementHeight;
            onAddDropdownCallback = AddDropdown;
        }


        void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Item Collection Providers");
        }

        void DrawElement(Rect rect, int elementIndex, bool isactive, bool isfocused)
        {
            rect.y += EditorGUIUtility.standardVerticalSpacing;
            var property = serializedProperty.GetArrayElementAtIndex(elementIndex);
            
            EditorGUI.PropertyField(rect, property, new GUIContent(property.GetManagedReferenceTypeName()), property.isExpanded);
        }

        float GetElementHeight(int elementIndex)
        {
            var property = serializedProperty.GetArrayElementAtIndex(elementIndex);
            return EditorGUI.GetPropertyHeight(property, property.isExpanded) + EditorGUIUtility.standardVerticalSpacing * 2;
        }

        void AddDropdown(Rect rect, ReorderableList elementsList)
        {
            GenericMenu menu = new GenericMenu();
            foreach (var providerType in typeof(IKnotItemCollectionProvider).GetDerivedTypesInfo())
            {
                menu.AddItem(providerType.Content, false, () =>
                {
                    var instance = providerType.GetInstance();
                    if (instance == null)
                        return;

                    serializedProperty.InsertArrayElementAtIndex(serializedProperty.arraySize);
                    serializedProperty.GetArrayElementAtIndex(serializedProperty.arraySize - 1).managedReferenceValue =
                        instance;

                    index = serializedProperty.arraySize - 1;
                    serializedProperty.serializedObject.ApplyModifiedProperties();
                });
            }

            menu.DropDown(rect);
        }


        public new void DoLayoutList()
        {
            if (serializedProperty == null)
                return;

            serializedProperty.serializedObject.UpdateIfRequiredOrScript();
            base.DoLayoutList();
            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}