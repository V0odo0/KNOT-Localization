using System.Linq;
using Knot.Core.Editor;
using Knot.Localization.Data;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public class KnotDatabaseEditorWindow : EditorWindow, IHasCustomMenu
    {
        public KnotEditorToolbarPanel EditorToolbarPanel { get; private set; }


        void OnEnable()
        {
            EditorApplication.projectChanged += UpdateActiveDatabase;
            EditorUtils.DefaultDatabaseChanged += ReloadLayout;

            ReloadLayout();
        }

        void OnDisable()
        {
            EditorApplication.projectChanged -= UpdateActiveDatabase;
            EditorUtils.DefaultDatabaseChanged -= ReloadLayout;
        }

        void OnFocus()
        {
            UpdateActiveDatabase();
        }

        void OnGUI()
        {
            if (Event.current?.commandName == "Find")
            {
                var searchField = rootVisualElement.Query<ToolbarSearchField>().First();
                if (searchField != null)
                {
                    searchField.Focus();
                    searchField.Q<TextField>()?.GetVisualInput()?.Focus();
                }
            }
        }


        void AddNoDatabaseContainer()
        {
            var noDatabaseContainer = new IMGUIContainer(() =>
            {
                EditorGUILayout.LabelField("No Database selected", EditorStyles.centeredGreyMiniLabel);
                if (GUILayout.Button("Create Database"))
                {
                    var db = Core.Editor.EditorUtils.RequestCreateAsset<KnotDatabase>("KnotDatabase", true);
                    if (db != null)
                    {
                        EditorUtils.ActiveDatabase = db;
                        ReloadLayout();
                    }
                }
            });

            noDatabaseContainer.AddToClassList("margin3px");
            rootVisualElement.Add(noDatabaseContainer);
        }
        
        void UpdateActiveDatabase()
        {
            if (EditorUtils.ActiveDatabase != null)
                return;

            EditorUtils.UpdateDatabaseAssets();
            if (hasFocus)
            {
                ReloadLayout();
                Repaint();
            }
        }

        void SetActiveDatabase(KnotDatabase dataBase)
        {
            if (dataBase == null)
                return;

            EditorUtils.ActiveDatabase = dataBase;
            ReloadLayout();
            EditorGUIUtility.PingObject(dataBase);
        }


        public void ReloadLayout()
        {
            rootVisualElement.Clear();
            rootVisualElement.styleSheets.Add(EditorUtils.EditorStyles);

            if (EditorUtils.ActiveDatabase == null)
            {
                AddNoDatabaseContainer();
                titleContent.text = "Database Editor";
            }
            else
            {
                titleContent.text = EditorUtils.ActiveDatabase.name;

                EditorToolbarPanel = new KnotEditorToolbarPanel();
                rootVisualElement.Add(EditorToolbarPanel);
            }
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            if (!Application.isPlaying)
                menu.AddDisabledItem(new GUIContent("Live Reload (Play Mode only)"));
            else menu.AddItem(new GUIContent("Live Reload"), false, EditorUtils.PerformPlayModeLiveReload);

            menu.AddSeparator(string.Empty);
            
            menu.AddItem(new GUIContent("Database/New..."), false, () =>
                {
                    SetActiveDatabase(Core.Editor.EditorUtils.RequestCreateAsset<KnotDatabase>());
                    EditorUtils.UpdateDatabaseAssets();
                });

            if (EditorUtils.DatabaseAssets.Any())
            {
                menu.AddSeparator("Database/");
                foreach (var db in EditorUtils.DatabaseAssets)
                {
                    menu.AddItem(new GUIContent($"Database/{db.name}{(db == KnotLocalization.ProjectSettings.DefaultDatabase ? " _[Default]" : string.Empty)}"),
                        EditorUtils.ActiveDatabase == db, () => { SetActiveDatabase(db); });
                }
            }

            menu.AddItem(new GUIContent("Project Settings"), false, KnotProjectSettingsEditor.Open);
        }


        [OnOpenAsset]
        static bool OnOpedDatabaseAsset(int instanceId, int line)
        {
            KnotDatabase dataBase = EditorUtility.InstanceIDToObject(instanceId) as KnotDatabase;
            if (dataBase == null)
                return false;

            EditorUtils.OpenDatabaseEditor(dataBase);
            
            return true;
            //
        }

        [MenuItem("Tools/" + KnotLocalization.CorePath + "Database Editor", false, 1000)]
        public static void Open()
        {
            var window = GetWindow<KnotDatabaseEditorWindow>();
            window.minSize = new Vector2(385, 300);
            window.titleContent.text = "Database Editor";
            window.titleContent.image = EditorUtils.CoreIcon;

            if (HasOpenInstances<KnotDatabaseEditorWindow>())
                window.ReloadLayout();
        }
    }
}