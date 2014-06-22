using NntpClientLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yEnc;

namespace Slouch.Core
{
    public class GrouchInternalUsenetClient
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
        
        private Rfc977NntpClientWithExtensions _client;
        private Boolean _initialised;

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

            _client = new Rfc977NntpClientWithExtensions();
        }

        // ===========================================================================
        // = Public Methods
        // ===========================================================================

        public void DownloadArticle(String inGroup, String inMessageId, String inTargetDirectory)
        {
            EnsureInitialised();

            var messageId = CreateValidMessageId(inMessageId);

            var decoder = new GrouchInternalDecoder(inTargetDirectory);
            _client.SelectNewsgroup(inGroup);
            _client.RetrieveArticle(messageId, decoder, decoder);
        }

        // ===========================================================================
        // = Private Methods
        // ===========================================================================
        
        private String CreateValidMessageId(String inMessageId)
        {
            var messageId = inMessageId;

            if (!messageId.StartsWith("<"))
                messageId = "<" + messageId;

            if (!messageId.EndsWith(">"))
                messageId = messageId + ">";

            return messageId;
        }

        private void EnsureInitialised()
        {
            if (_initialised)
                return;

            _client.Connect(HostName, Port, UseSsl);
            _client.AuthenticateUser(UserName, Password);

            _initialised = true;
        }
    }
}
