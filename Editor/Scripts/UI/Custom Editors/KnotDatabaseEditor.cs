using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Knot.Localization.Data;
using UnityEngine;
using UnityEditor;

namespace Knot.Localization.Editor
{
    [CustomEditor(typeof(KnotDatabase))]
    public class KnotDatabaseEditor : UnityEditor.Editor
    {
        protected KnotDatabase Target => Targets.FirstOrDefault();
        protected KnotDatabase[] Targets;

        void OnEnable()
        {
            Targets = targets.OfType<KnotDatabase>().ToArray();
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open", GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
            {
                KnotDatabaseUtils.OpenDatabaseEditor(Target);
            }
        }
    }
}