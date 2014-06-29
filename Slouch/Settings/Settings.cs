using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch
{
    public class Settings
    {
        public Int32 NumberOfThreads { get; set; }

        public String ServerHostName { get; set; }
        public String ServerUserName { get; set; }
        public String ServerPassword { get; set; }
        public Int32 ServerPort { get; set; }
        public Boolean ServerUseSsl { get; set; }

        public String Uri { get; set; }

        public Settings()
        {
            // Set defaults.
            NumberOfThreads = 30;
            ServerUseSsl = true;
            Uri = "http://localhost:9000";
        }
    }
}
