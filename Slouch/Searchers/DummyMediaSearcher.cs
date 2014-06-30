using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch
{
    public class DummyMediaSearcher : IMediaSearcher
    {
        public Boolean IsCompatibleWith(IMediaItem inItem)
        {
            return true;
        }

        public IMediaSearchResult Search(IMediaItem inItem)
        {
            return new DummyMediaSearchResult(inItem);
        }

        public class DummyMediaSearchResult : IMediaSearchResult, INzbMediaSearchResult
        {
            public Guid Id { get; private set; }
            public Boolean IsSuccess { get; private set; }
            public IMediaItem Item { get; private set; }

            public DummyMediaSearchResult(IMediaItem inItem)
            {
                Id = Guid.NewGuid();
                IsSuccess = true;
                Item = inItem;
            }

            public Uri NzbUri
            {
                get { return new Uri("http://slouch.nae.io/test.xml"); }
            }
        }
    }
}
