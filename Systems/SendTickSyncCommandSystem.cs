using DVG.Commands;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Server.Systems
{
    public class SendTickSyncCommandSystem : IPostTickableExecutor
    {
        private readonly ICommandSendService _commandSendService;

        public SendTickSyncCommandSystem(ICommandSendService commandSendService)
        {
            _commandSendService = commandSendService;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _commandSendService.SendToAll(new Command<TickSyncCommand>(0, tick, default));
        }
    }
}
