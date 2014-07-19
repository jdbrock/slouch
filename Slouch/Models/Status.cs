using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch
{
    public class Status
    {
        public List<DownloadStatus> Downloads { get; set; }
        public List<IMediaItem> JustArrived { get; set; }

        public Status()
        {
            Downloads = new List<DownloadStatus>();
            JustArrived = new List<IMediaItem>();
        }
    }
}
