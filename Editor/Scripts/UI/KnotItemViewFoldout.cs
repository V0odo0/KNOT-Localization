using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public class KnotItemViewFoldout : KnotEditorPanel
    {
        public event RequestContentEventHandler RequestAddContent;
        public event Action RequestRemoveContent;
        public event Action<FoldoutButtonState> StateButtonClicked;

        public new Foldout Root => base.Root as Foldout;

        public virtual bool IsActive
        {
            get => ToggleLabel?.enabledSelf ?? false;
            set
            {
                ToggleLabel?.SetEnabled(value);
                if (!value)
                    Root.value = false;
            }
        }
        public virtual string StateLabelText
        {
            get => StateLabel?.text;
            set
            {
                if (StateLabel != null)
                    StateLabel.text = value;
            }
        }

        protected string FoldoutStateName => $"{nameof(KnotItemViewFoldout)}_{Root.text}";
        
        public FoldoutButtonState ButtonState
        {
            get => _buttonState;
            set
            {
                if (value == _buttonState)
                    return;

                _buttonState = value;
                UpdateButtonState();
            }
        }
        private FoldoutButtonState _buttonState = FoldoutButtonState.None;

        protected readonly Toggle Toggle;
        protected readonly VisualElement ToggleLabel;
        protected readonly Button StateButton;
        protected readonly Label StateLabel;
        protected readonly VisualElement ContentContainer;


        public KnotItemViewFoldout(string name = "", bool readOnly = false, Texture icon = null) : base(nameof(KnotItemViewFoldout))
        {
            Toggle = Root.Q<Toggle>();
            ToggleLabel = Toggle.Children()?.FirstOrDefault();
            if (ToggleLabel != null)
                ToggleLabel.style.flexShrink = 1f;

            if (icon != null)
            {
                var checkMark = Root.Q("unity-checkmark");
                if (checkMark != null)
                {
                    var iconImage = new Image { image = icon };
                    iconImage.style.minWidth = 16;
                    checkMark?.parent.Insert(0, iconImage);
                }
            }

            ContentContainer = Root.Q<VisualElement>(nameof(ContentContainer));

            StateLabel = Root.Q<Label>(nameof(StateLabel));
            MoveToToggle(StateLabel);

            StateButton = Root.Q<Button>(nameof(StateButton));
            MoveToToggle(StateButton);

            StateButton.clickable.clicked += () =>
            {
                StateButtonClicked?.Invoke(ButtonState);
            };

            Root.text = name;
            Root.RegisterValueChangedCallback(evt =>
            {
                KnotEditorUtils.UserSettings.SetFoldoutState(FoldoutStateName, evt.newValue);
                if (!IsActive)
                    return;

                if (evt.newValue)
                    ContentContainer.Add(RequestAddContent?.Invoke());
                else RequestRemoveContent?.Invoke();
            });

            Root.RegisterCallback(new EventCallback<GeometryChangedEvent>(evt =>
            {
                UpdateAdjacency();
            }));

            UpdateButtonState();
            
            SetReadOnly(readOnly);
        }
        

        protected override void OnPanelAdded()
        {
            base.OnPanelAdded();

            Root.value = KnotEditorUtils.UserSettings.GetFoldoutState(FoldoutStateName);

            UpdateAdjacency();

            if (Root.value)
                ContentContainer.Add(RequestAddContent?.Invoke());
        }

        protected override void OnPanelRemoved()
        {
            base.OnPanelRemoved();

            RequestRemoveContent?.Invoke();
        }


        void UpdateButtonState()
        {
            if (ButtonState == FoldoutButtonState.None)
            {
                if (Toggle.Contains(StateButton))
                    Toggle.Remove(StateButton);
            }
            else
            {
                if (!Toggle.Contains(StateButton))
                {
                    Toggle.Add(StateButton);
                    StateButton.BringToFront();
                }

                switch (ButtonState)
                {
                    default:
                        StateButton.tooltip = string.Empty;
                        break;
                    case FoldoutButtonState.Add:
                    case FoldoutButtonState.AddContextMenu:
                        StateButton.tooltip = "Add value to collection";
                        break;
                    case FoldoutButtonState.Remove:
                        StateButton.tooltip = "Remove value from collection";
                        break;

                }

                UpdateStateButtonIcon();
            }
        }

        void UpdateStateButtonIcon()
        {
            Image icon = StateButton.Q<Image>();
            if (icon == null)
            {
                StateButton.Add(icon = new Image());
                icon.pickingMode = PickingMode.Ignore;
            }

            switch (ButtonState)
            {
                case FoldoutButtonState.Add:
                    icon.image = KnotEditorUtils.GetIcon("Toolbar Plus");
                    break;
                case FoldoutButtonState.AddContextMenu:
                    icon.image = KnotEditorUtils.GetIcon("Toolbar Plus More");
                    break;
                case FoldoutButtonState.Remove:
                    icon.image = KnotEditorUtils.GetIcon("Toolbar Minus");
                    break;
            }
        }

        void UpdateAdjacency()
        {
            if (Root.parent == null)
                return;
            
            var childFoldouts = Root.parent.Children().Where(e => e.GetType() == Root.GetType()).ToArray();
            bool isFirst = childFoldouts.FirstOrDefault() == Root;
            bool isLast = childFoldouts.LastOrDefault() == Root;
            
            Root.style.borderTopRightRadius = Root.style.borderTopLeftRadius = isFirst ? 3 : 0;
            Root.style.borderBottomRightRadius = Root.style.borderBottomLeftRadius = isLast ? 3 : 0;
            Root.style.borderTopWidth = isFirst ? 1 : 0;
        }

        void MoveToToggle(VisualElement e)
        {
            if (e == null)
                return;

            if (e.parent != null && e.parent != Toggle)
                e.parent.Remove(e);

            Toggle.Add(e);
        }


        public void SetReadOnly(bool readOnly)
        {
            StateButton?.SetEnabled(!readOnly);
        }


        public enum FoldoutButtonState
        {
            None, Add, AddContextMenu, Remove
        }

        public delegate VisualElement RequestContentEventHandler();
    }
}