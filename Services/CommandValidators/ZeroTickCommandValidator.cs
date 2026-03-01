using DVG.Commands;
using DVG.SkyPirates.Server.IServices;

namespace DVG.SkyPirates.Server.Services.CommandValidators
{
    internal class ZeroTickCommandValidator : IGeneralCommandValidator
    {
        public bool IsValid<T>(Command<T> cmd)
        {
            return cmd.Tick > 0;
        }
    }
}
