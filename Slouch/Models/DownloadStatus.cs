using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch
{
    public class DownloadStatus
    {
        public Guid Id { get; set; }

        public IMediaItem Media { get; set; }
        public Int32 CompletionPercentage { get; set; }
    }
}
