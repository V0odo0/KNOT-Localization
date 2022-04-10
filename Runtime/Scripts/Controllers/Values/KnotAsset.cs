using System.Collections.Generic;
using System.Linq;
using System.Text;
using Knot.Localization.Data;
using Object = UnityEngine.Object;

namespace Knot.Localization
{
    public class KnotAsset : IKnotAsset
    {
        public virtual Object Value { get; private set; }

        public virtual IList<IKnotMetadata> Metadata => _metadata;
        private List<IKnotMetadata> _metadata = new List<IKnotMetadata>();


        public KnotAsset() { }

        public KnotAsset(Object value, params IKnotMetadata[] metadata)
        {
            Value = value;
            _metadata.AddRange(metadata);

            ForceUpdateValue();
        }


        public virtual void ForceUpdateValue()
        {
            foreach (var formatter in Metadata.OfType<IKnotAssetFormatterMetadata>())
                Value = formatter.Format(Value);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Value: ");
            sb.Append(Value?.name ?? "Null");

            if (Metadata.Count > 0)
            {
                sb.AppendLine("Metadata Types: ");
                foreach (var metadata in Metadata)
                    sb.Append(metadata?.GetType().Name ?? "Null");
            }

            return sb.ToString();
        }


        public static implicit operator Object(KnotAsset asset) => asset.Value;
    }
}