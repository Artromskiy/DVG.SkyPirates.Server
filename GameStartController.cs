using DVG;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Services.Netcode;
using System;
using System.Diagnostics;
using System.Threading;

namespace DVG.SkyPirates.Server
{
    public sealed class GameStartController
    {
        private readonly Riptide.Server _server;
        private readonly SkyPiratesSessionProvider _session;
        private readonly ITickableService<IPreTickable> _preTickableService;
        private readonly ITickableService<IPostTickable> _postTickableService;
        private readonly Stopwatch _clock = new();

        public GameStartController(
            Riptide.Server server,
            SkyPiratesSessionProvider session,
            ITickableService<IPreTickable> preTickableService,
            ITickableService<IPostTickable> postTickableService)
        {
            _server = server;
            _session = session;
            _preTickableService = preTickableService;
            _postTickableService = postTickableService;
        }

        public void Loop()
        {
            _clock.Start();
            while (true)
            {
                long targetStep = _clock.Elapsed.Ticks * Constants.TicksPerSecond / TimeSpan.TicksPerSecond;
                if (_session.CurrentStep != targetStep)
                {
                    _server.Update();
                    int tick = checked((int)targetStep);
                    _preTickableService.Tick(tick);
                    _session.Tick(targetStep);
                    _postTickableService.Tick(tick);
                }

                Thread.Yield();
            }
        }
    }
}
