# WPF-Auto-Update
A C# class that provides automatic updating for WPF applications.

Namespace: WPF_Auto_Update

Special Note: WPF-Auto-Update uses the command line switch "-wpfautoupdate" for signaling.  If your app handles command line arguments, make sure that one is allowed.

Usage:
* Add WPF-Auto-Update.dll to your project references.
  * Hint: Use Fody/Costura (https://github.com/Fody/Costura) to seamlessly embed DLLs into your EXE!
* Set the URIs in Updater.RemoteFileURI and Updater.ServiceURI.
  * The RemoteFileURI property should be the absolute URI from where the latest EXE can be downloaded.
  * The ServiceURI property should be the absolute URI that will respond to a GET request with a string value of the latest version (e.g. "1.3.0.0").  It must be parsable by Version.Parse().
* Set the Updater.UpdateTimeout value, if desired.
  * This property specifies how long a downloaded update will attempt to replace the original file before timing out.  Use Duration.Forever to specify no timeout.  The default is 30 seconds.
* Call CheckCommandLineArgs() from the MainWindow constructor.
* Call CheckForUpdates() wherever, such as MainWindow.Loaded event.