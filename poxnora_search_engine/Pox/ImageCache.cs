using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace poxnora_search_engine.Pox
{
    public delegate void OnImageLoad(Bitmap result);

    public class ImageCache
    {
        public Dictionary<string, Bitmap> RuneImages { get; } = new Dictionary<string, Bitmap>();
        const string IMAGE_SERVER = "https://d2aao99y1mip6n.cloudfront.net/";

        System.Net.WebClient wc = new System.Net.WebClient();

        string QueuedRuneImageHash = "";
        string CurrentRuneImageHash = "";
        bool IsDownloadingRuneImage = false;
        
        public OnImageLoad ImageLoad_event;

        public void LoadRuneImage(string hash)
        {
            if(RuneImages.ContainsKey(hash))    // image already loaded
            {
                ImageLoad_event(RuneImages[hash]);
            }
            else if(IsRuneImageSavedOnDisk(hash))    // image not loaded, but saved on disk
            {
                Bitmap bmp = new Bitmap("images\\runes\\lg\\" + hash + ".jpg");
                RuneImages.Add(hash, bmp);
                ImageLoad_event(bmp);
            }
            else       //get image from net
            {
                QueuedRuneImageHash = hash;
                RequestRuneImageFromServer();
            }
        }

        public bool IsRuneImageSavedOnDisk(string hash)
        {
            if(Directory.Exists("images\\runes\\lg"))
            {
                if (File.Exists("images\\runes\\lg\\" + hash + ".jpg"))
                    return true;
            }

            return false;
        }

        void RequestRuneImageFromServer()
        {
            if (wc == null)
                return;

            if (IsDownloadingRuneImage)
                return;

            wc.DownloadDataCompleted += new System.Net.DownloadDataCompletedEventHandler(OnRuneImageDownloaded);

            Uri dw_string = new Uri(IMAGE_SERVER+"images/runes/lg/"+QueuedRuneImageHash+".jpg");

            CurrentRuneImageHash = QueuedRuneImageHash;
            QueuedRuneImageHash = "";
            IsDownloadingRuneImage = true;
            wc.DownloadDataAsync(dw_string);
        }

        void OnRuneImageDownloaded(object sender, System.Net.DownloadDataCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                try
                {
                    if (!Directory.Exists("images"))
                    {
                        Directory.CreateDirectory("images");
                        Directory.CreateDirectory("images\\runes");
                        Directory.CreateDirectory("images\\runes\\lg");
                    }
                    File.WriteAllBytes("images\\runes\\lg\\" + CurrentRuneImageHash + ".jpg", e.Result);
                    LoadRuneImage(CurrentRuneImageHash);
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("OnRuneImageDownloaded(): Uncaught exception, contents: " + ex.ToString());
                }
            }
            IsDownloadingRuneImage = false;
            wc.DownloadDataCompleted -= new System.Net.DownloadDataCompletedEventHandler(OnRuneImageDownloaded);
            CurrentRuneImageHash = "";



            if (QueuedRuneImageHash != "")
                RequestRuneImageFromServer();
        }
    }
}
