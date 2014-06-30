using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Slouch
{
    public class StatusController : ApiController
    {
        public Status Get()
        {
            return Server.Instance.Status;

            //return new Status
            //{
            //    Downloads = new List<DownloadStatus>
            //    {
            //        new DownloadStatus
            //        {
            //            CompletionPercentage = 30,
            //            Media = new TvMediaItem("Testing Angular Binding")
            //        },
            //    }
            //};
        }
    }
}
