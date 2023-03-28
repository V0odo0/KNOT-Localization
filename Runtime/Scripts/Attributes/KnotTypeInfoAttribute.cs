using System;

namespace Knot.Localization.Attributes
{
    /// <summary>
    /// Special attribute used to expand type info  
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class KnotTypeInfoAttribute : Attribute, IComparable<KnotTypeInfoAttribute>
    {
        public string MenuName => string.IsNullOrEmpty(MenuCustomName) ? DisplayName : MenuCustomName;

        public string DisplayName;
        public int Order;
        public string IconName;
        public string MenuCustomName;

        public KnotTypeInfoAttribute(string displayName, int order = 0, string iconName = "", string menuCustomName = "")
        {
            DisplayName = displayName;
            Order = order;
            IconName = iconName;
            MenuCustomName = menuCustomName;
        }

        public virtual int CompareTo(KnotTypeInfoAttribute other)
        {
            if (Order - other.Order == 0)
                return string.Compare(DisplayName, other.DisplayName, StringComparison.Ordinal);

            return Order - other.Order;
        }
    }
}