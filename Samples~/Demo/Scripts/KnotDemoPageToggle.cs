#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

namespace Knot.Localization.Demo
{
    [RequireComponent(typeof(Toggle))]
    public class KnotDemoPageToggle : MonoBehaviour
    {
        public Toggle Toggle => _toggle ?? (_toggle = GetComponent<Toggle>());
        private Toggle _toggle;

        [SerializeField] private Text _pageIdText;

        
        void OnEnable()
        {
            Toggle.onValueChanged.AddListener(UpdateToggle);
            UpdateToggle(false);
        }

        void OnDisable()
        {
            Toggle.onValueChanged.RemoveListener(UpdateToggle);
        }

        void UpdateToggle(bool selected)
        {
            _pageIdText.color = selected ? Color.white : Color.black;
        }


        public void SetId(int id) => _pageIdText.text = (id + 1).ToString();
    }
}