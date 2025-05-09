using System;

namespace DVG.SkyPirates.Server.IServices
{
    internal class ClientConnectionService : IClientConnectionService
    {
        public event Action<ushort>? OnClientConnected;
        public event Action<ushort>? OnClientDisconnected;

        private readonly Riptide.Server _server;

        public ClientConnectionService(Riptide.Server server)
        {
            _server = server;

            _server.ClientConnected += (o, c) =>
            {
                Console.WriteLine("Connected");
                OnClientConnected?.Invoke(c.Client.Id);
            };
            _server.ClientDisconnected += (o, c) => OnClientDisconnected?.Invoke(c.Client.Id);
        }
    }
}
