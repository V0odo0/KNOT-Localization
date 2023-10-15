using UnityEngine;

namespace Knot.Localization.Components
{
    [AddComponentMenu(KnotLocalization.CorePath + "Localized Sprite Renderer", 1000)]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public partial class KnotLocalizedSpriteRenderer : KnotLocalizedComponent<KnotSpriteKeyReference, Object>
    {
        protected SpriteRenderer SpriteRenderer => _spriteRenderer ?? (_spriteRenderer = GetComponent<SpriteRenderer>());
        private SpriteRenderer _spriteRenderer;


        protected override void OnValueUpdated(Object value)
        {
            if (SpriteRenderer != null)
                SpriteRenderer.sprite = value as Sprite;
        }
    }
}