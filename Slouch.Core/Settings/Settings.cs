using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch.Core
{
    public class Settings
    {
        public Int32 NumberOfThreads { get; set; }

        public String ServerHostName { get; set; }
        public String ServerUserName { get; set; }
        public String ServerPassword { get; set; }
        public Int32 ServerPort { get; set; }
        public Boolean ServerUseSsl { get; set; }
    }
}
