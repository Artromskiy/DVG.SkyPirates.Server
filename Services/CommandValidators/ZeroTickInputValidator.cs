using DVG.Commands;
using DVG.SkyPirates.Server.IServices;

namespace DVG.SkyPirates.Server.Services.CommandValidators
{
    internal class ZeroTickInputValidator : IGeneralCommandValidator
    {
        public bool Validate<T>(Command<T> cmd)
        {
            return cmd.Tick > 0;
        }
    }
}
