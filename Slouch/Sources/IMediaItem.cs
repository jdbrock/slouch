using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch
{
    public interface IMediaItem
    {
        String DisplayName { get; }

        Guid? LibraryId { get; }
        String LibraryFilePath { get; }
        DateTime? LibraryAddedDate { get; }

        MediaStatus Status { get; }

        Boolean IsDue { get; }
    }
}
