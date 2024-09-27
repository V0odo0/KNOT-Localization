using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Knot.Localization.Components
{
    [Serializable]
    [AddComponentMenu("")]
    public class KnotStringTargetMethod : KnotTargetMethod<string>
    {
        public KnotStringTargetMethod() { }

        public KnotStringTargetMethod(Object obj, string methodName) : base(obj, methodName) { }
    }
}