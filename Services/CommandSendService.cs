using DVG.Core;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Services;
using Riptide;
using System.Collections.Generic;

namespace DVG.SkyPirates.Server.Services
{
    internal class CommandSendService : ICommandSendService
    {
        private readonly Riptide.Server _server;

        private readonly MessageIO _messageWriter;
        private readonly List<Message> _messages = new();

        public CommandSendService(Riptide.Server server, ICommandSerializer commandSerializer)
        {
            _server = server;
            _messageWriter = new MessageIO(commandSerializer);
        }

        public void SendTo<T>(Command<T> data, int clientId)
            where T : ICommandData
        {
            _messages.Clear();
            _messageWriter.GetMessages(data, _messages);
            foreach (var message in _messages)
            {
                _server.Send(message, (ushort)clientId);
            }
        }

        public void SendToAll<T>(Command<T> data)
            where T : ICommandData
        {
            _messages.Clear();
            _messageWriter.GetMessages(data, _messages);
            foreach (var message in _messages)
            {
                _server.SendToAll(message);
            }
        }

        public void SendToAll<T>(Command<T> data, int exceptClient)
            where T : ICommandData
        {
            _messages.Clear();
            _messageWriter.GetMessages(data, _messages);
            foreach (var message in _messages)
            {
                _server.SendToAll(message, (ushort)exceptClient);
            }
        }
    }
}