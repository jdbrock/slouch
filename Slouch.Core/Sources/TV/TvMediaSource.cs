using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch.Core
{
    public class TvMediaSource : IMediaSource
    {
        public IEnumerable<IMediaItem> GetDueItems()
        {
            //throw new NotImplementedException();
            yield break;
        }

        public void ChangeStatus(IMediaItem inItem, MediaStatus inStatus)
        {
            throw new NotImplementedException();
        }
    }
}
