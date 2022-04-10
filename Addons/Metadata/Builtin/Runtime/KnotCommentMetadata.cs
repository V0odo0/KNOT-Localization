using System;
using Knot.Localization.Attributes;
using UnityEngine;

namespace Knot.Localization.Data
{
    /// <summary>
    /// Editor-only comment metadata for informational purpose
    /// </summary>
    [Serializable]
    [KnotMetadataInfo("Comment", KnotMetadataInfoAttribute.MetadataScope.All, false, true)]
    public class KnotCommentMetadata : IKnotMetadata
    {
        public string Comment
        {
            get => _comment;
            set => _comment = value;
        }
        [SerializeField, TextArea(0, 5)] private string _comment;


        public KnotCommentMetadata() { }

        public KnotCommentMetadata(string comment)
        {
            Comment = comment;
        }


        public object Clone() => new KnotCommentMetadata(Comment);
    }
}