using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Slouch
{
    public class GrouchDownloader
    {
        // ===========================================================================
        // = Private Fields
        // ===========================================================================
        
        private Settings _settings;
        private Func<GrouchInternalUsenetClient> _clientFactory;

        // ===========================================================================
        // = Construction
        // ===========================================================================
        
        public GrouchDownloader(Settings inSettings)
        {
            _settings = inSettings;

            _clientFactory = () => new GrouchInternalUsenetClient(
                _settings.ServerHostName,
                _settings.ServerUserName,
                _settings.ServerPassword,
                _settings.ServerPort,
                _settings.ServerUseSsl);
        }

        // ===========================================================================
        // = Public Methods
        // ===========================================================================
        
        public Boolean Download(IMediaSearchResult inSearchResult, IDownloadProgressHost inProgressHost)
        {
            if (inSearchResult is INzbMediaSearchResult)
            {
                var doc = XDocument.Load(((INzbMediaSearchResult)inSearchResult).NzbUri.OriginalString);
                var nzb = GrouchInternalNzb.FromXml(doc);

                Download(nzb, inSearchResult.Id, inProgressHost);

                return true;
            }
            else
                throw new NotImplementedException();
        }

        private void Download(GrouchInternalNzb inNzb, Guid inId, IDownloadProgressHost inProgressHost)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            var clients = Enumerable.Range(0, _settings.NumberOfThreads)
                .Select(X => _clientFactory())
                .ToDictionary(X => X, X => false);

            var lastProgressUpdate = DateTime.Now;

            try
            {
                var lockObject = new Object();

                var segments = inNzb.Files.SelectMany(X => X.Segments).ToList();

                var initialSegmentCount = segments.Count;
                var currentSegmentCount = segments.Count;

                while (true)
                {
                    var allClientsBusy = false;
                    var anyClientsBusy = false;
                    var queueEmpty = false;

                    GrouchInternalUsenetClient client = null;
                    GrouchInternalSegment segment = null;

                    lock (lockObject)
                    {
                        queueEmpty = segments.Count == 0;
                        allClientsBusy = clients.All(X => X.Value);
                        anyClientsBusy = clients.Any(X => X.Value);

                        if (!allClientsBusy && !queueEmpty)
                        {
                            client = clients.First(X => !X.Value).Key;
                            clients[client] = true;

                            segment = segments.First();
                            segments.RemoveAt(0);

                            currentSegmentCount = segments.Count;
                        }
                    }

                    // If all of the clients are busy and there are still files to download, sleep for a bit.
                    if (allClientsBusy && !queueEmpty)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    // If there are free clients and there are still files to download, start downloading.
                    if (!allClientsBusy && !queueEmpty)
                    {
                        Task.Run(() =>
                        {
                            // TODO: Try other groups if one fails.
                            try
                            {
                                client.DownloadArticle(segment.File.Groups.First().Name, segment.MessageId, tempPath);
                            }
                            catch { }

                            lock (lockObject)
                                clients[client] = false;
                        });
                    }

                    if (lastProgressUpdate.AddSeconds(1) < DateTime.Now)
                    {
                        var completionPercentage = 100 - (Int32)((((Double)currentSegmentCount) / initialSegmentCount) * 100);
                        inProgressHost.Update(inId, completionPercentage);

                        lastProgressUpdate = DateTime.Now;
                    }

                    // If there are no busy clients and the queue is empty, we're done.
                    if (!anyClientsBusy && queueEmpty)
                        return;
                }
            }
            finally
            {
                // TODO: Delete temp directory.
                Process.Start(tempPath);

                // Disconnect clients.
                foreach (var client in clients)
                    client.Key.Dispose();
            }
        }

        public void PostProcess()
        {
            //throw new NotImplementedException();
        }
    }
}
