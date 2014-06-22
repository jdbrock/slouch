using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slouch.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
namespace Slouch.Core.Tests
{
    [TestClass()]
    public class GrouchInternalNzbTests
    {
        [TestMethod()]
        public void FromXmlTest()
        {
            var xml = XDocument.Load("http://slouch.nae.io/test.xml");
            var nzb = GrouchInternalNzb.FromXml(xml);

            Assert.AreEqual(2, nzb.MetaData.Count, "MetaData items not parsed correctly.");
            Assert.AreEqual(65, nzb.Files.Count, "Files not parsed correctly.");

            Assert.AreEqual("1403431970.54890.1@eu.news.astraweb.com", nzb.Files[0].Segments[0].MessageId, "Segments not parsed correctly.");
        }
    }
}
