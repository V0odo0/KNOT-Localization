using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    [KnotTypeInfo("Settings", 300, "_Popup")]
    public class KnotDatabaseSettingsTabPanel : KnotEditorPanel, IKnotDatabaseEditorTab
    {
        public VisualElement RootVisualElement => Root;

        public readonly PropertyField LanguageSelector;

        public readonly IMGUIContainer TextKeyCollectionsContainer;
        public readonly PropertyField TextController;

        public readonly IMGUIContainer AssetKeyCollectionsContainer;
        public readonly PropertyField AssetController;

        public readonly Foldout MetadataContainerFoldout;
        public readonly KnotMetadataContainerEditor MetadataContainerEditor;


        protected KnotReorderableList TextKeyCollectionsList;
        protected KnotReorderableList AssetKeyCollectionsList;
        protected SerializedProperty TextKeyCollectionsProp;
        protected SerializedProperty AssetKeyCollectionsProp;


        public KnotDatabaseSettingsTabPanel() : base(nameof(KnotDatabaseSettingsTabPanel))
        {
            LanguageSelector = Root.Q<PropertyField>(nameof(LanguageSelector));

            TextKeyCollectionsList = GetKeyCollectionsList("Text Key Collections");
            TextKeyCollectionsContainer = Root.Q<IMGUIContainer>(nameof(TextKeyCollectionsContainer));
            TextKeyCollectionsContainer.onGUIHandler = () =>
            {
                DrawKeyCollectionsList(TextKeyCollectionsProp, TextKeyCollectionsList);
            };
            TextController = Root.Q<PropertyField>(nameof(TextController));

            AssetKeyCollectionsList = GetKeyCollectionsList("Asset Key Collections");
            AssetKeyCollectionsContainer = Root.Q<IMGUIContainer>(nameof(AssetKeyCollectionsContainer));
            AssetKeyCollectionsContainer.onGUIHandler = () =>
            {
                DrawKeyCollectionsList(AssetKeyCollectionsProp, AssetKeyCollectionsList);
            };
            AssetController = Root.Q<PropertyField>(nameof(AssetController));

            MetadataContainerFoldout = Root.Q<Foldout>(nameof(MetadataContainerFoldout));
            MetadataContainerEditor = new KnotMetadataContainerEditor(MetadataContainerFoldout, KnotMetadataInfoAttribute.MetadataScope.Database);
        }


        KnotReorderableList GetKeyCollectionsList(string name)
        {
            var reorderableList = new KnotReorderableList((SerializedObject) null, null);
            
            reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, name);
            reorderableList.drawElementCallback = (rect, index, active, focused) =>
            {
                rect.y += 1;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, reorderableList.serializedProperty.GetArrayElementAtIndex(index), true);
            };

            return reorderableList;
        }

        void DrawKeyCollectionsList(SerializedProperty property, ReorderableList list)
        {
            if (AssetKeyCollectionsList.serializedProperty == null || property == null)
                return;

            property.serializedObject.Update();
            list.DoLayoutList();
            property.serializedObject.ApplyModifiedProperties();
        }


        protected override void OnPanelAdded()
        {
            ReloadLayout();
            Undo.undoRedoPerformed += ReloadLayout;
        }

        protected override void OnPanelRemoved()
        {
            Undo.undoRedoPerformed -= ReloadLayout;
        }


        public override void ReloadLayout()
        {
            Bind(Database);
        }

        public void Bind(KnotDatabase database)
        {
            SerializedObject databaseObj = new SerializedObject(database);
            SerializedProperty settingsProp = databaseObj.FindProperty("_settings");

            LanguageSelector.Unbind();
            LanguageSelector.BindProperty(settingsProp.FindPropertyRelative("_languageSelector"));

            TextKeyCollectionsProp = databaseObj.FindProperty("_textKeyCollections");
            TextKeyCollectionsList.serializedProperty = TextKeyCollectionsProp;

            TextController.Unbind();
            TextController.BindProperty(settingsProp.FindPropertyRelative("_textController"));

            AssetKeyCollectionsProp = databaseObj.FindProperty("_assetKeyCollections");
            AssetKeyCollectionsList.serializedProperty = AssetKeyCollectionsProp;

            AssetController.Unbind();
            AssetController.BindProperty(settingsProp.FindPropertyRelative("_assetController"));

            MetadataContainerEditor.Bind(settingsProp.FindPropertyRelative("_metadata"));
        }
    }
}
