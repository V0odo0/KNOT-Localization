using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Knot.Localization.Editor
{
    public class KnotAssetTypePickerPopup : KnotItemPickerPopup<Type>
    {
        internal static readonly string[] ExcludedAssemblyNames =
        {
            "UnityEditor",
            "UnityEditorInternal",
            "SyntaxTree.VisualStudio.Unity.Bridge"
        };

        internal static HashSet<string> EditorAssemblyNames
        {
            get
            {
                return _editorAssemblyNames ?? 
                       (_editorAssemblyNames = new HashSet<string>(CompilationPipeline.GetAssemblies()
                           .Where(a => a.flags.HasFlag(AssemblyFlags.EditorAssembly) && !a.flags.HasFlag(AssemblyFlags.None))
                           .SelectMany(a => a.assemblyReferences).Select(a => a.name).Union(ExcludedAssemblyNames)));
            }
        }
        private static HashSet<string> _editorAssemblyNames;

        public static HashSet<Type> AllAssetTypes => _allAssetTypes ?? (_allAssetTypes = GetAllAssetTypes());
        private static HashSet<Type> _allAssetTypes;


        protected KnotAssetTypePickerPopup(List<PickerTreeViewItem> items, TreeViewState state = null) : base(items, state)
        {
            
        }


        static HashSet<Type> GetAllAssetTypes()
        {
            return new HashSet<Type>(TypeCache.GetTypesDerivedFrom(typeof(UnityEngine.Object))
                .Where(t => !EditorAssemblyNames.Contains(t.Assembly.FullName.Split(',').FirstOrDefault())).OrderBy(t => t.Name));
        }


        public static void Show(Rect rect, Action<Type> typePicked)
        {
            var treeViewItems = new List<PickerTreeViewItem>();

            int id = 0;
            foreach (var assetType in AllAssetTypes)
                treeViewItems.Add(new PickerTreeViewItem(assetType, id++, assetType.Name, AssetPreview.GetMiniTypeThumbnail(assetType)));

            var popup = new KnotAssetTypePickerPopup(treeViewItems);
            popup.ItemPicked += typePicked;

            ShowAndRememberLastFocus(rect, popup);
        }
    }
}
