using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slouch
{
    public interface IDownloadProgressHost
    {
        void Update(Guid inId, Int32 inCompletionPercentage);
    }
}
