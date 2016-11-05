# WPF-Auto-Update
A C# class that adds automatic updating to WPF applications.

Usage:
* Add AutoUpdater.dll to your project references.
  * Hint: Use Fody/Costura (https://github.com/Fody/Costura) to seamlessly embed DLLs into your EXE!
* Set the URIs in AutoUpdater.RemoteFileURI and AutoUpdater.ServiceURI (see comments).
* Call CheckCommandLineArgs() from the MainWindow constructor.
* Call CheckForUpdates() wherever, such as MainWindow.Loaded event.
