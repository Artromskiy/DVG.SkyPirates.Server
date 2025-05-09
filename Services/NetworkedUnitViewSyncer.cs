using DVG.Core;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.IViews;
using DVG.SkyPirates.Shared.Messages;
using System.Collections.Generic;

namespace DVG.SkyPirates.Server.Services
{
    internal class NetworkedUnitViewSyncer : IUnitViewSyncer, ITickable
    {
        private readonly Dictionary<IUnitView, uint> _ghostToId = new();
        private uint _lastAdded = 0;

        private readonly IMessageSendService _messageService;

        public NetworkedUnitViewSyncer(IClientConnectionService clientConnectionService, IMessageSendService messageService)
        {
            //clientConnectionService.OnClientConnected += SendSynced;
            _messageService = messageService;
        }

        //private void SendSynced(ushort clientId)
        //{
        //    _messageService.SendTo(new UnitSync(_ghostToId.Values.ToArray()), clientId);
        //}

        public void RegisterView(IUnitView view)
        {
            uint id = ++_lastAdded;
            _ghostToId[view] = id;
            _messageService.SendToAll(new RegisterUnit(id));
        }

        public void UnregisterView(IUnitView view)
        {
            _ghostToId.Remove(view, out var id);
            _messageService.SendToAll(new RegisterUnit(id));
        }

        public void Tick()
        {
            foreach (var item in _ghostToId)
            {
                _messageService.SendToAll(new UnitGhost(item.Value, item.Key));
            }
        }
    }
}
