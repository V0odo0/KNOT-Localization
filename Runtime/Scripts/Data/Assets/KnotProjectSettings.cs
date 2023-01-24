#pragma warning disable CS0649

using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization
{
    /// <summary>
    /// General project settings asset that keeps references to default <see cref="KnotDatabase"/> and <see cref="IKnotManager"/>.
    /// </summary>
    public class KnotProjectSettings : ScriptableObject
    {
        internal static KnotProjectSettings Empty => _empty ?? (_empty = CreateInstance<KnotProjectSettings>());
        private static KnotProjectSettings _empty;

        /// <summary>
        /// Default <see cref="KnotDatabase"/> that will be passed to <see cref="IKnotManager.SetDatabase"/>
        /// by <see cref="KnotLocalization"/> on <see cref="RuntimeInitializeLoadType.AfterAssembliesLoaded"/> during runtime
        /// if <see cref="LoadOnStartup"/> is enabled.
        /// </summary>
        public KnotDatabase DefaultDatabase => _defaultDatabase ?? KnotDatabase.Empty;
        [SerializeField, KnotCreateAssetField(typeof(KnotDatabase))] private KnotDatabase _defaultDatabase;

        /// <summary>
        /// <see cref="IKnotManager"/> that will be initialized by <see cref="KnotLocalization"/>
        /// on <see cref="RuntimeInitializeLoadType.AfterAssembliesLoaded"/> during runtime if <see cref="LoadOnStartup"/> is enabled.
        /// </summary>
        public IKnotManager Manager => _manager ?? (_manager = new KnotManager());
        [SerializeReference, KnotTypePicker(typeof(IKnotManager))] private IKnotManager _manager = new KnotManager();
        
        /// <summary>
        /// If enabled, <see cref="Manager"/> will be initialized by <see cref="KnotLocalization"/>
        /// on <see cref="RuntimeInitializeLoadType.AfterAssembliesLoaded"/> during runtime.
        /// </summary>
        public bool LoadOnStartup => _loadOnStartup;
        [SerializeField] private bool _loadOnStartup = true;
    }
}