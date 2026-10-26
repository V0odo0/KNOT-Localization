#if KNOT_UITOOLKIT
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Components
{
    /// <summary>
    /// UI Toolkit binding that writes the localized and formatted text of <see cref="TextKey"/> to the bound property (e.g. <see cref="TextElement.text"/>)
    /// and updates it whenever the language or the text value changes.
    /// <para>Outside of Play Mode (e.g. in UI Builder) the key itself is shown as a placeholder and the localization system is not accessed</para>
    /// <para>UXML: &lt;ui:Label&gt;&lt;Bindings&gt;&lt;Knot.Localization.Components.KnotLocalizedTextBinding property="text" key="My.Key" /&gt;&lt;/Bindings&gt;&lt;/ui:Label&gt;</para>
    /// <para>Code: <c>label.SetBinding("text", new KnotLocalizedTextBinding("My.Key"))</c> or <c>label.BindLocalizedText("My.Key")</c></para>
    /// </summary>
    [UxmlObject]
    public partial class KnotLocalizedTextBinding : CustomBinding
    {
        /// <summary>
        /// Text key with optional formatters. Treated as read-only by the binding: assign a new value (or reassign the same one) to apply changes
        /// </summary>
        [UxmlAttribute("key")]
        public KnotTextKeyReference TextKey
        {
            get => _textKey ?? (_textKey = new KnotTextKeyReference());
            set
            {
                _textKey = value;
                UpdateTextUpdatedCallback();
                MarkDirty();
            }
        }
        private KnotTextKeyReference _textKey;

        public string Key
        {
            get => TextKey.Key;
            set
            {
                //KnotTextKeyReference.Key setter registers callbacks in the localization manager, so a new reference is created instead
                var textKey = new KnotTextKeyReference(value);
                if (_textKey != null)
                    textKey.Formatters.AddRange(_textKey.Formatters);

                TextKey = textKey;
            }
        }

        private readonly Action<string> _textUpdated;
        private int _activeTargetsCount;
        private string _registeredKey;


        public KnotLocalizedTextBinding()
        {
            updateTrigger = BindingUpdateTrigger.WhenDirty;
            _textUpdated = _ => MarkDirty();
        }

        public KnotLocalizedTextBinding(string key) : this()
        {
            _textKey = new KnotTextKeyReference(key);
        }

        public KnotLocalizedTextBinding(KnotTextKeyReference textKey) : this()
        {
            _textKey = textKey;
        }


        void UpdateTextUpdatedCallback()
        {
            var key = _activeTargetsCount > 0 && Application.isPlaying ? _textKey?.Key : null;
            if (key == _registeredKey)
                return;

            if (!string.IsNullOrEmpty(_registeredKey))
                KnotLocalization.UnRegisterTextUpdatedCallback(_registeredKey, _textUpdated);

            _registeredKey = string.IsNullOrEmpty(key) ? null : key;

            if (_registeredKey != null)
                KnotLocalization.RegisterTextUpdatedCallback(_registeredKey, _textUpdated);
        }


        protected override void OnActivated(in BindingActivationContext context)
        {
            _activeTargetsCount++;
            UpdateTextUpdatedCallback();
            MarkDirty();
        }

        protected override void OnDeactivated(in BindingActivationContext context)
        {
            _activeTargetsCount = Math.Max(0, _activeTargetsCount - 1);
            UpdateTextUpdatedCallback();
        }

        protected override BindingResult Update(in BindingContext context)
        {
            string text;
            if (Application.isPlaying)
            {
                //Key may have been changed inside the shared reference
                UpdateTextUpdatedCallback();
                text = string.IsNullOrEmpty(_textKey?.Key) ? string.Empty : _textKey.Value;
            }
            else text = _textKey?.Key ?? string.Empty;

            var element = context.targetElement;

            if (ConverterGroups.TrySetValueGlobal(ref element, context.bindingId, text, out var returnCode))
                return new BindingResult(BindingStatus.Success);

            return new BindingResult(BindingStatus.Failure,
                $"{nameof(KnotLocalizedTextBinding)}: can't set \"{context.bindingId}\" of {element.GetType().Name} ({returnCode})");
        }
    }

    public static class KnotLocalizedTextBindingExtensions
    {
        /// <summary>
        /// Binds <paramref name="property"/> of <paramref name="element"/> to the localized text of <paramref name="key"/>
        /// </summary>
        public static T BindLocalizedText<T>(this T element, string key, string property = "text") where T : VisualElement
        {
            element?.SetBinding(property, new KnotLocalizedTextBinding(key));
            return element;
        }

        /// <summary>
        /// Binds <paramref name="property"/> of <paramref name="element"/> to the localized and formatted text of <paramref name="textKey"/>
        /// </summary>
        public static T BindLocalizedText<T>(this T element, KnotTextKeyReference textKey, string property = "text") where T : VisualElement
        {
            element?.SetBinding(property, new KnotLocalizedTextBinding(textKey));
            return element;
        }
    }
}
#endif
