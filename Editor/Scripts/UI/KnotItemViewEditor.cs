using System;
using System.Collections;
using System.Collections.Generic;
using Knot.Localization.Data;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public abstract class KnotItemViewEditor<TKeyView, TItemView, TItemData> : KnotEditorPanel
        where TKeyView : KnotKeyView<TItemView, TItemData>
        where TItemView : KnotItemView<TItemData>
        where TItemData : KnotItemData
    {
        public event Action ValueChanged;

        protected bool IsReadOnly => ItemView?.SourceCollection?.IsReadOnly ?? true;

        protected TKeyView KeyView { get; set; }
        protected TItemView ItemView { get; set; }


        protected KnotItemViewEditor(string className, Action valueChanged = null) : base(className)
        {
            ValueChanged += valueChanged;
        }

        protected virtual void OnBind() { }

        protected void OnValueChanged() => ValueChanged?.Invoke();


        public void Bind(TKeyView keyView, TItemView itemView)
        {
            KeyView = keyView;
            ItemView = itemView;

            OnBind();
        }
    }
}