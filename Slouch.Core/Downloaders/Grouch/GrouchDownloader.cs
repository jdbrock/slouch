using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Slouch.Core
{
    public class GrouchDownloader : IMediaDownloader
    {
        public Boolean Download(IMediaSearchResult inSearchResult)
        {
            throw new NotImplementedException();
        }

        public void Download(GrouchInternalNzb inNzb, Int32 inThreads, Func<GrouchInternalUsenetClient> inClientFactory)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            var lockObject = new Object();

            var segments = inNzb.Files.SelectMany(X => X.Segments).ToList();

            var clients = Enumerable.Range(0, inThreads)
                .Select(X => inClientFactory())
                .ToDictionary(X => X, X => false);

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
                        client.DownloadArticle(segment.File.Groups.First().Name, segment.MessageId, tempPath);

                        lock (lockObject)
                            clients[client] = false;
                    });
                }

                // If there are no busy clients and the queue is empty, we're done.
                if (!anyClientsBusy && queueEmpty)
                    return;
            }
        }

        public void PostProcess()
        {
            throw new NotImplementedException();
        }
    }
}
