using System.Linq;

namespace Knot.Localization.Editor
{
    public sealed class KnotTextKeysTreeView : KnotKeysTreeView<KnotTextKeyView>
    {
        public static IKnotKeyViewLabel<KnotTextKeyView>[] Labels
        {
            get
            {
                return _labels ?? (_labels = typeof(IKnotKeyViewLabel<KnotTextKeyView>).GetDerivedTypesInfo()
                    .Select(t => t.GetInstance()).OfType<IKnotKeyViewLabel<KnotTextKeyView>>().ToArray());
            }
        }
        private static IKnotKeyViewLabel<KnotTextKeyView>[] _labels;


        protected override IKnotKeyViewLabel<KnotTextKeyView>[] KeyViewLabels => Labels;


        public KnotTextKeysTreeView() : base(KnotEditorUtils.UserSettings.TextKeysTreeViewState)
        {

        }
    }
}