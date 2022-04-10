using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knot.Localization.Demo
{
    public class KnotDemoPage : MonoBehaviour
    {
        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}