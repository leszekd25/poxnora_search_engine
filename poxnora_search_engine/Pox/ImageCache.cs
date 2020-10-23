using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Threading;

namespace poxnora_search_engine.Pox
{
    public delegate void OnImageLoad(Bitmap result);

    public interface IImageCacheSubscriber
    {
        void OnImageLoad(Bitmap bmp);
    }

    public class ImageCache
    {
        public Dictionary<string, Bitmap> RuneImages { get; } = new Dictionary<string, Bitmap>();
        public Dictionary<string, Bitmap> RunePreviews { get; } = new Dictionary<string, Bitmap>();


        


        const string IMAGE_SERVER = "https://d2aao99y1mip6n.cloudfront.net/";

        Thread DownloadRuneImageThread = null;
        System.Timers.Timer CheckNewPreviewTimer = new System.Timers.Timer(10);
        bool NewPreviewRequested = false;

        System.Net.WebClient wc = new System.Net.WebClient();

        string QueuedRuneImageHash = "";
        string CurrentRuneImageHash = "";

        Queue<string> QueuedRunePreviewHashes = new Queue<string>();


        public HashSet<IImageCacheSubscriber> RuneImageSubscribers { get; } = new HashSet<IImageCacheSubscriber>();
        Dictionary<string, HashSet<IImageCacheSubscriber>> RunePreviewSubscribers { get; } = new Dictionary<string, HashSet<IImageCacheSubscriber>>();

        public ImageCache()
        {
            wc.DownloadDataCompleted += new System.Net.DownloadDataCompletedEventHandler(OnRunePreviewDownloaded);
            CheckNewPreviewTimer.Elapsed += CheckNewPreviewTimer_Elapsed;
            CheckNewPreviewTimer.AutoReset = true;
        }

        public bool IsRuneImageSavedOnDisk(string hash)
        {
            if (Directory.Exists("images\\runes\\lg"))
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

            // if this code is run on the downloading thread, don't start a new thread
            if (Thread.CurrentThread == DownloadRuneImageThread)
            {
                CurrentRuneImageHash = QueuedRuneImageHash;
                QueuedRuneImageHash = "";

                DownloadRuneImageProcedure();
            }
            else
            {
                if (DownloadRuneImageThread != null)
                {
                    if (DownloadRuneImageThread.IsAlive)
                        return;

                    DownloadRuneImageThread = null;
                }

                CurrentRuneImageHash = QueuedRuneImageHash;
                QueuedRuneImageHash = "";

                DownloadRuneImageThread = new Thread(DownloadRuneImageProcedure);
                DownloadRuneImageThread.Start();
            }

        }

        void DownloadRuneImageProcedure()
        {
            try
            {
                Uri dw_string = new Uri(IMAGE_SERVER + "images/runes/lg/" + CurrentRuneImageHash + ".jpg");
                byte[] data = wc.DownloadData(dw_string);

                if (!Directory.Exists("images\\runes\\lg"))
                    Directory.CreateDirectory("images\\runes\\lg");

                File.WriteAllBytes("images\\runes\\lg\\" + CurrentRuneImageHash + ".jpg", data);

                LoadRuneImage(CurrentRuneImageHash);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DownloadRuneImageProcedure(): Uncaught exception, contents: " + ex.ToString());
            }

            CurrentRuneImageHash = "";

            if (QueuedRuneImageHash != "")
                LoadRuneImage(QueuedRuneImageHash);
        }

        public void LoadRuneImage(string hash)
        {
            if(RuneImages.ContainsKey(hash))    // image already loaded
            {
                foreach (var s in RuneImageSubscribers)
                    s.OnImageLoad(RuneImages[hash]);
            }
            else if(IsRuneImageSavedOnDisk(hash))    // image not loaded, but saved on disk
            {
                Bitmap bmp = new Bitmap("images\\runes\\lg\\" + hash + ".jpg");
                RuneImages.Add(hash, bmp);

                foreach (var s in RuneImageSubscribers)
                    s.OnImageLoad(bmp);
            }
            else       //get image from net
            {
                QueuedRuneImageHash = hash;
                RequestRuneImageFromServer();
            }
        }

        public void AddRunePreviewSubscriber(string hash, IImageCacheSubscriber sub)
        {
            if (!RunePreviewSubscribers.ContainsKey(hash))
                RunePreviewSubscribers.Add(hash, new HashSet<IImageCacheSubscriber>());

            RunePreviewSubscribers[hash].Add(sub);
        }

        /*public void RemoveRunePreviewSubscriber(string hash, IImageCacheSubscriber sub)
        {
            if (!RunePreviewSubscribers.ContainsKey(hash))
                return;

            if (RunePreviewSubscribers[hash].Contains(sub))
                RunePreviewSubscribers[hash].Remove(sub);
        }*/

        public void RemoveRunePreviewSubscribers(string hash)
        {
            if (!RunePreviewSubscribers.ContainsKey(hash))
                return;

            RunePreviewSubscribers[hash].Clear();
        }

        /*public void ClearRunePreviewSubscribers()
        {
            foreach (var subs in RunePreviewSubscribers.Values)
                subs.Clear();
        }*/
        public void BreakRunePreviewDownload()
        {
            QueuedRunePreviewHashes.Clear();
            wc.CancelAsync();
            NewPreviewRequested = false;
            CheckNewPreviewTimer.Stop();
        }
        public bool IsRunePreviewSavedOnDisk(string hash)
        {
            if (Directory.Exists("images\\runes\\sm"))
            {
                if (File.Exists("images\\runes\\sm\\" + hash + ".png"))
                    return true;
            }

            return false;
        }

        public bool LoadRunePreview(string hash)
        {
            if (!RunePreviews.ContainsKey(hash))    // image not already loaded
            {
                if (IsRunePreviewSavedOnDisk(hash))    // image not loaded, but saved on disk
                {
                    Bitmap bmp;
                    try
                    {
                        bmp = new Bitmap("images\\runes\\sm\\" + hash + ".png");
                    }
                    catch(ArgumentException e)
                    {
                        File.Delete("images\\runes\\sm\\" + hash + ".png");
                        return false;
                    }
                    RunePreviews.Add(hash, bmp);
                }
                else       // get image from net
                {
                    return false;
                }
            }

            foreach (var s in RunePreviewSubscribers[hash])
                s.OnImageLoad(RunePreviews[hash]);

            RemoveRunePreviewSubscribers(hash);
            return true;
        }

        void RequestRunePreviewFromServer()
        {
            if (QueuedRunePreviewHashes.Count == 0)
                return;

            string hash = QueuedRunePreviewHashes.Peek();
            Uri dw_string = new Uri(IMAGE_SERVER + "images/runes/sm/" + hash + ".png");
            while (wc.IsBusy)
                Thread.Sleep(10);
            wc.DownloadDataAsync(dw_string);
        }

        void RequestRunePreviewsFromServer(List<string> image_hashes)
        {
            if (wc == null)
                return;

            BreakRunePreviewDownload();

            CheckNewPreviewTimer.Enabled = true;
            CheckNewPreviewTimer.Start();

            foreach (var h in image_hashes)
            {
                QueuedRunePreviewHashes.Enqueue(h);
            }

            RequestRunePreviewFromServer();
        }

        public void GetRunePreviews(List<string> image_hashes)
        {
            List<string> failed_images = new List<string>();
            foreach (var hash in image_hashes)
                if (!LoadRunePreview(hash))
                    failed_images.Add(hash);

            RequestRunePreviewsFromServer(failed_images);
        }


        void OnRunePreviewDownloaded(object sender, System.Net.DownloadDataCompletedEventArgs e)
        {
            if (QueuedRunePreviewHashes.Count == 0)
                return;

            string hash = QueuedRunePreviewHashes.Dequeue();

            try
            {
                if (!e.Cancelled)
                {
                    if (e.Error == null)
                    {
                        byte[] data = e.Result;

                        if (!Directory.Exists("images\\runes\\sm"))
                            Directory.CreateDirectory("images\\runes\\sm");

                        using (FileStream fs = new FileStream("images\\runes\\sm\\" + hash + ".png", FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            fs.Write(data, 0, data.Length);
                            fs.Flush();
                        }

                        LoadRunePreview(hash);
                    }
                    else
                        throw e.Error;
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            NewPreviewRequested = true;
        }

        private void CheckNewPreviewTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if(NewPreviewRequested)
            {
                if(QueuedRunePreviewHashes.Count == 0)
                {
                    CheckNewPreviewTimer.Stop();
                }
                else
                {
                    CheckNewPreviewTimer.Start();
                    RequestRunePreviewFromServer();
                }
                NewPreviewRequested = false;
            }
        }
    }
}
