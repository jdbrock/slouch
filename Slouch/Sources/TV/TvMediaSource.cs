using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch
{
    public class TvMediaSource : IMediaSource
    {
        // ===========================================================================
        // = IMediaSource Implementation
        // ===========================================================================
        
        public IEnumerable<IMediaItem> GetDueItems()
        {
            var episode = new TvEpisode
            {
                Number = 1,

                Season = new TvSeason
                {
                    Number = 1,

                    Series = new TvSeries
                    {
                        Id = Guid.NewGuid(),
                        Name = "House of Cards"
                    }
                }
            };

            yield return new TvMediaItem(episode);
        }

        public void ChangeStatus(IMediaItem inItem, MediaStatus inStatus)
        {
            throw new NotImplementedException();
        }
    }
}
