using System;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization
{
    [Serializable]
    [KnotTypeInfo("Default Text Controller")]
    public class KnotTextController : KnotController<KnotTextData, IKnotText, string>, IKnotTextController
    {
        protected const string KeyFormatArg = "{key}";


        [SerializeField] private string _fallbackValue = KeyFormatArg;


        protected override IKnotText CreateValueFromItemData(KnotTextData itemData, params IKnotMetadata[] metadata)
        {
            return new KnotText(itemData.RawText, metadata);
        }

        protected override IKnotText CreateEmptyValue(string key, params IKnotMetadata[] metadata)
        {
            return new KnotText(GetFallbackValue(key), metadata);
        }

        protected override IKnotText CreateValue(string value, params IKnotMetadata[] metadata)
        {
            return new KnotText(value, metadata);
        }
        

        public override string GetFallbackValue(string key)
        {
            return !_fallbackValue.Contains(KeyFormatArg) ?
                _fallbackValue :
                string.Format(_fallbackValue.Replace(KeyFormatArg, "{0}"), key);
        }

        public override object Clone() => new KnotTextController();
    }
}