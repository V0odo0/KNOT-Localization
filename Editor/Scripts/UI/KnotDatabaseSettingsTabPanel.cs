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


        protected ReorderableList TextKeyCollectionsList;
        protected ReorderableList AssetKeyCollectionsList;
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


        ReorderableList GetKeyCollectionsList(string name)
        {
            ReorderableList l = new ReorderableList((SerializedObject) null, null);
            
            l.drawHeaderCallback = rect => EditorGUI.LabelField(rect, name);
            l.drawElementCallback = (rect, index, active, focused) =>
            {
                rect.y += 1;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, l.serializedProperty.GetArrayElementAtIndex(index), true);
            };

            return l;
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

            LanguageSelector.BindProperty(settingsProp.FindPropertyRelative("_languageSelector"));

            TextKeyCollectionsProp = databaseObj.FindProperty("_textKeyCollections");
            TextKeyCollectionsList.serializedProperty = TextKeyCollectionsProp;
            TextController.BindProperty(settingsProp.FindPropertyRelative("_textController"));

            AssetKeyCollectionsProp = databaseObj.FindProperty("_assetKeyCollections");
            AssetKeyCollectionsList.serializedProperty = AssetKeyCollectionsProp;
            AssetController.BindProperty(settingsProp.FindPropertyRelative("_assetController"));

            MetadataContainerEditor.Bind(settingsProp.FindPropertyRelative("_metadata"));

        }
    }
}
