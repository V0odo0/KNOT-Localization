using System.Linq;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization.Components
{
    [AddComponentMenu(KnotLocalization.CoreName + "/Localized Text Mesh", 1000)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMesh))]
    public class KnotLocalizedTextMesh : KnotLocalizedComponent<KnotTextKeyReference, string>
    {
        protected TextMesh TextMesh => _textMesh ?? (_textMesh = GetComponent<TextMesh>());
        private TextMesh _textMesh;
        
        private Font _lastCustomFont;
        private Font _defaultFont;


        void Awake()
        {
            _defaultFont = TextMesh?.font;
        }

        protected virtual void UpdateFont()
        {
            var fontMetadata = KeyReference.Metadata.OfType<KnotCustomFontMetadata>().LastOrDefault();
            if (fontMetadata != null && fontMetadata.Font != null)
            {
                if (TextMesh.font != fontMetadata.Font)
                    TextMesh.font = _lastCustomFont = fontMetadata.Font;
            }
            else if (_lastCustomFont != null)
                TextMesh.font = _defaultFont;
        }

        protected override void OnValueUpdated(string value)
        {
            if (TextMesh == null)
                return;

            UpdateFont();
            TextMesh.text = value;
        }
    }
}