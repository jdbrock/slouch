using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch
{
    public class MovieMediaItem : IMediaItem
    {
        public String DisplayName { get; private set; }

        public Guid? LibraryId { get; set; }
        public String LibraryFilePath { get; set; }
        public DateTime? LibraryAddedDate { get; set; }

        public MediaStatus Status { get; set; }

        public Boolean IsDue { get { return false; } } // TODO

    }
}
