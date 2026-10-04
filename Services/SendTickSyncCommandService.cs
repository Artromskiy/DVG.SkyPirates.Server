using Delta.Netcode;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Server.Services
{
    public class SendTickSyncCommandService : ITickableExecutor
    {
        private readonly ICommandSender _commandSendService;

        public SendTickSyncCommandService(ICommandSender commandSendService)
        {
            _commandSendService = commandSendService;
        }

        public void Tick(int tick)
        {
            _commandSendService.SendToAll(SkyPiratesCommand.Create(0, tick, default(TickSyncCommand)));
        }
    }
}
