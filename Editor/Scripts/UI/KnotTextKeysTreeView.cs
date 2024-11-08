using System;
using System.Linq;
using Knot.Core.Editor;
using Knot.Localization.Data;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Knot.Localization.Editor
{
    public sealed class KnotTextKeysTreeView : KnotKeysTreeView<KnotTextKeyView>
    {
        public event Action<KnotTextKeyView[], KnotPluralForm> RequestCreatePluralFormKey;

        public static IKnotKeyViewLabel<KnotTextKeyView>[] Labels
        {
            get
            {
                return _labels ?? (_labels = typeof(IKnotKeyViewLabel<KnotTextKeyView>).GetDerivedTypesInfo()
                    .Select(t => t.GetInstance()).OfType<IKnotKeyViewLabel<KnotTextKeyView>>().ToArray());
            }
        }
        private static IKnotKeyViewLabel<KnotTextKeyView>[] _labels;


        protected override IKnotKeyViewLabel<KnotTextKeyView>[] KeyViewLabels => Labels;


        public KnotTextKeysTreeView() : base(EditorUtils.UserSettings.TextKeysTreeViewState)
        {

        }


        protected override void PrebuildGenericMenu(GenericMenu menu)
        {
            foreach (KnotPluralForm pluralForm in Enum.GetValues(typeof(KnotPluralForm)))
                menu.AddItem(new GUIContent($"Add Plural Form/{pluralForm.ToString()}"),
                    false, () => RequestCreatePluralFormKeySelected(pluralForm));
        }

        void RequestCreatePluralFormKeySelected(KnotPluralForm pluralForm)
        {
            if (!SelectedKeyItems.Any())
                return;

            RequestCreatePluralFormKey?.Invoke(SelectedKeyViews.ToArray(), pluralForm);
        }
    }
}