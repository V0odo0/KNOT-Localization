using Knot.Localization.Attributes;
using UnityEngine;

namespace Knot.Localization.Data
{
    /// <summary>
    /// Metadata used to store reference to custom <see cref="Font"/>. Implements <see cref="IKnotKeySharedMetadata"/>
    /// </summary>
    [KnotMetadataInfo("Custom Font", KnotMetadataInfoAttribute.MetadataScope.Database | KnotMetadataInfoAttribute.MetadataScope.Language | KnotMetadataInfoAttribute.MetadataScope.Text)]
    public class KnotCustomFontMetadata : IKnotKeySharedMetadata
    {
        public Font Font
        {
            get => _font;
            set => _font = value;
        }
        [SerializeField] private Font _font;


        public KnotCustomFontMetadata() { }

        public KnotCustomFontMetadata(Font font)
        {
            _font = font;
        }


        public object Clone() => new KnotCustomFontMetadata(_font);
    }
}