using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poxnora_search_engine
{
    public delegate void OnGetVersion(bool is_current_outdated, string new_version);

    public class Updater
    {
        Uri VersionSource = new Uri("https://raw.githubusercontent.com/leszekd25/poxnora_search_engine/master/README.md");
        public OnGetVersion _OnGetVersion = null;


        public void GetLatestVersion()
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            wc.DownloadStringCompleted += new System.Net.DownloadStringCompletedEventHandler(GetLatestVersion_completed);

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
                Log.Info("Updater.GetLatestVersion_completed(): New editor version available");
                _OnGetVersion?.Invoke(true, newest_version);
            }
            else
            {
                _OnGetVersion?.Invoke(false, "");
            }
        }
    }
}
