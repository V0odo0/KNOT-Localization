using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Knot.Localization.Data
{
    /// <summary>
    /// <see cref="IKnotMetadata"/> collection container used to split Runtime and Editor-only <see cref="IKnotMetadata"/> derived objects
    /// </summary>
    [Serializable]
    public class KnotMetadataContainer : IEnumerable<IKnotMetadata>
    {
        /// <summary>
        /// Runtime <see cref="IKnotMetadata"/> collection. Accessible either from build and editor
        /// </summary>
        public List<IKnotMetadata> Runtime
        {
            get => _runtime;
            set
            {
                if (value == null)
                    _runtime.Clear();
                else _runtime = value;
            }
        }
        [SerializeReference] private List<IKnotMetadata> _runtime = new List<IKnotMetadata>();

#if UNITY_EDITOR
        /// <summary>
        /// Editor-only <see cref="IKnotMetadata"/> collection that is not accessible nor included in build
        /// </summary>
        public List<IKnotMetadata> Editor
        {
            get => _editor;
            set
            {
                if (value == null)
                    _editor.Clear();
                else _editor = value;
            }
        }
        [SerializeReference] private List<IKnotMetadata> _editor = new List<IKnotMetadata>();
#endif

        public KnotMetadataContainer() { }

        public KnotMetadataContainer(KnotMetadataContainer other)
        {
            if (other == null)
                return;

            foreach (var runtime in other.Runtime)
                Runtime.Add((IKnotMetadata) runtime.Clone());

#if UNITY_EDITOR
            foreach (var editor in other.Editor)
                Editor.Add((IKnotMetadata) editor.Clone());
#endif
        }


        public IEnumerator<IKnotMetadata> GetEnumerator()
        {
#if UNITY_EDITOR
            return Runtime.Union(Editor).GetEnumerator();
#else
            return Runtime.GetEnumerator();
#endif
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}