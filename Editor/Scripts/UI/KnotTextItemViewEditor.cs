using System;
using Knot.Localization.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public class KnotTextItemViewEditor : KnotItemViewEditor<KnotTextKeyView, KnotTextItemView, KnotTextData>
    {
        public readonly TextField RawTextField;
        public readonly VisualElement RawTextInput;
        public readonly ContextualMenuManipulator TextInputContextMenu;

        private int _delayedSelectIndex;
        private string[] _dragAndDropKeys;


        public KnotTextItemViewEditor(Action valueChanged = null) : base(nameof(KnotTextItemViewEditor), valueChanged)
        {
            RawTextField = Root.Q<TextField>(nameof(RawTextField));
            RawTextField.multiline = RawTextField.isDelayed = true;

            RawTextInput = RawTextField.Q("unity-text-input");
            RawTextInput.style.whiteSpace = new StyleEnum<WhiteSpace>(WhiteSpace.Normal);

            TextInputContextMenu = new ContextualMenuManipulator(BuildTextInputContextMenu) {target = RawTextInput};

            RawTextField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue == evt.previousValue)
                    return;

                KnotEditorUtils.RegisterCompleteObjects("Text Changed",
                    () =>
                    {
                        ItemView.ItemData.RawText = evt.newValue;
                    }, ItemView.SourceAsset);

                OnValueChanged();
            });
        }


        protected virtual void BuildTextInputContextMenu(ContextualMenuPopulateEvent evt)
        {
            //todo
        }

        protected virtual void InsertTag(DropdownMenuAction dropdownMenuAction)
        {
            /*int selectRangeStart = Math.Min(ValueTextField.cursorIndex, ValueTextField.selectIndex);
            int selectRangeEnd = Math.Max(ValueTextField.cursorIndex, ValueTextField.selectIndex);

            UnityEditor.PopupWindow.Show(ValueTextInput.worldBound, new KnotTextPickerPopup(tag =>
            {
                string newValue = ValueTextField.value.Remove(selectRangeStart, selectRangeEnd - selectRangeStart).Insert(selectRangeStart, tag);
                ValueTextField.value = newValue;
                
                _delayedSelectIndex = selectRangeStart + tag.Length;
                ValueTextField.schedule.Execute(() =>
                {
                    ValueTextInput.Focus();
                    ValueTextField.SelectRange(_delayedSelectIndex, _delayedSelectIndex);
                });

            }, true, TextItemView == null ? new string[0] : new[] {TextItemView.Key}));*/
        }

        protected override void OnBind()
        {
            RawTextField.SetValueWithoutNotify(ItemView.ItemData.RawText);

            RawTextField.isReadOnly = IsReadOnly;
        }
    }
}