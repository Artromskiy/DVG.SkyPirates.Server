using DVG.Commands;
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
        private readonly ICommandRecieveService _recieveService;
        private readonly Stopwatch _mainSw = new();
        private readonly Stopwatch _perfSw = new();

        public GameStartController(Riptide.Server server, ITimelineService timeline, ICommandRecieveService recieveService)
        {
            _server = server;
            _timeline = timeline;
            _recieveService = recieveService;
            var subscrive = new DirtyCommandCallback(_timeline, _recieveService);
            CommandsRegistry.ForEach(ref subscrive);
        }

        public void Loop()
        {
            _mainSw.Start();
            while (true)
            {
                var ticks = _mainSw.Elapsed.Ticks;
                int tickFrame = (int)(ticks * Constants.TicksPerSecond / 1000 / 10000);
                if (_timeline.CurrentTick != tickFrame)
                {
                    _perfSw.Restart();
                    _server.Update();
                    _timeline.TickTo(tickFrame);
                    _perfSw.Stop();
                    Console.WriteLine($"Elapsed: {_perfSw.Elapsed.TotalMilliseconds}");
                }

                Thread.Yield();
            }
        }

        private readonly struct DirtyCommandCallback : IGenericAction
        {
            private readonly ITimelineService _timelineService;
            private readonly ICommandRecieveService _commandRecieveService;

            public DirtyCommandCallback(ITimelineService timelineService, ICommandRecieveService commandRecieveService)
            {
                _timelineService = timelineService;
                _commandRecieveService = commandRecieveService;
            }

            public void Invoke<T>()
            {
                var timeline = _timelineService;
                _commandRecieveService.RegisterReciever<T>((c) =>
                {
                    timeline.DirtyTick = Maths.Min(timeline.DirtyTick, c.Tick);
                });
            }
        }
    }
}
