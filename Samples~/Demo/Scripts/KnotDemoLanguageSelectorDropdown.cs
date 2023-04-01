using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Knot.Localization
{
    [RequireComponent(typeof(Dropdown))]
    public class KnotDemoLanguageSelectorDropdown : MonoBehaviour
    {
        public Dropdown Dropdown => _dropdown ?? (_dropdown = GetComponent<Dropdown>());
        private Dropdown _dropdown;

        void Awake()
        {
            Dropdown.ClearOptions();
            Dropdown.AddOptions(KnotLocalization.Manager.Languages.Select(l => l.NativeName).ToList());
            Dropdown.SetValueWithoutNotify(KnotLocalization.Manager.Languages.IndexOf(KnotLocalization.Manager.SelectedLanguage));
            Dropdown.onValueChanged.AddListener(arg0 =>
            {
                KnotLocalization.Manager.LoadLanguage(KnotLocalization.Manager.Languages[arg0]);
            });
        }
    }
}
