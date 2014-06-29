using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch
{
    public interface IMediaSearcher
    {
        Boolean IsCompatibleWith(IMediaItem inItem);
        IMediaSearchResult Search(IMediaItem inItem);
    }
}
