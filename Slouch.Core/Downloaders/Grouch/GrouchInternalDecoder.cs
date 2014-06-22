using NntpClientLib;
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
    public class GrouchInternalDecoder : IArticleBodyProcessor, IArticleHeadersProcessor, IDisposable
    {
        // ===========================================================================
        // = Public Properties
        // ===========================================================================

        public YEncDecoder YEncDecoder { get; private set; }

        public String TargetDirectory { get; private set; }

        private Boolean _started;
        private Int64 _lastFileSize;
        private String _lastFileName;
        private Stream _lastHandle;

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

        private void EnsureAllocated(String inFileName, Int64 inFileSize)
        {
            //var fullPath = Path.Combine(TargetDirectory, inFileName);

            //if (File.Exists(fullPath))
            //    return;

            //using (var file = File.OpenWrite(fullPath))
            //{
            //    for (var i = 0; i < inFileSize; i++)
            //        file.WriteByte(0);

            //    file.Flush();
            //    file.Close();
            //}
        }

        // ===========================================================================
        // = IArticleBodyProcessor Implementation
        // ===========================================================================

        public void AddText(String inLine)
        {
            if (inLine.ToLower().Contains("=ybegin"))
            {
                if (_lastHandle != null)
                {
                    _lastHandle.Flush();
                    _lastHandle.Close();
                    _lastHandle.Dispose();
                }

                _lastFileSize = Int64.Parse(Regex.Match(inLine, "size=([0-9]*)").Groups[1].Value);
                _lastFileName = SafeFileName(Regex.Match(inLine, "name=(.*)$").Groups[1].Value);

                EnsureAllocated(_lastFileName, _lastFileSize);

                _lastHandle = File.Open(Path.Combine(TargetDirectory, _lastFileName), FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);

                _started = true;
                return;
            }

            if (!_started)
                return;

            if (inLine.ToLower().Contains("=yend"))
            {
                _lastHandle.Flush();
                _lastHandle.Close();
                _lastHandle.Dispose();
                _lastHandle = null;
                return;
            }

            if (inLine.ToLower().Contains("=ypart"))
            {
                var begin = Int64.Parse(Regex.Match(inLine, "begin=([0-9]*)").Groups[1].Value);
                var seek = begin - 1;

                _lastHandle.Flush();
                _lastHandle.Seek(seek, SeekOrigin.Begin);

                return;
            }

            var encoding = Encoding.GetEncoding("iso-8859-1");
            var line = encoding.GetBytes(inLine);

            var destSize = YEncDecoder.GetByteCount(line, 0, line.Length, true);
            var dest = new Byte[destSize];
            var bytesWritten = YEncDecoder.GetBytes(line, 0, line.Length, dest, 0, true);

            _lastHandle.Write(dest, 0, bytesWritten);
            _lastHandle.Flush();
        }

        // ===========================================================================
        // = IArticleHeadersProcessor Implementation
        // ===========================================================================

        public void AddHeader(String header, String value) { }
        public void AddHeader(String headerAndValue) { }

        // ===========================================================================
        // = IDisposable Implementation
        // ===========================================================================

        public void Dispose()
        {
            if (_lastHandle != null)
            {
                _lastHandle.Flush();
                _lastHandle.Close();
                _lastHandle.Dispose();
            }
        }

        //public void Decode(IEnumerable<String> inLines)
        //{
        //    foreach (var line in inLines)
        //        AddText(line);
        //}
    }
}
