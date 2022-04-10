using System;
using Knot.Localization.Data;
using UnityEditor;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public class KnotEditorPanel
    {
        public virtual KnotDatabase Database => KnotDatabaseUtils.ActiveDatabase;

        public SerializedObject DatabaseObj => _databaseObj ?? (_databaseObj = new SerializedObject(Database));
        private SerializedObject _databaseObj;

        public readonly VisualElement Root;


        public KnotEditorPanel(string className)
        {
            if (KnotEditorUtils.EditorPanels.TryGetValue(className, out var asset))
                Root = asset.CloneTree().Q<VisualElement>(className);
            else throw new Exception($"{nameof(VisualElement)} was not found for {className}");

            Root.RegisterCallback<AttachToPanelEvent>(evt =>
            {
                OnPanelAdded();
            });
            Root.RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                OnPanelRemoved();
            });

        }

        public KnotEditorPanel(VisualElement element)
        {
            Root = element;
        }
        

        protected virtual void OnPanelAdded() { }

        protected virtual void OnPanelRemoved() { }
        

        public virtual void ReloadLayout() { }


        public static implicit operator VisualElement(KnotEditorPanel panel)
        {
            return panel.Root;
        }
    }
}