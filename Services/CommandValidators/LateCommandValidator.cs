using DVG.Commands;
using DVG.SkyPirates.Client.IServices;
using DVG.SkyPirates.Server.IServices;

namespace DVG.SkyPirates.Server.Services.CommandValidators
{
    public class LateCommandValidator : IGeneralCommandValidator
    {
        private readonly ITickCounterService _tickCounter;

        public LateCommandValidator(ITickCounterService tickCounter)
        {
            _tickCounter = tickCounter;
        }

        public bool IsValid<T>(Command<T> cmd)
        {
            return cmd.Tick > (_tickCounter.TickCounter - Constants.ValidTicksCount);
        }
    }
}
