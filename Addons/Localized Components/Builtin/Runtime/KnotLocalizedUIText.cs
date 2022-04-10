#if UNITY_UGUI
using System.Linq;
using Knot.Localization.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Knot.Localization.Components
{
    [AddComponentMenu(KnotLocalization.CoreName + "/Localized UI Text", 1000)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    public partial class KnotLocalizedUIText : KnotLocalizedComponent<KnotTextKeyReference, string>
    {
        protected Text Text => _text ?? (_text = GetComponent<Text>());
        private Text _text;

        private Font _lastCustomFont;
        private Font _defaultFont;


        void Awake()
        {
            _defaultFont = Text?.font;
        }


        protected virtual void UpdateFont()
        {
            var fontMetadata = KeyReference.Metadata.OfType<KnotCustomFontMetadata>().LastOrDefault();
            if (fontMetadata != null && fontMetadata.Font != null)
            {
                if (Text.font != fontMetadata.Font)
                    Text.font = _lastCustomFont = fontMetadata.Font;
            }
            else if (_lastCustomFont != null)
                Text.font = _defaultFont;
        }

        protected override void OnValueUpdated(string value)
        {
            if (Text == null)
                return;

            UpdateFont();
            Text.text = value;
        }
    }
}
#endif