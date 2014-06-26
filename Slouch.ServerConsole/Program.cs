using Slouch.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            // Start server.
            Console.WriteLine("Starting server...");
            Server.Instance.Start();
            Console.WriteLine("Server started. Press escape to exit.");

            Process.Start(Server.Instance._settings.Uri);

            // Wait until the user hits escape.
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }

            // Stop server.
            Console.WriteLine("Stopping server...");
            Server.Instance.Stop();

            //var settingsArray = File.ReadAllLines(@"C:\temp\usenet.txt");
            //var settings = new Settings
            //{
            //    ServerUserName = settingsArray[0],
            //    ServerPassword = settingsArray[1],
            //    ServerHostName = settingsArray[2],
            //    ServerPort = Int32.Parse(settingsArray[3]),
            //    ServerUseSsl = true,
            //    NumberOfThreads = 30
            //};

            //var xml = XDocument.Load("http://slouch.nae.io/test.xml");
            //var nzb = GrouchInternalNzb.FromXml(xml);

            //var downloader = new GrouchDownloader(settings);
            //downloader.Download(nzb);
        }
    }
}
