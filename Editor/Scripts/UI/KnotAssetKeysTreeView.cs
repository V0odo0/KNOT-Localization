using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Knot.Localization.Editor
{
    public sealed class KnotAssetKeysTreeView : KnotKeysTreeView<KnotAssetKeyView>
    {
        public static IKnotKeyViewLabel<KnotAssetKeyView>[] Labels
        {
            get
            {
                return _labels ?? (_labels = typeof(IKnotKeyViewLabel<KnotAssetKeyView>).GetDerivedTypesInfo()
                    .Select(t => t.GetInstance()).OfType<IKnotKeyViewLabel<KnotAssetKeyView>>().ToArray());
            }
        }
        private static IKnotKeyViewLabel<KnotAssetKeyView>[] _labels;


        protected override IKnotKeyViewLabel<KnotAssetKeyView>[] KeyViewLabels => Labels;


        public KnotAssetKeysTreeView() : base(KnotEditorUtils.UserSettings.AssetKeysTreeViewState)
        {

        }
    }
}