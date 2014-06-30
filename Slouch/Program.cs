using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Slouch
{
    public class Program
    {
        public static void Main(String[] args)
        {
            // Start server.
            System.Console.WriteLine("Starting server...");
            Server.Instance.Start();
            System.Console.WriteLine("Server started. Press escape to exit.");
            System.Console.WriteLine();

            Process.Start(Server.Instance._settings.Uri);

            // Wait until the user hits escape.
            while (System.Console.ReadKey().Key != ConsoleKey.Escape) { }

            // Stop server.
            System.Console.WriteLine("Stopping server...");
            Server.Instance.Stop();
        }
    }
}
