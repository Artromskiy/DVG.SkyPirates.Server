using DVG.SkyPirates.Shared.Messages;
using Riptide;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DVG.SkyPirates.Server.IServices
{
    internal class MessageRecieveService : IMessageRecieveService
    {
        private readonly Riptide.Server _server;
        private readonly Dictionary<int, Action<Message, int>> _registeredRecievers;

        public MessageRecieveService(Riptide.Server server)
        {
            _registeredRecievers = new Dictionary<int, Action<Message, int>>();
            _server = server;
            _server.MessageReceived += OnMessageRecieved;
        }

        private void OnMessageRecieved(object? _, MessageReceivedEventArgs e)
        {
            if (_registeredRecievers.TryGetValue(e.MessageId, out var callback))
                callback.Invoke(e.Message, e.FromConnection.Id);
        }

        public void RegisterReciever<T>(Action<T, int> reciever) where T: unmanaged
        {
            if(!ActionContainer<T>.HasTargets)
                _registeredRecievers.Add(MessageIds.GetMessageId<T>(), ActionContainer<T>.Invoke);
            ActionContainer<T>.Recievers += reciever;
        }

        public void UnregisterReciever<T>(Action<T, int> reciever) where T : unmanaged
        {
            ActionContainer<T>.Recievers -= reciever;
            if (!ActionContainer<T>.HasTargets)
                _registeredRecievers.Remove(MessageIds.GetMessageId<T>());
        }

        private static class ActionContainer<T> where T: unmanaged
        {
            public static event Action<T, int>? Recievers;
            private static byte[] _tempBytes = Array.Empty<byte>();
            public static bool HasTargets => Recievers?.GetInvocationList().Length > 0;

            public static void Invoke(Message m, int clientId)
            {
                var data = GetData(m);
                Recievers?.Invoke(data, clientId);
            }

            private static T GetData(Message message)
            {
                var bytesLength = message.UnreadBits / 8;
                if (_tempBytes.Length < bytesLength)
                    Array.Resize(ref _tempBytes, bytesLength);
                message.GetBytes(_tempBytes);
                return MemoryMarshal.Read<T>(_tempBytes.AsSpan(0, bytesLength));
            }
        }

    }
}
