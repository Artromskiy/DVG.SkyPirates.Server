using DVG.Commands;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Services;
using Riptide;
using System.Collections.Generic;

namespace DVG.SkyPirates.Server.Services
{
    internal class CommandSender : ICommandSender
    {
        private readonly Riptide.Server _server;

        private readonly MessageIO _messageWriter;
        private readonly List<Message> _messages = new();

        public CommandSender(Riptide.Server server)
        {
            _server = server;
            _messageWriter = new MessageIO();
        }

        public void SendTo<T>(Command<T> data, int clientId)
        {
            _messages.Clear();
            _messageWriter.GetMessages(data, _messages);
            foreach (var message in _messages)
            {
                _server.Send(message, (ushort)clientId);
            }
        }

        public void SendToAll<T>(Command<T> data)
        {
            _messages.Clear();
            _messageWriter.GetMessages(data, _messages);
            foreach (var message in _messages)
            {
                _server.SendToAll(message);
            }
        }

        public void SendToAll<T>(Command<T> data, int exceptClient)
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