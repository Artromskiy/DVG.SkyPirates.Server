using DVG.SkyPirates.Server.IServices;
using System;

namespace DVG.SkyPirates.Server.Services
{
    internal class ClientConnectionService : IClientConnectionService
    {
        public event Action<int>? OnClientConnected;
        public event Action<int>? OnClientDisconnected;

        private readonly Riptide.Server _server;

        public ClientConnectionService(Riptide.Server server)
        {
            _server = server;

            _server.ClientConnected += (o, c) =>
            {
                Console.WriteLine("Connected");
                OnClientConnected?.Invoke(c.Client.Id);
            };
            _server.ClientDisconnected += (o, c) =>
            {
                Console.WriteLine("Disconnected");
                OnClientDisconnected?.Invoke(c.Client.Id);
            };
        }
    }
}
