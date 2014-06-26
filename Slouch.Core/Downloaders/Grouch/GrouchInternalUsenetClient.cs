using Paperboy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEnc;

namespace Slouch.Core
{
    public class GrouchInternalUsenetClient : IDisposable
    {
        // ===========================================================================
        // = Public Properties
        // ===========================================================================
        
        public String HostName { get; private set; }
        public String UserName { get; private set; }
        public String Password { get; private set; }
        public Boolean UseSsl { get; private set; }
        public Int32 Port { get; private set; }

        // ===========================================================================
        // = Private Fields
        // ===========================================================================
        
        private UsenetClient _client;

        // ===========================================================================
        // = Construction
        // ===========================================================================

        public GrouchInternalUsenetClient(String inHostName, String inUserName, String inPassword, Int32 inPort, Boolean inUseSsl)
        {
            HostName = inHostName;
            UserName = inUserName;
            Password = inPassword;
            UseSsl = inUseSsl;
            Port = inPort;

            _client = new UsenetClient(inHostName, inPort, inUserName, inPassword, inUseSsl);
        }

        // ===========================================================================
        // = Public Methods
        // ===========================================================================

        public void DownloadArticle(String inGroup, String inMessageId, String inTargetDirectory)
        {
            DownloadArticle(new String[] { inGroup }, inMessageId, inTargetDirectory);
        }

        public void DownloadArticle(IEnumerable<String> inGroups, String inMessageId, String inTargetDirectory)
        {
            var decoder = new GrouchInternalDecoder(inTargetDirectory);
            var data = _client.TryGetArticle(inGroups, inMessageId);

            decoder.Process(data);
        }

        // ===========================================================================
        // = IDisposable Implementation
        // ===========================================================================
        
        public void Dispose()
        {
            _client.TryDispose();
        }
    }
}
