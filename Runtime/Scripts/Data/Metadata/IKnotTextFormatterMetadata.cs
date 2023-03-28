using System.Text;

namespace Knot.Localization.Data
{
    /// <summary>
    /// An interface that should implement text <see cref="Format"/> logic.
    /// <see cref="Format"/> is called by <see cref="KnotText"/> to update <see cref="KnotText.Value"/>
    /// </summary>
    public interface IKnotTextFormatterMetadata : IKnotMetadata
    {
        void Format(StringBuilder sb);
    }
}