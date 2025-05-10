using System;

namespace DVG.SkyPirates.Server.IServices
{
    public interface ICommandRecieveService
    {
        public void RegisterReciever<T>(Action<T, int> reciever) where T : unmanaged;
        public void UnregisterReciever<T>(Action<T, int> reciever) where T : unmanaged;
    }
}
