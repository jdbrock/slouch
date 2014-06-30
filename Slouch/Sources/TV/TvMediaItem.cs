using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch
{
    public class TvMediaItem : IMediaItem
    {
        // ===========================================================================
        // = Public Properties
        // ===========================================================================
        
        public TvEpisode Episode { get; set; }

        public String DisplayName
        {
            get
            {
                return String.Format("{0} - Season {1} - Episode {2}",
                    Episode.Season.Series.Name,
                    Episode.Season.Number,
                    Episode.Number);
            }
        }

        // ===========================================================================
        // = Construction
        // ===========================================================================
        
        public TvMediaItem(TvEpisode inEpisode)
        {
            Episode = inEpisode;
        }
    }
}
