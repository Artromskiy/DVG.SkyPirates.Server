using DVG.Commands;
using DVG.Core;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
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
            RiptideLogger.Initialize(Console.WriteLine, true);
            Message.MaxPayloadSize = 256;
            _container = new ServerContainer();
            LogIPs();
            var server = _container.GetInstance<Riptide.Server>();

            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)) { }
            Console.WriteLine("Started");

            var worldDataLoader = _container.GetInstance<IPathFactory<WorldData>>();
            var history = _container.GetInstance<IHistorySystem>();
            var worldData = worldDataLoader.Create("Configs/Maps/Map1");
            history.ApplySnapshot(worldData);
            history.SaveBaseline();
            server.ClientConnected += ClientConnected;

            _container.GetInstance<GameStartController>().Loop();
        }

        private static void ClientConnected(object? sender, ServerConnectedEventArgs e)
        {
            e.Client.CanQualityDisconnect = false;
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
            var sendService = _container.GetInstance<ICommandSender>();
            var timeline = _container.GetInstance<ITimelineService>();
            var history = _container.GetInstance<IHistorySystem>();
            var commands = _container.GetInstance<ICommandExecutorService>();
            var timelineTick = timeline.CurrentTick;
            var timelineRollbackTick = timelineTick - Constants.ValidTicksCount;
            var worldData = history.GetSnapshot(timelineRollbackTick);

            var cmd = new Command<LoadWorldCommand>(0, timelineRollbackTick, new() { WorldData = worldData });
            sendService.SendTo(cmd, clientId);
            var sendCommands = new SendCommandsAction(sendService, commands, timelineRollbackTick, clientId);
            CommandsRegistry.ForEach(ref sendCommands);
        }

        private readonly struct SendCommandsAction : IGenericAction
        {
            private readonly ICommandSender _sendService;
            private readonly ICommandExecutorService _commandExecutor;
            private readonly int _tick;
            private readonly int _clientId;

            public SendCommandsAction(ICommandSender sendService, ICommandExecutorService commandExecutor, int tick, int clientId)
            {
                _sendService = sendService;
                _commandExecutor = commandExecutor;
                _tick = tick;
                _clientId = clientId;
            }

            public void Invoke<T>()
            {
                var commands = _commandExecutor.GetCommands<T>();
                if (commands == null)
                    return;
                foreach (var item in commands)
                {
                    if (item.Key > _tick)
                        foreach (var command in item.Value)
                            _sendService.SendTo(command, _clientId);
                }
            }
        }

        private static void CreateSquad(int clientId)
        {
            var recieveService = _container.GetInstance<ICommandReciever>();
            var timeline = _container.GetInstance<ITimelineService>();
            recieveService.InvokeCommand(new Command<SpawnSquadCommand>(clientId, timeline.CurrentTick + 1, new()));
        }
    }
}
