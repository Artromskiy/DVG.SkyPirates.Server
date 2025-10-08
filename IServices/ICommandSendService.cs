using DVG.Core;

namespace DVG.SkyPirates.Server.IServices
{
    public interface ICommandSendService
    {
        void SendToAll<T>(Command<T> data, int exceptClient) where T : ICommandData;
        void SendToAll<T>(Command<T> data) where T : ICommandData;
        void SendTo<T>(Command<T> data, int clientId) where T : ICommandData;
    }
}
