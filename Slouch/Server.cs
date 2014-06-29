using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Slouch.Core
{
    public class Server
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

        // ===========================================================================
        // = Private Fields
        // ===========================================================================
        
        private static readonly TimeSpan _timerInterval = TimeSpan.FromMinutes(1);

        private static Server _instance;

        private Timer _timer;
        private Boolean _timerActive;
        private Boolean _timerCallbackRunning;

        private IEnumerable<IMediaSource> _sources;
        private IEnumerable<IMediaSearcher> _searchers;
        private GrouchDownloader _downloader;

        private IDisposable _webServer;

        public Settings _settings;

        // ===========================================================================
        // = Construction
        // ===========================================================================
        
        private Server()
        {
            LoadSettings();

            _timer = new Timer(OnServerTick, null, TimeSpan.Zero, _timerInterval);

            _downloader = new GrouchDownloader(_settings);

            _sources = new List<IMediaSource>
            {
                new MovieMediaSource(),
                new TvMediaSource()
            };

            // There doesn't appear to be a way to start and stop the web server; calling dispose doesn't seem to work.
            _webServer = WebApp.Start<WebServer>(_settings.Uri);
        }

        // ===========================================================================
        // = Public Methods
        // ===========================================================================
        
        public void Start()
        {
            _timerActive = true;
            _timer.Change(TimeSpan.Zero, _timerInterval);
        }

        public void Stop()
        {
            _timerActive = false;

            SaveSettings();
        }

        // ===========================================================================
        // = Private Methods
        // ===========================================================================

        private void OnServerTick(Object inState)
        {
            if (_timerActive)
                return;

            if (_timerCallbackRunning)
                return;

            try
            {
                _timerCallbackRunning = true;
                OnServerTickCore();
            }
            finally
            {
                _timerCallbackRunning = false;
            }
        }

        private void OnServerTickCore()
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

            foreach (var source in _sources)
                foreach (var item in source.GetDueItems())
                {
                    // Search for the item.
                    var results = _searchers
                        .Where(X => X.IsCompatibleWith(item))
                        .Select(X => X.Search(item))
                        .AsParallel()
                        .ToList();

                    // Did we find anything?
                    if (results.Count == 0 || !results.Any(X => X.IsSuccess))
                        continue;

                    // Pick the the best result (TODO).
                    var best = results.First();

                    // Download it.
                    if (_downloader.Download(best))
                        source.ChangeStatus(item, MediaStatus.Incoming);
                }

            // Post-process.
            _downloader.PostProcess();
        }

        private String GetSettingsPath()
        {
            var appDataPath  = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var slouchPath   = Path.Combine(appDataPath, "Slouch");
            var settingsPath = Path.Combine(slouchPath, "settings.json");

            Directory.CreateDirectory(slouchPath);

            return settingsPath;
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
    }
}
