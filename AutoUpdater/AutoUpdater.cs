using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoUpdater
{
    public static class AutoUpdater
    {
        // The absolute URI of the remote EXE file whose version will be compared to the executing assembly's version.
        public static string RemoteFileURI { get; set; }
        // The URI that will respond to an HTTP GET request with the server file's assembly version (e.g. "1.3.0").  It must be parsable by Version.Parse().
        public static string ServiceURI { get; set; }

        public static string AppName
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            }
        }
        public static string FileName
        {
            get
            {
                return System.IO.Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }
        // Call this from the MainWindow constructor.
        public static void CheckCommandLineArgs()
        {
            if (ServiceURI == null || RemoteFileURI == null)
            {
                throw new Exception("AutoUpdater - RemoteFileURI and ServiceURI must be set.");
            }
            var success = false;
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && System.IO.File.Exists(args[1]))
            {
                var startTime = DateTime.Now;
                while (DateTime.Now - startTime < TimeSpan.FromSeconds(30) && !success)
                {
                    try
                    {
                        System.IO.File.Copy(System.Reflection.Assembly.GetExecutingAssembly().Location, args[1], true);
                        success = true;
                    }
                    catch
                    {
                        success = false;
                    }
                    System.Threading.Thread.Sleep(500);
                }

                if (success == false)
                {
                    MessageBox.Show("Update failed.  Please close all " + AppName + " windows, then try again.", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Update successful!  " + AppName + " will now restart.", "Update Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                    Process.Start(args[1]);
                }
                Application.Current.Shutdown();
                return;
            }
        }
        // Call on MainWindow.Loaded event to check on start-up.
        public static async Task CheckForUpdates(bool Silent)
        {
            if (ServiceURI == null || RemoteFileURI == null)
            {
                throw new Exception("AutoUpdater - RemoteFileURI and ServiceURI must be set.");
            }
            System.Net.WebClient webClient = new System.Net.WebClient();
            System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
            var result = await httpClient.GetAsync(ServiceURI);
            var strVersion = await result.Content.ReadAsStringAsync();
            MessageBox.Show("Parsing version " + strVersion);
            var serverVersion = Version.Parse(strVersion);
            var thisVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            MessageBox.Show(String.Format("Comparing {0} to {1}.", serverVersion, thisVersion));
            if (serverVersion > thisVersion)
            {
                var strFilePath = System.IO.Path.GetTempPath() + FileName;
                var dialogResult = System.Windows.MessageBox.Show("A new version of " + AppName + " is available!  Would you like to download it now?  It's a no-fuss, instant process.", "New Version Available", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    if (System.IO.File.Exists(strFilePath))
                    {
                        System.IO.File.Delete(strFilePath);
                    }
                    try
                    {
                        var windowProgress = new System.Windows.Window();
                        windowProgress.DragMove();
                        windowProgress.Height = 150;
                        windowProgress.Width = 400;
                        windowProgress.WindowStyle = WindowStyle.None;
                        windowProgress.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                        var progressControl = new DownloadProgressControl();
                        windowProgress.Content = progressControl;
                        webClient.DownloadProgressChanged += (sender, args) => {
                            progressControl.progressBar.Value = args.ProgressPercentage;
                        };
                        windowProgress.Show();
                        await webClient.DownloadFileTaskAsync(new Uri(RemoteFileURI), strFilePath);

                        windowProgress.Close();
                    }
                    catch
                    {
                        if (!Silent)
                        {
                            MessageBox.Show("Unable to contact the server.  Check your network connection or try again later.", "Server Unreachable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        return;
                    }
                    Process.Start(strFilePath, "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"");
                    Application.Current.Shutdown();
                    return;
                }
            }
            else
            {
                if (!Silent)
                {
                    MessageBox.Show(AppName + " is up-to-date.", "No Updates", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
