using System.Globalization;

namespace Knot.Localization.Data
{
    /// <summary>
    /// An interface for <see cref="CultureInfo"/> dependent values.
    /// <see cref="SetCulture"/> is called by <see cref="KnotController{TItemData,TValue,TValueType}"/> on build time.
    /// Use it to store <see cref="CultureInfo"/> of selected language 
    /// </summary>
    public interface IKnotCultureSpecificMetadata : IKnotMetadata
    {
        /// <summary>
        /// Called by <see cref="KnotController{TItemData,TValue,TValueType}"/> during <seealso cref="KnotController{TItemData,TValue,TValueType}.BuildAsync"/>
        /// </summary>
        /// <param name="cultureInfo">Current <see cref="CultureInfo"/> of <see cref="IKnotManager.SelectedLanguage"/></param>
        void SetCulture(CultureInfo cultureInfo);
    }
}