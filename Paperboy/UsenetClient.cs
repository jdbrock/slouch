using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Paperboy
{
    public class UsenetClient : IDisposable
    {
        // ===========================================================================
        // = Private Properties
        // ===========================================================================
        
        private String HostName { get; set; }
        private Int32 Port { get; set; }
        private String UserName { get; set; }
        private String Password { get; set; }
        private Boolean UseSsl { get; set; }

        // ===========================================================================
        // = Private Fields
        // ===========================================================================
        
        private static readonly Encoding _encoding = Encoding.GetEncoding("iso-8859-1");

        private TcpClient _client;
        private TextWriter _writer;
        private TextReader _reader;

        private String _selectedGroup;

        // ===========================================================================
        // = Construction
        // ===========================================================================
        
        public UsenetClient(String inHostName, Int32 inPort, String inUserName, String inPassword, Boolean inUseSsl)
        {
            HostName = inHostName;
            Port = inPort;
            UserName = inUserName;
            Password = inPassword;
            UseSsl = inUseSsl;
        }

        // ===========================================================================
        // = Public Methods
        // ===========================================================================

        public IEnumerable<String> TryGetArticle(IEnumerable<String> inGroups, String inMessageId)
        {
            EnsureConnected();
            EnsureGroupSelected(inGroups);

            var messageId = inMessageId;

            if (!messageId.StartsWith("<"))
                messageId = "<" + messageId;

            if (!messageId.EndsWith(">"))
                messageId = messageId + ">";

            WriteCommand("ARTICLE {0}", messageId);

            if (ReadResponseCode() != ResponseCode.ArticleRetrieved)
                yield break;

            var readingHeader = true;

            while (true)
            {
                var line = _reader.ReadLine();

                if (line == ".")
                    yield break;

                if (line.StartsWith(".."))
                    line = line.Substring(1);

                if (readingHeader)
                {
                    if (line.Length == 0)
                        readingHeader = false;
                }
                else
                    yield return line;
            }
        }

        // ===========================================================================
        // = Private Methods
        // ===========================================================================

        private void EnsureConnected()
        {
            if (_client == null)
                Connect();
        }

        private void Connect()
        {
            _client = new TcpClient(HostName, Port);

            var stream = (Stream)_client.GetStream();

            if (UseSsl)
            {
                var sslStream = new SslStream(stream);
                sslStream.AuthenticateAsClient(HostName);
                stream = sslStream;
            }

            var writer = new StreamWriter(stream, _encoding);
            writer.AutoFlush = true;
            _writer = writer;

            _reader = new UsenetStreamReader(stream, _encoding);

            var response = ReadResponseCode();

            if (response != ResponseCode.ServerReadyPostingAllowed && response != ResponseCode.ServerReadyNoPostingAllowed)
                throw new ApplicationException("Unexpected response code on connect.");

            Authenticate();
        }

        private void Authenticate()
        {
            WriteCommand("AUTHINFO USER {0}", UserName);
            var userResponse = ReadResponseCode();

            if (userResponse == ResponseCode.PasswordRequired)
            {
                WriteCommand("AUTHINFO PASS {0}", Password);
                var passwordResponse = ReadResponseCode();

                if (passwordResponse != ResponseCode.AuthenticationAccepted)
                    throw new ApplicationException("Authentication failed.");
            }
        }

        private void EnsureGroupSelected(IEnumerable<String> inGroups)
        {
            if (_selectedGroup != null && inGroups.Contains(_selectedGroup))
                return;

            foreach (var group in inGroups)
                if (TrySelectGroup(group))
                {
                    _selectedGroup = group;
                    return;
                }
        }

        private Boolean TrySelectGroup(String inGroup)
        {
            WriteCommand("GROUP {0}", inGroup);

            var response = ReadResponseCode();

            if (response == ResponseCode.NoSuchNewsgroup)
                return false;

            if (response == ResponseCode.NewsgroupSelected)
                return true;

            throw new ApplicationException("Unexpected response to GROUP command: " + response);
        }

        private void WriteCommand(String inFormat, params Object[] inParams)
        {
            _writer.WriteLine(String.Format(inFormat, inParams));
        }

        private String ReadResponse()
        {
            return _reader.ReadLine();
        }

        private ResponseCode ReadResponseCode()
        {
            var response = _reader.ReadLine();
            response = response.Substring(0, 3);

            return (ResponseCode)Convert.ToInt32(response);
        }

        // ===========================================================================
        // = IDisposable Implementation
        // ===========================================================================
        
        public void Dispose()
        {
            _writer.TryDispose();
            _reader.TryDispose();

            try { _client.Close(); } catch { }

            _client.TryDispose();
        }
    }
}
