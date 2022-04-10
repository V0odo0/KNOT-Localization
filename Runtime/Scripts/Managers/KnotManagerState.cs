namespace Knot.Localization
{
    /// <summary>
    /// <see cref="IKnotManager"/> state
    /// </summary>
    public enum KnotManagerState
    {
        /// <summary>
        /// <see cref="IKnotManager"/> has no <see cref="Data.KnotLanguageData"/> loaded
        /// </summary>
        LanguageNotLoaded,
        /// <summary>
        /// <see cref="IKnotManager.SelectedLanguage"/> is in loading process
        /// </summary>
        LoadingLanguage,
        /// <summary>
        /// <see cref="IKnotManager.SelectedLanguage"/> has been loaded
        /// </summary>
        LanguageLoaded
    }
}