using Delta.Netcode;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Services.Netcode;

namespace DVG.SkyPirates.Server.Services
{
    public class SendTickSyncCommandService : ITickableExecutor
    {
        private readonly SkyPiratesSessionProvider _session;

        public SendTickSyncCommandService(SkyPiratesSessionProvider session)
        {
            _session = session;
        }

        public void Tick(int tick)
        {
            _session.Send(default(TickSyncCommand), tick + 1L);
        }
    }
}
