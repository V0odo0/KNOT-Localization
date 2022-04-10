using System;
using System.Collections.Generic;
using Knot.Localization.Data;

namespace Knot.Localization
{
    [Serializable]
    public class KnotTextKeyReference : KnotKeyReference<string>
    {
        public override string Value => KnotLocalization.GetText(Key);
        public override IEnumerable<IKnotMetadata> Metadata => KnotLocalization.Manager.GetTextValue(Key)?.Metadata;


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