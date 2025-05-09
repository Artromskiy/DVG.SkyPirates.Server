using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Server.Presenters;
using DVG.SkyPirates.Shared.Messages;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Server.Services
{
    internal sealed class InputService : IInputService, IDisposable
    {
        private readonly IMessageRecieveService _messageRecieve;
        private readonly Dictionary<int, InputPm> _inputs = new Dictionary<int, InputPm>();

        public InputService(IMessageRecieveService messageRecieve)
        {
            _messageRecieve = messageRecieve;
            _messageRecieve.RegisterReciever<InputGhost>(UpdateInput);
        }

        public void Dispose()
        {
            _messageRecieve.UnregisterReciever<InputGhost>(UpdateInput);
        }

        public void RegisterInput(InputPm inputPm, int clientId)
        {
            _inputs.Add(clientId, inputPm);
        }

        private void UpdateInput(InputGhost inputGhost, int clientId)
        {
            var input = _inputs[clientId];
            input.Fixation = inputGhost.fixation;
            input.Position = inputGhost.position;
            input.Rotation = inputGhost.rotation;
        }
    }
}
