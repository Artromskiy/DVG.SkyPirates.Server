using Delta.Netcode;

namespace DVG.SkyPirates.Server.IServices
{
    public interface ICommandValidatorService
    {
        bool IsValid<T>(Command<T> cmd);
    }
}
