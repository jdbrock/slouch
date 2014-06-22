using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slouch.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Diagnostics;
namespace Slouch.Core.Tests
{
    [TestClass()]
    public class GrouchInternalUsenetClientTests
    {
        [TestMethod()]
        public void DownloadArticleTest()
        {
            var settings = File.ReadAllLines(@"C:\temp\usenet.txt");
            var client = new GrouchInternalUsenetClient(settings[2], settings[0], settings[1], Int32.Parse(settings[3]), false);

            var tempDirectoryPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectoryPath);

            client.DownloadArticle("alt.binaries.teevee", "1403431970.54890.1@eu.news.astraweb.com", tempDirectoryPath);
        }
    }
}
