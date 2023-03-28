## 1.0.0 (2023-01-24)

* KnotTextKeyReference is now have it's own Text Formatters metadata. You can add them in inspector by clicking [F] button 
* New Text Formatter Metadata - Prefix
* New Text Formatter Metadata - Postfix
* New Text Formatter Metadata - Replace
* Added DISABLE_KNOT_LOCALIZATION define symbol. You can define it to completely disable KNOT Localization for specific build target

* [breaking change] IKnotTextFormatterMetadata is now processing StringBuilder instead of string
* Better KnotTypePickerDrawer for SerializedReference

* Fixed Addressables error: Attempting to load AssetReference that has already been loaded
* Fixed key search behaviour in Database Editor

## 0.9.3 (2023-01-24)

* Fixed missing Default Database reference after Library folder rebuilding

## 0.9.2 (2023-01-24)

* Fixed search field not responsable to changes
* Fixed KnotLocalizedTextMeshPro not applying custom font metadata

* KnotLocalizedTextMeshPro is now universal for all TextMeshPO text components

## 0.9.1 (2022-04-23)

* TextMeshPRO support
* Live Reload - applies all changes and reloads active language data in Play Mode
![LiveReload](https://user-images.githubusercontent.com/10213769/162785846-7615edc5-b27e-46c3-a7f3-8e447409791e.gif)
* [Addressables](https://docs.unity3d.com/Manual/com.unity.addressables.html) Item Collection Provider 

* Removed redundant KNOT_LOCALIZATION define symbol
* Key Reference Picker QOL changes
* Key View Labels feature has been temporary disabled
* Removed unnecessary "Reset User Settings" function from Project Settings
* IKnotController is now requires ICloneable interface implementation. Clone() must return a new controller instance that will be used in runtime by IKnotManager whenever SetDatabase() is called.

* Fixed NullReferenceException error caused by Key Reference Picker when fetching key list
* Fixed incorrect key selection highlight & expanded state after using search in Keys Tree View
* Fixed an issue with annoying "Null Database is provided or the same Database is already assigned to this Manager" warning when using Enter PLay Mode feature without Assembly Reload checked
* Fixed Reorderable List not responding to reorder action
