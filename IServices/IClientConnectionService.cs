using System;

namespace DVG.SkyPirates.Server.IServices
{
    internal interface IClientConnectionService
    {
        event Action<int> OnClientConnected;
        event Action<int> OnClientDisconnected;
    }
}
