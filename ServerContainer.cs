using DVG.Core;
using DVG.SkyPirates.Server.Factories;
using DVG.SkyPirates.Server.IFactories;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Server.Presenters;
using DVG.SkyPirates.Server.Services;

using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Models;

using SimpleInjector;
using SimpleInjector.Diagnostics;
using SimpleInjector.Lifestyles;
using System.Diagnostics;

namespace DVG.SkyPirates.Server
{
    internal class ServerContainer : Container
    {
        public ServerContainer() : base()
        {
            Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            Register<Riptide.Server>(() => new Riptide.Server(), Lifestyle.Scoped);
            Register<IUnitViewFactory, NetworkedUnitViewFactory>(Lifestyle.Scoped);
            Register<IInputService, InputService>(Lifestyle.Scoped);
            Register<IUnitViewSyncer, NetworkedUnitViewSyncer>(Lifestyle.Scoped);
            Register<IClientConnectionService, ClientConnectionService>(Lifestyle.Scoped);
            Register<IMessageSendService, MessageSendService>(Lifestyle.Scoped);
            Register<IMessageRecieveService, MessageRecieveService>(Lifestyle.Scoped);

            Register<IPlayerLoopSystem, PlayerLoopSystem>(Lifestyle.Scoped);
            RegisterInitializer<IPlayerLoopItem>((item) => GetInstance<IPlayerLoopSystem>().Add(item));

            Register<IPathFactory<SquadModel>, ResourcesFactory<SquadModel>>(Lifestyle.Scoped);
            Register<IPathFactory<UnitModel>, ResourcesFactory<UnitModel>>(Lifestyle.Scoped);
            Register<IPathFactory<PackedCirclesModel>, ResourcesFactory<PackedCirclesModel>>(Lifestyle.Scoped);
            Register<IUnitModelFactory, UnitModelFactory>(Lifestyle.Scoped);
            Register<IUnitFactory, UnitFactory>(Lifestyle.Scoped);
            Register<IInputFactory, InputFactory>(Lifestyle.Scoped);

            Register<WorldPresenter>(Lifestyle.Scoped);

            Verify(VerificationOption.VerifyAndDiagnose);
            Analyze(this);
        }

        private static void Analyze(Container container)
        {
            foreach (var item in Analyzer.Analyze(container))
            {
                Debug.WriteLine(item.Description);
            }
        }
    }
}
