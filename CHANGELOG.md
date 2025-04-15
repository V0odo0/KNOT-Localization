## 1.4.3 (2025-04-15)

* Fixed Add Key button not accessible in layout

## 1.4.2 (2025-01-12)

* Fixed Text/Asset search filter update not framing selected items in tree view

## 1.4.1 (2024-11-19)

* Fixed Database Editor window was forced to open after each Assembly Reload

## 1.4.0 (2024-11-08)

* New: Plural Forms
* Added KNOT_LOCALIZATION_DISABLE_EXTENSIONS define symbol. You can define it to get rid of project-wide extension methods
* Updated Samples/Demo

## 1.3.2 (2024-10-25)

* KnotTextKeyReference & KnotAssetReference ToString impl

## 1.3.0 (2024-09-27)

* [breaking change] Asset Store - Moving common parts of code to Knot.Core assembly. Please, delete asset folder before updating
* [breaking change] UPM (GitHub) - Moving common parts of code to Knot.Core assembly. Please, use scope registry installation method
* Migrating to Unity 2021.3+
* Tests assembly moved to Package's Samples
* Removed About window. Links are moved to package description

* Fixed IKnotManager.SetDatabase controller callbacks transfer
* Fixed NullReferenceException on Database asset deletion

## 1.2.5 (2024-08-23)

* Fixed KnotTypePicker'r attribute drawer not responding to property changes

## 1.2.4 (2024-08-12)

* Added Localize extension method for string type

## 1.2.3 (2024-05-02)

* IKnotCultureSpecificMetadata is now marked as Obsolete. Instead, the culture will be provided to IKnotTextFormatterMetadata new implementation
* Fixed LocalizedText component's F (Formatters) button not responding in Unity 2022+

## 1.2.2 (2023-11-03)

* Fixed disabled TMPro addon for Unity 2023.2+ 

## 1.2.1 (2023-10-15)

* Editor menu path generalization

## 1.2.0 (2023-06-27)

* New Addon (Experimental): CSV Import Export
* New Text Formatter Metadata - Localized Text Override
* New Text Formatter Metadata - Replace with Localized Text
* Fixed Component's Text Formatters not being applied if Database has been loaded after Localized Component initialization
* Fixed incorrect database search logic resulting in an empty list
* Fixed Addressables validation check

## 1.1.0 (2023-04-01)

* New Addon: Open AI Autotranslator

## 1.0.1 (2023-03-30)

* KnotTextKeyReference is now have it's own Text Formatters metadata. You can add them in inspector by clicking [F] button 
* New Text Formatter Metadata - Prefix
* New Text Formatter Metadata - Postfix
* New Text Formatter Metadata - Replace
* Added DISABLE_KNOT_LOCALIZATION define symbol. You can define it to completely disable KNOT Localization for specific build target

* [breaking change] IKnotTextFormatterMetadata is now processing StringBuilder instead of string
* Better KnotTypePickerDrawer for SerializedReference

* Fixed Addressables error: Attempting to load AssetReference that has already been loaded
* Fixed key search behaviour in Database Editor
* Fixed error "An element can track properties on only one serializedObject at a time"

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
