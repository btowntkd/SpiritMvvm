## Project Description
SpiritMVVM is a Model-View-ViewModel library, focused on richness of feature-set and cross-platform compatibility, using the Portable Class Library. It was designed from the beginning to incorporate many modern features and "niceties" found scattered in other MVVM libraries, but only truly combined here.

## Project Goals
The SpiritMVVM library offers the following features and goals:

* A complete set of base-classes for implementing [View-Models](http://en.wikipedia.org/wiki/Model_View_ViewModel) or other observable objects. 
* A small collection of pre-built, generic View-Models for use with common MVVM scenarios.
* Classes to support decoupled messaging.
* Utility classes for monitoring and raising [INotifyPropertyChanged](http://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged.aspx) events. 
* Utility classes for wrapping and accessing values from an underlying Model. 
* An attribute-based dependency-mapping solution for PropertyChanged event notifications.
* A fluent-syntax-based dependency-mapping solution for PropertyChanged event notifications.
* Asynchronous options for notification or validation functionality.
* Very pluggable: injection-oriented classes make it easy to incorporate third-party frameworks like
[NInject](http://www.ninject.org/), [FluentValidation](https://github.com/JeremySkinner/FluentValidation), and [Reactive Extensions](http://msdn.microsoft.com/en-us/data/gg577609.aspx). 
* A completely unit-tested feature-set. 
* Cross-Platform compatibility through the [Portable Class Library](http://msdn.microsoft.com/en-us/library/gg597391.aspx).

## Supported Platforms
The following list identifies the currently-supported platforms of SpiritMVVM. This list is expected to grow as the Portable Class Library matures.

* Microsoft .NET 4.5
* .NET for Windows Store Apps (Windows 8)
* Windows Phone 8