using DVG.Core;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;
using System;
using System.Diagnostics;

namespace DVG.SkyPirates.Server
{
    public class GameStartController
    {
        private static readonly fix TargetFrameRate = Constants.TicksPerSecond;
        private readonly Riptide.Server _server;
        private readonly ICommandSendService _sendService;
        private readonly ICommandRecieveService _recieveService;
        private readonly ITimelineService _timeline;


        Stopwatch _mainSw = new();
        Stopwatch _perfSw = new();

        public GameStartController(Riptide.Server server, ICommandSendService sendService, ICommandRecieveService recieveService, ITimelineService timeline)
        {
            _server = server;
            _sendService = sendService;
            _recieveService = recieveService;
            _timeline = timeline;
        }

        public void Begin()
        {
            _sendService.SendToAll(new Command<StartGameCommand>());
            foreach (var item in _server.Clients)
            {
                _recieveService.InvokeCommand(new Command<SpawnSquadCommand>(0, item.Id, 0, new()));
            }
            
            Loop();
        }

        private void Loop()
        {
            int lastFrame = 0;
            var frameTimeInMs = 1000 / TargetFrameRate;
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
            }
        }
    }
}
