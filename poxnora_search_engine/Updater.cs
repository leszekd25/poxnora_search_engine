using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using Newtonsoft.Json;

namespace poxnora_search_engine
{
    public delegate void OnGetVersion(bool is_current_outdated, string new_version);
    public delegate void OnGetArchiveFailed();
    public delegate void OnGetArchiveSuccess();

    public class Updater
    {
        Uri VersionSource = new Uri("https://raw.githubusercontent.com/leszekd25/poxnora_search_engine/master/README.md");
        Uri VersionArchive = new Uri("https://github.com/leszekd25/poxnora_search_engine/raw/master/latest/poxnora%20search%20engine.zip");

        public OnGetVersion _OnGetVersion = null;
        public OnGetArchiveFailed _OnGetArchiveFailed = null;
        public OnGetArchiveSuccess _OnGetArchiveSuccess = null;

        System.Net.WebClient wc = new System.Net.WebClient();

        public Updater()
        {
            DeleteOldAssemblies();
            wc.DownloadStringCompleted += new System.Net.DownloadStringCompletedEventHandler(GetLatestVersion_completed);
            wc.DownloadDataCompleted += new System.Net.DownloadDataCompletedEventHandler(GetVersionArchive_completed);
        }

        public void GetLatestVersion()
        {
            wc.DownloadStringAsync(VersionSource);
        }

        void GetLatestVersion_completed(object sender, System.Net.DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                Log.Warning("Updater.GetLatestVersion_completed(): Could not retrieve update info");
                _OnGetVersion?.Invoke(false, "");
                return;
            }

            if (e.Error != null)
            {
                Log.Error("Updater.GetLatestVersion_completed(): Error while retrieving update info");
                _OnGetVersion?.Invoke(false, "");
                return;
            }

            string str = e.Result;
            int i = str.IndexOf("Current version:");
            if (i == Utility.NO_INDEX)
            {
                Log.Error("Updater.GetLatestVersion_completed(): Invalid update info");
                _OnGetVersion?.Invoke(false, "");
                return;
            }

            string newest_version = str.Substring(i + "Current version:".Length).Trim();
            if (newest_version != Utility.APP_VERSION)
            {
                _OnGetVersion?.Invoke(true, newest_version);
            }
            else
            {
                _OnGetVersion?.Invoke(false, "");
            }
        }

        public void ForceInstallLatestVersion()
        {
            wc.DownloadDataAsync(VersionArchive);
        }

        void MoveOldAssemblies()
        {
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

        void DeleteOldAssemblies()
        {
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
                Log.Warning("Updater.GetVersionArchive_completed(): Could not retrieve newest version archive");
                return;
            }

            if (e.Error != null)
            {
                Log.Error("Updater.GetLatestVersion_completed(): Error while retrieving newest version archive");
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

                            f.ExtractToFile(destinationPath, true);
                        }
                    }
                }

                _OnGetArchiveSuccess();
            }
            catch(Exception ex)
            {
                Log.Error("Updater.GetVersionArchive_completed(): Error while unpacking archive: " + ex.ToString());
                _OnGetArchiveFailed?.Invoke();
            }
        }
    }
}
