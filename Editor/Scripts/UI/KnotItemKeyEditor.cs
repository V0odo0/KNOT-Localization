using System.Collections;
using System.Collections.Generic;
using Knot.Localization.Data;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public class KnotItemKeyEditor
    {
        public readonly TextField TextField;


        private readonly VisualElement _textInputElement;
        

        public KnotItemKeyEditor(TextField textField)
        {
            TextField = textField;
            TextField.isDelayed = true;

            _textInputElement = TextField.Q("unity-text-input");
        }


        public void FocusInput()
        {
            TextField.SelectAll();
            _textInputElement.Focus();
        }

        public void Bind(SerializedObject obj, string bindingPath)
        {
            if (obj != null && !string.IsNullOrEmpty(bindingPath))
            {
                TextField.bindingPath = bindingPath;
                TextField.BindProperty(obj);
            }
        }
    }
}

