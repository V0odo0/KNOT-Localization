using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Knot.Localization.Attributes;
using UnityEngine;
using Knot.Localization.Data;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Knot.Localization
{
    [CreateAssetMenu(fileName = "KnotLocalizationImportExport", menuName = KnotLocalization.CorePath + "Addons/Import Export", order = 50)]
    public class KnotLocalizationImportExport : ScriptableObject
    {
        public KnotDatabase SrcDatabase => _srcDatabase;
        [SerializeField] private KnotDatabase _srcDatabase;

        public List<ImportExportSolver> Solvers => _solvers;
        [KnotTypePicker(typeof(ImportExportSolver)), SerializeReference]
        private List<ImportExportSolver> _solvers;


        [Serializable]
        public abstract class ImportExportSolver
        {
            public bool Enabled = true;
            public KnotTextCollection TextCollection;

            public abstract void Import(KnotDatabase database);

            public abstract void Export(KnotDatabase database);
        }

        [Serializable]
        public class ImportExportCsvSolver : ImportExportSolver
        {
            public TextAsset TextAsset;
            public IOSettings Settings;
            

            public override void Import(KnotDatabase database)
            {
#if UNITY_EDITOR
                if (!Enabled || TextCollection == null || string.IsNullOrEmpty(Settings.ImportDelimiter) || TextAsset == null)
                    return;

                Undo.RegisterCompleteObjectUndo(TextCollection, "Import");

                TextCollection.Clear();
                int failedImportLines = 0;
                foreach (var l in TextAsset.text.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    try
                    {
                        var importKeyIndex = l.IndexOf(Settings.ImportDelimiter, StringComparison.CurrentCulture);
                        if (importKeyIndex < 0)
                        {
                            failedImportLines++;
                            continue;
                        }

                        var keyVal = SplitAt(l, importKeyIndex);
                        if (string.IsNullOrEmpty(keyVal.left) || string.IsNullOrEmpty(keyVal.right))
                        {
                            failedImportLines++;
                            continue;
                        }

                        if (Settings.ImportHasQuotes)
                        {
                            if (keyVal.left.StartsWith("\"") && keyVal.left.EndsWith("\""))
                            {
                                keyVal.left = keyVal.left.Remove(0, 1);
                                keyVal.left = keyVal.left.Remove(keyVal.left.Length - 1, 1);
                            }

                            if (keyVal.right.StartsWith("\"") && keyVal.right.EndsWith("\""))
                            {
                                keyVal.right = keyVal.right.Remove(0, 1);
                                keyVal.right = keyVal.right.Remove(keyVal.right.Length - 1, 1);
                            }
                        }
                        

                        TextCollection.Add(new KnotTextData(keyVal.left, keyVal.right));
                    }
                    catch
                    {
                        failedImportLines++;
                    }
                    
                }

                EditorUtility.SetDirty(TextCollection);

                if (failedImportLines > 0)
                    Debug.Log($"{this.GetType()} {nameof(failedImportLines)}: {failedImportLines}");
#endif
            }

            public override void Export(KnotDatabase database)
            {
#if UNITY_EDITOR
                if (!Enabled || TextCollection == null || TextAsset == null || string.IsNullOrEmpty(Settings.ImportDelimiter))
                    return;

                StringBuilder sb = new StringBuilder();
                
                if (Settings.ExportWithQuotes)
                {
                    foreach (var t in TextCollection.OrderBy(d => d.Key))
                        sb.AppendLine($"\"{t.Key}\"{Settings.ExportDelimiter}\"{t.RawText}\"");
                }
                else
                {
                    foreach (var t in TextCollection.OrderBy(d => d.Key))
                        sb.AppendLine($"{t.Key}{Settings.ExportDelimiter}{t.RawText}");
                }

                var path = AssetDatabase.GetAssetPath(TextAsset);
                File.WriteAllText(path, sb.ToString());
#endif
            }


            [Serializable]
            public class IOSettings
            {
                public string ImportDelimiter = ";";
                public string ExportDelimiter = ";";
                public bool ExportWithQuotes = true;
                public bool ImportHasQuotes = true;
            }
        }

        static (string left, string right) SplitAt(string text, int index) =>
            (text.Substring(0, index), text.Substring(index + 1));
    }
}
