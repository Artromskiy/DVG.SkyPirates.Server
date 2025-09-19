using DVG.Core;

namespace DVG.SkyPirates.Server.IServices
{
    public interface ICommandValidatorService
    {
        bool ValidateCommand<T>(Command<T> cmd) where T : ICommandData;
        bool ValidateClientId<T>(int clientId, Command<T> cmd) where T : ICommandData;
    }
}
