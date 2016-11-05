# WPF-Auto-Update
A C# class that adds automatic updating to WPF applications.

Usage:
* Add AutoUpdater.cs, DownloadProgressControl.xaml, and DownloadProgressControl.xaml.cs to your project.
* Set the URIs in AutoUpdater.RemoteFileURI and AutoUpdater.ServiceURI (see comments).
* Call CheckCommandLineArgs() from the MainWindow constructor.
* Call CheckForUpdates() wherever, such as MainWindow.Loaded event.

Note: This doesn't work when adding AutoUpdater as a compiled DLL.  I don't know why yet.  I need to figure it out when I have time.
