using System.Globalization;
using Knot.Localization.Data;

namespace Knot.Localization
{
    /// <summary>
    /// Data container that is used as input for controller <see cref="IKnotController{TItemData,TValue,TValueType}.BuildAsync"/> method
    /// </summary>
    /// <typeparam name="TItemData"></typeparam>
    public class KnotControllerBuildData<TItemData> where TItemData : KnotItemData
    {
        /// <summary>
        /// Language <see cref="CultureInfo"/>
        /// </summary>
        public CultureInfo Culture = CultureInfo.InvariantCulture;

        /// <summary>
        /// <see cref="IKnotMetadata"/> collection that is assigned to <see cref="KnotDatabase"/>
        /// </summary>
        public IKnotMetadata[] GlobalMetadata = new IKnotMetadata[0];

        /// <summary>
        /// <see cref="IKnotMetadata"/> collection that is assigned to <see cref="KnotLanguageData"/>
        /// </summary>
        public IKnotMetadata[] LanguageMetadata = new IKnotMetadata[0];

        /// <summary>
        /// <see cref="KnotItemData"/> collection
        /// </summary>
        public TItemData[] ItemData = new TItemData[0];

        /// <summary>
        /// <see cref="KnotKeyData"/> collection
        /// </summary>
        public KnotKeyData[] KeyData = new KnotKeyData[0];
    }
}