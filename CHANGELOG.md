## 0.9.1-preview - 2022-04-11

### Added
* TextMeshPRO support
* Live Reload - applies all changes and reloads active language data in Play Mode
![LiveReload](https://user-images.githubusercontent.com/10213769/162785846-7615edc5-b27e-46c3-a7f3-8e447409791e.gif)

### Changed
* Removed redundant KNOT_LOCALIZATION define symbol
* Key Reference Picker QOL changes
* Key View Labels feature has been temporary disabled
* Removed unnecessary "Reset User Settings" function from Project Settings
* IKnotController is now requires ICloneable interface implementation. Clone() must return a new controller instance that will be used in runtime by IKnotManager whenever SetDatabase() is called.

### Fixed
* Fixed NullReferenceException error caused by Key Reference Picker when fetching key list
* Fixed incorrect key selection highlight & expanded state after using search in Keys Tree View
* Fixed an issue with annoying "Null Database is provided or the same Database is already assigned to this Manager" warning when using Enter PLay Mode feature without Assembly Reload checked
* Fixed Reorderable List not responding to reorder action
