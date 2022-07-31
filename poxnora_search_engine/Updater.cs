using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;

namespace poxnora_search_engine
{
    public delegate void OnGetVersion(bool is_current_outdated, string new_version);
    public delegate void OnGetArchiveFailed();
    public delegate void OnGetArchiveSuccess();
    public delegate void OnArchiveProgressChanged(int p);

    public class Updater
    {
        Uri VersionSource = new Uri("https://raw.githubusercontent.com/leszekd25/poxnora_search_engine/master/README.md");
        Uri VersionArchive = new Uri("https://github.com/leszekd25/poxnora_search_engine/raw/master/latest/poxnora%20search%20engine.zip");

        public OnGetVersion _OnGetVersion = null;
        public OnGetArchiveFailed _OnGetArchiveFailed = null;
        public OnGetArchiveSuccess _OnGetArchiveSuccess = null;
        public OnArchiveProgressChanged _OnArchiveProgressChanged = null;

        Thread DownloadVersionStringThread = null;

        System.Net.WebClient wc = new System.Net.WebClient();

        public Updater()
        {
            Log.Info(Log.LogSource.Net, "Updater.Updater() called");
            DeleteOldAssemblies();
            wc.DownloadDataCompleted += GetVersionArchive_completed;
            wc.DownloadProgressChanged += GetArchive_ProgressChanged;
        }

        public void GetLatestVersion()
        {
            if(DownloadVersionStringThread != null)
            {
                Log.Info(Log.LogSource.Net, "Updater.GetLatestVersion(): Shutting down old thread");
                DownloadVersionStringThread.Abort();
                DownloadVersionStringThread = null;
            }

            Log.Info(Log.LogSource.Net, "Updater.GetLatestVersion(): Starting new thread");
            DownloadVersionStringThread = new Thread(DownloadStringProcedure);
            DownloadVersionStringThread.Start();
        }

        void DownloadStringProcedure()
        {
            Log.Info(Log.LogSource.Net, "Updater.DownloadStringProcedure() called");

            try
            {
                string s = wc.DownloadString(VersionSource);

                int i = s.IndexOf("Current version:");
                if (i == Utility.NO_INDEX)
                {
                    Log.Error(Log.LogSource.Net, "Updater.GetLatestVersion_completed(): Invalid update info");
                    _OnGetVersion?.Invoke(false, "");
                    return;
                }

                string newest_version = s.Substring(i + "Current version:".Length).Trim();
                if (newest_version != Utility.APP_VERSION)
                {
                    _OnGetVersion?.Invoke(true, newest_version);
                }
                else
                {
                    _OnGetVersion?.Invoke(false, "");
                }
            }
            catch(System.Net.WebException e)
            {
                Log.Warning(Log.LogSource.Net, "Updater.GetLatestVersion_completed(): Could not retrieve update info");
                _OnGetVersion?.Invoke(false, "");
            }
            finally
            {
                Log.Info(Log.LogSource.Net, "Updater.DownloadStringProcedure() finished");
            }
        }

        public void ForceInstallLatestVersion()
        {
            Log.Info(Log.LogSource.Net, "Updater.ForceInstallLatestVersion() called");
            wc.DownloadDataAsync(VersionArchive);
        }

        // creates a backup of executable files
        void MoveOldAssemblies()
        {
            Log.Info(Log.LogSource.Utility, "Updater.MoveOldAssemblies() called");

            // https://visualstudiomagazine.com/articles/2017/12/15/replace-running-app.aspx
            Assembly currentAssembly = Assembly.GetEntryAssembly();
            Assembly jsonAssembly = Assembly.GetAssembly(typeof(JsonReader));

            foreach (var a in new Assembly[] { currentAssembly, jsonAssembly })
            {
                string appFolder = Path.GetDirectoryName(a.Location);
                string appName = Path.GetFileNameWithoutExtension(a.Location);
                string appExtension = Path.GetExtension(a.Location);
                string tempPath = Path.Combine(appFolder, appName + "_old" + appExtension);

                if(File.Exists(a.Location))
                    File.Move(a.Location, tempPath);
            }
        }

        // deletes old executable files (if any)
        void DeleteOldAssemblies()
        {
            Log.Info(Log.LogSource.Utility, "Updater.DeleteOldAssemblies() called");

            // https://visualstudiomagazine.com/articles/2017/12/15/replace-running-app.aspx
            Assembly currentAssembly = Assembly.GetEntryAssembly();
            Assembly jsonAssembly = Assembly.GetAssembly(typeof(JsonReader));

            foreach (var a in new Assembly[] { currentAssembly, jsonAssembly })
            {
                string appFolder = Path.GetDirectoryName(a.Location);
                string appName = Path.GetFileNameWithoutExtension(a.Location);
                string appExtension = Path.GetExtension(a.Location);
                string tempPath = Path.Combine(appFolder, appName + "_old" + appExtension);

                if(File.Exists(tempPath))
                    File.Delete(tempPath);
            }
        }

        void GetVersionArchive_completed(object sender, System.Net.DownloadDataCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Log.Warning(Log.LogSource.Net, "Updater.GetVersionArchive_completed(): Could not retrieve newest version archive");
                return;
            }

            if (e.Error != null)
            {
                Log.Error(Log.LogSource.Net, "Updater.GetLatestVersion_completed(): Error while retrieving newest version archive");
                return;
            }

            byte[] data = e.Result;
            try
            {
                string appFile = Assembly.GetEntryAssembly().Location;
                string appFolder = Path.GetDirectoryName(appFile);

                MoveOldAssemblies();

                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (ZipArchive arch = new ZipArchive(ms))
                    {
                        foreach(var f in arch.Entries)
                        {
                            string destinationPath = Path.GetFullPath(Path.Combine(appFolder, f.FullName));
                            string destinationDirectory = Path.GetDirectoryName(destinationPath);
                            Directory.CreateDirectory(destinationDirectory);

                            if(f.ToString().Contains('.'))
                                f.ExtractToFile(destinationPath, true);
                        }
                    }
                }

                _OnGetArchiveSuccess();
            }
            catch(Exception ex)
            {
                Log.Error(Log.LogSource.Net, "Updater.GetVersionArchive_completed(): Error while unpacking archive: " + ex.ToString());
                _OnGetArchiveFailed?.Invoke();
            }
        }

        void GetArchive_ProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            _OnArchiveProgressChanged?.Invoke((int)(100 * (e.BytesReceived / 1000000f)));
        }
    }
}
