using Knot.Localization.Data;

namespace Knot.Localization
{
    /// <summary>
    /// An interface that should implement startup language selection & save selected language logic.
    /// </summary>
    public interface IKnotLanguageSelector
    {
        /// <summary>
        /// Returns startup language. Called by <see cref="KnotManager"/> in <see cref="KnotManager.SetDatabase"/>.
        /// </summary>
        KnotLanguageData GetStartupLanguage(params KnotLanguageData[] availableLanguageData);

        /// <summary>
        /// Saves selected language to restore it later in <see cref="GetStartupLanguage"/>. Called by <see cref="KnotManager"/> in <see cref="KnotManager.LoadLanguage"/>.
        /// </summary>
        void SaveSelectedLanguage(KnotLanguageData selectedLanguageData);
    }
}