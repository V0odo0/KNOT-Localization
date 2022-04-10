using System;
using System.Linq;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public class KnotItemViewFoldout : KnotEditorPanel
    {
        public event ContentRequest RequestFoldoutContent;
        public event Action FoldoutContentHidden;
        public event Action<FoldoutButtonState> StateButtonClicked;

        public new Foldout Root => base.Root as Foldout;

        public virtual bool IsActive
        {
            get => _toggleLabel?.enabledSelf ?? false;
            set
            {
                _toggleLabel?.SetEnabled(value);
                if (!value)
                    Root.value = false;
            }
        }
        public bool IsReadOnly
        {
            set => SwitchReadOnly(value);
        }
        public virtual string Name
        {
            get => Root.text;
            set => Root.text = value;
        }

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

        public readonly Toggle Toggle;
        public readonly Button StateButton;
        public readonly Label StateLabel;
        public readonly VisualElement ContentContainer;

        private readonly VisualElement _toggleLabel;


        public KnotItemViewFoldout(string name = "", bool isReadOnly = false) : base(nameof(KnotItemViewFoldout))
        {
            Toggle = Root.Q<Toggle>();
            _toggleLabel = Toggle.Children()?.FirstOrDefault();
            if (_toggleLabel != null)
                _toggleLabel.style.flexShrink = 1f;

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
                if (evt.newValue && !IsActive)
                    Root.SetValueWithoutNotify(false);

                if (Root.value)
                    ShowFoldoutContent();
                else HideFoldoutContent();

                KnotEditorUtils.UserSettings.SetFoldoutState(Name, Root.value);
            });

            Root.RegisterCallback(new EventCallback<GeometryChangedEvent>(evt =>
            {
                UpdateAdjacency();
            }));

            UpdateButtonState();
            
            IsReadOnly = isReadOnly;
        }
        

        protected override void OnPanelAdded()
        {
            base.OnPanelAdded();

            Root.SetValueWithoutNotify(KnotEditorUtils.UserSettings.GetFoldoutState(Name));

            UpdateAdjacency();
            ShowFoldoutContent();
        }

        protected override void OnPanelRemoved()
        {
            base.OnPanelRemoved();

            HideFoldoutContent();
        }


        void ShowFoldoutContent()
        {
            if (!Root.value)
                return;

            ContentContainer.Add(RequestFoldoutContent?.Invoke());
        }

        void HideFoldoutContent()
        {
            ContentContainer.Clear();

            FoldoutContentHidden?.Invoke();
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

        void SwitchReadOnly(bool isReadOnly)
        {
            StateButton.SetEnabled(!isReadOnly);
        }

        void MoveToToggle(VisualElement e)
        {
            if (e == null)
                return;

            if (e.parent != null && e.parent != Toggle)
                e.parent.Remove(e);

            Toggle.Add(e);
        }


        public enum FoldoutButtonState
        {
            None, Add, AddContextMenu, Remove
        }

        public delegate VisualElement ContentRequest();
    }
}