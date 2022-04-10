using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace Knot.Localization.Editor
{
    public class KnotLanguageDataEditorPanel : KnotEditorPanel
    {
        public readonly PropertyField CultureName;
        public readonly PropertyField NativeName;
        public readonly PropertyField SystemLanguage;
        public readonly IMGUIContainer ProvidersListContainer;
        public readonly Foldout MetadataContainerFoldout;

        public readonly KnotCollectionProviderReorderableList ProviderReorderableList;
        public readonly KnotMetadataContainerEditor MetadataContainerEditor;
        

        public KnotLanguageDataEditorPanel() : base(nameof(KnotLanguageDataEditorPanel))
        {
            CultureName = Root.Q<PropertyField>(nameof(CultureName));
            NativeName = Root.Q<PropertyField>(nameof(NativeName));
            SystemLanguage = Root.Q<PropertyField>(nameof(SystemLanguage));
            
            ProviderReorderableList = new KnotCollectionProviderReorderableList();

            ProvidersListContainer = Root.Q<IMGUIContainer>(nameof(ProvidersListContainer));
            ProvidersListContainer.onGUIHandler = () => ProviderReorderableList.DoLayoutList();

            MetadataContainerFoldout = Root.Q<Foldout>(nameof(MetadataContainerFoldout));
            MetadataContainerEditor = new KnotMetadataContainerEditor(MetadataContainerFoldout, KnotMetadataInfoAttribute.MetadataScope.Language);
        }


        public void Bind(KnotLanguageData data)
        {
            if (data == null)
                return;
            
            DatabaseObj.Update();
            var languageDataProp = DatabaseObj.FindProperty("_languages").GetArrayElementAtIndex(Database.Languages.IndexOf(data));

            var cultureNameProp = languageDataProp.FindPropertyRelative("_cultureName");
            var nativeNameProp = languageDataProp.FindPropertyRelative("_nativeName");
            var systemLangProp = languageDataProp.FindPropertyRelative("_systemLanguage");
            var collectionProvidersProp = languageDataProp.FindPropertyRelative("_collectionProviders");
            var metadataProp = languageDataProp.FindPropertyRelative("_metadata");

            CultureName.BindProperty(cultureNameProp);
            NativeName.BindProperty(nativeNameProp);
            SystemLanguage.BindProperty(systemLangProp);
            ProviderReorderableList.serializedProperty = collectionProvidersProp;
            MetadataContainerEditor.Bind(metadataProp);
        }
    }
}

