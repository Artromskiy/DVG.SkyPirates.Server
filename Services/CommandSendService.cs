using DVG.Core;
using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.IServices;
using Riptide;
using System;
using System.Buffers;

namespace DVG.SkyPirates.Server.Services
{
    internal class CommandSendService : ICommandSendService
    {
        private readonly Riptide.Server _server;
        private byte[] _tempBytes = Array.Empty<byte>();
        private readonly ICommandSerializer _commandSerializer;
        private readonly ArrayBufferWriter<byte> _buffer;

        public CommandSendService(Riptide.Server server, ICommandSerializer commandSerializer)
        {
            _buffer = new ArrayBufferWriter<byte>();
            _server = server;
            _commandSerializer = commandSerializer;
        }


        public void SendTo<T>(Command<T> data, int clientId)
            where T : ICommandData
        {
            var message = CreateMessage(data);
            _server.Send(message, (ushort)clientId);
        }

        public void SendToAll<T>(Command<T> data)
            where T : ICommandData
        {
            var message = CreateMessage(data);
            _server.SendToAll(message);
        }

        public void SendToAll<T>(Command<T> data, int exceptClient)
            where T : ICommandData
        {
            var message = CreateMessage(data);
            _server.SendToAll(message, (ushort)exceptClient);
        }

        private Message CreateMessage<T>(Command<T> data)
            where T : ICommandData
        {
            _buffer.ResetWrittenCount();
            _commandSerializer.Serialize(_buffer, ref data);
            int length = _buffer.WrittenCount;
            if (length > _tempBytes.Length)
                Array.Resize(ref _tempBytes, length);
            _buffer.WrittenSpan.CopyTo(_tempBytes);
            var id = data.CommandId;
            Message message = Message.Create(MessageSendMode.Reliable, (ushort)id);
            message.AddBytes(_tempBytes, 0, length);
            return message;
        }

    }
}
