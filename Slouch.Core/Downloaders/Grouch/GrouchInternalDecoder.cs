using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using yEnc;

namespace Slouch.Core
{
    public class GrouchInternalDecoder
    {
        // ===========================================================================
        // = private Properties
        // ===========================================================================

        private YEncDecoder YEncDecoder { get; set; }
        private String TargetDirectory { get; set; }

        // ===========================================================================
        // = Construction
        // ===========================================================================

        public GrouchInternalDecoder(String inTargetDirectory)
        {
            YEncDecoder = new YEncDecoder();
            TargetDirectory = inTargetDirectory;
        }

        // ===========================================================================
        // = Private Methods
        // ===========================================================================

        private String SafeFileName(String inFileName)
        {
            return inFileName
                .Replace("..", "__")
                .Replace("\\", "_")
                .Replace("/", "_");
        }

        // ===========================================================================
        // = IArticleBodyProcessor Implementation
        // ===========================================================================

        public void Process(IEnumerable<String> inLines)
        {
            Boolean started = false;
            Int64 lastFileSize = 0;
            String lastFileName = null;
            Stream lastHandle = null;

            try
            {
                foreach (var line in inLines)
                {
                    if (line.ToLower().Contains("=ybegin"))
                    {
                        if (lastHandle != null)
                        {
                            lastHandle.Flush();
                            lastHandle.Close();
                            lastHandle.Dispose();
                        }

                        lastFileSize = Int64.Parse(Regex.Match(line, "size=([0-9]*)").Groups[1].Value);
                        lastFileName = SafeFileName(Regex.Match(line, "name=(.*)$").Groups[1].Value);

                        lastHandle = File.Open(Path.Combine(TargetDirectory, lastFileName), FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

                        started = true;
                        continue;
                    }

                    if (!started)
                        continue;

                    if (line.ToLower().Contains("=yend"))
                    {
                        lastHandle.Flush();
                        lastHandle.Close();
                        lastHandle.Dispose();
                        lastHandle = null;
                        continue;
                    }

                    if (line.ToLower().Contains("=ypart"))
                    {
                        var begin = Int64.Parse(Regex.Match(line, "begin=([0-9]*)").Groups[1].Value);
                        var seek = begin - 1;

                        lastHandle.Flush();
                        lastHandle.Seek(seek, SeekOrigin.Begin);

                        continue;
                    }

                    var encoding = Encoding.GetEncoding("iso-8859-1");
                    var lineBytes = encoding.GetBytes(line);

                    var destSize = YEncDecoder.GetByteCount(lineBytes, 0, lineBytes.Length, true);
                    var dest = new Byte[destSize];
                    var bytesWritten = YEncDecoder.GetBytes(lineBytes, 0, lineBytes.Length, dest, 0, true);

                    lastHandle.Write(dest, 0, bytesWritten);
                    lastHandle.Flush();
                }
            }
            finally
            {
                lastHandle.TryDispose();
            }
        }
    }
}
