using DVG.Commands;

namespace DVG.SkyPirates.Server.IServices
{
    public interface ICommandValidatorService
    {
        bool ValidateCommand<T>(Command<T> cmd);
    }
}
