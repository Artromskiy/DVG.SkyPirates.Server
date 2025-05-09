using DVG.SkyPirates.Server.Presenters;

namespace DVG.SkyPirates.Server.IServices
{
    internal interface IInputService
    {
        void RegisterInput(InputPm inputPm, int clientId);
    }
}
