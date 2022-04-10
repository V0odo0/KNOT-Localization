using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

namespace Knot.Localization.Editor
{
    public abstract class KnotPopupWindowContent : PopupWindowContent
    {
        private static EditorWindow _lastFocusedWindow;

        public readonly KnotEditorPanel Panel;

        private VisualElement _deferredFocusTarget;


        protected KnotPopupWindowContent(string className)
        {
            Panel = new KnotEditorPanel(className);
        }

        protected KnotPopupWindowContent(VisualElement element)
        {
            Panel = new KnotEditorPanel(element);
        }


        protected virtual void HandleKeys()
        {
            if (Event.current.type == EventType.KeyDown)
            {
                switch (Event.current.keyCode)
                {
                    case KeyCode.Escape:
                        Close();
                        break;
                }
            }
        }

        protected void SetDeferredFocusTarget(VisualElement element)
        {
            _deferredFocusTarget = element;
        }


        public override void OnGUI(Rect rect)
        {
            if (_deferredFocusTarget != null)
            {
                _deferredFocusTarget.Focus();
                _deferredFocusTarget = null;
            }

            HandleKeys();
        }

        public override void OnOpen()
        {
            editorWindow.rootVisualElement.Add(Panel);
        }

        public virtual void Close()
        {
            editorWindow.Close();

            if (_lastFocusedWindow != null)
                _lastFocusedWindow.Focus();
        }


        protected static void ShowAndRememberLastFocus(Rect rect, PopupWindowContent content)
        {
            _lastFocusedWindow = EditorWindow.focusedWindow;

            

            PopupWindow.Show(rect, content);
        }
    }
}