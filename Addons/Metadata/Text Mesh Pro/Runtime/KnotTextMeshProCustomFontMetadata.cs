#if KNOT_TEXTMESHPRO
using Knot.Localization.Attributes;
using TMPro;
using UnityEngine;

namespace Knot.Localization.Data
{
    /// <summary>
    /// Metadata used to store reference to custom <see cref="TMPro.TMP_FontAsset"/>. Implements <see cref="IKnotKeySharedMetadata"/>
    /// </summary>
    [KnotMetadataInfo("Custom Font (Text Mesh PRO)", KnotMetadataInfoAttribute.MetadataScope.Database | KnotMetadataInfoAttribute.MetadataScope.Language | KnotMetadataInfoAttribute.MetadataScope.Text)]
    public class KnotTextMeshProCustomFontMetadata : IKnotKeySharedMetadata
    {
        public TMP_FontAsset Font
        {
            get => _font;
            set => _font = value;
        }
        [SerializeField] private TMP_FontAsset _font;


        public KnotTextMeshProCustomFontMetadata() { }

        public KnotTextMeshProCustomFontMetadata(TMP_FontAsset font)
        {
            _font = font;
        }


        public object Clone() => new KnotTextMeshProCustomFontMetadata(_font);
    }
}
#endif