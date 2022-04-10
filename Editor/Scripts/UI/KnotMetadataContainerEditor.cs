using System;
using Knot.Localization.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public class KnotMetadataContainerEditor : KnotEditorPanel
    {
        public Action MetadataChanged;

        public bool DrawMetadataList { get; set; } = true;

        public new Foldout Root => base.Root as Foldout;

        public readonly IMGUIContainer MetadataContainer;
        public readonly KnotMetadataReorderableList RuntimeMetadataList;
        public readonly KnotMetadataReorderableList EditorMetadataList;


        public KnotMetadataContainerEditor(Foldout foldout, KnotMetadataInfoAttribute.MetadataScope scope, SerializedProperty metadataContainerProperty = null, Action onMetadataChanged = null) : base(foldout)
        {
            RuntimeMetadataList = new KnotMetadataReorderableList(scope);
            EditorMetadataList = new KnotMetadataReorderableList(scope, true);
            
            foldout.viewDataKey = $"MetadataFoldout.{scope}";

            MetadataContainer = new IMGUIContainer();
            MetadataContainer.AddToClassList("margin3px");
            Root.contentContainer.Add(MetadataContainer);

            MetadataChanged += onMetadataChanged;

            MetadataContainer.onGUIHandler += () =>
            {
                if (Root.value && DrawMetadataList)
                {
                    EditorGUI.BeginChangeCheck();

                    RuntimeMetadataList.DoLayoutList();
                    GUILayout.Space(3);
                    EditorMetadataList.DoLayoutList();

                    if (EditorGUI.EndChangeCheck())
                        MetadataChanged?.Invoke();
                }
            };

            Bind(metadataContainerProperty);
        }


        public void Bind(SerializedProperty metadataContainerProperty)
        {
            if (metadataContainerProperty == null)
                return;

            SerializedProperty runtime = metadataContainerProperty?.FindPropertyRelative("_runtime");
            SerializedProperty editor = metadataContainerProperty?.FindPropertyRelative("_editor");

            RuntimeMetadataList.serializedProperty = runtime;
            EditorMetadataList.serializedProperty = editor;
        }
    }
}