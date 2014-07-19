using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Slouch
{
    public class Server : IDownloadProgressHost
    {
        // ===========================================================================
        // = Public Properties
        // ===========================================================================

        public static Server Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Server();

                return _instance;
            }
        }

        public Status Status { get; set; }

        // ===========================================================================
        // = Private Fields
        // ===========================================================================
        
        private static readonly TimeSpan _timerInterval = TimeSpan.FromSeconds(10);

        private static Server _instance;

        private IEnumerable<IMediaSource> _sources;
        private IEnumerable<IMediaSearcher> _searchers;
        private GrouchDownloader _downloader;

        private IDisposable _webServer;

        private ConcurrentQueue<IMediaSearchResult> _downloadQueue;
        private Task _downloadTask;
        private Task _scheduleTask;

        internal Settings _settings;

        // ===========================================================================
        // = Construction
        // ===========================================================================
        
        private Server()
        {
            LoadSettings();

            Cabinet.Initialise(GetCabinetPath());

            _downloader = new GrouchDownloader(_settings);

            _sources = new List<IMediaSource>
            {
                new MovieMediaSource(),
                new TvMediaSource()
            };

            _searchers = new List<IMediaSearcher>
            {
                new DummyMediaSearcher()
            };

            Status = new Status();

            _downloadQueue = new ConcurrentQueue<IMediaSearchResult>();
            _downloadTask = new Task(DownloadTask);
            _scheduleTask = new Task(ScheduleTask);

            // There doesn't appear to be a way to start and stop the web server; calling dispose doesn't seem to work.
            _webServer = WebApp.Start<WebServer>(_settings.Uri);
        }

        // ===========================================================================
        // = Public Methods
        // ===========================================================================
        
        public void Start()
        {
            _downloadTask.Start();
            _scheduleTask.Start();
        }

        public void Stop()
        {
            SaveSettings();
        }

        // ===========================================================================
        // = Private Methods
        // ===========================================================================

        private void DownloadTask()
        {
            while (true)
            {
                IMediaSearchResult x;

                if (_downloadQueue.TryDequeue(out x))
                    _downloader.Download(x, this);

                Thread.Sleep(100);
            }
        }

        private void ScheduleTask()
        {
            // Find everything that is scheduled for download.
            
            // Movies
            //  1) Movies that were just added.
            //  2) Movies that have recently been released.
            //  3) Movies that we've been unable to find for a while.

            // TV
            //  1) TV shows or episodes that were just added.
            //  2) TV episodes that have recently aired.
            //  3) TV episodes we've been unable to find for a while.

            // Post-process everything that has downloaded or failed.
            //  1) Run through list of download transactions (each download is tagged with a GUID)
            //  2) Transactions that have failed will have their item's status updated to reflect this (FailedStillWanted).
            //  3) Transactions that are in progress will have their item's progress updated.
            //  4) Transactions that have been completed will have their item's status updated, the item moved to the correct
            //     location on disk for that media source (e.g. TV/movie libraries) and notifications will be generated.

            while (true)
            {
                foreach (var source in _sources)
                    foreach (var item in source.GetDueItems())
                    {
                        Console.WriteLine(String.Format("Due: {0}", item.DisplayName));
                        Console.WriteLine("Searching...");

                        // Search for the item.
                        var results = _searchers
                            .Where(X => X.IsCompatibleWith(item))
                            .Select(X => X.Search(item))
                            .AsParallel()
                            .ToList();

                        Console.WriteLine(String.Format("Found {0} results.", results.Count));

                        // Did we find anything?
                        if (results.Count == 0 || !results.Any(X => X.IsSuccess))
                            continue;

                        // Pick the the best result (TODO).
                        var best = results.First();

                        // Add to download queue.
                        QueueForDownload(best);

                        Console.WriteLine("Added to download queue.");

                        source.ChangeStatus(item, MediaStatus.Incoming);
                    }

                // Post-process.
                _downloader.PostProcess();

                Console.WriteLine();
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }

        private void QueueForDownload(IMediaSearchResult inResult)
        {
            Status.Downloads.Add(new DownloadStatus
            {
                Id = inResult.Id,
                Media = inResult.Item,
                CompletionPercentage = 0
            });

            _downloadQueue.Enqueue(inResult);
        }

        private String GetBasePath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var slouchPath = Path.Combine(appDataPath, "Slouch");

            Directory.CreateDirectory(slouchPath);

            return slouchPath;
        }

        private String GetSettingsPath()
        {
            return Path.Combine(GetBasePath(), "settings.json");
        }

        private String GetCabinetPath()
        {
            var cabinetPath = Path.Combine(GetBasePath(), "Data");

            Directory.CreateDirectory(cabinetPath);

            return cabinetPath;
        }

        private void LoadSettings()
        {
            var serializer = new JsonSerializer();
            var settingsPath = GetSettingsPath();

            if (!File.Exists(settingsPath))
                _settings = new Settings();
            else
                using (var settingsFile = File.OpenRead(settingsPath))
                {
                    _settings = serializer.Deserialize<Settings>(new JsonTextReader(new StreamReader(settingsFile)));
                }
        }

        public void SaveSettings()
        {
            var serializer = new JsonSerializer();
            var settingsPath = GetSettingsPath();

            using (var settingsFile = File.Create(settingsPath))
            {
                var streamWriter = new StreamWriter(settingsFile);
                var jsonWriter   = new JsonTextWriter(streamWriter);

                serializer.Serialize(jsonWriter, _settings);

                jsonWriter.Flush();
                streamWriter.Flush();
                settingsFile.Flush();
            }
        }

        // ===========================================================================
        // = IDownloadProgressHost Implementation
        // ===========================================================================

        void IDownloadProgressHost.Update(Guid inId, Int32 inCompletionPercentage)
        {
            // TODO: This lookup will be far too slow.
            Status.Downloads.First(X => X.Id == inId).CompletionPercentage = inCompletionPercentage;
        }
    }
}
