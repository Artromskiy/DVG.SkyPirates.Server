using DVG.Core;
using DVG.SkyPirates.Server.Factories;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Server.Services;
using DVG.SkyPirates.Server.Services.CommandMutators;
using DVG.SkyPirates.Server.Services.CommandValidators;
using DVG.SkyPirates.Server.Systems;
using DVG.SkyPirates.Shared.DI;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Services.CommandSerializers;
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
                server.HeartbeatInterval = 1000 / Constants.TicksPerSecond;
                server.Start(7788, 16, useMessageHandlers: false);
                server.TimeoutTime = 3_000;
                return server;
            });
            RegisterSingleton<ICommandSerializer, CompressedJsonUTF8Serializer>();
            RegisterSingleton<ICommandSender, CommandSender>();
            RegisterSingleton<ICommandReciever, CommandReciever>();
            RegisterSingleton<ICheatLoggerService, CheatLoggerService>();

            RegisterSingleton(typeof(IPathFactory<>), typeof(ResourcesFactory<>));

            RegisterSingleton<CommandsResender>();
            RegisterSingleton<GameStartController>();

            // Validate => Mutate => Execute
            RegisterSingleton<ICommandValidatorService, CommandValidatorService>();
            RegisterSingleton<ICommandMutatorService, CommandMutatorService>();

            Collection.Register<ICommandValidator>(CommandValidators, Lifestyle.Singleton);
            Collection.Register<ICommandMutator>(CommandMutators, Lifestyle.Singleton);
            Collection.Register<IPreTickableExecutor>(PreTickableExecutors, Lifestyle.Singleton);
            Collection.Register<IPostTickableExecutor>(PostTickableExecutors, Lifestyle.Singleton);

            Verify(VerificationOption.VerifyAndDiagnose);
            Analyze(this);
        }

        private static void Analyze(Container container)
        {
            foreach (var item in Analyzer.Analyze(container))
                Console.WriteLine(item.Description);
        }

        private static Type[] CommandValidators => new Type[]
        {
            //typeof(EmptyCommandValidator)
        };

        private static Type[] CommandMutators => new Type[]
        {
            //typeof(EmptyCommandMutator),
            typeof(SpawnCommandMutator)
        };

        private static Type[] PreTickableExecutors => new Type[]
        {

        };

        private static Type[] PostTickableExecutors => new Type[]
        {
            typeof(SendTickSyncCommandSystem)
        };
    }
}
