using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Knot.Localization.Data;
using UnityEngine;

namespace Knot.Localization
{
    public class KnotText : IKnotText
    {
        public static StringBuilder SharedStringBuilder = new StringBuilder();

        public string Value { get; private set; }
        private string _rawValue;

        public IList<IKnotMetadata> Metadata => _metadata;
        private List<IKnotMetadata> _metadata = new List<IKnotMetadata>();


        public KnotText() { }

        public KnotText(string value, params IKnotMetadata[] metadata)
        {
            Value = value;
            _rawValue = value;
            _metadata.AddRange(metadata);

            ForceUpdateValue();
        }


        public void ForceUpdateValue()
        {
            Value = Format(_rawValue, Metadata.OfType<IKnotTextFormatterMetadata>());
        }

        public override string ToString()
        {
            lock (SharedStringBuilder)
            {
                SharedStringBuilder.Clear();

                SharedStringBuilder.Append("Value: ");
                SharedStringBuilder.Append(Value);

                if (Metadata.Count > 0)
                {
                    SharedStringBuilder.AppendLine("Metadata Types: ");
                    foreach (var metadata in Metadata)
                        SharedStringBuilder.Append(metadata?.GetType().Name ?? "Null");
                }

                return SharedStringBuilder.ToString();
            }
        }

        public static string Format(string inputString, IEnumerable<IKnotTextFormatterMetadata> formatters)
        {
            lock (SharedStringBuilder)
            {
                SharedStringBuilder.Clear();
                SharedStringBuilder.Append(inputString);

                foreach (var f in formatters)
                {
                    if (f is IKnotCultureSpecificMetadata cultureSpecificMetadata)
                        cultureSpecificMetadata.SetCulture(KnotLocalization.Manager.SelectedLanguage.CultureInfo);

                    f?.Format(SharedStringBuilder);
                }

                return SharedStringBuilder.ToString();
            }
        }

        public static implicit operator string(KnotText text) => text.Value;
    }
}
