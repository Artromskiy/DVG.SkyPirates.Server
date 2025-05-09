using System;

namespace DVG.SkyPirates.Server.IServices
{
    internal interface IClientConnectionService
    {
        event Action<ushort> OnClientConnected;
        event Action<ushort> OnClientDisconnected;
    }
}
