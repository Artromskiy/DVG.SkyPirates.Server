using DVG.Core;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;
using Riptide;
using Riptide.Utils;
using SimpleInjector;
using System;
using System.Net;
using System.Net.Sockets;

namespace DVG.SkyPirates.Server
{
    internal class Program
    {
        private static Container _container;

        private static void Main(string[] args)
        {
            _container = new ServerContainer();
            RiptideLogger.Initialize(Console.WriteLine, true);
            Message.MaxPayloadSize = 256;

            var server = _container.GetInstance<Riptide.Server>();
            server.ClientDisconnected += Server_ClientDisconnected;
            server.ClientConnected += Server_ClientConnected;
            server.HeartbeatInterval = 1000 / Constants.TicksPerSecond;
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

            _container.GetInstance<GameStartController>().Begin();
        }

        private static void Server_ClientDisconnected(object? sender, ServerDisconnectedEventArgs e)
        {
            Console.WriteLine($"Client {e.Client} disconnected: {e.Reason}");
        }

        private static void Server_ClientConnected(object? sender, ServerConnectedEventArgs e)
        {
            e.Client.MaxSendAttempts = 500;
            e.Client.TimeoutTime = 10_000;
            e.Client.CanQualityDisconnect = true;
            SendSyncData(e.Client.Id);
            CreateSquad(e.Client.Id);
        }

        private static void LogIPs()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName(), AddressFamily.InterNetwork);
            Console.WriteLine("IPs:");

            foreach (var ip in host.AddressList)
                Console.WriteLine(ip.ToString());
        }

        private static void SendSyncData(int clientId)
        {
            var sendService = _container.GetInstance<ICommandSendService>();
            var timeline = _container.GetInstance<ITimelineService>();

            var cmdData = timeline.GetIniter();
            var cmd = new Command<LoadWorldCommand>(0, clientId, timeline.CurrentTick, cmdData);

            sendService.SendTo(cmd, clientId);
        }

        private static void CreateSquad(int clientId)
        {
            var recieveService = _container.GetInstance<ICommandRecieveService>();
            var timeline = _container.GetInstance<ITimelineService>();
            recieveService.InvokeCommand(new Command<SpawnSquadCommand>(0, clientId, timeline.CurrentTick, new()));
        }
    }
}
