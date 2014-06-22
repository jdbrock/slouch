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

            //Console.WriteLine("Downloading...");

            //var settingsArray = File.ReadAllLines(@"C:\temp\usenet.txt");
            //var settings = new Settings
            //{
            //    ServerUserName  = settingsArray[0],
            //    ServerPassword  = settingsArray[1],
            //    ServerHostName  = settingsArray[2],
            //    ServerPort      = Int32.Parse(settingsArray[3]),
            //    ServerUseSsl    = false,
            //    NumberOfThreads = 30
            //};

            //var xml = XDocument.Load("http://slouch.nae.io/test.xml");
            //var nzb = GrouchInternalNzb.FromXml(xml);

            //var downloader = new GrouchDownloader(settings);
            //downloader.Download(nzb);

            //Console.WriteLine("Done.");
            //Console.ReadKey();
        }
    }
}
