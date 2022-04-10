using System.Collections.Generic;
using Knot.Localization.Attributes;
using Knot.Localization.Data;

namespace Knot.Localization.Editor
{
    public class KnotTextKeyViewEditorPanel : KnotKeyViewEditorPanel<KnotTextKeyView, KnotTextItemView, KnotTextData>
    {
        protected override KnotMetadataInfoAttribute.MetadataScope MetadataScope =>
            KnotMetadataInfoAttribute.MetadataScope.Text;

        protected override List<KnotKeyCollection> KeyCollections => Database.TextKeyCollections;


        protected override KnotTextData GetNewItem(KnotTextKeyView keyView) => new KnotTextData(keyView.Key);

        protected override KnotItemViewEditor<KnotTextKeyView, KnotTextItemView, KnotTextData> GetNewItemViewEditor(KnotTextKeyView keyView, KnotTextItemView itemView)
        {
            return new KnotTextItemViewEditor();
        }
    }
}
