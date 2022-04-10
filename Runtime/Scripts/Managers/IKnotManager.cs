using System;
using System.Collections.Generic;
using Knot.Localization.Data;

namespace Knot.Localization
{
    /// <summary>
    /// Base interface that should implement <see cref="KnotDatabase"/> processing, <see cref="KnotLanguageData"/> runtime selection
    /// and <see cref="IKnotController{TItemData,TValue,TValueType}"/> access
    /// </summary>
    public interface IKnotManager : IDisposable
    {
        /// <summary>
        /// Current <see cref="State"/> update callback
        /// </summary>
        event Action<KnotManagerState> StateChanged;

        /// <summary>
        /// <see cref="KnotDatabase"/> assigned to this manager using <see cref="SetDatabase"/> 
        /// </summary>
        KnotDatabase Database { get; }

        /// <summary>
        /// Current <see cref="KnotManagerState"/>
        /// </summary>
        KnotManagerState State { get; }

        /// <summary>
        /// Selected <see cref="KnotLanguageData"/>
        /// </summary>
        KnotLanguageData SelectedLanguage { get; }

        /// <summary>
        /// Currently available <see cref="KnotLanguageData"/> collection
        /// </summary>
        IList<KnotLanguageData> Languages { get; }

        /// <summary>
        /// Provides access to <see cref="IKnotText"/> collection from <see cref="SelectedLanguage"/>
        /// </summary>
        IKnotTextController TextController { get; }

        /// <summary>
        /// Provides access to <see cref="IKnotAsset"/> collection from <see cref="SelectedLanguage"/>
        /// </summary>
        IKnotAssetController AssetController { get; }


        /// <summary>
        /// Assigns <see cref="KnotDatabase"/> to this manager
        /// </summary>
        /// <param name="loadStartupLanguage">Is startup <see cref="KnotLanguageData"/> should be loaded instantly?</param>
        void SetDatabase(KnotDatabase database, bool loadStartupLanguage = false);

        /// <summary>
        /// Loads provided <see cref="KnotLanguageData"/>
        /// </summary>
        void LoadLanguage(KnotLanguageData languageData);

        /// <summary>
        /// Returns <see cref="IKnotText"/> with given <paramref name="key"/> from <see cref="TextController"/>
        /// </summary>
        IKnotText GetTextValue(string key);

        /// <summary>
        /// Returns <see cref="IKnotAsset"/> with given <paramref name="key"/> from <see cref="AssetController"/>
        /// </summary>
        IKnotAsset GetAssetValue(string key);
    }
}