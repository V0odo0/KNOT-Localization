#if UNITY_UGUI
using UnityEngine;
using UnityEngine.UI;

namespace Knot.Localization.Components
{
    [AddComponentMenu(KnotLocalization.CorePath + "Localized UI Raw Image", 1000)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RawImage))]
    public partial class KnotLocalizedUIRawImage : KnotLocalizedComponent<KnotTextureKeyReference, Object>
    {
        public RawImage RawImage => _rawImage ?? (_rawImage = GetComponent<RawImage>());
        private RawImage _rawImage;


        protected override void OnValueUpdated(Object value)
        {
            if (RawImage != null && value is Texture texture)
                RawImage.texture = texture;
        }
    }
}
#endif