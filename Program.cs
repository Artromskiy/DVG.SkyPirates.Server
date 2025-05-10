using DVG.Core;
using DVG.SkyPirates.Server.Presenters;
using Riptide.Utils;
using SimpleInjector.Lifestyles;
using System;
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
            playerLoop.ExceptionHandler += Console.WriteLine;
            var server = _scope.GetInstance<Riptide.Server>();
            RiptideLogger.Initialize(Console.WriteLine, true);
            server.Start(7777, 16, useMessageHandlers: false);
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
