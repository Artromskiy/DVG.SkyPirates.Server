using Riptide.Utils;
using System;
using System.Net;
using System.Net.Sockets;

namespace DVG.SkyPirates.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ServerContainer container = new ServerContainer();
            var server = container.GetInstance<Riptide.Server>();
            server.ClientConnected += Server_ClientConnected;
            RiptideLogger.Initialize(Console.WriteLine, true);
            server.Start(7788, 16, useMessageHandlers: false);
            LogIPs();
            while (true)
            {
                server.Update();
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                {
                    Console.WriteLine("Started");
                    break;
                }
            }

            container.GetInstance<GameStartController>().Begin();
        }

        private static void Server_ClientConnected(object? sender, Riptide.ServerConnectedEventArgs e)
        {
            e.Client.MaxSendAttempts = 500;
            e.Client.TimeoutTime = 5_000;
        }

        private static void LogIPs()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName(), AddressFamily.InterNetwork);
            Console.WriteLine("IPs:");

            foreach (var ip in host.AddressList)
                Console.WriteLine(ip.ToString());
        }
    }
}
