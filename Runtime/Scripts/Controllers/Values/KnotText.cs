using System.Collections.Generic;
using System.Linq;
using System.Text;
using Knot.Localization.Data;

namespace Knot.Localization
{
    public class KnotText : IKnotText
    {
        public string Value { get; private set; }

        public IList<IKnotMetadata> Metadata => _metadata;
        private List<IKnotMetadata> _metadata = new List<IKnotMetadata>();


        public KnotText() { }

        public KnotText(string value, params IKnotMetadata[] metadata)
        {
            Value = value;
            _metadata.AddRange(metadata);

            ForceUpdateValue();
        }


        public void ForceUpdateValue()
        {
            foreach (var formatter in Metadata.OfType<IKnotTextFormatterMetadata>())
                Value = formatter.Format(Value);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Value: ");
            sb.Append(Value);

            if (Metadata.Count > 0)
            {
                sb.AppendLine("Metadata Types: ");
                foreach (var metadata in Metadata)
                    sb.Append(metadata?.GetType().Name ?? "Null");
            }

            return sb.ToString();
        }


        public static implicit operator string(KnotText text) => text.Value;
    }
}
