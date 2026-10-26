#if KNOT_UITOOLKIT
using System;
using UnityEditor.UIElements;
using UnityEngine;

namespace Knot.Localization.Editor
{
    /// <summary>
    /// Writes <see cref="KnotTextKeyReference"/> to UXML as a plain key, or as JSON when it has formatters
    /// </summary>
    public class KnotTextKeyReferenceUxmlConverter : UxmlAttributeConverter<KnotTextKeyReference>
    {
        public override KnotTextKeyReference FromString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return new KnotTextKeyReference();

            if (value.TrimStart().StartsWith("{"))
            {
                try
                {
                    return JsonUtility.FromJson<KnotTextKeyReference>(value) ?? new KnotTextKeyReference();
                }
                catch (Exception e)
                {
                    KnotLocalization.Log($"Could not parse text key from UXML: {e.Message}", LogType.Warning);
                    return new KnotTextKeyReference();
                }
            }

            return new KnotTextKeyReference(value);
        }

        public override string ToString(KnotTextKeyReference value)
        {
            if (value == null)
                return string.Empty;

            return value.Formatters.Count == 0 ? value.Key ?? string.Empty : JsonUtility.ToJson(value);
        }
    }
}
#endif
