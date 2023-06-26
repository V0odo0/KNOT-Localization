using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Knot.Localization.Editor
{
    [CustomEditor(typeof(KnotLocalizationImportExport))]
    public class KnotLocalizationImportExportEditor : UnityEditor.Editor
    {
        private KnotLocalizationImportExport _target;

        void OnEnable()
        {
            _target = target as KnotLocalizationImportExport;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            try
            {
                if (GUILayout.Button("Import All\n[Text Asset > Text Collection]",
                        GUILayout.Height(EditorGUIUtility.singleLineHeight * 3)))
                {
                    var importSolvers = _target.Solvers.Where(s => s.Enabled).ToArray();
                    if (EditorUtility.DisplayDialog(KnotLocalization.CoreName, $"{importSolvers.Length} solvers will be imported. Continue?", "Yes", "Cancel"))
                    {
                        foreach (var s in _target.Solvers.Where(s => s.Enabled))
                            s.Import(_target.SrcDatabase);
                    }
                }

                if (GUILayout.Button("Export All\n[Text Collection > Text Asset]",
                        GUILayout.Height(EditorGUIUtility.singleLineHeight * 3)))
                {
                    var exportSolvers = _target.Solvers.Where(s => s.Enabled).ToArray();
                    if (EditorUtility.DisplayDialog(KnotLocalization.CoreName, $"{exportSolvers.Length} solvers will be exported. Continue?", "Yes", "Cancel"))
                    {
                        foreach (var s in _target.Solvers.Where(s => s.Enabled))
                            s.Export(_target.SrcDatabase);
                    }
                }
            }
            catch
            {
                //
            }
        }

    }
}
