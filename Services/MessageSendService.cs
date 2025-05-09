using DVG.SkyPirates.Server.IServices;
using DVG.SkyPirates.Shared.Messages;
using Riptide;
using System;
using System.Runtime.InteropServices;

namespace DVG.SkyPirates.Server.Services
{
    internal class MessageSendService : IMessageSendService
    {
        private readonly Riptide.Server _server;
        private byte[] _tempBytes = Array.Empty<byte>();

        public MessageSendService(Riptide.Server server)
        {
            _server = server;
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
            var id = MessageIds.GetMessageId<T>();
            var span = MemoryMarshal.CreateSpan(ref data, 1);
            Span<byte> spanBytes = MemoryMarshal.AsBytes(span);
            int targetLength = spanBytes.Length;
            if (_tempBytes.Length > targetLength)
                Array.Resize(ref _tempBytes, targetLength);
            spanBytes.CopyTo(_tempBytes);

            Message message = Message.Create(MessageSendMode.Reliable, id);
            message.AddBytes(_tempBytes, 0, targetLength);
            return message;
        }
    }
}
