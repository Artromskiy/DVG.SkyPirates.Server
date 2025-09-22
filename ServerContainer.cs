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
using DVG.SkyPirates.Shared.Systems.HistorySystems;
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
            Register(() => new Riptide.Server(new UdpServer()), Lifestyle.Singleton);
            Register<ICommandSerializer, JsonCommandSerializer>(Lifestyle.Singleton);
            Register<ICommandSendService, CommandSendService>(Lifestyle.Singleton);
            Register<ICommandRecieveService, CommandRecieveService>(Lifestyle.Singleton);
            Register<ICheatLoggerService, CheatLoggerService>(Lifestyle.Singleton);

            // Validate => Mutate => Execute
            var commandValidators = new Type[]
            {
                typeof(EmptyCommandValidator)
            };
            Register<ICommandValidatorService, CommandValidatorService>(Lifestyle.Singleton);
            Collection.Register<ICommandValidator>(commandValidators, Lifestyle.Singleton);

            var commandMutators = new Type[]
            {
                typeof(EmptyCommandMutator),
                typeof(SpawnCommandMutator)
            };
            Register<ICommandMutatorService, CommandMutatorService>(Lifestyle.Singleton);
            Collection.Register<ICommandMutator>(commandMutators, Lifestyle.Singleton);

            Register<IClientConnectionService, ClientConnectionService>(Lifestyle.Singleton);
            Register<CommandsResender>(Lifestyle.Singleton);

            Register(typeof(IPathFactory<>), typeof(ResourcesFactory<>), Lifestyle.Singleton);
            //Register<IPathFactory<SquadConfig>, ResourcesFactory<SquadConfig>>(Lifestyle.Singleton);
            //Register<IPathFactory<UnitConfig>, ResourcesFactory<UnitConfig>>(Lifestyle.Singleton);
            //Register<IPathFactory<PackedCirclesConfig>, ResourcesFactory<PackedCirclesConfig>>(Lifestyle.Singleton);
            Register<IUnitConfigFactory, UnitConfigFactory>(Lifestyle.Singleton);
            Register<IUnitFactory, UnitFactory>(Lifestyle.Singleton);
            Register<ISquadFactory, SquadFactory>(Lifestyle.Singleton);
            Register<GameStartController>(Lifestyle.Singleton);

            var postTickableExecutors = new Type[] { };
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
