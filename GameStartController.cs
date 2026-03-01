using DVG.Commands;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Diagnostics;
using System.Threading;

namespace DVG.SkyPirates.Server
{
    public class GameStartController
    {
        private readonly Riptide.Server _server;
        private readonly ITimelineService _timeline;
        private readonly ITickableService<IPreTickable> _preTickableService;
        private readonly ITickableService<IPostTickable> _postTickableService;
        private readonly ICommandReciever _recieveService;
        private readonly Stopwatch _mainSw = new();
        private readonly Stopwatch _perfSw = new();

        public GameStartController(Riptide.Server server, ITimelineService timeline, ICommandReciever recieveService, ITickableService<IPreTickable> preTickableService, ITickableService<IPostTickable> postTickableService)
        {
            _server = server;
            _timeline = timeline;
            _preTickableService = preTickableService;
            _postTickableService = postTickableService;

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
                int tickFrame = (int)(ticks * Constants.TicksPerSecond / 10_000_000);
                if (_timeline.CurrentTick != tickFrame)
                {
                    _perfSw.Restart();
                    _server.Update();
                    _preTickableService.Tick(tickFrame);
                    _timeline.Tick(tickFrame);
                    _postTickableService.Tick(tickFrame);
                    _perfSw.Stop();
                    Console.WriteLine($"Elapsed: {_perfSw.Elapsed.TotalMilliseconds}");
                }

                Thread.Yield();
            }
        }

        private readonly struct DirtyCommandCallback : IGenericAction
        {
            private readonly ITimelineService _timelineService;
            private readonly ICommandReciever _commandRecieveService;

            public DirtyCommandCallback(ITimelineService timelineService, ICommandReciever commandRecieveService)
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
