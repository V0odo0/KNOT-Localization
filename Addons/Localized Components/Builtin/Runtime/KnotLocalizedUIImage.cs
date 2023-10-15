#if UNITY_UGUI
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Knot.Localization.Components
{
    [AddComponentMenu(KnotLocalization.CorePath + "Localized UI Image", 1000)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    public partial class KnotLocalizedUIImage : KnotLocalizedComponent<KnotSpriteKeyReference, Object>
    {
        protected Image Image => _image ?? (_image = GetComponent<Image>());
        private Image _image;


        protected override void OnValueUpdated(Object value)
        {
            if (Image != null && value is Sprite sprite)
                Image.sprite = sprite;
        }
    }
}
#endif