using DVG.Core;
using DVG.SkyPirates.Server.Presenters;
using SimpleInjector.Lifestyles;
using System.Threading;

namespace DVG.SkyPirates.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ServerContainer serverScope = new ServerContainer();
            using var _scope = AsyncScopedLifestyle.BeginScope(serverScope);
            var world = _scope.GetInstance<WorldPresenter>();
            var playerLoop = _scope.GetInstance<IPlayerLoopSystem>();
            var server = _scope.GetInstance<Riptide.Server>();
            server.Start(7777, 16);
            while (true)
            {
                server.Update();
                playerLoop.Tick();
                playerLoop.FixedTick();
                Thread.Yield();
            }
        }
    }
}
