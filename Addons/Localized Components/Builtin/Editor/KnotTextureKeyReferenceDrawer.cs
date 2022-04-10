using System;
using Knot.Localization.Components;
using UnityEditor;
using UnityEngine;

namespace Knot.Localization.Editor
{
    [CustomPropertyDrawer(typeof(KnotTextureKeyReference), true)]
    public class KnotTextureKeyReferenceDrawer : KnotAssetKeyReferenceDrawer
    {
        protected override Type GetTargetType(SerializedProperty parentProperty) => typeof(Texture);
    }
}
