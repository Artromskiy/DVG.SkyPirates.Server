using DVG.Commands;

namespace DVG.SkyPirates.Server.IServices
{
    public interface ICommandSender
    {
        void SendToAll<T>(Command<T> data, int exceptClient);
        void SendToAll<T>(Command<T> data);
        void SendTo<T>(Command<T> data, int clientId);
    }
}
