using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slouch
{
    public class TvEpisode
    {
        // ===========================================================================
        // = Public Properties
        // ===========================================================================
        
        public TvSeason Season { get; set; }
        public Int32 Number { get; set; }
        public DateTime AirDate { get; set; }
        public String Title { get; set; }
    }
}
