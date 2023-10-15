#if UNITY_UGUI
using System.Linq;
using Knot.Localization.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Knot.Localization.Components
{
    [AddComponentMenu(KnotLocalization.CorePath + "Localized UI Text", 1000)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public partial class KnotLocalizedUIText : KnotLocalizedComponent<KnotTextKeyReference, string>
    {
        protected Text Text => _text ?? (_text = GetComponent<Text>());
        private Text _text;


        protected virtual void UpdateFont()
        {
            var fontMetadata = KeyReference.Metadata.OfType<KnotCustomFontMetadata>().LastOrDefault();
            if (fontMetadata?.Font != null && Text.font != fontMetadata.Font)
                Text.font = fontMetadata.Font;
        }

        protected override void OnValueUpdated(string value)
        {
            if (Text == null)
                return;

            UpdateFont();
            Text.text = KeyReference.Value;
        }

        protected override void OnValidate()
        {
            ForceUpdateValue();
        }
    }
}
#endif