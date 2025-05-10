using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;
using Riptide;
using System;

namespace DVG.SkyPirates.Server.Services
{
    internal class CommandSendService : ICommandSendService
    {
        private readonly Riptide.Server _server;
        private byte[] _tempBytes = Array.Empty<byte>();
        private readonly ICommandSerializer _commandSerializer;

        public CommandSendService(Riptide.Server server, ICommandSerializer commandSerializer)
        {
            _server = server;
            _commandSerializer = commandSerializer;
        }


        public void SendTo<T>(T data, int clientId) where T : unmanaged
        {
            var message = CreateMessage(data);
            _server.Send(message, (ushort)clientId);
        }

        public void SendToAll<T>(T data) where T : unmanaged
        {
            var message = CreateMessage(data);
            _server.SendToAll(message);
        }

        private Message CreateMessage<T>(T data) where T : unmanaged
        {
            var span = _commandSerializer.Serialize(ref data);
            int length = span.Length;
            if (length > _tempBytes.Length)
                Array.Resize(ref _tempBytes, length);
            span.CopyTo(_tempBytes);

            var id = CommandIds.GetId<T>();
            Message message = Message.Create(MessageSendMode.Reliable, (ushort)id);
            message.AddBytes(_tempBytes, 0, length);
            return message;
        }
    }
}
