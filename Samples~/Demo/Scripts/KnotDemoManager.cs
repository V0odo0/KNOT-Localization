#pragma warning disable 0649

using System.Collections.Generic;
using System.Linq;
using Knot.Localization.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Knot.Localization.Demo
{
    public class KnotDemoManager : MonoBehaviour
    {
        [SerializeField] private RectTransform _pageTogglesRoot;
        [SerializeField] private ToggleGroup _pageToggleGroup;
        [SerializeField] private RectTransform _pagesRoot;
        [SerializeField] private Dropdown _languageDropdown;

        [Space]
        [SerializeField] private KnotDemoPage[] _pagesSrc;
        [SerializeField] private KnotDemoPageToggle _pageToggleSrc;
        [SerializeField] private KnotDatabase _sourceDatabase;

        private KnotDemoPage _currentPage;
        private List<KnotDemoPage> _pages = new List<KnotDemoPage>();
        private List<KnotDemoPageToggle> _pageToggles = new List<KnotDemoPageToggle>();
        

        void Awake()
        {
            if (KnotLocalization.ProjectSettings.DefaultDatabase != _sourceDatabase || !KnotLocalization.ProjectSettings.LoadOnStartup)
                KnotLocalization.Manager.SetDatabase(_sourceDatabase, true);

            SetupPages();
        }

        void OnEnable()
        {
            UpdateLanguageDropdown();
            _languageDropdown.onValueChanged.AddListener(OnLanguageSelected);
        }

        void OnDisable()
        {
            _languageDropdown.onValueChanged.RemoveListener(OnLanguageSelected);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                ReloadScene();
        }

        void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        void SetupPages()
        {
            for (int i = 0; i < _pagesSrc.Length; i++)
            {
                int pageId = i;

                KnotDemoPage newPage = Instantiate(_pagesSrc[i]).GetComponent<KnotDemoPage>();
                newPage.gameObject.SetActive(false);
                newPage.GetComponent<RectTransform>().SetParent(_pagesRoot, false);
                _pages.Add(newPage);

                KnotDemoPageToggle newPageToggle = Instantiate(_pageToggleSrc).GetComponent<KnotDemoPageToggle>();
                newPageToggle.SetId(pageId);
                newPageToggle.GetComponent<RectTransform>().SetParent(_pageTogglesRoot, false);
                newPageToggle.Toggle.group = _pageToggleGroup;
                newPageToggle.Toggle.SetIsOnWithoutNotify(false);
                newPageToggle.Toggle.onValueChanged.AddListener(arg0 => SetPage(pageId));
                _pageToggles.Add(newPageToggle);
            }
            
            _pageToggles.First().Toggle.isOn = true;
        }

        void SetPage(int pageId)
        {
            if (!_pages.Any())
                return;

            pageId = Mathf.Clamp(pageId, 0, _pages.Count - 1);
            
            if (_currentPage != null)
                _currentPage.Hide();
            _currentPage = _pages[pageId];
            _currentPage.Show();
        }
        
        void UpdateLanguageDropdown()
        {
            _languageDropdown.ClearOptions();
            _languageDropdown.AddOptions(KnotLocalization.Manager.Languages.Select(d => d.NativeName).ToList());

            int selectedLanguageId =
                KnotLocalization.Manager.Languages.IndexOf(KnotLocalization.Manager.SelectedLanguage);

            if (selectedLanguageId >= 0)
                _languageDropdown.SetValueWithoutNotify(selectedLanguageId);
        }


        void OnLanguageSelected(int langId)
        {
            KnotLocalization.Manager.LoadLanguage(KnotLocalization.Manager.Languages[langId]);
        }
    }
}