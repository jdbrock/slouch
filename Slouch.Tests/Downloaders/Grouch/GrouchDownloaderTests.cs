using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slouch;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.Linq;

namespace Slouch.Tests
{
    [TestClass()]
    public class GrouchDownloaderTests
    {
        //[TestMethod()]
        //public void DownloadTest()
        //{
        //    var settingsArray = File.ReadAllLines(@"C:\temp\usenet.txt");
        //    var settings = new Settings
        //    {
        //        ServerUserName = settingsArray[0],
        //        ServerPassword = settingsArray[1],
        //        ServerHostName = settingsArray[2],
        //        ServerPort = Int32.Parse(settingsArray[3]),
        //        ServerUseSsl = false,
        //        NumberOfThreads = 30
        //    };

        //    var xml = XDocument.Load("http://slouch.nae.io/test.xml");
        //    var nzb = GrouchInternalNzb.FromXml(xml);

        //    var downloader = new GrouchDownloader(settings);
        //    downloader.Download(nzb);
        //}

        //[TestMethod()]
        //public void DownloadTest2()
        //{
        //    var settingsArray = File.ReadAllLines(@"C:\temp\usenet.txt");
        //    var settings = new Settings
        //    {
        //        ServerUserName = settingsArray[0],
        //        ServerPassword = settingsArray[1],
        //        ServerHostName = settingsArray[2],
        //        ServerPort = Int32.Parse(settingsArray[3]),
        //        ServerUseSsl = true,
        //        NumberOfThreads = 30
        //    };

        //    var xml = XDocument.Load("http://slouch.nae.io/test2.xml");
        //    var nzb = GrouchInternalNzb.FromXml(xml);

        //    var downloader = new GrouchDownloader(settings);
        //    downloader.Download(nzb);
        //}
    }
}
