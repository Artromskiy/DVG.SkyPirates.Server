using DVG.Core;

namespace DVG.SkyPirates.Server.IServices
{
    public interface ICommandSendService
    {
        public void SendToAll<T>(Command<T> data, int exceptClient) where T : ICommandData;
        public void SendToAll<T>(Command<T> data) where T : ICommandData;
        public void SendTo<T>(Command<T> data, int clientId) where T : ICommandData;
    }
}
