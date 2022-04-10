using System;
using Knot.Localization.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Knot.Localization.Data
{
    [Serializable]
    public partial class KnotDatabaseSettings
    {
        public IKnotLanguageSelector LanguageSelector
        {
            get => _languageSelector ?? (_languageSelector = new KnotDefaultLanguageSelector());
            set => _languageSelector = value ?? new KnotDefaultLanguageSelector();
        }
        [SerializeReference, KnotTypePicker(typeof(IKnotLanguageSelector))] private IKnotLanguageSelector _languageSelector = new KnotDefaultLanguageSelector();

        public IKnotTextController TextController
        {
            get => _textController ?? (_textController = new KnotTextController());
            set => _textController = value ?? new KnotTextController();
        }
        [SerializeReference, KnotTypePicker(typeof(IKnotTextController))] private IKnotTextController _textController = new KnotTextController();

        public IKnotAssetController AssetController
        {
            get => _assetController ?? (_assetController = new KnotAssetController());
            set => _assetController = value ?? new KnotAssetController();
        }
        [SerializeReference, KnotTypePicker(typeof(IKnotAssetController))] private IKnotAssetController _assetController = new KnotAssetController();

        public KnotMetadataContainer Metadata => _metadata;
        [SerializeField] private KnotMetadataContainer _metadata = new KnotMetadataContainer();
    }
}