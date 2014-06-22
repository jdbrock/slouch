using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slouch.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.Linq;

namespace Slouch.Core.Tests
{
    [TestClass()]
    public class GrouchDownloaderTests
    {
        [TestMethod()]
        public void DownloadTest()
        {
            var settings = File.ReadAllLines(@"C:\temp\usenet.txt");

            Func<GrouchInternalUsenetClient> clientFactory = 
                () => new GrouchInternalUsenetClient(settings[2], settings[0], settings[1], Int32.Parse(settings[3]), false);

            var xml = XDocument.Load("http://slouch.nae.io/test.xml");
            var nzb = GrouchInternalNzb.FromXml(xml);

            var downloader = new GrouchDownloader();
            downloader.Download(nzb, 30, clientFactory);
        }
    }
}
