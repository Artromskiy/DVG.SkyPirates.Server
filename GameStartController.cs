using DVG.SkyPirates.Shared;
using DVG.SkyPirates.Shared.IServices;
using System;
using System.Diagnostics;
using System.Threading;

namespace DVG.SkyPirates.Server
{
    public class GameStartController
    {
        private readonly Riptide.Server _server;
        private readonly ITimelineService _timeline;
        private readonly WorldIniter _worldIniter;
        private readonly Stopwatch _mainSw = new();
        private readonly Stopwatch _perfSw = new();

        public GameStartController(Riptide.Server server, ITimelineService timeline, WorldIniter worldIniter)
        {
            _server = server;
            _timeline = timeline;
            _worldIniter = worldIniter;
        }

        public void Begin()
        {
            _worldIniter.Init();
            //Thread.Sleep(1000);
            Loop();
        }

        private void Loop()
        {
            int lastFrame = 0;
            var frameTimeInMs = (fix)1000 / Constants.TicksPerSecond;
            _mainSw.Start();

            while (true)
            {
                _server.Update();
                var ticks = _mainSw.Elapsed.Ticks;
                int ms = (int)(ticks / 10000);
                int tickFrame = (int)(ms / frameTimeInMs);
                for (int i = lastFrame; i < tickFrame; i++)
                {
                    _perfSw.Restart();
                    _timeline.Tick();
                    
                    _perfSw.Stop();
                    Console.WriteLine($"Elapsed: {_perfSw.Elapsed.TotalMilliseconds}");
                }
                lastFrame = tickFrame;
                Thread.Yield();
            }
        }
    }
}
