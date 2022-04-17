using Knot.Localization.Data;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Knot.Localization.Editor
{
    public class KnotCollectionProviderReorderableList : KnotReorderableList
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

            var labelContent = EditorGUIUtility.TrTempContent(property.GetManagedReferenceTypeName());
            EditorGUI.PropertyField(rect, property, labelContent, property.isExpanded);
        }

        float GetElementHeight(int elementIndex)
        {
            var property = serializedProperty.GetArrayElementAtIndex(elementIndex);
            return EditorGUI.GetPropertyHeight(property, property.isExpanded) + EditorGUIUtility.standardVerticalSpacing * 2;
        }

        void AddDropdown(Rect rect, ReorderableList elementsList)
        {
            GenericMenu menu = new GenericMenu();
            var collectionProviderTypes = typeof(IKnotItemCollectionProvider).GetDerivedTypesInfo();
            foreach (var providerType in collectionProviderTypes)
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