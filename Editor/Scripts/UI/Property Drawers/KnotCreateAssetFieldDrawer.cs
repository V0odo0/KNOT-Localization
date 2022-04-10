using System.Linq;
using System.Reflection;
using Knot.Localization.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Knot.Localization.Editor
{
    [CustomPropertyDrawer(typeof(KnotCreateAssetFieldAttribute))]
    public class KnotCreateAssetFieldDrawer : PropertyDrawer
    {
        protected const int BUTTON_WIDTH = 50;

        public KnotCreateAssetFieldAttribute Attribute => attribute as KnotCreateAssetFieldAttribute;

        protected GUIContent NewButtonContent => _newButtonContent ?? (_newButtonContent = new GUIContent("New"));
        private GUIContent _newButtonContent;

        protected virtual bool IsValidProperty(SerializedProperty property)
        {
            if (!Attribute.AssetType.IsSubclassOf(typeof(ScriptableObject)))
                return false;

            if (property.propertyType != SerializedPropertyType.ObjectReference)
                return false;

            return true;
        }

        protected virtual void CreateAsset(Rect atPos, SerializedProperty property)
        {
            var assetTypes = TypeCache.GetTypesDerivedFrom(Attribute.AssetType).Union(new []{ Attribute.AssetType })
                .Where(t => !t.IsAbstract && !t.IsGenericType).OrderBy(t => t.Name).ToArray();

            if (assetTypes.Length == 0)
                return;

            Object asset = null;
            if (assetTypes.Length == 1)
            {
                asset = KnotEditorUtils.RequestCreateAsset(assetTypes.First());
            }
            else
            {
                GenericMenu menu = new GenericMenu();
                foreach (var assetType in assetTypes)
                {
                    GUIContent label = new GUIContent(assetType.Name);
                    var typeInfo = assetType.GetCustomAttribute<KnotTypeInfoAttribute>();
                    if (typeInfo != null)
                        label.text = typeInfo.DisplayName;

                    menu.AddItem(label, false, () =>
                        {
                            asset = KnotEditorUtils.RequestCreateAsset(assetType);

                            if (asset == null)
                                return;

                            property.objectReferenceValue = asset;
                            property.serializedObject.ApplyModifiedProperties();
                        });
                }

                menu.DropDown(atPos);
            }

            if (asset == null)
                return;

            property.objectReferenceValue = asset;
            property.serializedObject.ApplyModifiedProperties();
        }


        public override VisualElement CreatePropertyGUI(SerializedProperty property)
            => property.GetFallbackPropertyGUI();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (!IsValidProperty(property))
            {
                EditorGUI.PropertyField(position, property, label);
                property.serializedObject.ApplyModifiedProperties();
                EditorGUI.EndProperty();
                return;
            }
            
            Rect objectFieldPos = position;
            if (property.objectReferenceValue == null)
            {
                objectFieldPos.width -= BUTTON_WIDTH;

                Rect buttonPos = objectFieldPos;
                buttonPos.x = objectFieldPos.x + objectFieldPos.width + 2;
                buttonPos.width = BUTTON_WIDTH - 2;
                if (EditorGUI.DropdownButton(buttonPos, NewButtonContent, FocusType.Keyboard))
                    CreateAsset(buttonPos, property);
            }

            EditorGUI.ObjectField(objectFieldPos, property, label);
            property.serializedObject.ApplyModifiedProperties();

            EditorGUI.EndProperty();
        }
    }
}