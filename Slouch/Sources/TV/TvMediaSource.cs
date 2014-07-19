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

        private IEnumerable<TvEpisode> GetAll()
        {
            var episodeKeys = Cabinet.GetSubkeys<TvEpisode>(true);

            foreach (var episodeKey in episodeKeys)
                yield return Cabinet.GetAsync<TvEpisode>(episodeKey).Result;
        }

        public IEnumerable<IMediaItem> GetJustArrived()
        {
            return GetAll()
                .OrderByDescending(X => X.LibraryAddedDate)
                .Take(20);
        }

        public IEnumerable<IMediaItem> GetDueItems()
        {
            return GetAll()
                .Where(X => X.IsDue);

            //TvEpisode episode;

            //if (!data.Exists<TvSeries>(seriesId))
            //{
            //    var series = new TvSeries
            //    {
            //        Id = seriesId,
            //        Name = "House of Cards"
            //    };

            //    await data.SetAsync(series, series.Id);

            //    var season = new TvSeason
            //    {
            //        SeriesId = series.Id,
            //        Number = 1
            //    };

            //    await data.SetAsync(season, series.Id, season.Number);

            //    episode = new TvEpisode
            //    {
            //        AirDate = DateTime.Now.Date,
            //        Number = 1,
            //        SeasonNumber = 1,
            //        SeriesId = series.Id,
            //        Title = "Uh oh..."
            //    };

            //    await data.SetAsync(episode, series.Id, season.Number, episode.Number);
            //}
            //else
            //    episode = await data.GetAsync<TvEpisode>(seriesId, 1, 1);

            //yield return new TvMediaItem(episode);
        }

        public void ChangeStatus(IMediaItem inItem, MediaStatus inStatus)
        {
            var item = (TvEpisode)inItem;
            item.Status = inStatus;

            Cabinet.SetAsync(item).Wait();
        }
    }
}
