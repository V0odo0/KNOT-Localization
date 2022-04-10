using System;
using Knot.Localization.Components;
using UnityEditor;
using UnityEngine;

namespace Knot.Localization.Editor
{
    [CustomPropertyDrawer(typeof(KnotSpriteKeyReference), true)]
    public class KnotSpriteKeyReferenceDrawer : KnotAssetKeyReferenceDrawer
    {
        protected override Type GetTargetType(SerializedProperty parentProperty) => typeof(Sprite);
    }
}