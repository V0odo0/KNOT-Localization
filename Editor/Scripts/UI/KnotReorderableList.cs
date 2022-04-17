using System;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

namespace Knot.Localization.Editor
{
    public class KnotReorderableList : ReorderableList
    {
        public KnotReorderableList(IList elements, Type elementType) : base(elements, elementType)
        {
            onReorderCallbackWithDetails = ReorderCallbackWithDetails;
        }

        public KnotReorderableList(IList elements, Type elementType, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) : base(elements, elementType, draggable, displayHeader, displayAddButton, displayRemoveButton)
        {
            onReorderCallbackWithDetails = ReorderCallbackWithDetails;
        }

        public KnotReorderableList(SerializedObject serializedObject, SerializedProperty elements) : base(serializedObject, elements)
        {
            onReorderCallbackWithDetails = ReorderCallbackWithDetails;
        }

        public KnotReorderableList(SerializedObject serializedObject, SerializedProperty elements, bool draggable, bool displayHeader, bool displayAddButton, bool displayRemoveButton) : base(serializedObject, elements, draggable, displayHeader, displayAddButton, displayRemoveButton)
        {
            onReorderCallbackWithDetails = ReorderCallbackWithDetails;
        }


        protected virtual void ReorderCallbackWithDetails(ReorderableList list, int fromIndex, int toIndex)
        {
            if (fromIndex == toIndex)
                return;

            var fromExpanded = list.serializedProperty.GetArrayElementAtIndex(fromIndex).isExpanded;
            var toExpanded = list.serializedProperty.GetArrayElementAtIndex(toIndex).isExpanded;
            serializedProperty.GetArrayElementAtIndex(toIndex).isExpanded = fromExpanded;
            serializedProperty.GetArrayElementAtIndex(fromIndex).isExpanded = toExpanded;

            serializedProperty.serializedObject.UpdateIfRequiredOrScript();
            serializedProperty.MoveArrayElement(fromIndex, toIndex);
            serializedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}