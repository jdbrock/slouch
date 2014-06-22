using Slouch.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Slouch.ServerConsole
{
    public class Program
    {
        public static void Main(String[] args)
        {
            //var s = new Server();
            //s.Start();

            //while (Console.ReadKey().Key != ConsoleKey.Escape);

            Console.WriteLine("Downloading...");

            var settings = File.ReadAllLines(@"C:\temp\usenet.txt");

            Func<GrouchInternalUsenetClient> clientFactory =
                () => new GrouchInternalUsenetClient(settings[2], settings[0], settings[1], Int32.Parse(settings[3]), false);

            var xml = XDocument.Load("http://slouch.nae.io/test.xml");
            var nzb = GrouchInternalNzb.FromXml(xml);

            var downloader = new GrouchDownloader();
            downloader.Download(nzb, 30, clientFactory);

            Console.WriteLine("Done.");
            Console.ReadKey();
        }
    }
}
