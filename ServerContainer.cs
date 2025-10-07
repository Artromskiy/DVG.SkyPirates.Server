using DVG.Core;
using DVG.SkyPirates.Server.Factories;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Server.Services;
using DVG.SkyPirates.Server.Services.CommandMutators;
using DVG.SkyPirates.Server.Services.CommandValidators;
using DVG.SkyPirates.Shared.DI;
using DVG.SkyPirates.Shared.Factories;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Services.CommandSerializers;
using Riptide.Transports.Udp;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using System;

namespace DVG.SkyPirates.Server
{
    internal class ServerContainer : Container
    {
        public ServerContainer() : base()
        {
            RegisterSingleton(() => new Riptide.Server(new UdpServer()));
            RegisterSingleton<ICommandSerializer, CompressedJsonUTF8Serializer>();
            RegisterSingleton<ICommandSendService, CommandSendService>();
            RegisterSingleton<ICommandRecieveService, CommandRecieveService>();
            RegisterSingleton<ICheatLoggerService, CheatLoggerService>();

            // Validate => Mutate => Execute
            var commandValidators = new Type[]
            {
                typeof(EmptyCommandValidator)
            };
            RegisterSingleton<ICommandValidatorService, CommandValidatorService>();
            Collection.Register<ICommandValidator>(commandValidators, Lifestyle.Singleton);

            var commandMutators = new Type[]
            {
                typeof(EmptyCommandMutator),
                typeof(SpawnCommandMutator)
            };
            RegisterSingleton<ICommandMutatorService, CommandMutatorService>();
            Collection.Register<ICommandMutator>(commandMutators, Lifestyle.Singleton);

            RegisterSingleton<IClientConnectionService, ClientConnectionService>();
            RegisterSingleton<CommandsResender>();

            RegisterSingleton(typeof(IPathFactory<>), typeof(ResourcesFactory<>));

            RegisterSingleton<GameStartController>();
            RegisterSingleton<WorldIniter>();

            var preTickableExecutors = Array.Empty<Type>();
            var postTickableExecutors = new Type[]
            {
                typeof(SendTickSyncCommandSystem)
            };
            Collection.Register<IPreTickableExecutor>(preTickableExecutors, Lifestyle.Singleton);
            Collection.Register<IPostTickableExecutor>(postTickableExecutors, Lifestyle.Singleton);

            SharedRegistration.Register(this);

            Verify(VerificationOption.VerifyAndDiagnose);
            Analyze(this);
        }

        private static void Analyze(Container container)
        {
            foreach (var item in Analyzer.Analyze(container))
                Console.WriteLine(item.Description);
        }
    }
}
