using System;
using System.Collections;
using System.Collections.Generic;
using Knot.Localization.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Knot.Localization.Demo
{
    public class KnotDemoPluralFormSetter : MonoBehaviour
    {
        [SerializeField] private KnotTextKeyReference _keyReference;
        [SerializeField] private Slider _countSlider;
        [SerializeField] private Text _countText;

        void Awake()
        {
            _countSlider.onValueChanged.AddListener(arg0 =>
            {
                UpdateCountText();
            });
            _countSlider.value = 1;
        }

        void OnEnable()
        {
            _keyReference.ValueUpdated += KeyReferenceOnValueUpdated;
        }

        void OnDisable()
        {
            _keyReference.ValueUpdated -= KeyReferenceOnValueUpdated;
        }

        void KeyReferenceOnValueUpdated(string obj)
        {
            UpdateCountText();
        }

        void UpdateCountText()
        {
            var count = Convert.ToInt32(_countSlider.value);
            switch (count)
            {
                default:
                    _countText.text = $"{count} {_keyReference.GetValue(KnotPluralForm.Many)}";
                    break;
                case 1:
                    _countText.text = $"{count} {_keyReference.GetValue(KnotPluralForm.One)}";
                    break;
            }

        }
    }
}
