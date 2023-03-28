using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using Knot.Localization.Data;
using Knot.Localization.Editor;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Knot.Localization.Tests.Editor
{
    public class KnotEditorTests
    {
        protected KnotDatabase Database;
        protected SerializedObject DatabaseObj;

        protected KnotKeyCollection TextKeyCollection;
        protected KnotKeyCollection AssetKeyCollection;
        protected KnotTextCollection TextCollection;
        protected KnotAssetCollection AssetCollection;


        protected void SaveTempAsset(Object asset)
        {
            if (EditorUtility.IsPersistent(asset))
                return;

            string assetName = asset.GetType().Name;

            AssetDatabase.CreateAsset(asset, $"Assets/{assetName}_test_{asset.GetInstanceID()}.asset");
            AssetDatabase.SaveAssets();
        }

        protected void DeleteTempAsset(Object asset)
        {
            if (EditorUtility.IsPersistent(asset))
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(asset));
        }

        
        [OneTimeSetUp]
        public void SetUp()
        {
            Database = ScriptableObject.CreateInstance<KnotDatabase>();
            DatabaseObj = new SerializedObject(Database);

            TextKeyCollection = ScriptableObject.CreateInstance<KnotKeyCollection>();
            AssetKeyCollection = ScriptableObject.CreateInstance<KnotKeyCollection>();

            Database.TextKeyCollections.Add(TextKeyCollection);
            Database.AssetKeyCollections.Add(AssetKeyCollection);

            TextCollection = ScriptableObject.CreateInstance<KnotTextCollection>();
            AssetCollection = ScriptableObject.CreateInstance<KnotAssetCollection>();
            
            Database.Languages.Add(new KnotLanguageData(SystemLanguage.English));
            Database.Languages.First().CollectionProviders.Add(new KnotAssetCollectionProvider(TextCollection));
            Database.Languages.First().CollectionProviders.Add(new KnotAssetCollectionProvider(AssetCollection));

            SaveTempAsset(Database);
            SaveTempAsset(TextKeyCollection);
            SaveTempAsset(TextCollection);
        }


        [Test, Order(900)]
        public void EditorResourcesExist()
        {
            Assert.IsTrue(KnotEditorUtils.EditorPanels.Any());
            Assert.AreNotEqual(KnotEditorUtils.EditorStyles, null);
        }

        [Test, Order(1000)]
        public void IsTestAssetsPersistent()
        {
            Assert.IsTrue(EditorUtility.IsPersistent(Database));
            Assert.IsTrue(EditorUtility.IsPersistent(TextKeyCollection));
            Assert.IsTrue(EditorUtility.IsPersistent(TextCollection));
        }

        [Test]
        public void ValidateManagedReferenceType()
        {
            Database.Settings.LanguageSelector = new KnotDefaultLanguageSelector();
            DatabaseObj.Update();

            Assert.IsTrue(DatabaseObj.FindProperty("_settings").FindPropertyRelative("_languageSelector").GetManagedReferenceType() == typeof(KnotDefaultLanguageSelector));
        }

        [Test]
        public void MetadataContainerHasInstanceOfType()
        {
            var keyData = new KnotKeyData("testKey");
            keyData.Metadata.Runtime.Add(new KnotCommentMetadata("TestComment"));

            TextKeyCollection.Add(keyData);

            var textKeyCollectionObj = new SerializedObject(TextKeyCollection);
            textKeyCollectionObj.Update();

            int index = TextKeyCollection.IndexOf(keyData);

            var runtimeMetadataProp = textKeyCollectionObj.FindProperty("_keyData").GetArrayElementAtIndex(index)
                .FindPropertyRelative("_metadata").FindPropertyRelative("_runtime");

            Assert.IsTrue(runtimeMetadataProp.HasInstanceOfTypeInArray(typeof(KnotCommentMetadata)));

            TextKeyCollection.Remove(keyData);
        }

        [Test]
        public void CultureToSystemLanguage()
        {
            Assert.IsTrue(KnotLanguageData.GetSystemLanguageFromCulture(CultureInfo.GetCultureInfo("en-US")) == SystemLanguage.English);
            Assert.IsTrue(KnotLanguageData.GetSystemLanguageFromCulture(CultureInfo.GetCultureInfo("de")) == SystemLanguage.German);
            Assert.IsTrue(KnotLanguageData.GetSystemLanguageFromCulture(CultureInfo.GetCultureInfo("hi")) == SystemLanguage.Unknown);
        }

        [Test]
        public void SystemLanguageToCulture()
        {
            Assert.IsTrue(KnotLanguageData.GetCultureFromSystemLanguage(SystemLanguage.English).Name == "en");
            Assert.IsTrue(KnotLanguageData.GetCultureFromSystemLanguage(SystemLanguage.German).Name == "de");
            Assert.IsTrue(KnotLanguageData.GetCultureFromSystemLanguage(SystemLanguage.Unknown).Name == CultureInfo.InvariantCulture.Name);
        }

        [Test]
        public void ValidateAssetTypeRestrictionType()
        {
            KnotAssetTypeRestrictionMetadata metadata = new KnotAssetTypeRestrictionMetadata(typeof(int));
            Assert.IsTrue(metadata.AssetType == typeof(Object));

            metadata.AssetType = typeof(ScriptableObject);
            Assert.IsTrue(metadata.AssetType == typeof(ScriptableObject));

            metadata.AssetType = typeof(KnotDatabase);
            Assert.IsTrue(metadata.AssetType == typeof(KnotDatabase));
        }

        [UnityTest]
        public IEnumerator CreateManagerAndLoadLanguage()
        {
            KnotManager m = new KnotManager();
            m.SetDatabase(Database, true);

            while (m.State != KnotManagerState.LanguageLoaded)
                yield return null;

            Assert.Pass();

            m.Dispose();
        }

        [UnityTest]
        public IEnumerator CreateManagerAndSwitchLanguage()
        {
            KnotTextData enItemData = new KnotTextData("TestKey", SystemLanguage.English.ToString());
            KnotTextData afItemData = new KnotTextData("TestKey", SystemLanguage.Afrikaans.ToString());

            TextCollection.Add(enItemData);

            KnotTextCollection afTextCollection = ScriptableObject.CreateInstance<KnotTextCollection>();
            afTextCollection.Add(afItemData);

            KnotLanguageData afLanguageData = new KnotLanguageData(SystemLanguage.Afrikaans);
            afLanguageData.CollectionProviders.Add(new KnotAssetCollectionProvider(afTextCollection));

            Database.Languages.Add(afLanguageData);

            KnotManager m = new KnotManager();
            m.SetDatabase(Database);
            m.LoadLanguage(m.Languages.First(d => d.SystemLanguage == SystemLanguage.English));

            while (m.State != KnotManagerState.LanguageLoaded)
                yield return null;

            Assert.IsTrue(m.SelectedLanguage.SystemLanguage == SystemLanguage.English);
            Assert.IsTrue(m.GetTextValue(enItemData.Key).Value == enItemData.RawText);

            m.LoadLanguage(m.Languages.First(d => d.SystemLanguage == SystemLanguage.Afrikaans));

            while (m.State != KnotManagerState.LanguageLoaded)
                yield return null;

            Assert.IsTrue(m.SelectedLanguage.SystemLanguage == SystemLanguage.Afrikaans);
            Assert.IsTrue(m.GetTextValue(afItemData.Key).Value == afItemData.RawText);

            TextCollection.Remove(enItemData);
            Database.Languages.Remove(afLanguageData);
            Object.DestroyImmediate(afTextCollection);
            m.Dispose();
        }

        [UnityTest]
        public IEnumerator CreateManagerAndSwitchLanguageWithOverrideValue()
        {
            KnotManager m = new KnotManager();
            m.SetDatabase(Database);

            var afLanguageData = new KnotLanguageData(SystemLanguage.Afrikaans);

            m.Languages.Add(afLanguageData);
            m.LoadLanguage(m.Languages.First(d => d.SystemLanguage == SystemLanguage.English));

            while (m.State != KnotManagerState.LanguageLoaded)
                yield return null;

            string overrideKey = "OverrideKey";
            string overrideValue = "OverrideValue";

            m.TextController.AddOverride(overrideKey, overrideValue);

            Assert.IsTrue(m.GetTextValue(overrideKey).Value == overrideValue);

            m.LoadLanguage(afLanguageData);

            while (m.State != KnotManagerState.LanguageLoaded)
                yield return null;

            Assert.IsTrue(m.GetTextValue(overrideKey).Value == overrideValue);
        }

        [UnityTest]
        public IEnumerator CreateManagerAndGetValue()
        {
            KnotTextData textItemData = new KnotTextData("TestKey", "TestValue");
            KnotAssetData assetItemData = new KnotAssetData("TestKey", Database);
            
            TextCollection.Add(textItemData);
            AssetCollection.Add(assetItemData);

            KnotManager m = new KnotManager();
            m.SetDatabase(Database, true);

            while (m.State != KnotManagerState.LanguageLoaded)
                yield return null;
            
            Assert.IsTrue(m.GetTextValue(textItemData.Key).Value == textItemData.RawText);
            Assert.IsTrue(m.GetAssetValue(assetItemData.Key).Value == Database);

            TextCollection.Remove(textItemData);
            AssetCollection.Remove(assetItemData);

            m.Dispose();
        }
        
        [UnityTest]
        public IEnumerator CreateManagerAndGetOverrideValue()
        {
            string overrideKey = "OverrideKey";
            string overrideValue = "OverrideValue";

            KnotManager m = new KnotManager();
            m.SetDatabase(Database);
            m.LoadLanguage(m.Languages.FirstOrDefault());

            while (m.State != KnotManagerState.LanguageLoaded)
                yield return null;

            m.TextController.AddOverride(overrideKey, overrideValue);

            Assert.IsTrue(m.State == KnotManagerState.LanguageLoaded && m.GetTextValue(overrideKey).Value == overrideValue);

            m.Dispose();
        }

        [Test]
        public void ValidateCultureSpecificTextMetadata()
        {
            KnotTextController controller = new KnotTextController();

            var culture = CultureInfo.GetCultureInfo("en");
            var keyData = new KnotKeyData("Test", new TextFormatterCultureSpecificMetadataTest());
            var textData = new KnotTextData("Test", "Test");

            controller.BuildAsync(new KnotControllerBuildData<KnotTextData>
            {
                Culture = culture,
                KeyData = new[] { keyData },
                ItemData = new[] { textData }
            }).GetAwaiter().GetResult();

            Assert.AreEqual(controller["Test"].Value, $"{culture.Name}{textData.RawText}");

            controller.Dispose();
        }


        [OneTimeTearDown]
        public void TearDown()
        {
            DeleteTempAsset(Database);
            DeleteTempAsset(TextKeyCollection);
            DeleteTempAsset(TextCollection);
        }


        internal class TextFormatterKeySharedMetadataTest : IKnotKeySharedMetadata, IKnotTextFormatterMetadata
        {
            public void Format(StringBuilder sb)
            {
                sb.Insert(0, "(shared)");
            }

            public object Clone() => new TextFormatterKeySharedMetadataTest();
        }

        internal class TextFormatterCultureSpecificMetadataTest : IKnotCultureSpecificMetadata, IKnotTextFormatterMetadata
        {
            private CultureInfo _culture;

            public string Format(string inputString) => $"{_culture?.Name}{inputString}";

            public void Format(StringBuilder sb)
            {
                sb.Insert(0, _culture?.Name);
            }

            public void SetCulture(CultureInfo cultureInfo) => _culture = cultureInfo;

            public object Clone() => new TextFormatterKeySharedMetadataTest();
        }
    }
}