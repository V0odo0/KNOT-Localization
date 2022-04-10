using UnityEngine;
using UnityEngine.UI;

namespace Knot.Localization.Demo
{
    [RequireComponent(typeof(InputField))]
    public class KnotDemoRuntimeTextInput : MonoBehaviour
    {
        public InputField InputField => _inputField ?? (_inputField = GetComponent<InputField>());
        private InputField _inputField;

        public string TargetKey
        {
            get => _targetKey;
            set
            {
                if (_targetKey == value)
                    return;

                _targetKey = value;
                OnTextChanged(InputField.text);
            }
        }
        [SerializeField] private string _targetKey;


        void OnEnable()
        {
            InputField.onValueChanged.AddListener(OnTextChanged);
        }

        void OnDisable()
        {
            InputField.onValueChanged.RemoveListener(OnTextChanged);
        }

        private void OnTextChanged(string text)
        {
            KnotLocalization.Manager.TextController.AddOverride(TargetKey, text);
        }
    }
}