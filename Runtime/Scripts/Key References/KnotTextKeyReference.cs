using System;
using System.Collections.Generic;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization
{
    [Serializable]
    public class KnotTextKeyReference : KnotKeyReference<string>
    {
        public override string Value
        {
            get
            {
                if (_formatters == null || _formatters.Count == 0)
                    return KnotLocalization.GetText(Key);

                return KnotText.Format(KnotLocalization.GetText(Key), Formatters);
            }
        }
        public override IEnumerable<IKnotMetadata> Metadata => KnotLocalization.Manager.GetTextValue(Key)?.Metadata ?? Array.Empty<IKnotMetadata>();

        public List<IKnotTextFormatterMetadata> Formatters =>
            _formatters ?? (_formatters = new List<IKnotTextFormatterMetadata>());
        [SerializeReference, KnotTypePicker(typeof(IKnotTextFormatterMetadata))] private List<IKnotTextFormatterMetadata> _formatters;


        public KnotTextKeyReference() { }

        public KnotTextKeyReference(string key) : base(key) { }


        protected override void RegisterValueUpdatedCallback(string key, Action<string> valueUpdated)
        {
            KnotLocalization.RegisterTextUpdatedCallback(key, valueUpdated);
        }

        protected override void UnRegisterValueUpdatedCallback(string key, Action<string> valueUpdated)
        {
            KnotLocalization.UnRegisterTextUpdatedCallback(key, valueUpdated);
        }
    }
}