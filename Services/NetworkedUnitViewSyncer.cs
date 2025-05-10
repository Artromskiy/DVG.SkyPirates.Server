using DVG.Core;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IViews;
using System.Collections.Generic;

namespace DVG.SkyPirates.Server.Services
{
    internal class NetworkedUnitViewSyncer : IUnitViewSyncer, ITickable
    {
        private readonly Dictionary<IUnitView, int> _ghostToId = new();
        private int _lastAdded = 0;

        private readonly ICommandSendService _messageService;

        public NetworkedUnitViewSyncer(IClientConnectionService clientConnectionService, ICommandSendService messageService)
        {
            _messageService = messageService;
        }

        public void RegisterView(IUnitView view)
        {
            int id = ++_lastAdded;
            _ghostToId[view] = id;
            _messageService.SendToAll(new RegisterUnitCommand(id));
        }

        public void UnregisterView(IUnitView view)
        {
            _ghostToId.Remove(view, out var id);
            _messageService.SendToAll(new RegisterUnitCommand(id));
        }

        public void Tick()
        {
            foreach (var item in _ghostToId)
            {
                _messageService.SendToAll(new UpdateUnitCommand(item.Value, item.Key));
            }
        }
    }
}
