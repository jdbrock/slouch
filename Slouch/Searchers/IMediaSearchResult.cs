using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch
{
    public interface IMediaSearchResult
    {
        Guid Id { get; }
        IMediaItem Item { get; }
        Boolean IsSuccess { get; }
    }
}
