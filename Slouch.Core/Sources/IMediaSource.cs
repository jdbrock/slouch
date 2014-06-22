using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch.Core
{
    public interface IMediaSource
    {
        IEnumerable<IMediaItem> GetDueItems();
        void ChangeStatus(IMediaItem inItem, MediaStatus inStatus);
    }
}
