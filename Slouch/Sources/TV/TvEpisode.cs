using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slouch
{
    [CabinetKey("SeriesId", "SeasonNumber", "Number")]
    public class TvEpisode : IMediaItem
    {
        // ===========================================================================
        // = Public Properties
        // ===========================================================================

        public Guid SeriesId { get; set; }
        public Int32 SeasonNumber { get; set; }
        public Int32 Number { get; set; }

        public DateTime AirDate { get; set; }
        public String Title { get; set; }

        // ===========================================================================
        // = IMediaItem Implementation
        // ===========================================================================

        public Guid? LibraryId { get; set; }
        public String LibraryFilePath { get; set; }
        public DateTime? LibraryAddedDate { get; set; }

        public MediaStatus Status { get; set; }

        public Boolean IsDue { get { return Status == MediaStatus.Wanted && AirDate <= DateTime.Today.Date; } }

        public String DisplayName
        {
            get
            {
                var series = Cabinet.GetAsync<TvSeries>(SeriesId).Result;

                return String.Format("{0} - Season {1} - Episode {2}",
                    series.Name,
                    SeasonNumber,
                    Number);
            }
        }

    }
}
