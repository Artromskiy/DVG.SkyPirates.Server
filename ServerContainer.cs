using DVG.Core;
using DVG.SkyPirates.Server.Factories;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Server.Services;
using DVG.SkyPirates.Shared.DI;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Services;
using Riptide.Transports.Udp;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using System;

namespace DVG.SkyPirates.Server
{
    internal class ServerContainer : SharedContainer
    {
        public ServerContainer()
        {
            RegisterSingleton(() =>
            {
                var server = new Riptide.Server(new UdpServer());
                server.Start(7788, 16, useMessageHandlers: false);
                server.HeartbeatInterval = Constants.TickDurationMs;
                server.TimeoutTime = Constants.MaxHistoryDurationMs;
                return server;
            });

            RegisterSingleton<ICommandSender, CommandSender>();
            RegisterSingleton<ICommandReciever, CommandReciever>();
            RegisterSingleton<ICheatLoggerService, CheatLoggerService>();
            RegisterSingleton<ITickCounterService, TickCounterService>();
            RegisterSingleton(typeof(IPathFactory<>), typeof(ResourcesFactory<>));

            RegisterSingleton<CommandsResender>();
            RegisterSingleton<GameStartController>();
            RegisterSingleton<IHashSumService, HashSumService>();

            Collection.Register<IPreTickable>(PreTickables, Lifestyle.Singleton);
            Collection.Register<IPostTickable>(PostTickables, Lifestyle.Singleton);
            Collection.Register<IInTickable>(InTickables, Lifestyle.Singleton);

            Verify(VerificationOption.VerifyAndDiagnose);
            Analyze(this);
        }

        private static void Analyze(Container container)
        {
            foreach (var item in Analyzer.Analyze(container))
                Console.WriteLine(item.Description);
        }

        private static Type[] PreTickables => new Type[]
        {
            typeof(ITickCounterService)
        };

        private static Type[] PostTickables => new Type[]
        {
            typeof(SendTickSyncCommandService),
            typeof(TimelineWriter),
            typeof(TimelineSaver),
        };

        private readonly Type[] InTickables = new Type[]
        {
            //typeof(IHashSumService)
        };
    }
}
