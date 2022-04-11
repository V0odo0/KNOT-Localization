using System;
using System.Collections.Generic;
using System.Linq;
using Knot.Localization.Attributes;
using Knot.Localization.Data;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine;

namespace Knot.Localization.Editor
{
    public abstract class KnotKeyViewEditorPanel<TKeyView, TItemView, TItemData> : KnotEditorPanel
        where TKeyView : KnotKeyView<TItemView, TItemData>
        where TItemView : KnotItemView<TItemData>
        where TItemData : KnotItemData
    {
        protected static GUIContent AddToKeyCollectionContent =>
            _addToKeyCollectionContent ?? (_addToKeyCollectionContent = new GUIContent("Add to Key Collection"));
        private static GUIContent _addToKeyCollectionContent;

        public event Action<string, string> KeyChanged;
        public event Action ItemChanged;
        public event Action<KnotKeyCollection, TKeyView> AddToKeyCollection;

        public TKeyView KeyView => KeyViews.FirstOrDefault();
        public IReadOnlyList<TKeyView> KeyViews { get; protected set; } = new TKeyView[0];

        public readonly TextField KeyInputField;
        public readonly VisualElement ItemViewListContainer;
        public readonly Foldout MetadataContainerFoldout;
        public readonly KnotMetadataContainerEditor MetadataContainerEditor;

        protected abstract KnotMetadataInfoAttribute.MetadataScope MetadataScope { get; }
        protected abstract List<KnotKeyCollection> KeyCollections { get; }

        protected HashSet<KnotItemViewEditor<TKeyView, TItemView, TItemData>> VisibleItemViewEditors = new HashSet<KnotItemViewEditor<TKeyView, TItemView, TItemData>>();


        protected KnotKeyViewEditorPanel() : base("KnotKeyViewEditorPanel")
        {
            var scrollView = Root.Q<ScrollView>();
            if (scrollView != null)
                scrollView.viewDataKey = $"{GetType().Name}.ScrollView";

            KeyInputField = Root.Q<TextField>(nameof(KeyInputField));
            KeyInputField.isDelayed = true;
            KeyInputField.RegisterValueChangedCallback(evt =>
            {
                if (!RenameKey(evt.previousValue, evt.newValue))
                    KeyInputField.SetValueWithoutNotify(evt.previousValue);
                else KeyChanged?.Invoke(evt.previousValue, evt.newValue);
            });

            ItemViewListContainer = Root.Q(nameof(ItemViewListContainer));

            MetadataContainerFoldout = Root.Q<Foldout>(nameof(MetadataContainerFoldout));
            MetadataContainerEditor = new KnotMetadataContainerEditor(MetadataContainerFoldout, MetadataScope, null, OnMetadataChanged);
            MetadataContainerEditor.MetadataContainer.onGUIHandler += () =>
            {
                if (KeyView.KeyData == null)
                {
                    if (KeyCollections.Any(c => c != null && c.IsPersistent()))
                    {
                        bool isMultipleCollections = KeyCollections.Distinct().Count(c => c != null) > 1;
                        bool add = isMultipleCollections
                            ? EditorGUILayout.DropdownButton(AddToKeyCollectionContent, FocusType.Passive)
                            : GUILayout.Button(AddToKeyCollectionContent);

                        if (add)
                        {
                            if (isMultipleCollections)
                            {
                                GenericMenu m = new GenericMenu();
                                foreach (var keyCollection in KeyCollections.Where(c => c != null).Distinct())
                                {
                                    m.AddItem(new GUIContent(keyCollection.name), false, () =>
                                    {
                                        AddToKeyCollection?.Invoke(keyCollection, KeyView);
                                    });
                                }

                                m.ShowAsContext();
                            }
                            else AddToKeyCollection?.Invoke(KeyCollections.First(), KeyView);
                        }
                    }
                    else
                    {
                        GUILayout.Label("No Key Collection assigned");
                    }
                    
                }
            };
        }


        protected virtual void BuildItemViewFoldouts(TKeyView keyView)
        {
            ItemViewListContainer.Clear();
            VisibleItemViewEditors.Clear();

            foreach (var lang in Database.Languages)
            {
                string foldoutName = lang.CultureInfo.GetDisplayName();
                KnotItemViewFoldout foldout = new KnotItemViewFoldout(foldoutName, icon:KnotEditorUtils.GetIcon(KnotLanguagesTabPanel.LanguageIconName))
                {
                    IsActive = false
                };

                KnotItemCollection[] collections = lang.CollectionProviders.OfType<IKnotPersistentItemCollectionProvider>().
                    Select(p => p.Collection).Distinct().Where(c => c is IKnotItemCollection<TItemData>).ToArray();

                if (!collections.Any())
                {
                    foldout.ButtonState = KnotItemViewFoldout.FoldoutButtonState.None;
                    foldout.StateLabelText = "No Item Collection assigned";
                }
                else
                {
                    TItemView itemView = keyView.LanguageItems.ContainsKey(lang) ?
                        keyView.LanguageItems[lang].FirstOrDefault(item => collections.Contains(item.SourceAsset)) :
                        keyView.LanguageItems.Values.SelectMany(list => list).FirstOrDefault(item => collections.Contains(item.SourceAsset));
                    
                    if (itemView != null)
                    {
                        foldout.SetReadOnly(itemView.SourceCollection?.IsReadOnly ?? true);
                        foldout.IsActive = true;
                        foldout.ButtonState = KnotItemViewFoldout.FoldoutButtonState.Remove;
                        foldout.StateLabelText = itemView.SourceAsset.Name;
                        foldout.StateButtonClicked += state =>
                        {
                            if (itemView.SourceCollection is IKnotItemCollection<TItemData> tCollection && RemoveItem(tCollection))
                                OnItemChanged();
                        };

                        var itemViewEditor = CreateItemViewEditor(keyView, itemView);
                        foldout.RequestAddContent += () =>
                        {
                            itemViewEditor.Bind(keyView, itemView);
                            VisibleItemViewEditors.Add(itemViewEditor);
                            return itemViewEditor;
                        };
                        foldout.RequestRemoveContent += () =>
                        {
                            VisibleItemViewEditors.Remove(itemViewEditor);
                        };
                    }
                    else
                    {
                        if (collections.Length == 1)
                        {
                            IKnotItemCollection<TItemData> itemCollection =
                                collections.First() as IKnotItemCollection<TItemData>;

                            foldout.SetReadOnly(itemCollection == null || itemCollection.IsReadOnly);
                            foldout.ButtonState = KnotItemViewFoldout.FoldoutButtonState.Add;
                            foldout.StateButtonClicked += state =>
                            {
                                if (itemCollection != null && AddItem(itemCollection))
                                {
                                    foldout.Root.value = true;
                                    OnItemChanged();
                                }
                            };
                        }
                        else
                        {
                            foldout.ButtonState = KnotItemViewFoldout.FoldoutButtonState.AddContextMenu;
                            foldout.StateButtonClicked += state =>
                            {
                                GenericMenu menu = new GenericMenu();
                                foreach (var collection in collections)
                                {
                                    var itemCollection = collection as IKnotItemCollection<TItemData>;
                                    var nameContent = new GUIContent(collection.Name);

                                    void AddItemInternal()
                                    {
                                        if (AddItem(itemCollection))
                                        {
                                            foldout.Root.value = true;
                                            OnItemChanged();
                                        }
                                    }

                                    if (itemCollection != null && !itemCollection.IsReadOnly)
                                        menu.AddItem(nameContent, false, AddItemInternal);
                                    else menu.AddDisabledItem(nameContent, false);
                                    
                                }
                                menu.ShowAsContext();
                            };
                        }
                    }
                }

                ItemViewListContainer.Add(foldout);
            }
        }

        protected virtual KnotItemViewEditor<TKeyView, TItemView, TItemData> CreateItemViewEditor(TKeyView keyView, TItemView itemView)
        {
            var editor = GetNewItemViewEditor(keyView, itemView);

            editor.ValueChanged += OnItemChanged;

            return editor;
        }


        protected abstract TItemData GetNewItem(TKeyView keyView);

        protected abstract KnotItemViewEditor<TKeyView, TItemView, TItemData> GetNewItemViewEditor(TKeyView keyView, TItemView itemView);



        protected virtual bool RenameKey(string oldKey, string newKey)
        {
            if (string.IsNullOrEmpty(newKey) || oldKey == newKey || KeyCollections.Any(c => c.ContainsKey(newKey)))
                return false;

            if (KeyView.LanguageItems.Values.Any(list => list.Any(itemView => itemView.ItemData.Key == newKey)))
                return false;

            var collectionAssets = KeyView.LanguageItems.Values.SelectMany(list => list).Select(itemView => itemView.SourceAsset as UnityEngine.Object);
            KnotEditorUtils.RegisterCompleteObjects("Rename Key", () =>
            {
                if (KeyView.SourceCollection != null && KeyView.SourceCollection.ContainsKey(oldKey))
                    KeyView.SourceCollection[oldKey].Key = newKey;

                foreach (var itemView in KeyView.LanguageItems.Values.SelectMany(list => list))
                    itemView.ItemData.Key = newKey;

            }, collectionAssets.Append(KeyView.SourceCollection).ToArray());

            return true;
        }

        protected virtual bool AddItem(IKnotItemCollection<TItemData> collection)
        {
            if (collection.Any(item => item.Key == KeyView.Key))
                return false;

            KnotEditorUtils.RegisterCompleteObjects("Add Text Item", () =>
            {
                collection.Add(GetNewItem(KeyView));
            }, collection as UnityEngine.Object);

            return true;
        }

        protected virtual bool RemoveItem(IKnotItemCollection<TItemData> collection)
        {
            var itemToRemove = collection.FirstOrDefault(item => item.Key == KeyView.Key);
            if (itemToRemove == null)
                return false;

            KnotEditorUtils.RegisterCompleteObjects("Remove Text Item", () =>
            {
                collection.Remove(itemToRemove);
            }, collection as UnityEngine.Object);

            return true;
        }

        protected virtual void OnItemChanged()
        {
            ItemChanged?.Invoke();
        }

        protected virtual void OnMetadataChanged()
        {
            foreach (var editor in VisibleItemViewEditors)
            {
                editor.ReloadLayout();
            }
        }


        public virtual void Bind(params TKeyView[] keyViews)
        {
            KeyViews = keyViews;
            if (keyViews.Length == 0 || KeyViews.Count > 1)
            {
                Root.parent?.Remove(Root);
                return;
            }

            KeyInputField.SetValueWithoutNotify(KeyView.Key);

            BuildItemViewFoldouts(KeyView);

            SerializedProperty metadataContainerProp = null;

            if (KeyView.KeyData != null)
            {
                var collectionObj = new SerializedObject(KeyView.SourceCollection);
                int index = KeyView.SourceCollection.IndexOf(KeyView.KeyData);

                if (index >= 0)
                    metadataContainerProp = collectionObj.FindProperty("_keyData")
                        .GetArrayElementAtIndex(index).FindPropertyRelative("_metadata");
            }

            MetadataContainerEditor.Bind(metadataContainerProp);
            MetadataContainerEditor.DrawMetadataList = KeyView.KeyData != null && KeyView.SourceCollection.IsPersistent();
        }
    }
}