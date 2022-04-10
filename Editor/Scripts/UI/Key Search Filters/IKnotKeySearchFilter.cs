using System.Collections.Generic;

namespace Knot.Localization.Editor
{
    //todo: Implement default key search filters
    public interface IKnotKeySearchFilter<TKeyView> where TKeyView : KnotTreeViewKeyItem
    {
        string Name { get; }


        void ApplyFilter(string searchString, List<TKeyView> keyViews, ref HashSet<TKeyView> filteredKeyViews);
    }
}