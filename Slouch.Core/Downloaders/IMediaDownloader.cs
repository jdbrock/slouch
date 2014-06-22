using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch.Core
{
    public interface IMediaDownloader
    {
        Boolean Download(IMediaSearchResult inSearchResult);
        void PostProcess();
    }
}
