using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Knot.Localization.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace Knot.Localization.Editor
{
    public class KnotCreateKeyPopup : KnotPopupWindowContent
    {
        static string _lastInputKey;
        
        public event Action<KnotKeyCollection, string> CreateKeyClicked;

        public readonly TextField KeyTextField;
        public readonly Button CreateKeyButton;

        private readonly string _inputKey;
        private readonly KnotKeyCollection[] _keyCollections;


        protected KnotCreateKeyPopup(List<KnotKeyCollection> keyCollections, string inputKey) : base(nameof(KnotCreateKeyPopup))
        {
            _keyCollections = keyCollections?.Where(c => c != null).Distinct().ToArray() ?? new KnotKeyCollection[0];
            _inputKey = inputKey;

            KeyTextField = Panel.Root.Q<TextField>(nameof(KeyTextField));
            KeyTextField.RegisterCallback(new EventCallback<KeyDownEvent>(evt =>
            {
                switch (evt.keyCode)
                {
                    case KeyCode.Escape:
                        Close();
                        break;
                    case KeyCode.Return:
                        CreateKey();
                        break;
                }
            }));
            KeyTextField.RegisterValueChangedCallback(evt =>
            {
                _lastInputKey = KeyTextField.value;
                CreateKeyButton.SetEnabled(!string.IsNullOrEmpty(KeyTextField.value));
            });

            CreateKeyButton = Panel.Root.Q<Button>(nameof(CreateKeyButton));
            CreateKeyButton.clicked += CreateKey;
            CreateKeyButton.SetEnabled(_keyCollections.Any());
            CreateKeyButton.Add(new Image{image = KnotEditorUtils.GetIcon(_keyCollections.Length == 1 ? "Toolbar Plus" : "Toolbar Plus More") });
            
            SetDeferredFocusTarget(KeyTextField.GetVisualInput());
        }

        void ShowSelectTargetCollectionMenu(Action<KnotKeyCollection> collectionSelected)
        {
            
        }

        void CreateKey()
        {
            if (string.IsNullOrEmpty(KeyTextField.text))
                return;

            if (_keyCollections.Length > 1)
            {
                GenericMenu m = new GenericMenu();
                foreach (var keyCollection in _keyCollections)
                    m.AddItem(new GUIContent(keyCollection.name), false, () =>
                    {
                        _lastInputKey = string.Empty;
                        CreateKeyClicked?.Invoke(keyCollection, KeyTextField.text);
                        Close();
                    });

                m.ShowAsContext();
            }
            else
            {
                _lastInputKey = string.Empty;
                CreateKeyClicked?.Invoke(_keyCollections.FirstOrDefault(), KeyTextField.text);
                Close();
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();

            KeyTextField.value = string.IsNullOrEmpty(_lastInputKey) ? _inputKey : _lastInputKey;
            CreateKeyButton.SetEnabled(!string.IsNullOrEmpty(KeyTextField.value));
        }

        public override Vector2 GetWindowSize() => new Vector2(300, 8 + EditorGUIUtility.singleLineHeight);


        public static KnotCreateKeyPopup Show(Rect rect, List<KnotKeyCollection> keyCollections, Action<KnotKeyCollection, string> onCreateKeyClicked, string inputKey = "")
        {
            KnotCreateKeyPopup window = new KnotCreateKeyPopup(keyCollections, inputKey);
            window.CreateKeyClicked += onCreateKeyClicked;
            ShowAndRememberLastFocus(rect, window);

            return window;
        }
    }
}