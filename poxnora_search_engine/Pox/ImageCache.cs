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

    public struct HashSubscriberTuple
    {
        public string Hash;
        public IImageCacheSubscriber Subscriber;
    }


    public class ImageCache
    {
        public Dictionary<string, Bitmap> RuneImages { get; } = new Dictionary<string, Bitmap>();
        public Dictionary<string, Bitmap> RunePreviews { get; } = new Dictionary<string, Bitmap>();
        public Dictionary<string, Bitmap> AbilityImages { get; } = new Dictionary<string, Bitmap>();   // 45x44

        const string IMAGE_SERVER = "https://d2aao99y1mip6n.cloudfront.net/";

        Thread DownloadRuneImageThread = null;
        string QueuedRuneImageHash = "";
        string CurrentRuneImageHash = "";
        System.Net.WebClient wc = new System.Net.WebClient();

        System.Timers.Timer CheckNewPreviewTimer = new System.Timers.Timer(100);

        System.Collections.Concurrent.ConcurrentDictionary<string, int> QueuedRunePreviewHashes = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();
        System.Collections.Concurrent.ConcurrentDictionary<string, int> ProcessedRunePreviewHashes = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();
        System.Collections.Concurrent.ConcurrentDictionary<string, int> ResolvedRunePreviewHashes = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();

        System.Collections.Concurrent.ConcurrentDictionary<string, int> QueuedAbilityImages = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();
        System.Collections.Concurrent.ConcurrentDictionary<string, int> ProcessedAbilityImages = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();
        System.Collections.Concurrent.ConcurrentDictionary<string, int> ResolvedAbilityImages = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();

        public HashSet<IImageCacheSubscriber> RuneImageSubscribers { get; } = new HashSet<IImageCacheSubscriber>();
        public List<HashSubscriberTuple> RunePreviewRequestors { get; } = new List<HashSubscriberTuple>();    // requests removed once image preview is resolved
        public List<HashSubscriberTuple> AbilityImageRequestors { get; } = new List<HashSubscriberTuple>();  // requests removed once ability image is resolved

        public ImageCache()
        {
            CheckNewPreviewTimer.Elapsed += CheckNewPreviewTimer_Elapsed;
            CheckNewPreviewTimer.AutoReset = true;
            CheckNewPreviewTimer.Start();
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
            if (hash == "")
            {
                sub.OnImageLoad(null);
            }
            else if (!LoadRunePreview(hash))
            {
                RunePreviewRequestors.Add(new HashSubscriberTuple() { Hash = hash, Subscriber = sub });
                QueuedRunePreviewHashes.TryAdd(hash, 0);
            }
            else
            {
                sub.OnImageLoad(RunePreviews[hash]);
            }
        }

        public void RemoveRunePreviewSubscriber(IImageCacheSubscriber sub)//(string hash, IImageCacheSubscriber sub)
        {
            for (int i = 0; i < RunePreviewRequestors.Count; i++)
            {
                if (RunePreviewRequestors[i].Subscriber == sub)
                {
                    RunePreviewRequestors.RemoveAt(i);
                    i--;
                }
            }
        }

        public void RemoveRunePreviewSubscribers(string hash)
        {
            RunePreviewRequestors.Clear();
        }

        public void BreakRunePreviewDownload()
        {
            QueuedRunePreviewHashes.Clear();
            ProcessedRunePreviewHashes.Clear();

            RunePreviewRequestors.Clear();
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
                        RunePreviews.Add(hash, bmp);
                    }
                    catch (Exception e)
                    {
                        RunePreviews.Add(hash, null);
                    }
                }
                else       // get image from net
                {
                    return false;
                }
            }
            return true;
        }


        private void DownloadRunePreview(Object stateInfo)
        {
            string hash = (string)stateInfo;

            System.Net.WebClient wc = new System.Net.WebClient();

            Uri dw_string = new Uri(IMAGE_SERVER + "images/runes/sm/" + hash + ".png");

            try
            {
                byte[] img_data = null;
                try
                {
                    img_data = wc.DownloadData(dw_string);
                }
                catch (Exception e)
                {

                }

                // dummy file
                if (img_data == null)
                    img_data = new byte[] { 0, 0, 0, 0 };

                if (!Directory.Exists("images\\runes\\sm"))
                    Directory.CreateDirectory("images\\runes\\sm");

                using (FileStream fs = new FileStream("images\\runes\\sm\\" + hash + ".png", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(img_data, 0, img_data.Length);
                    fs.Flush();
                }
            }
            catch (Exception e2)
            {

            }

            int dummy;
            ResolvedRunePreviewHashes.TryAdd(hash, 0);
            ProcessedRunePreviewHashes.TryRemove(hash, out dummy);
            QueuedRunePreviewHashes.TryRemove(hash, out dummy);
        }

        private void CheckNewPreviewTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // rune preview
            foreach (var hash in QueuedRunePreviewHashes.Keys)
            {
                if (!ProcessedRunePreviewHashes.ContainsKey(hash))
                {
                    ProcessedRunePreviewHashes.TryAdd(hash, 0);

                    ThreadPool.QueueUserWorkItem(DownloadRunePreview, hash);
                }
            }

            foreach (var hash in ResolvedRunePreviewHashes.Keys)
            {
                LoadRunePreview(hash);
                foreach (var request in RunePreviewRequestors)
                {
                    if (request.Hash == hash)
                        request.Subscriber.OnImageLoad(RunePreviews[hash]);
                }
                for (int i = 0; i < RunePreviewRequestors.Count; i++)
                {
                    if (RunePreviewRequestors[i].Hash == hash)
                    {
                        RunePreviewRequestors.RemoveAt(i);
                        i--;
                    }
                }
            }
            ResolvedRunePreviewHashes.Clear();

            // ability images
            foreach (var hash in QueuedAbilityImages.Keys)
            {
                if (!ProcessedAbilityImages.ContainsKey(hash))
                {
                    ProcessedAbilityImages.TryAdd(hash, 0);

                    ThreadPool.QueueUserWorkItem(DownloadAbilityImage, hash);
                }
            }

            foreach (var hash in ResolvedAbilityImages.Keys)
            {
                LoadAbilityImage(hash);
                foreach (var request in AbilityImageRequestors)
                {
                    if (request.Hash == hash)
                        request.Subscriber.OnImageLoad(AbilityImages[hash]);
                }
                for (int i = 0; i < AbilityImageRequestors.Count; i++)
                {
                    if (AbilityImageRequestors[i].Hash == hash)
                    {
                        AbilityImageRequestors.RemoveAt(i);
                        i--;
                    }
                }
            }
            ResolvedAbilityImages.Clear();
        }


        public void AddAbilityImageSubscriber(string hash, IImageCacheSubscriber sub)
        {
            if (hash == "")
            {
                sub.OnImageLoad(null);
            }
            else if (!LoadAbilityImage(hash))
            {
                AbilityImageRequestors.Add(new HashSubscriberTuple() { Hash = hash, Subscriber = sub });
                QueuedAbilityImages.TryAdd(hash, 0);
            }
            else
            {
                sub.OnImageLoad(AbilityImages[hash]);
            }
        }

        public void RemoveAbilityImageSubscriber(IImageCacheSubscriber sub)
        {
            for (int i = 0; i < AbilityImageRequestors.Count; i++)
            {
                if (AbilityImageRequestors[i].Subscriber == sub)
                {
                    AbilityImageRequestors.RemoveAt(i);
                    i--;
                }
            }
        }

        public void RemoveAbilityImageSubscribers(string hash)
        {
            AbilityImageRequestors.Clear();
        }

        public void BreakAbilityImageDownload()
        {
            QueuedAbilityImages.Clear();
            ProcessedAbilityImages.Clear();

            AbilityImageRequestors.Clear();
        }

        public bool IsAbilityImageSavedOnDisk(string hash)
        {
            if (Directory.Exists("images\\ability_icons\\large"))
            {
                if (File.Exists("images\\ability_icons\\large\\icon_" + hash + ".gif"))
                    return true;
            }

            return false;
        }

        public bool LoadAbilityImage(string hash)
        {
            if (!AbilityImages.ContainsKey(hash))    // image not already loaded
            {
                if (IsAbilityImageSavedOnDisk(hash))    // image not loaded, but saved on disk
                {
                    Bitmap bmp;
                    try
                    {
                        bmp = new Bitmap("images\\ability_icons\\large\\icon_" + hash + ".gif");
                        AbilityImages.Add(hash, bmp);
                    }
                    catch (Exception e)
                    {
                        AbilityImages.Add(hash, null);
                    }
                }
                else       // get image from net
                {
                    return false;
                }
            }
            return true;
        }


        private void DownloadAbilityImage(Object stateInfo)
        {
            string hash = (string)stateInfo;

            System.Net.WebClient wc = new System.Net.WebClient();

            Uri dw_string = new Uri(IMAGE_SERVER + "images\\ability_icons\\large\\icon_" + hash + ".gif");

            try
            {
                byte[] img_data = null;
                try
                {
                    img_data = wc.DownloadData(dw_string);
                }
                catch (Exception e)
                {

                }

                // attempt to download small icon if large icon does not exist (sad)
                if (img_data == null)
                {
                    try
                    {
                        dw_string = new Uri(IMAGE_SERVER + "images\\ability_icons\\small\\icon_" + hash + ".gif");
                        img_data = wc.DownloadData(dw_string);
                    }
                    catch (Exception e)
                    {

                    }
                }
                // dummy file
                if (img_data == null)
                    img_data = new byte[] { 0, 0, 0, 0 };

                if (!Directory.Exists("images\\ability_icons\\large"))
                    Directory.CreateDirectory("images\\ability_icons\\large");

                using (FileStream fs = new FileStream("images\\ability_icons\\large\\icon_" + hash + ".gif", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(img_data, 0, img_data.Length);
                    fs.Flush();
                }
            }
            catch (Exception e2)
            {

            }

            int dummy;
            ResolvedAbilityImages.TryAdd(hash, 0);
            ProcessedAbilityImages.TryRemove(hash, out dummy);
            QueuedAbilityImages.TryRemove(hash, out dummy);
        }
    }
}
