using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch.Core
{
    public class MovieMediaSource : IMediaSource
    {
        public IEnumerable<IMediaItem> GetDueItems()
        {
            yield break;
            //throw new NotImplementedException();
        }

        public void ChangeStatus(IMediaItem inItem, MediaStatus inStatus)
        {
            throw new NotImplementedException();
        }
    }
}
